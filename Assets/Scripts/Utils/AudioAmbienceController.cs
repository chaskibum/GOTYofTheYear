using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace Utils
{
    public class AudioAmbienceController : MonoBehaviour
    {
        [SerializeField] private AudioSource ambienceSource;

        [SerializeField] private bool fadeIn = true;

        [SerializeField] private bool changeSnapshot = false;

        [SerializeField] private AudioMixerSnapshot snapshot;
        [SerializeField] private AudioMixerSnapshot reverbSnapshot;


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (fadeIn)
                ambienceSource.DOFade(1f, 3f);
            else
                ambienceSource.DOFade(0f, 3f);

            if (changeSnapshot)
            {
                reverbSnapshot.TransitionTo(1.5f);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (fadeIn)
                ambienceSource.DOFade(0f, 3f);
            else
                ambienceSource.DOFade(1f, 3f);

            if (changeSnapshot)
            {
                snapshot.TransitionTo(1.5f);
            }
        }
    }
}