using UnityEngine;

namespace Z3.Audio.FMODIntegration
{
    public enum DisableAction
    {
        None,
        StopImmediate,
        StopWithFade
    }

    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private SoundData soundData;
        [SerializeField] private DisableAction disableAction = DisableAction.StopWithFade;

        private SoundInstance instance;

        private void OnEnable()
        {
            instance = soundData.PlaySound(transform);
            AudioManager.AddToPauseSoundsList(instance);
        }

        private void OnDisable()
        {
            switch (disableAction)
            {
                case DisableAction.None:
                    break;

                case DisableAction.StopImmediate:
                    instance.StopImmediate();
                    break;

                case DisableAction.StopWithFade:
                    instance.StopWithFade();
                    break;

                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}
