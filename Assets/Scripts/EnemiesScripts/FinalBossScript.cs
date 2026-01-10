using System;
using System.Collections;
using DG.Tweening;
using Managers;
using PlayerScripts;
using UnityEngine;
using Utils;

namespace EnemiesScripts
{
    public class FinalBossScript : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        private Rigidbody2D _rb;
        private Animator _animator;
        private Vector3 _originalPos;
        private Vector3 _thunderPos;
        private bool _standStill;
        private bool _coroutineStarted = false;
        private Coroutine _babitaCoroutine;
        private Camera _cam;
        [SerializeField] private GameObject _vieja;
        [SerializeField] private Collider2D hitbox;
        [SerializeField] private GlobalLightManager globalLight;
        
        
        private PlayerController _player;
        private Vector3 _playerPos;
        private bool _playerToTheRight;

        [Header("Wandering Paths")]
        private Vector3[] _verticalPath;
        private Vector3[] _horizontalPath;
        private Tween _movementPattern;
        public float speed;

        [Header("Attacks")]
        [SerializeField] private FinalBossBabita attack1;
        [SerializeField] private FinalBossBabita attack2;
        [SerializeField] private FinalBossThunder thunder1;
        [SerializeField] private FinalBossThunder thunder2;
        
        private void Start()
        {
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _originalPos = transform.position;
            _thunderPos = new Vector3(582, -58, 0);
            _standStill = false;
            speed = 8f;
            _cam = Camera.main;
            
            _player = GameManager.Instance.GetPlayer;
            
            CreatePaths();
            ShowAttacks();
            AddListeners(true);
        }
        
        private void AddListeners(bool add)
        {
            if (add)
            {
                GameManager.Instance.GetPlayerRespawn.AddListener(ResetEnemy);
                GameManager.Instance.GetPlayer.GetPlayerRevived.AddListener(ResetEnemy);
                GameManager.Instance.GetGameRestarted.AddListener(ResetEnemy);
                return;
            }

            GameManager.Instance.GetPlayerRespawn.RemoveListener(ResetEnemy);
            GameManager.Instance.GetPlayer.GetPlayerRevived.RemoveListener(ResetEnemy);
            GameManager.Instance.GetGameRestarted.RemoveListener(ResetEnemy);
        }

        private void OnDestroy()
        {
            AddListeners(false);
        }

        private void ResetEnemy()
        {
            transform.position = _originalPos;
            _movementPattern.Pause();
            DeactivateCoroutine();
            Disappear();
            thunder1.HideThunders();
            thunder2.HideThunders();
            if (_standStill) globalLight.ChangeLight(0.1f);
            speed = 8f;
            ShowAttacks();
        }

        private void Update()
        {
            GetPlayerPosition();
            
            if (!_vieja.activeInHierarchy && _coroutineStarted && !_standStill)
                transform.position = Vector3.MoveTowards(transform.position, _playerPos, speed * Time.deltaTime);
        }
        
        public void DeactivateCoroutine()
        {
            if (_babitaCoroutine != null)
            {
                ShowAttacks(false);
                StopCoroutine(_babitaCoroutine);
                _babitaCoroutine = null;
                _coroutineStarted = false;
                if (_cam.orthographicSize != 7f)
                {
                    DOTween.PauseAll();
                    _cam.DOOrthoSize(7f, 1.5f);
                    globalLight.ChangeLight(0.1f);
                }
            }
        }

        public void StartFight()
        {
            if (_movementPattern == null) MoveAround(true);
            else _movementPattern.Play();
            if (_babitaCoroutine == null)
            {
                _babitaCoroutine = StartCoroutine(nameof(BabitAttack));
                _coroutineStarted = true;
            }
        }

        private void ShowAttacks(bool show = true)
        {
            attack1.gameObject.SetActive(show);
            attack2.gameObject.SetActive(show);
            thunder1.gameObject.SetActive(show);
            thunder2.gameObject.SetActive(show);
        }

