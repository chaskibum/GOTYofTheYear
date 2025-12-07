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
        private bool _coroutineStarted = false;
        [SerializeField] private GameObject _vieja;
        [SerializeField] private Collider2D hitbox;
        
        
        private PlayerController _player;
        private Vector3 _playerPos;
        private bool _playerToTheRight;

        [Header("Wandering Paths")]
        private Vector3[] _verticalPath;
        private Vector3[] _horizontalPath;

        [Header("Attacks")]
        [SerializeField] private FinalBossBabita attack1;
        [SerializeField] private FinalBossBabita attack2;

        private void Start()
        {
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
            
            _player = GameManager.Instance.GetPlayer;
            
            CreatePaths();
            MoveAround();

            // StartCoroutine(BavitAttack());
        }

        /*private void Update()
        {
            GetPlayerPosition();
            CalculateDirection();
            
            transform.localPosition = _playerPos;
        }*/

        private void Update()
        {
            if (!_vieja.activeInHierarchy && !_coroutineStarted)
            {
                StartCoroutine(BavitAttack());
                _coroutineStarted = true;
            }
        }

        private IEnumerator BavitAttack()
        {
            while (true)
            {
                print("starting...");
                yield return new WaitForSeconds(2f);
                StartCoroutine(attack1.BabAttack());
                yield return new WaitForSeconds(10f);
                StartCoroutine(attack2.BabAttack());
                yield return new WaitForSeconds(15f);
                StartCoroutine(attack1.BabAttack());
                StartCoroutine(attack2.BabAttack());
                yield return new WaitForSeconds(10f);
                print("attack finished!");
            }
        }

        private void CreatePaths()
        {
            _verticalPath = new Vector3[]
            {
                transform.position + new Vector3(0, 0),
                transform.position + new Vector3(-2, -2),
                transform.position + new Vector3(0, -4),
                transform.position + new Vector3(-2, -6),
                transform.position + new Vector3(0, -8),
                transform.position + new Vector3(2, -6),
                transform.position + new Vector3(0, -4),
                transform.position + new Vector3(2, -2),
                transform.position + new Vector3(0, 0),
            };
            
            _horizontalPath = new Vector3[]
            {
                transform.localPosition + new Vector3(0, 0),
                transform.localPosition + new Vector3(2, -2),
                transform.localPosition + new Vector3(4, 0),
                transform.localPosition + new Vector3(6, -2),
                transform.localPosition + new Vector3(8, 0),
                transform.localPosition + new Vector3(6, 2),
                transform.localPosition + new Vector3(4, 0),
                transform.localPosition + new Vector3(2, 2),
                transform.localPosition + new Vector3(0, 0),
            };
        }

        private void MoveAround()
        {
            // _sprite.flipX = !_playerToTheRight;

            // Mathf.Randint(0, 2);
            transform.DOPath(_horizontalPath, 4f, PathType.CatmullRom)
                .SetEase(Ease.Linear)
                .SetLoops(10);
        }
        
        private void CalculateDirection()
        {
            _playerToTheRight = _playerPos.x > transform.position.x;
        }
        
        private void GetPlayerPosition()
        {
            _playerPos = _player.GetPlayerTarget;
        }
    }
}
