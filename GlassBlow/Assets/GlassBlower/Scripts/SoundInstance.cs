using UnityEngine;

namespace GlassBlower.Scripts
{
    [DefaultExecutionOrder(-50)]
    public class SoundInstance : MonoBehaviour
    {
        [SerializeField] private AudioSource _source;

        private static SoundInstance _instance;

        private void Awake()
        {
            _instance = this;
        }

        public static void Play(AudioClip clip)
        {
            _instance._source.PlayOneShot(clip);
        }
    }
}