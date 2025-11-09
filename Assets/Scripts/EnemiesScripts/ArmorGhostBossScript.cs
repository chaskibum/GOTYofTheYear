using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EnemiesScripts
{
    public class ArmorGhostBossScript : EnemyController
    {
        [SerializeField] private GameObject unlockableSkill;
        [SerializeField] private GameObject attack;
        [SerializeField] private GameObject lockDoor;
        [SerializeField] private List<GameObject> enemies;
        [SerializeField] private List<GameObject> objects;
        private Coroutine _spawnCoroutine;
        private Vector3 _startingLockPosition = new Vector3(214, -111, 0);
        private Vector3 _targetLockPosition = new Vector3(214, -104, 0);
        
        private bool BossSlain;

        protected override void ResetEnemy()
        {
            if (!BossSlain)
            {
                base.ResetEnemy();
                unlockableSkill.SetActive(false);
                attackHitbox.SetActive(false);
                lockDoor.transform.position = _startingLockPosition;

                if (_spawnCoroutine != null)
                {
                    StopCoroutine(_spawnCoroutine);
                    _spawnCoroutine = null;
                }
                
                foreach (GameObject enemy in enemies)
                    enemy.SetActive(false);
                
                foreach (GameObject obj in objects)
                    obj.SetActive(false);
            }
        }
        
        protected override void RestartEnemy()
        {
            base.RestartEnemy();
            unlockableSkill.SetActive(false);
        }

        protected override void ChaseState()
        {
            base.ChaseState();

            if (_spawnCoroutine == null)
            {
                _spawnCoroutine = StartCoroutine(nameof(SpawnEnemies));
            }

            if (lockDoor.transform.position != _targetLockPosition)
            {
                lockDoor.transform.position = Vector3.Lerp(lockDoor.transform.position, _targetLockPosition, 3 * Time.deltaTime);
            }
        }

        public override void GetHit()
        {
            base.GetHit();
            EndAttack();
            
            if (Hp > 0) AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorHit, true);
            else
            {
                AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorDeath);
                BossSlain = true;
                unlockableSkill?.SetActive(true);
                lockDoor.SetActive(false);
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
                yield return new WaitForSeconds(Random.Range(5f, 8f));
                objects[Random.Range(0, objects.Count)].SetActive(true);
                yield return new WaitForSeconds(Random.Range(10f, 15f));
                enemies[Random.Range(0, enemies.Count)].SetActive(true);
                RelocateObjects();
                
                if (_spawnCoroutine != null) StartCoroutine(nameof(SpawnEnemies));
            }
        }

        private void RelocateObjects()
        {
            GameManager.Instance.GetResetObjects.Invoke();
            
            foreach (GameObject obj in objects)
            {
                obj.transform.position = new Vector3(10, 23, 0);
                obj.SetActive(false);
            }
        }

        protected override void DieState()
        {
            base.DieState();
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }
}
