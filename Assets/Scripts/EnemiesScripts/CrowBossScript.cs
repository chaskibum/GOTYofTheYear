using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace EnemiesScripts
{
    public class CrowBossScript : EnemyController
    {
        protected bool BossSlain;
        protected float ChaseTimer;

        [SerializeField] private GameObject unlockableSkill;
        [SerializeField] private List<GameObject> waves;
        private Coroutine _waveCoroutine;

        protected override void ResetEnemy()
        {
            if (!BossSlain)
            {
                base.ResetEnemy();
                attackHitbox.SetActive(true);

                if (_waveCoroutine != null)
                {
                    StopCoroutine(_waveCoroutine);
                    _waveCoroutine = null;
                }
                
                foreach (GameObject wave in waves)
                    wave.SetActive(false);
            }
        }

        private IEnumerator WaveSpawner()
        {
            if (!BossSlain)
            {
                foreach (GameObject wave in waves)
                {
                    yield return new WaitUntil(() => Hp % 4 == 0);
                    wave.SetActive(true);
                    AudioManager.Instance.PlayClip(AudioManager.AudioList.CrowScream);
                    yield return new WaitWhile(() => Hp % 4 == 0);
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

            if (_waveCoroutine == null)
                _waveCoroutine = StartCoroutine(WaveSpawner());
                
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
            print(Hp);
            if (Hp <= 0)
            {
                BossSlain = true;
                unlockableSkill.SetActive(true);
                foreach (GameObject wave in waves)
                    Destroy(wave);
            }
        }
    }
}
