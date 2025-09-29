using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemiesScripts
{
    public class CrowBossScript : EnemyController
    {
        protected bool BossSlain;
        protected float ChaseTimer;

        [SerializeField] private GameObject gameWonCollectable;
        [SerializeField] private List<GameObject> waves;

        protected override void ResetEnemy()
        {
            if (!BossSlain)
            {
                base.ResetEnemy();
                attackHitbox.SetActive(true);
            }
        }

        private IEnumerator WaveSpawner()
        {
            if (!BossSlain)
            {
                foreach (GameObject wave in waves)
                {
                    foreach (Transform child in wave.transform)
                    {
                        if (!child.gameObject.activeInHierarchy) child.gameObject.SetActive(true);
                    }
                    wave.SetActive(true);
                    yield return new WaitForSeconds(5f);
                }
            }
        }

        protected override void GetPlayerPosition()
        {
            PlayerPos = Player.GetPlayerTarget;
            PlayerPos.y += 1f;
        }

        protected override void ChaseState()
        {
            base.ChaseState();
            StartCoroutine(WaveSpawner());
            ChaseTimer += Time.deltaTime;
            if (ChaseTimer >= 3f)
            {
                SetState(State.Idle);
                StartCoroutine(LockStateForSeconds(data.moveSpeed));
                ChaseTimer = 0f;
            }
        }

        public override void GetHit()
        {
            base.GetHit();
            if (Hp <= 0) BossSlain = true;
            gameWonCollectable.SetActive(true);
        }
    }
}
