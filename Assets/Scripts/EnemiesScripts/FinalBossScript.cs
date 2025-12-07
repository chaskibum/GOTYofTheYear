using System;
using System.Collections;
using DG.Tweening;
using Managers;
using PlayerScripts;
using UnityEngine;

namespace EnemiesScripts
{
    public class FinalBossScript : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        private Rigidbody2D _rb;
        private Animator _animator;
        private Vector3 _originalPos;
        private bool _coroutineStarted = false;
        private Coroutine _babitaCoroutine;
        [SerializeField] private GameObject _vieja;
        [SerializeField] private Collider2D hitbox;
        
        
        private PlayerController _player;
        private Vector3 _playerPos;
        private bool _playerToTheRight;

        [Header("Wandering Paths")]
        private Vector3[] _verticalPath;
        private Vector3[] _horizontalPath;
        private float _speed = 8f;
        private Tween _movementPattern;

        [Header("Attacks")]
        [SerializeField] private FinalBossBabita attack1;
        [SerializeField] private FinalBossBabita attack2;

        private void Start()
        {
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _originalPos = transform.position;
            
            _player = GameManager.Instance.GetPlayer;
            
            CreatePaths();
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
        }

        private void Update()
        {
            GetPlayerPosition();
            
            if (!_vieja.activeInHierarchy && _coroutineStarted)
                transform.position = Vector3.MoveTowards(transform.position, _playerPos, _speed * Time.deltaTime);
        }
        
        private void DeactivateCoroutine()
        {
            if (_babitaCoroutine != null)
            {
                StopCoroutine(_babitaCoroutine);
                _babitaCoroutine = null;
                _coroutineStarted = false;
            }
        }

        public void StartFight()
        {
            if (_movementPattern == null) MoveAround(true);
            else _movementPattern.Play();
            if (_babitaCoroutine == null)
            {
                _babitaCoroutine = StartCoroutine(nameof(BavitAttack));
                _coroutineStarted = true;
            }
        }

        private IEnumerator BavitAttack()
        {
            while (true)
            {
                Appear();
                yield return new WaitForSeconds(2f);
                StartCoroutine(attack1.BabAttack());
                _animator.SetTrigger("Attack");
                yield return new WaitForSeconds(10f);
                StartCoroutine(attack2.BabAttack());
                _animator.SetTrigger("Attack");
                yield return new WaitForSeconds(13.5f);
                StartCoroutine(attack1.BabAttack());
                StartCoroutine(attack2.BabAttack());
                _animator.SetTrigger("Attack");
                yield return new WaitForSeconds(1.5f);
                Disappear();
                yield return new WaitForSeconds(8f);
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
            _movementPattern = _sprite.transform.DOLocalPath(path, 5f, PathType.CatmullRom)
                .SetEase(Ease.Linear)
                .SetLoops(7)
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
    }
}
