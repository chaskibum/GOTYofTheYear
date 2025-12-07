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

        private void Awake()
        {
            _sprites = GetComponentsInChildren<SpriteRenderer>();
            transform.position = basePosition;
        }
        
        public IEnumerator BabAttack()
        {
            FadeIn();
            transform.DOMove(targetPosition, duration).SetEase(Ease.Linear);
            yield return new WaitForSeconds(duration - 0.5f);
            FadeOut();
            yield return new WaitForSeconds(1f);
            transform.position = basePosition;
        }

        private void FadeOut()
        {
            foreach (var babita in _sprites)
            {
                if (babita == null) continue;
                babita.DOFade(0f, 1f);
            }
        }

        private void FadeIn()
        {
            foreach (var babita in _sprites)
            {
                if (babita == null) continue;
                babita.DOFade(1f, 1f);
            }
        }
    }
}
