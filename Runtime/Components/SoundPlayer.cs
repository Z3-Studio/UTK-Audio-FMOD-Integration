using UnityEngine;

namespace Z3.Audio.FMODIntegration
{
    public enum DisableAction
    {
        None,
        StopImmediate,
        StopWithFade,
        Pause,
    }

    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private SoundData soundData;
        [SerializeField] private DisableAction disableAction = DisableAction.StopWithFade;

        private SoundInstance instance;

        private void OnEnable()
        {
            if (disableAction == DisableAction.Pause)
            {
                instance?.Unpause();
            }
            else
            {
                instance = soundData.PlaySound(transform);
            }
        }

        private void OnDisable()
        {
            switch (disableAction)
            {
                case DisableAction.None:
                    instance = null;
                    break;

                case DisableAction.StopImmediate:
                    instance.StopImmediate();
                    instance = null;
                    break;

                case DisableAction.StopWithFade:
                    instance.StopWithFade();
                    instance = null;
                    break;

                case DisableAction.Pause:
                    instance.Pause();
                    break;

                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}
