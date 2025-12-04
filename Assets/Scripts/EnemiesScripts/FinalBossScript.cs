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
        [SerializeField] private Collider2D hitbox;
        
        
        private PlayerController _player;
        private Vector3 _playerPos;
        private bool _playerToTheRight;

        [Header("Wandering Paths")]
        private Vector3[] _verticalPath;
        private Vector3[] _horizontalPath;


        private void Start()
        {
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
            
            _player = GameManager.Instance.GetPlayer;
            
            
            CreatePaths();
            MoveAround();
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
                transform.position + new Vector3(0, 0),
                transform.position + new Vector3(2, -2),
                transform.position + new Vector3(4, 0),
                transform.position + new Vector3(6, -2),
                transform.position + new Vector3(8, 0),
                transform.position + new Vector3(6, 2),
                transform.position + new Vector3(4, 0),
                transform.position + new Vector3(2, 2),
                transform.position + new Vector3(0, 0),
            };
        }

        private void MoveAround()
        {
            GetPlayerPosition();
            CalculateDirection();
            _sprite.flipX = !_playerToTheRight;
            
            transform.DOPath(_horizontalPath, 4f, PathType.CatmullRom)
                .SetEase(Ease.Linear)
                .SetLoops(15);
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