        private IEnumerator BabitAttack()
        {
            while (true)
            {
                Appear();
                _standStill = false;
                yield return new WaitForSeconds(2f);
                StartCoroutine(attack1.BabAttack());
                AudioManager.Instance.PlayClip(AudioManager.AudioList.CalderoAttack1);
                AudioManager.Instance.PlayClip(AudioManager.AudioList.Babita1);
                _cam.DOShakePosition(.6f, .6f);
                _animator.SetTrigger("Attack");
                yield return new WaitForSeconds(10f);
                StartCoroutine(attack2.BabAttack());
                AudioManager.Instance.PlayClip(AudioManager.AudioList.CalderoAttack2);
                AudioManager.Instance.PlayClip(AudioManager.AudioList.Babita2);
                _cam.DOShakePosition(.6f, .6f);
                _animator.SetTrigger("Attack");
                yield return new WaitForSeconds(13.5f);
                StartCoroutine(attack1.BabAttack());
                StartCoroutine(attack2.BabAttack());
                AudioManager.Instance.PlayClip(AudioManager.AudioList.CalderoAttack3);
                AudioManager.Instance.PlayClip(AudioManager.AudioList.Babita1);
                AudioManager.Instance.PlayClip(AudioManager.AudioList.Babita2);
                _cam.DOShakePosition(.6f, .6f);
                _animator.SetTrigger("Attack");
                yield return new WaitForSeconds(1.5f);
                Disappear();
                yield return new WaitForSeconds(8f);
                _standStill = true;
                transform.position = _thunderPos;
                Appear();
                globalLight.ChangeLight(0);
                yield return new WaitForSeconds(1f);
                _cam.transform.position = _thunderPos;
                StartCoroutine(thunder1.ThunderAttack());
                StartCoroutine(thunder2.ThunderAttack());
                AudioManager.Instance.PlayClip(AudioManager.AudioList.ThunderAttack);
                _cam.DOShakePosition(8f, 0.2f);
                _cam.DOOrthoSize(23f, 1.5f);
                yield return new WaitForSeconds(8f);
                _cam.DOOrthoSize(7f, 3f);
                globalLight.ChangeLight(0.1f);
            }
        }

        private void CreatePaths()
        {
            var basePos = _sprite.transform.localPosition;
            
            _verticalPath = new Vector3[]
            {
                basePos + new Vector3(0, 0),
                basePos + new Vector3(-2, -2),
                basePos + new Vector3(0, -4),
                basePos + new Vector3(-2, -2),
                basePos + new Vector3(0, 0),
                basePos + new Vector3(2, 2),
                basePos + new Vector3(0, 4),
                basePos + new Vector3(2, 2),
                basePos + new Vector3(0, 0),
            };
            
            _horizontalPath = new Vector3[]
            {
                basePos + new Vector3(0, 0),
                basePos + new Vector3(2, -2),
                basePos + new Vector3(4, 0),
                basePos + new Vector3(2, -2),
                basePos + new Vector3(0, 0),
                basePos + new Vector3(-2, 2),
                basePos + new Vector3(-4, 0),
                basePos + new Vector3(-2, 2),
                basePos + new Vector3(0, 0),
            };
        }

        private void MoveAround(bool moveHorizontal)
        {
            var path = moveHorizontal ? _horizontalPath : _verticalPath;
            _movementPattern = _sprite.transform.DOLocalPath(path, 10f, PathType.CatmullRom)
                .SetEase(Ease.Linear)
                .SetLoops(3)
                .OnComplete(() => MoveAround(!moveHorizontal));
        }

        private void Appear()
        {
            _sprite.DOFade(1, 1);
        }
        
        private void Disappear()
        {
            _sprite.DOFade(0, 1);
        }
        
        private void GetPlayerPosition()
        {
            _playerPos = _player.GetPlayerTarget;
        }
        
        public Animator GetAnimator => _animator;
    }
}
