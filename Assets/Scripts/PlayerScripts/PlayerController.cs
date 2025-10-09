using UnityEngine;
using UnityEngine.Events;

namespace PlayerScripts
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly int Moving = Animator.StringToHash("Moving");

        // We link the PlayerData Scriptable Object to the Player
        [SerializeField] private PlayerData data;
        
        [SerializeField] private LayerMask groundLayer;

        [SerializeField] private GameObject playerWeapon;
        
        [SerializeField] private Transform playerTarget;
        [SerializeField] private PlayerVisuals visuals;
        [SerializeField] private GameObject shieldVisuals;
        
        private Animator _animator;
        private PlayerHealth _playerHealth;

        #region States
        public enum State { Idle, Move, Jump, Fall, Attack, GetHit, Die, Possessed, Dash, }
        
        private State _state = State.Idle;
        private bool _isOnFloor;
        
        #endregion
        
        [Header("Dash")]
        [SerializeField] private bool _dashUnlocked;
        private bool _canDash;
        
        [Header("Jump")]
        [SerializeField] private bool _doubleJumpUnlocked;
        private float _coyoteCounter;
        private float _lastJumpPressedTime;
        private float _jumpKeyTimePressed;
        private bool _canDoubleJump;
        
        [Header("Immunity Skill")]
        [SerializeField] private bool _immuneSkillUnlocked;
        private float _cooldown = 0f;
        

        [Header("Properties")] 
        private int _hp;
        private Vector3 _respawnPosition;
        
        // PA BORRAR DESPUÉS
        private bool _isImmune;
        private bool _isAttacking;
        
        private Rigidbody2D _body;

        private float _xInput;

        private bool _isPossessed;
        private int _inputCount;
        private float _randomPossessedDirection;
        
        [Header("Events")]
        private readonly UnityEvent<int> _onPlayerHit = new UnityEvent<int>();
        private readonly UnityEvent _onPlayerPossessed = new UnityEvent();
        private readonly UnityEvent _onPlayerExorcised = new UnityEvent();
        
        private void Start()
        {
            _body = gameObject.GetComponent<Rigidbody2D>();
            _animator = gameObject.GetComponent<Animator>();
            _playerHealth = GetComponent<PlayerHealth>();

            _hp = data.baseHp;
            
            GameManager.Instance.GetGameRestarted.AddListener(Restart);
            // GameManager.Instance.GetPlayerRespawn.AddListener(Respawn);
            _onPlayerHit.AddListener(GetHit);
            _onPlayerPossessed.AddListener(PlayerPossessed);
            
            transform.position = new Vector3(PlayerPrefs.GetFloat("XPosition"), PlayerPrefs.GetFloat("YPosition"), 0f);
        }

        private void PlayerPossessed()
        {
            _randomPossessedDirection = GetRandomDirection();
            _isPossessed = true;
            SetState(State.Possessed);
        }
        
        private void PossessedState()
        {
            _body.linearVelocity = new Vector2(_randomPossessedDirection * data.moveSpeed, _body.linearVelocity.y);
            if (Input.GetKeyDown(KeyCode.W)) _inputCount += 1;
            if (Input.GetKeyDown(KeyCode.A)) _inputCount += 1;
            if (Input.GetKeyDown(KeyCode.S)) _inputCount += 1;
            if (Input.GetKeyDown(KeyCode.D)) _inputCount += 1;
            if (Input.GetKeyDown(KeyCode.Space)) _inputCount += 1;
            if (Input.GetKeyDown(KeyCode.H)) _inputCount += 1;
            //print(_inputCount);
            if (_inputCount >= 30) Exorcised();
        }

        private float GetRandomDirection()
        {
            return Random.value < 0.5f ? -1f : 1f;
        }

        private void Exorcised()
        {
            _onPlayerExorcised.Invoke();
            _inputCount = 0;
            _isPossessed = false;
            SetState(State.Idle);
        }
        
        private void Update()
        {
            if (GameManager.Instance.GetGameOver || Time.timeScale == 0.2f) return;
            
            CheckIfGrounded();
            CheckJumpInput();
            _cooldown -= Time.deltaTime;
            CheckImmunitySkillInput();
            CheckDashInput();
            CheckMovementInput();
            CheckAttackInput();
            
            UpdateState();
            UpdatePhysicsState();
        }

        private void CheckIfGrounded()
        {
            RaycastHit2D ray = Physics2D.Raycast(
                _body.transform.position, 
                Vector2.down, 
                0.1f, 
                groundLayer);
            

            if (ray.collider) // El raycast toca el suelo
            {
                Debug.DrawRay(_body.transform.position, Vector2.down * 0.1f, Color.red);
                _isOnFloor = true;
                _coyoteCounter = data.coyoteTime; // Reseteamos el tiempo de coyote
                _body.gravityScale = data.regularGravity;
            }
            else // Si el raycast no toca el suelo...
            {
                if (_isOnFloor) // Acabamos de dejar el suelo
                    _coyoteCounter = data.coyoteTime;

                _isOnFloor = false;
                _coyoteCounter -= Time.deltaTime;
                Debug.DrawRay(_body.transform.position, Vector2.down * 0.1f, Color.green);
            }
        }
        
        private void CheckJumpInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _lastJumpPressedTime = Time.time;
            
            // Si el jugador está en el piso o dentro de la ventana del coyote time -> puede saltar
            if ((_isOnFloor || _coyoteCounter > 0f) && Time.time - _lastJumpPressedTime <= data.jumpBufferTime)
            {
                Jump();
                _lastJumpPressedTime = -Mathf.Infinity;
                if (_doubleJumpUnlocked) _canDoubleJump = true;
            }
            // Sino, si el jugador esta en el aire y puede hacer doble salto -> saltar en el aire
            else if ((!_isOnFloor && _canDoubleJump) && Time.time - _lastJumpPressedTime <= data.jumpBufferTime)
            {
                Jump();
                _canDoubleJump = false;
                _lastJumpPressedTime = -Mathf.Infinity;
            }
        }

        private void CheckMovementInput()
        {
            _xInput = Input.GetAxis("Horizontal");
        }

        private void CheckAttackInput()
        {
            if (Input.GetKeyDown(KeyCode.H) && !_isPossessed)
            {
                Attack();
                SetState(State.Attack);
            }
        }

        private void CheckDashInput()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Dash();
            }
        }


        private void Dash()
        {
            if (!_canDash) return;
            _body.linearVelocity = Vector2.zero;
            float direction = visuals.GetPlayerVisuals.flipX ? -1 : 1;
            _body.AddForce(new Vector2(direction * data.dashSpeed, 0f), ForceMode2D.Impulse);
            _canDash = false;
            SetState(State.Dash);
        }

        private void DashState()
        {
            _body.gravityScale = data.dashGravity;
            Invoke(nameof(EndDash), data.dashDuration);
        }

        private void EndDash()
        {
            _body.linearVelocity = new Vector2(0f, _body.linearVelocity.y);
            _body.gravityScale = data.regularGravity;
            SetState(_isOnFloor ? State.Idle : State.Jump);
        }
        
        private void CheckImmunitySkillInput()
        {
            if (!_immuneSkillUnlocked) return;
            if (Input.GetKeyDown(KeyCode.J))
            {
                ActivateImmunity();
            }
        }

        private void ActivateImmunity()
        {
            if (_cooldown > 0f) return;
            _isImmune = true;
            shieldVisuals.gameObject.SetActive(true);
            Invoke(nameof(EndImmunityTime), data.shieldDuration);
            _cooldown = data.shieldCooldown;
        }
        
        private void Attack()
        {
            playerWeapon.SetActive(true);
            if (_isAttacking) return;
            _isAttacking = true;
            Invoke(nameof(EndAttack), data.attackSpeed);
            AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerMissedAttack, true);
        }
        
        private void EndAttack()
        {
            _isAttacking = false;
            playerWeapon.SetActive(false);
        }
        
        private void IdleState()
        {
            if (_doubleJumpUnlocked) _canDoubleJump = true;
            if (_dashUnlocked) _canDash = true;
            if (_xInput != 0) SetState(State.Move);
            else _body.linearVelocity = new Vector2(0, _body.linearVelocity.y);
            // Here we play the Idle animation
            // PA BORRAR DESPUÉS
            _animator.SetBool(Moving, false);
        }
        
        private void MoveState()
        {
            CheckIfIsFalling();
            
            if (_xInput == 0) SetState(State.Idle);
            
            Move();
            // PA BORRAR DESPUÉS
            _animator.SetBool(Moving, true);
        }

        private void Move()
        {
            _body.linearVelocity = new Vector2(_xInput * data.moveSpeed, _body.linearVelocity.y);
        }

        private void JumpState()
        {
            if (_xInput != 0) Move();

            _jumpKeyTimePressed += Time.deltaTime;
            
            if (Input.GetKeyUp(KeyCode.Space) || _jumpKeyTimePressed > data.jumpDuration)
                SetState(State.Fall);
            
            CheckIfIsFalling();
        }

        private void Jump()
        {
            if (_state == State.Possessed) return;
            
            _isOnFloor = false;
            SetState(State.Jump);
            
            _body.gravityScale = data.regularGravity;
            _body.linearVelocity = new Vector2(_body.linearVelocity.x, 0f);
            
            _body.AddForceY(data.jumpForce, ForceMode2D.Impulse);
            
            AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerJump, true);
        }

        private void FallState()
        {
            if (_xInput != 0) Move();
            
            if (_body.linearVelocityY > 1) _body.linearVelocityY = 1;
            _body.gravityScale = data.regularGravity * data.fallGravity;
            _jumpKeyTimePressed = 0f;
            
            if (_isOnFloor) SetState(State.Idle);
        }

        private void CheckIfIsFalling()
        {
            if (_body.linearVelocityY < 0)
            {
                SetState(State.Fall);
            }
        }
        
        private void AttackState()
        {
            Move();
            if (!_isAttacking) SetState(State.Idle);
        }
        
        // PARA BORRAR DESPUÉS (ALGO)
        private void GetHitState()
        {
            // _rigidbody2D.AddForce(Vector2.up * data.pushForce, ForceMode2D.Impulse);
            _isImmune = true;
            Invoke(nameof(EndImmunityTime), data.immunityTime);
            SetState(State.Idle);
        }

        private void EndImmunityTime()
        {
            _isImmune = false;
            shieldVisuals.gameObject.SetActive(false);
        }

        private void GetHit(int damage)
        {
            if (_isImmune) return;
            
            _hp -= damage;
            
            // Frena el impulso en Y del player (por si venia de un salto)
            _body.linearVelocity = new Vector2(_body.linearVelocity.x, 0f);
            // Hace que el jugador pegue un saltito como feedback a recibir daño
            _body.AddForce(Vector2.up * data.pushForce, ForceMode2D.Impulse);
            
            GameManager.Instance.GetHpAmountChanged?.Invoke(_hp);

            if (_hp <= 0)
            {
                SetState(State.Die);
                AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerDeath);
                if (!GameManager.Instance.GetGameOver) Invoke(nameof(Respawn), data.deathAnimationTime);
            }
            else
            {
                SetState(State.GetHit);
                AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerHit, true);
            }
        }
        
        private void DieState()
        {
            _isImmune = true;
            Time.timeScale = 0.2f;
        }

        private void Respawn()
        {
            GameManager.Instance.GetPlayerRespawn.Invoke();
            transform.position = _respawnPosition;
            _hp = data.baseHp;
            _body.linearVelocity = new Vector2(0, 0);
            Time.timeScale = 1f;
            _isImmune = false;
            _isPossessed = false;
            _inputCount = 0;
            _cooldown = 0f;
            SetState(State.Idle);
        }

        private void Restart()
        {
            SetRespawnPosition(Vector3.zero);
            transform.position = _respawnPosition;
            _hp = data.baseHp;
            _body.linearVelocity = new Vector2(0, 0);
            EndImmunityTime();
            _cooldown = 0f;
            CancelInvoke();
            SetState(State.Idle);
        }
        
        private void UpdateState()
        {
            switch (_state)
            {
                case State.Idle: IdleState(); break;
                case State.Attack: AttackState(); break;
                case State.Die: DieState(); break;
                case State.Possessed: PossessedState(); break;
            }
        }

        private void UpdatePhysicsState()
        {
            switch (_state)
            {
                case State.Move: MoveState(); break;
                case State.Jump: JumpState(); break;
                case State.Fall: FallState(); break;
                case State.GetHit: GetHitState(); break;
                case State.Dash: DashState(); break;
            }
        }

        private void SetState(State newState)
        {
            _state = newState;
            // PA BORRAR DESPUÉS
            if (newState != State.Move) _animator.SetBool(Moving, false);
        }
        
        public void SetRespawnPosition(Vector3 newPosition)
        {
            _respawnPosition = newPosition;
            SavePlayerStats();
        }
        
        public void SetPlayerMaterial(PhysicsMaterial2D material)
        {
            _body.sharedMaterial = material;
        }
        
        public State GetState => _state;

        public float GetMovementInput => _xInput;
        
        public Vector3 GetPlayerPosition => transform.position;
        
        public Vector3 GetPlayerTarget => playerTarget.position;
        
        public int GetPlayerLives => PlayerPrefs.GetInt("Lives");
        
        public float GetCooldown => _cooldown;

        public PlayerData GetPlayerData => data;


        #region PlayerPrefs

        public void SavePlayerStats()
        {
            PlayerPrefs.SetFloat("XPosition", _respawnPosition.x);
            PlayerPrefs.SetFloat("YPosition", _respawnPosition.y);
            
            PlayerPrefs.SetInt("Lives", _playerHealth.GetPlayerLives);
            PlayerPrefs.SetInt("BaseLives", _playerHealth.GetPlayerBaseLives);
        }

        #endregion

        #region GetEvents

        public UnityEvent<int> GetPlayerHitEvent => _onPlayerHit;

        public UnityEvent GetPlayerPossessedEvent => _onPlayerPossessed;
        
        public UnityEvent GetPlayerExorcisedEvent => _onPlayerExorcised;

        #endregion


    }
}
