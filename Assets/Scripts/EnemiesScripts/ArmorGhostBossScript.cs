using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameplayElements;
using Managers;
using Utils;

namespace EnemiesScripts
{
    public class ArmorGhostBossScript : EnemyController
    {
        [SerializeField] private SkillCollectable unlockableSkill;
        [SerializeField] private GameObject attack;
        [SerializeField] private GameObject lockDoor;
        [SerializeField] private List<GameObject> enemies;
        [SerializeField] private List<GameObject> objects;
        [SerializeField] private HealthBar healthBar;
        [SerializeField] private CanvasGroup healthGroup;
        [SerializeField] private ParticleSystem deathParticles;
        private Coroutine _spawnCoroutine;
        private Vector3 _startingLockPosition = new Vector3(214, -111, 0);
        private Vector3 _targetLockPosition = new Vector3(214, -104, 0);

        private bool BossSlain;
        private Camera _cam;

        [Header("Audio")]
        [SerializeField] private AudioSource musicStart;
        [SerializeField] private AudioSource musicLoop;
        [SerializeField] private AudioSource musicEnd;
        [SerializeField] private AudioSource swordAttack;
        [SerializeField] private AudioSource scream;

        protected override void Start()
        {
            base.Start();
            if (PlayerPrefs.GetString("ArmorDead") == "yes")
            {
                gameObject.SetActive(false);
                BossSlain = true;
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
                healthBar.SetHealth(Hp);
                healthGroup.gameObject.SetActive(true);
                healthGroup.alpha = 0;
                musicStart.volume = 1;
                attackHitbox.SetActive(false);
                lockDoor.transform.position = _startingLockPosition;
                Animator?.SetBool("Attacking", false);
                unlockableSkill.HideSkill();
                StopMusic();

                DeactivateCoroutine();
            }
        }

        protected override void RestartEnemy()
        {
            base.RestartEnemy();
            healthGroup.gameObject.SetActive(true);
            healthGroup.alpha = 0;
            musicStart.volume = 1;
            healthBar.SetMaxHealth(data.baseHp);
            healthBar.SetHealth(data.baseHp);
            BossSlain = false;
            lockDoor.transform.position = _startingLockPosition;
            PlayerPrefs.SetString("ArmorDead", "no");
            Animator?.SetBool("Attacking", false);
            DeactivateCoroutine();
            unlockableSkill.HideSkill();
            StopMusic();
        }

        protected override void ChaseState()
        {
            base.ChaseState();

            Animator?.SetBool("Hit", false);

            if (healthGroup.alpha < 1)
            {
                PlayMusic();
                healthGroup.DOFade(1, 0.5f);
            }

            if (_spawnCoroutine == null)
            {
                _spawnCoroutine = StartCoroutine(nameof(SpawnEnemies));
            }

            if (lockDoor.transform.position != _targetLockPosition)
            {
                lockDoor.transform.position = Vector3.Lerp(lockDoor.transform.position, _targetLockPosition, 3 * Time.deltaTime);
            }
        }

        private void PlayMusic()
        {
            if (!musicStart.isPlaying)
            {
                musicStart.Play();
                StartCoroutine(PlayLoop());
            }
        }

        private IEnumerator PlayLoop()
        {
            yield return new WaitForSecondsRealtime(3.05f);
            musicStart.volume = 0;
            if (!musicLoop.isPlaying)
                musicLoop.Play();
        }

        public override void GetHit()
        {
            base.GetHit();

            if (Hp > 0)
            {
                healthBar.SetHealth(Hp);
                GamepadVibration.Instance.Rumble(0.3f, 0.6f, 0.15f);
                AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorHit, true);
            }
            else
            {
                Die();
                Invoke(nameof(StopSlowMotion), 0.5f);
            }
        }

        private void Die()
        {
            SetState(State.Die);
            GamepadVibration.Instance.Rumble(0.5f, 0.7f, 1f);
            healthGroup.gameObject.SetActive(false);
            PlayerPrefs.SetString("ArmorDead", "yes");
            AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorDeath);
            AudioManager.Instance.PlayClip(AudioManager.AudioList.BossKill);
            BossSlain = true;
            unlockableSkill.Activate();
            musicLoop.Stop();
            musicEnd.Play();
            deathParticles.Play();
            foreach (GameObject enemy in enemies)
                if (enemy)
                    enemy.SetActive(false);
        }

        protected override void Attack()
        {
            if (!CanAttack) return;
            Animator?.SetBool("Attacking", true);
            Body.linearVelocityX = 0f;
            AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorBossAttack, false, 0.8f);
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
            if (CurrentState == State.Die)
                Time.timeScale = 0.3f;
            else
                Time.timeScale = 1;
        }

        private IEnumerator SpawnEnemies()
        {
            while (!BossSlain)
            {
                yield return new WaitForSeconds(Random.Range(5f, 8f));

                if (BossSlain) break;

                objects[Random.Range(0, objects.Count)].SetActive(true);
                swordAttack.Play();
                _cam.DOShakePosition(1.2f, 1f);
                GamepadVibration.Instance.Rumble(0.6f, 0.9f, 0.6f);
                scream.Play();
                Animator?.SetBool("Screaming", true);
                SetState(State.Idle);
                StartCoroutine(LockStateForSeconds(1.5f));

                yield return new WaitForSeconds(Random.Range(10f, 15f));

                if (BossSlain) break;

                enemies[Random.Range(0, enemies.Count)].SetActive(true);
                scream.Play();
                Animator?.SetBool("Screaming", true);
                SetState(State.Idle);
                StartCoroutine(LockStateForSeconds(1.5f));
                RelocateObjects();
            }
            _spawnCoroutine = null;
        }

        private void DeactivateCoroutine()
        {
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

        public void StopScreaming()
        {
            SetState(State.Chase);
            Animator?.SetBool("Screaming", false);
            Animator?.SetBool("Hit", false);
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

        private void StopMusic()
        {
            musicStart.Stop();
            musicLoop.Stop();
            musicEnd.Stop();
        }

        protected override void DieState()
        {
            base.DieState();
            Animator?.SetBool("Hit", true);
            lockDoor.transform.position = Vector3.Lerp(lockDoor.transform.position, _startingLockPosition, 3 * Time.deltaTime);
            DeactivateCoroutine();
            _spawnCoroutine = null;
        }

        private void StopSlowMotion()
        {
            Time.timeScale = 1f;
        }
    }
}
