using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        private Camera _cam;

        [SerializeField] private SkillCollectable unlockableSkill;
        [SerializeField] private List<GameObject> waves;
        [SerializeField] private HealthBar healthBar;
        [SerializeField] private CanvasGroup healthGroup;
        [SerializeField] private AudioSource chillido;
        [SerializeField] private AudioSource painScream;
        [SerializeField] private AudioSource drums1;
        [SerializeField] private AudioSource drums2;
        [SerializeField] private AudioSource songEnding;
        [SerializeField] private ParticleSystem deathParticles;
        [SerializeField] private AudioSource spookySound1;
        [SerializeField] private AudioSource spookySound2;
        private Coroutine _waveCoroutine;

        protected override void Start()
        {
            base.Start();
            if (PlayerPrefs.GetString("CrowDead") == "yes")
            {
                gameObject.SetActive(false);
                BossSlain = true;
                spookySound1.Stop();
                spookySound2.Stop();
            }
            else
            {
                healthBar.SetMaxHealth(data.baseHp);
                healthBar.SetHealth(data.baseHp);
            }
            _cam = Camera.main;
        }
        
        protected override void ResetEnemy()
        {
            if (!BossSlain)
            {
                base.ResetEnemy();
                healthBar.SetHealth(data.baseHp);
                unlockableSkill.HideSkill();
                attackHitbox.SetActive(true);
                wentUp = false;
                chillidoPlayed = false;
                modifyPosition = 0;
                healthGroup.gameObject.SetActive(true);
                healthGroup.alpha = 0;
                RestartSongs();

                DeactivateWaves();
            }
        }

        private void DeactivateWaves()
        {
            if (_waveCoroutine != null)
            {
                StopCoroutine(_waveCoroutine);
                _waveCoroutine = null;
            }
            foreach (GameObject wave in waves)
                if (wave)
                    wave.SetActive(false);
        }

        protected override void RestartEnemy()
        {
            base.RestartEnemy();
            healthGroup.gameObject.SetActive(true);
            healthGroup.alpha = 0;
            healthBar.SetMaxHealth(data.baseHp);
            healthBar.SetHealth(data.baseHp);
            PlayerPrefs.SetString("CrowDead", "no");
            BossSlain = false;
            attackHitbox.SetActive(true);
            unlockableSkill.HideSkill();
            wentUp = false;
            chillidoPlayed = false;
            modifyPosition = 0;
            RestartSongs();
            DeactivateWaves();
        }

        private IEnumerator WaveSpawner()
        {
            if (!BossSlain)
            {
                foreach (GameObject wave in waves)
                {
                    yield return new WaitUntil(() => Hp % 4 == 0);
                    wave.SetActive(true);
                    painScream.Play();
                    _cam.DOShakePosition(1.2f, 1f);
                    if (Hp < data.baseHp / 2 + 1)
                        drums1.loop = false;
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

            if (healthGroup.alpha < 1)
                healthGroup.DOFade(1, 1.5f);
            
            if (_waveCoroutine == null)
                _waveCoroutine = StartCoroutine(WaveSpawner());
            
            if (modifyPosition == 0 && !chillidoPlayed)
            {
                chillido.Play();
                chillidoPlayed = true;
            }
            
            if (!drums1.isPlaying && Hp > data.baseHp / 2)
                drums1.Play();
            else if (!drums1.isPlaying && !drums2.isPlaying && Hp < data.baseHp / 2 + 1)
                drums2.Play();
                
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
                Invoke(nameof(StopSlowMotion), 0.5f);
            }
            else
            {
                healthBar.SetHealth(Hp);
            }
        }

        private void Die()
        {
            SetState(State.Die);
            healthGroup.gameObject.SetActive(false);
            PlayerPrefs.SetString("CrowDead", "yes");
            BossSlain = true;
            unlockableSkill.Activate();
            RestartSongs();
            songEnding.Play();
            deathParticles.Play();
            spookySound1.Stop();
            spookySound2.Stop();
            AudioManager.Instance.PlayClip(AudioManager.AudioList.BossKill);
            /*foreach (GameObject wave in waves)
                Destroy(wave);*/
            DeactivateWaves();
        }

        protected override IEnumerator EndFeedback()
        {
            yield return new WaitForSecondsRealtime(0.05f);
            Visuals.color = new Color(0.5f, 1, 0.5f);
            if (CurrentState == State.Die)
                Time.timeScale = 0.3f;
            else
                Time.timeScale = 1;
        }

        private void RestartSongs()
        {
            spookySound1.Play();
            spookySound2.Play();
            drums1.loop = true;
            drums1.Stop();
            drums2.Stop();
            songEnding.Stop();
        }

        private void StopSlowMotion()
        {
            Time.timeScale = 1f;
            print(Time.timeScale);
        }
    }
}
