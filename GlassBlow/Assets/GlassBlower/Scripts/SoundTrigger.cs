using UnityEngine;

namespace GlassBlower.Scripts
{
    public class SoundTrigger : MonoBehaviour
    {
        [SerializeField] private AudioClip _clip;

        public void Play()
        {
            SoundInstance.Play(_clip);
        }
    }
}