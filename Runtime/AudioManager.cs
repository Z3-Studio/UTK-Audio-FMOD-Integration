using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Z3.Utils;

namespace Z3.Audio.FMODIntegration
{
    public enum SoundGroup
    {
        Master,
        Music,
        SFX,
        Voice
    }

    public enum UISound
    {
        Submit,
        Cancel,
        Select
    }

    /// <summary>
    /// Manage the sound requests and returns a SoundInstance.
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("UI")]
        [SerializeField] private string untagged = "Untagged";
        [SerializeField] private SoundData submit;
        [SerializeField] private SoundData cancel;
        [SerializeField] private SoundData select;

        private static SoundInstance currentMusic;

        // TODO: Dictionary< String tag, Enum/Object State> InstanceState -> Block paused list to play
        private static readonly Dictionary<string, List<SoundInstance>> tagLists = new();

        #region Public Methodsx
        public static void SetCurrentMusic(SoundData music)
        {
            if (currentMusic == music)
                return;

            if (currentMusic)
            {
                currentMusic.StopWithFade();
                currentMusic = null;
            }

            if (music)
                currentMusic = music.PlaySound();
        }

        public static void RemoveMusic(SoundData music)
        {
            if (currentMusic != music)
                return;

            currentMusic.StopWithFade();
        }

        public static void PlayUISound(UISound soundType)
        {
            SoundData sound = soundType switch
            {
                UISound.Submit => Instance.submit,
                UISound.Cancel => Instance.cancel,
                UISound.Select => Instance.select,
                _ => throw new NotImplementedException(),
            };

            sound.PlaySound();
        }

        /// <summary> Set the VCA mixer </summary>
        /// <param name="volume">0 to 1</param>
        public static void SetVolume(SoundGroup soundGroup, float volume)
        {
            VCA vca = RuntimeManager.GetVCA($"vca:/{soundGroup}");
            vca.setVolume(volume);
        }
        #endregion

        internal static SoundInstance PlaySound(EventReference eventReference, Transform transform, List<string> tags)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            if (transform)
                RuntimeManager.AttachInstanceToGameObject(eventInstance, transform);
            //else
            //eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(new Vector3()));

            return CreateSoundInstance(eventInstance, tags);
        }

        internal static SoundInstance PlaySound(EventReference eventReference, Vector3 position, List<string> tags)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

            return CreateSoundInstance(eventInstance, tags);
        }

        private static SoundInstance CreateSoundInstance(EventInstance eventInstance, List<string> tags)
        {
            SoundInstance soundInstance = new SoundInstance(eventInstance);
            soundInstance.Start();

            // Add to list
            if (tags.Count > 0)
            {
                foreach (string tag in tags)
                {
                    if (!tagLists.TryGetValue(tag, out List<SoundInstance> instances))
                    {
                        instances = new List<SoundInstance>();
                        tagLists[tag] = instances;
                    }

                    instances.Add(soundInstance);
                }
            }
            else
            {
                string untagged = Instance.untagged;
                if (!tagLists.TryGetValue(untagged, out List<SoundInstance> instances))
                {
                    instances = new List<SoundInstance>();
                    tagLists[untagged] = instances;
                }

                instances.Add(soundInstance);
            }


            return soundInstance;
        }

        #region Tags

        // Stop immediately
        public static void StopImmediateTags(params string[] tags) => StopImmediateTags(tags.ToHashSet());
        public static void StopImmediateTags(HashSet<string> tags) => ProcessTags(tags, i => i.StopImmediate(), tag => tagLists.Remove(tag));

        public static void StopImmediateTagsExcept(params string[] exceptTag) => StopImmediateTagsExcept(exceptTag.ToHashSet());
        public static void StopImmediateTagsExcept(HashSet<string> exceptTag) => ProcessTagsExcept(exceptTag, i => i.StopImmediate(), tag => tagLists.Remove(tag));

        // Stop with fade
        // TODO: Fade should not remove instance from list until fade is done
        public static void StopWithFadeTags(params string[] tags) => StopWithFadeTags(tags.ToHashSet());
        public static void StopWithFadeTags(HashSet<string> tags) => ProcessTags(tags, i => i.StopWithFade(), tag => tagLists.Remove(tag));

        public static void StopWithFadeTagsExcept(params string[] exceptTag) => StopWithFadeTagsExcept(exceptTag.ToHashSet());
        public static void StopWithFadeTagsExcept(HashSet<string> exceptTag) => ProcessTagsExcept(exceptTag, i => i.StopWithFade(), tag => tagLists.Remove(tag));

        // Pause
        public static void PauseTags(params string[] tags) => PauseTags(tags.ToHashSet());
        public static void PauseTags(HashSet<string> tags) => ProcessTags(tags, i => i.Pause(), delegate { });

        public static void PauseTagsExcept(params string[] exceptTag) => PauseTagsExcept(exceptTag.ToHashSet());
        public static void PauseTagsExcept(HashSet<string> exceptTag) => ProcessTagsExcept(exceptTag, i => i.Pause(), delegate { });

        // Resume
        public static void UnpauseTags(params string[] tags) => UnpauseTags(tags.ToHashSet());
        public static void UnpauseTags(HashSet<string> tags) => ProcessTags(tags, i => i.Unpause(), delegate { });

        public static void UnpauseTagsExcept(params string[] exceptTag) => UnpauseTagsExcept(exceptTag.ToHashSet());
        public static void UnpauseTagsExcept(HashSet<string> exceptTag) => ProcessTagsExcept(exceptTag, i => i.Unpause(), delegate { });

        public static void ProcessTags(HashSet<string> tags, Action<SoundInstance> instanceAction, Action<string> afterAction)
        {
            foreach (string tag in tagLists.Keys.ToList())
            {
                if (!tags.Contains(tag))
                    continue;

                List<SoundInstance> list = tagLists[tag];

                foreach (SoundInstance instance in list.ToList())
                {
                    if (instance.SoundFinished())
                    {
                        list.Remove(instance);
                    }
                    else
                    {
                        instanceAction.Invoke(instance);
                    }
                }

                afterAction.Invoke(tag);
            }
        }

        public static void ProcessTagsExcept(HashSet<string> exceptTags, Action<SoundInstance> instanceAction, Action<string> afterAction)
        {
            foreach (string tag in tagLists.Keys.ToList())
            {
                if (exceptTags.Contains(tag))
                    continue;

                List<SoundInstance> list = tagLists[tag];

                foreach (SoundInstance instance in list.ToList())
                {
                    if (instance.SoundFinished())
                    {
                        list.Remove(instance);
                    }
                    else
                    {
                        instanceAction.Invoke(instance);
                    }
                }

                afterAction.Invoke(tag);
            }
        }
        #endregion
    }
}