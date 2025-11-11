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
                Animator?.SetBool("Attacking", false);

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
            Animator?.SetBool("Attacking", false);
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
            Animator?.SetBool("Attacking", true);
            Body.linearVelocityX = 0f;
            AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorAttack, false, 0.8f);
            base.Attack();
        }
        
        protected override void EndAttack()
        {
            base.EndAttack();
            Animator?.SetBool("Attacking", false);
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
                Animator?.SetBool("Screaming", true);
                SetState(State.Idle);
                StartCoroutine(LockStateForSeconds(1.5f));
                yield return new WaitForSeconds(Random.Range(10f, 15f));
                enemies[Random.Range(0, enemies.Count)].SetActive(true);
                Animator?.SetBool("Screaming", true);
                SetState(State.Idle);
                StartCoroutine(LockStateForSeconds(1.5f));
                RelocateObjects();
                
                if (_spawnCoroutine != null) StartCoroutine(nameof(SpawnEnemies));
            }
        }

        public void StopScreaming()
        {
            SetState(State.Chase);
            Animator?.SetBool("Screaming", false);
        }

        private void RelocateObjects()
        {
            GameManager.Instance.GetResetObjects.Invoke();
            
            foreach (GameObject obj in objects)
            {
                obj.transform.localPosition = new Vector3(0, 23, 0);
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
