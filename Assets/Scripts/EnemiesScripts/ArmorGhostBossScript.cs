using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EnemiesScripts
{
    public class ArmorGhostBossScript : EnemyController
    {
        [SerializeField] private GameObject unlockableSkill;
        [SerializeField] private GameObject attack;
        [SerializeField] private List<GameObject> enemies;
        private Coroutine _spawnCoroutine;
        private float _rotation;
        
        protected bool BossSlain;

        protected override void ResetEnemy()
        {
            if (!BossSlain)
            {
                base.ResetEnemy();
                attackHitbox.SetActive(false);

                if (_spawnCoroutine != null)
                {
                    StopCoroutine(_spawnCoroutine);
                    _spawnCoroutine = null;
                }
                
                foreach (GameObject enemy in enemies)
                    enemy.SetActive(false);
            }
        }

        protected override void ChaseState()
        {
            base.ChaseState();

            if (_spawnCoroutine == null)
            {
                _spawnCoroutine = StartCoroutine(nameof(SpawnEnemies));
            }
        }

        public override void GetHit()
        {
            base.GetHit();
            if (Hp > 0) AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorHit, true);
            else
            {
                AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorDeath);
                BossSlain = true;
                unlockableSkill.SetActive(true);
                foreach (GameObject enemy in enemies)
                    Destroy(enemy);
            }
        }

        protected override void Attack()
        {
            if (!CanAttack) return;
            Body.linearVelocityX = 0f;
            AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorAttack, false, 0.8f);
            base.Attack();
        }
        
        protected override IEnumerator EndFeedback()
        {
            yield return new WaitForSecondsRealtime(0.05f);
            Visuals.color = new Color(0.5f, 1, 0.5f);
            Time.timeScale = 1;
        }

        private IEnumerator SpawnEnemies()
        {
            if (!BossSlain)
            {
                print("im spawning an enemy");
                yield return new WaitForSeconds(Random.Range(7f, 12f));
                enemies[Random.Range(0, enemies.Count)].SetActive(true);
                
                if (_spawnCoroutine != null) StartCoroutine(nameof(SpawnEnemies));
            }
        }
    }
}
