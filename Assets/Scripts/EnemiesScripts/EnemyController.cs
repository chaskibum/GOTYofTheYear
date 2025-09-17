using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EnemiesScripts
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyData data;

        // [SerializeField] private GhostAttackZone attackScript;
        [SerializeField] private GameObject attackZone;
        
        [SerializeField] private TMP_Text stateText;
        
        public enum State { Idle, Chase, Attack, GetHit, Die, }
        
        private State _state = State.Idle;
        
        [Header("Properties")]
        private int _hp;
        private Vector3 _startingPosition;
        private float _attackTime;
        private bool _canAttack = true;
        // public bool facingRight = true;
        
        private Rigidbody2D _body;
        private SpriteRenderer _visuals;
        
        private Vector3 _playerPosition;

        private void Awake()
        {
            _startingPosition = transform.position;
            _attackTime = data.attackAnimationTime;
        }

        private void Start()
        {
            _hp = data.baseHp;
            _startingPosition = transform.position;
            
            GameManager.Instance.GetGameRestarted.AddListener(() => Reset());
            _body = GetComponent<Rigidbody2D>();
            _visuals = GetComponentInChildren<SpriteRenderer>();
        }

        private void Reset()
        {
            _state = State.Idle;
            transform.position = _startingPosition;
            _hp = data.baseHp;
            attackZone.SetActive(false);
            _canAttack = true;
            
            // PA BORRAR DESPUES
            var color = _visuals.color;
            color.a = 1;
            _visuals.color = color;
        }

        private void Update()
        {
            _playerPosition = GameManager.Instance.GetPlayer.GetPlayerPosition;
            UpdateState();
        }

        private void IdleState()
        {
            if (Vector3.Distance(_body.position, _playerPosition) < data.detectionRange) SetState(State.Chase);
        }
        
        private void ChaseState()
        {
            // Resetea la fuerza del impulso de GetHit
            _body.linearVelocity = Vector2.zero;
            
            Vector3 newPosition = Vector3.MoveTowards(
                _body.position,
                _playerPosition,
                data.moveSpeed * Time.deltaTime);
            
            _body.MovePosition(newPosition);
            
            if (Vector3.Distance(_body.position, _playerPosition) >= data.detectionRange) SetState(State.Idle);
            if (Vector3.Distance(_body.position, _playerPosition) < data.attackRange && _canAttack) SetState(State.Attack);
        }
        
        private void AttackState()
        {
            _attackTime -= Time.deltaTime;
            if (_attackTime <= 0f)
            {
                attackZone.SetActive(false);
                SetState(State.Idle);
                _attackTime = data.attackAnimationTime;
            }
            else
            {
                attackZone.SetActive(true);
                _canAttack = false;
                StartCoroutine("CanAttackAgain");
            }
        }

        private IEnumerator CanAttackAgain()
        {
            yield return new WaitForSeconds(data.attackCooldown);
            _canAttack = true;
        }

        public void GetHit()
        {
            _body.AddForce(Vector2.left * 6f, ForceMode2D.Impulse);
            SetState(State.GetHit);
        }
        
        private void GetHitState()
        {
            StartCoroutine(DelayChaseState());
        }

        private IEnumerator DelayChaseState()
        {
            yield return new WaitForSeconds(0.2f);
            SetState(State.Chase);
        }
        
        private void DieState()
        {
            _body.AddForce(Vector2.up * 0.1f, ForceMode2D.Impulse);
            StartCoroutine("Disappear");
        }

        // PA BORRAR DESPUES
        private IEnumerator Disappear()
        {
            if (_visuals.color.a > 0)
            {
                var color = _visuals.color;
                color.a = color.a - 0.01f;
                _visuals.color = color;
                yield return new WaitForSeconds(0.05f);
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
                
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
            if (newState == _state) return;
            _state = newState;
            stateText.text = _state.ToString();
        }
        
        public State GetState => _state;
    }
}
