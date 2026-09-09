using DG.Tweening;
using UnityEngine;

namespace Utils
{
    public class AudioAmbienceController : MonoBehaviour
    {
        [SerializeField] private AudioSource ambienceSource;

        [SerializeField] private bool fadeIn = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (fadeIn)
                ambienceSource.DOFade(1f, 3f);
            else
                ambienceSource.DOFade(0f, 3f);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (fadeIn)
                ambienceSource.DOFade(0f, 3f);
            else
                ambienceSource.DOFade(1f, 3f);
        }
    }
}