using UnityEngine;
using UnityEngine.Events;

namespace PlayerScripts
{
    public class PlayerController : MonoBehaviour
    {
        // We link the PlayerData Scriptable Object to the Player
        [SerializeField] private PlayerData data;
        
        [SerializeField] private LayerMask groundLayer;

        [SerializeField] private PlayerWeapon playerWeapon;

        #region States
        public enum State { Idle, Move, Jump, Fall, Attack, GetHit, Die, }
        
        private State _state = State.Idle;
        private bool _isOnFloor;
        
        #endregion
        
        [Header("CoyoteTime")]
        private float _coyoteCounter;
        
        [Header("JumpBuffering")]
        private float _lastJumpPressedTime;

        [Header("Properties")] 
        private int _hp;
        // private Vector3 _respawnPosition = new Vector3(-7f, -2f, 0f);
        private Vector3 _respawnPosition = new Vector3();
        
        // PA BORRAR DESPUES
        private bool _isImmune;
        
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidbody2D;

        private float _xInput;
        
        public static PlayerController Instance { get; private set; }
        
        [Header("Events")]
        private UnityEvent<int> _onPlayerHit = new UnityEvent<int>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
        
        private void Start()
        {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();

            _hp = data.baseHp;
            
            GameManager.Instance.GetGameRestarted.AddListener(() => Restart());
            _onPlayerHit.AddListener((damage) => GetHit(damage));
        }
        
        private void Update()
        {
            CheckIfGrounded();
            CheckJumpInput();
            CheckMovementInput();
            CheckAttackInput();
            
            UpdateState();
        }

        private void CheckIfGrounded()
        {
            RaycastHit2D ray = Physics2D.BoxCast(
                transform.position, 
                new Vector2(0.2f, 0.1f),
                0f, 
                Vector2.down,
                0.1f, 
                groundLayer);

            if (ray.collider != null) // El raycast toca el suelo
            {
                _isOnFloor = true;
                _coyoteCounter = data.coyoteTime; // Reseteamos el tiempo de coyotetime
                _rigidbody2D.gravityScale = data.regularGravity;
            }
            else // Si el raycast no toca el suelo...
            {
                if (_isOnFloor) // Acabamos de dejar el suelo
                    _coyoteCounter = data.coyoteTime;

                _isOnFloor = false;
                _coyoteCounter -= Time.deltaTime;
            }
        }
        
        private void CheckJumpInput()
        {
            if (Input.GetButtonDown("Jump"))
                _lastJumpPressedTime = Time.time;
            
            // Si el jugador esta en el piso o dentro de la ventana del coyote time -> puede saltar
            if ((_isOnFloor || _coyoteCounter > 0f) && Time.time - _lastJumpPressedTime <= data.jumpBufferTime)
            {
                Jump();
                SetState(State.Jump);
                _lastJumpPressedTime = -Mathf.Infinity;
            }
        }

        private void CheckMovementInput()
        {
            _xInput = Input.GetAxis("Horizontal");
        }

        private void CheckAttackInput()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                playerWeapon.Attack();
                SetState(State.Attack);
            }
        }
        
        private void IdleState()
        {
            if (_xInput != 0) SetState(State.Move);
            // Here we play the Idle animation
        }
        
        private void MoveState()
        {
            CheckIfIsFalling();
            
            if (_xInput == 0) SetState(State.Idle);
            
            Move();
            // Here we play the animations and sounds for when the character moves
        }

        private void Move()
        {
            _rigidbody2D.linearVelocity = new Vector2(_xInput * data.moveSpeed, _rigidbody2D.linearVelocity.y);
        }

        private void JumpState()
        {
            if (_xInput != 0) Move();
            
            CheckIfIsFalling();
        }
        
        private void Jump()
        {
            _isOnFloor = false;
            
            _rigidbody2D.gravityScale = data.regularGravity;
            
            _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, 0f);
            _rigidbody2D.AddForce(Vector2.up * data.jumpForce, ForceMode2D.Impulse);
            // Here we play the animations and sounds for when the character jumps
        }

        private void FallState()
        {
            if (_xInput != 0) Move();
            
            if (_isOnFloor) SetState(State.Idle);
        }

        private void CheckIfIsFalling()
        {
            if (_rigidbody2D.linearVelocityY < 0)
            {
                SetState(State.Fall);
                _rigidbody2D.gravityScale = data.regularGravity * data.fallGravity;
            }
        }
        
        private void AttackState()
        {
            Move();
            if (!playerWeapon.isAttacking) SetState(State.Idle);
        }
        
        // PA BORRAR DESPUES (ALGO)
        private void GetHitState()
        {
            // _rigidbody2D.AddForce(Vector2.up * data.pushForce, ForceMode2D.Impulse);
            _isImmune = true;
            Invoke("StopInmunityTime", 1f);
            SetState(State.Idle);
        }

        private void StopInmunityTime()
        {
            _isImmune = false;
        }

        public void GetHit(int damage)
        {
            if (_isImmune) return;
            
            _hp -= damage;
            
            // Frena el impuslo en Y del player (por si venia de un salto)
            _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, 0f);
            // Hace que el jugador pegue un saltito como feedback a recibir daño
            _rigidbody2D.AddForce(Vector2.up * data.pushForce, ForceMode2D.Impulse);
            
            GameManager.Instance.GetLifeAmountChanged?.Invoke(_hp);

            if (_hp <= 0)
                SetState(State.Die);
            else
                SetState(State.GetHit);
        }
        
        private void DieState()
        {
            GameManager.Instance.GetGameOverEvent?.Invoke();
        }

        private void Restart()
        {
            transform.position = _respawnPosition;
            _hp = data.baseHp;
            _rigidbody2D.linearVelocity = new Vector2(0, 0);
            SetState(State.Idle);
        }
        
        private void UpdateState()
        {
            switch (_state)
            {
                case State.Idle: IdleState(); break;
                case State.Move: MoveState(); break;
                case State.Jump: JumpState(); break;
                case State.Fall: FallState(); break;
                case State.Attack: AttackState(); break;
                case State.GetHit: GetHitState(); break;
                case State.Die: DieState(); break;
            }
        }

        private void SetState(State newState)
        {
            _state = newState;
        }

        // PA CAMBIAR DESPUES ?
        public void SetRespawnPosition(Vector3 newPosition)
        {
            _respawnPosition = newPosition;
        }
        
        public State GetState => _state;

        public float GetMovementInput => _xInput;

        #region GetEvents

        public UnityEvent<int> GetPlayerHitEvent => _onPlayerHit;

        #endregion
        
        
        // PA BORRAR DESPUES
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                transform.SetParent(collision.transform);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                transform.SetParent(null);
            }
        }
    }
}
