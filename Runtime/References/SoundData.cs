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
        [SerializeField] private EventReference eventReference;

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
    }
}
