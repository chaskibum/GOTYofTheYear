using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace EnemiesScripts
{
    public class FinalBossThunder : MonoBehaviour
    {
        private ParticleSystem _thunder;
        private Collider2D _hitbox;

        [SerializeField] private Vector3 basePosition;
        [SerializeField] private Vector3 targetPosition;
        
        [SerializeField] private float duration;

        private void Awake()
        {
            transform.position = basePosition;
            _thunder = GetComponentInChildren<ParticleSystem>();
            _hitbox = GetComponent<Collider2D>();
        }

        private void Start()
        {
            HideThunders();
        }
        
        public IEnumerator ThunderAttack()
        {
            ShowThunders();
            yield return new WaitForSeconds(2f);
            transform.DOMove(targetPosition, duration).SetEase(Ease.Linear);
            yield return new WaitForSeconds(duration - 0.5f);
            HideThunders();
            yield return new WaitForSeconds(1f);
            transform.position = basePosition;
        }

        private void ShowThunders()
        {
            _thunder.Clear();
            _thunder.Play();
            Invoke(nameof(HitboxOn), 2f);
        }

        private void HitboxOn()
        {
            _hitbox.enabled = true;
        }
        
        public void HideThunders()
        {
            _thunder.Stop();
            _hitbox.enabled = false;
        }
    }
}
