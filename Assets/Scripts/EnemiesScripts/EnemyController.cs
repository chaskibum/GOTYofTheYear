using System.Collections;
using PlayerScripts;
using TMPro;
using UnityEngine;

namespace EnemiesScripts
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyData data;
        
        [SerializeField] private GameObject attackHitbox;
        
        [SerializeField] private TMP_Text stateText;
        
        public enum State { Idle, Chase, Attack, GetHit, Die, }
        
        private State _state = State.Idle;
        
        [Header("Properties")]
        private Vector3 _startingPosition;
        private Vector2 _direction;
        private float _attackTime;
        private bool _canAttack = true;
        
        private Rigidbody2D _body;
        private SpriteRenderer _visuals;
        
        private PlayerController _player;
        private Vector3 _playerPosition;
        private bool _playerToTheRight;
        private ApplyMovement _moveScript;

        private void Awake()
        {
            _startingPosition = transform.position;
            _attackTime = data.attackAnimationTime;
        }

        private void Start()
        {
            _startingPosition = transform.position;
            
            GameManager.Instance.GetGameRestarted.AddListener(Reset);
            _body = GetComponent<Rigidbody2D>();
            _visuals = GetComponentInChildren<SpriteRenderer>();
            TryGetComponent<ApplyMovement>(out _moveScript);
            
            _player = GameManager.Instance.GetPlayer;
        }

        private void Reset()
        {
            SetState(State.Idle);
            transform.position = _startingPosition;
            _canAttack = true;
            attackHitbox.SetActive(true);
            
            // PARA BORRAR DESPUÉS (se va a hacer mediante animaciones)
            var color = _visuals.color;
            color.a = 1;
            _visuals.color = color;
        }

        private void Update()
        {
            CalculateDirection();
            UpdateState();
        }

        // Calculamos la posicion del player y con ello la dirección hacia donde tendríamos que estar mirando.
        private void CalculateDirection()
        {
            _playerPosition = _player.GetPlayerPosition;
            _playerToTheRight = _playerPosition.x > transform.position.x;
            _direction = _playerToTheRight ? Vector2.left : Vector2.right;
        }

        private void IdleState()
        {
            if (!_moveScript) _visuals.flipX = _playerToTheRight;
            if (Vector3.Distance(_body.position, _playerPosition) < data.detectionRange) SetState(State.Chase);
        }
        
        private void ChaseState()
        {
            // Resetea la fuerza del impulso de GetHit
            _body.linearVelocity = Vector2.zero;
            // Desactiva la hitbox si el personaje persigue
            attackHitbox.SetActive(false);

            // Si no está TAN cerca del player se acerca lentamente
            if (Vector3.Distance(_body.position, _playerPosition) >= 1f)
            {
                _visuals.flipX = _playerToTheRight;
                
                Vector3 newPosition = Vector3.MoveTowards(
                    _body.position,
                    _playerPosition,
                    data.moveSpeed * Time.deltaTime);
            
                _body.MovePosition(newPosition);
            }
            // Si está muy cerca del player se aleja rápidamente
            else
            {
                _body.AddForce(_direction * 4f, ForceMode2D.Impulse);
            }
            
            if (Vector3.Distance(_body.position, _playerPosition) >= data.detectionRange) SetState(State.Idle);
            if (Vector3.Distance(_body.position, _playerPosition) < data.attackRange && _canAttack) SetState(State.Attack);
        }
        
        private void AttackState()
        {
            // Si el tiempo de la animación de ataque termino, desactivamos la hitbox
            _attackTime -= Time.deltaTime;
            if (_attackTime <= 0f)
            {
                attackHitbox.SetActive(false);
                SetState(State.Idle);
                _attackTime = data.attackAnimationTime;
            }
            // Si no la activamos y hacemos que no se pueda volver a atacar hasta en cierto tiempo
            else
            {
                attackHitbox.SetActive(true);
                _canAttack = false;
                StartCoroutine(nameof(CanAttackAgain));
            }
        }

        private IEnumerator CanAttackAgain()
        {
            yield return new WaitForSeconds(data.attackCooldown);
            _canAttack = true;
        }

        public void GetHit()
        {
            _body.AddForce(_direction * 6f, ForceMode2D.Impulse);
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
            attackHitbox.SetActive(false);
            _body.AddForce(Vector2.up * 0.1f, ForceMode2D.Impulse);
            StartCoroutine(nameof(Disappear));
        }

        // PARA BORRAR DESPUÉS (cuando tengamos la animación de muerte)
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
                transform.gameObject.SetActive(false);
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

        public bool GetCanAttack => _canAttack;
    }
}
