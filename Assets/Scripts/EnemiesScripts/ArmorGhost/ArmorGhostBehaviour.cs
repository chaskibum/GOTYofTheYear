using UnityEngine;

namespace EnemiesScripts.ArmorGhost
{
    public class ArmorGhostBehaviour : MonoBehaviour
    {
        [SerializeField] private EnemyData data;

        [SerializeField] private GhostAttackZone attackScript;
        
        public enum State { Idle, Chase, Attack, GetHit, Die, }
        
        private State _state = State.Idle;
        
        [Header("Properties")] 
        private int _hp;
        private Vector3 _startingPosition;
        public bool facingRight = true;
        
        private Rigidbody2D _body;
        
        private Vector3 _playerPosition;

        private void Awake()
        {
            _startingPosition = transform.position;
        }

        private void Start()
        {
            _hp = data.baseHp;
            _startingPosition = transform.position;
            
            GameManager.Instance.GetGameRestarted.AddListener(() => Reset());
            _body = GetComponent<Rigidbody2D>();
        }

        private void Reset()
        {
            _state = State.Idle;
            transform.position = _startingPosition;
            _hp = data.baseHp;
        }

        private void Update()
        {
            _playerPosition = GameManager.Instance.GetPlayer.GetPlayerPosition;
            UpdateState();
        }

        private void IdleState()
        {
            if (Vector3.Distance(_body.position, _playerPosition) < 12f) SetState(State.Chase);
        }
        
        private void ChaseState()
        {
            Vector3 newPosition = Vector3.MoveTowards(
                _body.position,
                _playerPosition,
                data.moveSpeed * Time.deltaTime);
            
            _body.MovePosition(newPosition);
            
            if (Vector3.Distance(_body.position, _playerPosition) >= 12f) SetState(State.Idle);
            if (Vector3.Distance(_body.position, _playerPosition) < 1.5f) SetState(State.Attack);
        }
        
        private void AttackState()
        {
            if (attackScript.isAttacking) return;
            
            attackScript.Attack();
        }
        
        private void GetHitState()
        {
            
        }
        
        private void DieState()
        {
            
        }
        
        private void UpdateState()
        {
            switch (_state)
            {
                case State.Idle: IdleState(); break;
                case State.Chase: ChaseState(); break;
                case State.Attack: AttackState(); break;
                case State.GetHit: GetHitState(); break;
                case State.Die: DieState(); break;
            }
        }

        public void SetState(State newState)
        {
            _state = newState;
        }
    }
}
