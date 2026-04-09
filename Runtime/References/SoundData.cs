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
        [field: SerializeField] internal EventReference EventReference { get; private set; }
        [SerializeField, System.Obsolete] internal EventReference eventReference;

        internal FMOD.GUID Guid => eventReference.Guid;

#if UNITY_EDITOR
        public string Path => !eventReference.IsNull ? eventReference.Path : "NULL EVENT REFERENCE";
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

        public static bool operator ==(SoundInstance instance, SoundData reference)
        {
            if (instance && reference)
                return instance.Guid == reference.Guid;

            return !instance && !reference; // Both is null
        }

        public static bool operator !=(SoundInstance a, SoundData b) => !(a == b);
        public static bool operator ==(SoundData a, SoundInstance b) => b == a;
        public static bool operator !=(SoundData a, SoundInstance b) => !(b == a);

        public bool IsNull() => eventReference.IsNull;

        public bool IsSameSound(SoundData other) => Guid == other.Guid;

        public override string ToString() => Path;
    }
}
