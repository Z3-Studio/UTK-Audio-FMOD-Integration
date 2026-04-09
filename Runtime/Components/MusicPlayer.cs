using UnityEngine;

namespace Z3.Audio.FMODIntegration
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private SoundData soundData;

        private void Awake()
        {
            AudioManager.SetCurrentMusic(soundData);
        }

        private void OnDisable()
        {
            AudioManager.RemoveMusic(soundData);
        }
    }
}