using FMODUnity;
using UnityEngine;
using Z3.Utils;

namespace Z3.Audio.FMODIntegration
{
    /// <summary>
    /// SoundReference Scriptable Object. You can give position through Transform, Vector2 or nothing at all.
    /// Store a SoundInstance through PlaySound to have more control of it, if needed.
    /// </summary>
    [CreateAssetMenu(menuName = Z3Path.ScriptableObjects + "Sound Data (FMOD)", fileName = "NewSoundData")]
    public class SoundData : ScriptableObject 
    {
        [SerializeField] internal EventReference EventReference { get; private set; }
        [SerializeField, System.Obsolete] internal EventReference eventReference;

#if UNITY_EDITOR
        public string Path => !eventReference.IsNull ? eventReference.Path : "NULL";
#else
        public string Path => eventReference.ToString();
#endif

        public SoundInstance PlaySound(Transform transform)
        {
            return AudioManager.PlaySound(eventReference, transform);
        }

        public SoundInstance PlaySound(Vector3 position)
        {
            return AudioManager.PlaySound(eventReference, position);
        }

        public SoundInstance PlaySound()
        {
            return AudioManager.PlaySound(eventReference, null);
        }

        public bool IsNull() => eventReference.IsNull;

        public bool IsSameSound(SoundData other) => eventReference.Guid == other.eventReference.Guid;

        public override string ToString() => Path;
    }
}
