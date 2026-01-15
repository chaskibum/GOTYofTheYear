using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace EnemiesScripts
{
    public class FinalBossBabita : MonoBehaviour
    {
        private SpriteRenderer[] _sprites;

        [SerializeField] private Vector3 basePosition;
        [SerializeField] private Vector3 targetPosition;
        
        [SerializeField] private float duration;

        [SerializeField] private ParticleSystem[] babitas;
        [SerializeField] private ParticleSystem[] brillitos;

        private void Awake()
        {
            _sprites = GetComponentsInChildren<SpriteRenderer>();
            transform.position = basePosition;
        }

        public IEnumerator BabAttack()
        {
            FadeIn();
            transform.DOMove(targetPosition, duration).SetEase(Ease.Linear);
            yield return new WaitForSeconds(duration);
            FadeOut();
            yield return new WaitForSeconds(1f);
            transform.position = basePosition;
        }

        private void FadeOut()
        {
            foreach (var brillito in brillitos)
            {
                if (brillito == null) continue;
                brillito.Stop();
            }
        }

        private void FadeIn()
        {
            foreach (var babita in babitas)
            {
                if (babita == null) continue;
                babita.Play();
                // babita.transform.DOKill();
                // babita.transform.DOBlendableLocalRotateBy(new Vector3(0, 0, 360), duration).SetEase(Ease.Linear);
            }

            foreach (var brillito in brillitos)
            {
                if (brillito == null) continue;
                brillito.Play();
            }
        }
    }
}
