using UnityEngine;

namespace Z3.Audio.FMODIntegration
{
    /// <summary>
    /// SoundReference animation event, to trigger sound Scriptable Objects.
    /// </summary>
    public class SoundTrigger : MonoBehaviour 
    {
        public void OnPlaySound(SoundData soundData)
        {
            soundData.PlaySound(transform);
        }
    }
}
