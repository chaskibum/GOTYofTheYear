using System.Collections;
using System.Collections.Generic;
using GameplayElements;
using Managers;
using UnityEngine;

namespace EnemiesScripts
{
    public class CrowBossScript : EnemyController
    {
        private bool BossSlain;
        private float ChaseTimer;
        private bool wentUp;
        private int modifyPosition;
        private bool chillidoPlayed;

        [SerializeField] private GameObject unlockableSkill;
        [SerializeField] private List<GameObject> waves;
        [SerializeField] private HealthBar healthBar;
        private Coroutine _waveCoroutine;

        protected override void Start()
        {
            base.Start();
            if (PlayerPrefs.GetString("CrowDead") == "yes")
            {
                gameObject.SetActive(false);
            }
            else
            {
                healthBar.SetMaxHealth(data.baseHp);
                healthBar.SetHealth(data.baseHp);
            }
        }
        
        protected override void ResetEnemy()
        {
            if (!BossSlain)
            {
                base.ResetEnemy();
                healthBar.SetHealth(Hp);
                healthBar.transform.parent.gameObject.SetActive(false);
                unlockableSkill.SetActive(false);
                attackHitbox.SetActive(true);
                wentUp = false;
                chillidoPlayed = false;
                modifyPosition = 0;

                if (_waveCoroutine != null)
                {
                    StopCoroutine(_waveCoroutine);
                    _waveCoroutine = null;
                }
                
                foreach (GameObject wave in waves)
                    wave.SetActive(false);
            }
        }

        protected override void RestartEnemy()
        {
            base.RestartEnemy();
            healthBar.transform.parent.gameObject.SetActive(false);
            healthBar.SetMaxHealth(data.baseHp);
            healthBar.SetHealth(data.baseHp);
            PlayerPrefs.SetString("CrowDead", "no");
            print("restarted!");
            attackHitbox.SetActive(true);
            unlockableSkill.SetActive(false);
            wentUp = false;
            chillidoPlayed = false;
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
            PlayerPos.y += modifyPosition;
        }

        protected override void ChaseState()
        {
            base.ChaseState();

            if (_waveCoroutine == null)
                _waveCoroutine = StartCoroutine(WaveSpawner());
            
            /*if (modifyPosition == 0 && !chillidoPlayed)
            {
                AudioManager.Instance.PlayClip(AudioManager.AudioList.Chillido);
                chillidoPlayed = true;
            }*/
                
            ChaseTimer += Time.deltaTime;
            if (ChaseTimer >= data.stepAwayForce)
            {
                UpAndDownMovement();
                SetState(State.Idle);
                StartCoroutine(LockStateForSeconds(data.stepAwayForce));
                ChaseTimer = 0f;
            }
        }

        private void UpAndDownMovement()
        {
            if (!wentUp)
            {
                modifyPosition += 6;
                wentUp = true;
                chillidoPlayed = false;
            }
            else
            {
                modifyPosition -= 6;
                wentUp = false;
            }
        }

        public override void GetHit()
        {
            base.GetHit();
            if (Hp <= 0)
            {
                Die();
            }
            else
            {
                healthBar.transform.parent.gameObject.SetActive(true);
                healthBar.SetHealth(Hp);
            }
        }

        private void Die()
        {
            SetState(State.Die);
            healthBar.transform.parent.gameObject.SetActive(false);
            PlayerPrefs.SetString("CrowDead", "yes");
            BossSlain = true;
            unlockableSkill.SetActive(true);
            foreach (GameObject wave in waves)
                Destroy(wave);
        }

        protected override IEnumerator EndFeedback()
        {
            yield return new WaitForSecondsRealtime(0.05f);
            Visuals.color = new Color(0.5f, 1, 0.5f);
            Time.timeScale = 1;
        }
    }
}
