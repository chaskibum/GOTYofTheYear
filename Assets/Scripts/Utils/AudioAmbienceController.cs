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

        [SerializeField] private FadeDuration fadeDuration;

        private enum FadeDuration
        {
            Short = 1,
            Medium = 3,
            Long = 6
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (fadeIn)
                ambienceSource.DOFade(1f, (float)fadeDuration);
            else
                ambienceSource.DOFade(0f, (float)fadeDuration);

            if (changeSnapshot)
            {
                reverbSnapshot.TransitionTo((float)fadeDuration);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (fadeIn)
                ambienceSource.DOFade(0f, (float)fadeDuration);
            else
                ambienceSource.DOFade(1f, (float)fadeDuration);

            if (changeSnapshot)
            {
                snapshot.TransitionTo((float)fadeDuration);
            }
        }
    }
}