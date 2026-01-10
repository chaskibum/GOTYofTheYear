using System.Collections;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace PlayerScripts
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly int Moving = Animator.StringToHash("Moving");
        private static readonly int Attacking = Animator.StringToHash("Attacking");
        private static readonly int Jumping = Animator.StringToHash("Jumping");
        private static readonly int Dashing = Animator.StringToHash("Dashing");
        private static readonly int Possessed = Animator.StringToHash("Possessed");
        private static readonly int GotHit = Animator.StringToHash("Get Hit");
        private static readonly int DoubleJumping = Animator.StringToHash("DoubleJumping");
        private static readonly int Falling = Animator.StringToHash("Falling");

        [SerializeField] private PlayerData data;
        
        [SerializeField] private LayerMask groundLayer;

        [SerializeField] private GameObject playerWeapon;
        
        [SerializeField] private Transform playerTarget;
        [SerializeField] private PlayerVisuals visuals;
        [SerializeField] private GameObject shieldVisuals;
        [SerializeField] private ParticleSystem jumpParticles;
        
        private Animator _animator;
        private PlayerHealth _playerHealth;

        #region States
        public enum State { Idle, Move, Jump, Fall, Attack, GetHit, Die, Possessed, Dash }
        
        private State _state = State.Idle;
        private bool _isOnFloor;
        
        #endregion
        
        [Header("Dash")]
        [SerializeField] private bool _dashUnlocked;
        private bool _canDash;
        private float _dashTimer;
        
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
        private Vector3 _mainRespawnPosition;
        private Vector3 _endingPosition;
        private bool _canChangeState = true;
        public bool inDialog = false;
        private bool _isImmune;
        private bool _isAttacking;
        private float _attackCooldown;
        private PlayerWeapon _weapon;
        
        private Rigidbody2D _body;

        private float _xInput;

        private bool _isPossessed;
        private int _inputCount;
        private float _randomPossessedDirection;
        private Camera _cam;
        
        [Header("Events")]
        private readonly UnityEvent<int> _onPlayerHit = new UnityEvent<int>();
        private readonly UnityEvent _onPlayerPossessed = new UnityEvent();
        private readonly UnityEvent _onPlayerExorcised = new UnityEvent();
        private readonly UnityEvent _onPlayerRevived = new UnityEvent();
        private readonly UnityEvent _onPlayerImmunity = new UnityEvent();
        
        private void Start()
        {
            _body = gameObject.GetComponent<Rigidbody2D>();
            _animator = gameObject.GetComponent<Animator>();
            _playerHealth = GetComponent<PlayerHealth>();
            _weapon = playerWeapon.GetComponent<PlayerWeapon>();
            _cam = Camera.main;

            _endingPosition = new Vector3(545, -74.5f, 0);
            
            _hp = data.baseHp;
            _attackCooldown = data.attackCooldown;
            data.immunityTime = data.baseImmunityTime;
            
            AddListeners(true);
            
            // Skills
            if (PlayerPrefs.GetString("DashUnlocked") == "Dash") UnlockDash();
            if (PlayerPrefs.GetString("DoubleJumpUnlocked") == "DoubleJump") UnlockDoubleJump();
            if (PlayerPrefs.GetString("ImmunityUnlocked") == "Immunity") UnlockImmunity();

            HandleRespawnOnStart();
        }

        private void HandleRespawnOnStart()
        {
            if (PlayerPrefs.GetFloat("XPosition") == 0)
            {
                GameManager.Instance.RevivePlayer();
            }
            else
            {
                _respawnPosition = new Vector3(PlayerPrefs.GetFloat("XPosition"), PlayerPrefs.GetFloat("YPosition"), 0f);
                _mainRespawnPosition = new Vector3(PlayerPrefs.GetFloat("MainXPosition"), PlayerPrefs.GetFloat("MainYPosition"), 0f);
                transform.position = _respawnPosition;
            }
        }

        private void OnDestroy()
        {
            AddListeners(false);
        }
        
        private void AddListeners(bool add)
        {
            var player = GameManager.Instance.GetPlayer;

            if (add)
            {
                GameManager.Instance.GetGameRestarted.AddListener(RestartGame);
                GameManager.Instance.GetDashUnlocked.AddListener(UnlockDash);
                GameManager.Instance.GetDoubleJumpUnlocked.AddListener(UnlockDoubleJump);
                GameManager.Instance.GetImmunityUnlocked.AddListener(UnlockImmunity);
                GameManager.Instance.GetGameWonEvent.AddListener(GameWon);
                _onPlayerHit.AddListener(GetHit);
                _onPlayerPossessed.AddListener(PlayerPossessed);
                _onPlayerRevived.AddListener(Revive);
            }
            else
            {
                GameManager.Instance.GetGameRestarted.RemoveListener(RestartGame);
                GameManager.Instance.GetDashUnlocked.RemoveListener(UnlockDash);
                GameManager.Instance.GetDoubleJumpUnlocked.RemoveListener(UnlockDoubleJump);
                GameManager.Instance.GetImmunityUnlocked.RemoveListener(UnlockImmunity);
                GameManager.Instance.GetGameWonEvent.RemoveListener(GameWon);
                _onPlayerHit.RemoveListener(GetHit);
                _onPlayerPossessed.RemoveListener(PlayerPossessed);
                _onPlayerRevived.RemoveListener(Revive);
            }
        }

        private void Update()
        {
            _cooldown -= Time.deltaTime;
            _attackCooldown -= Time.deltaTime;
            _dashTimer -= Time.deltaTime;
            CheckIfGrounded();
            
            if (GameManager.Instance.GetGameOver || Time.timeScale == 0.2f || !_canChangeState || GameManager.Instance.gameWon) return;
            if (inDialog)
            {
                _animator.SetBool(Moving, false);
                _animator.SetBool(Jumping, false);
                _animator.SetBool(DoubleJumping, false);
                _animator.SetBool(Falling, false);
                AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName("PlayerFall")) {
                    _animator.SetTrigger("TouchedFloor");
                }
                _body.linearVelocity = new Vector2(0, _body.linearVelocity.y);
                return;
            }
            
            CheckJumpInput();
            CheckImmunitySkillInput();
            CheckDashInput();
            CheckMovementInput();
            CheckAttackInput();
            
            UpdateState();
            UpdatePhysicsState();
        }

        
        #region States
        
        private void IdleState()
        {
            _animator.SetBool(Dashing, false);
            _animator.SetBool(GotHit, false);
            _body.gravityScale = data.regularGravity;
            if (_xInput != 0) SetState(State.Move);
            else _body.linearVelocity = new Vector2(0, _body.linearVelocity.y);
            
            _animator.SetBool(Moving, false);
            _animator.SetBool(Jumping, false);
            _animator.SetBool(DoubleJumping, false);
            _animator.SetBool(Falling, false);
        }
        
        private void MoveState()
        {
            CheckIfIsFalling();
            
            if (_xInput == 0) SetState(State.Idle);
            
            Move();
            
            if (_isOnFloor) _animator.SetBool(Moving, true);
        }
        
        
        private void JumpState()
        {
            if (_xInput != 0) Move();

            _jumpKeyTimePressed += Time.deltaTime;

            if (JumpReleased() || _jumpKeyTimePressed > data.jumpDuration)
            {
                SetState(State.Fall);
            }
            
            CheckIfIsFalling();
        }
        
        private void FallState()
        {
            if (_xInput != 0) Move();
            
            if (_body.linearVelocityY > 1) _body.linearVelocityY = 1;
            _body.gravityScale = data.regularGravity * data.fallGravity;
            _jumpKeyTimePressed = 0f;
            _animator.SetBool(Jumping, false);
            _animator.SetBool(Falling, true);
            _animator.SetBool(Dashing, false);
            _animator.SetBool(DoubleJumping, false);

            if (_isOnFloor)
            {
                _animator.SetBool(Falling, false);
                _animator.SetTrigger("TouchedFloor");
                SetState(State.Idle);
            }
        }
        
        private void AttackState()
        {
            _jumpKeyTimePressed = 0f;
            Move();
            if (!_isAttacking)
            {
                SetState(State.Idle);
            }
        }
        
        private void DashState()
        {
            _body.gravityScale = data.dashGravity;
            // Invoke(nameof(EndDash), data.dashDuration);
        }
        
        private void GetHitState()
        {
            if (!_isPossessed) StartCoroutine(LockStateForSeconds(data.deathAnimationTime));
            _isImmune = true;
            CancelInvoke(nameof(EndDash));
            Invoke(nameof(EndImmunityTime), data.immunityTime);
            // _animator.SetBool(GotHit, false);
            SetState(State.Idle);
        }
        
        private void PossessedState()
        {
            _body.linearVelocity = new Vector2(_randomPossessedDirection * data.moveSpeed, _body.linearVelocity.y);

            if (Input.GetButtonDown("Jump")) _inputCount += 1;
            if (Input.GetButtonDown("Horizontal")) _inputCount += 1;
            if (Input.GetButtonDown("Attack")) _inputCount += 1;
            if (Input.GetButtonDown("Dash")) _inputCount += 1;
            if (Input.GetButtonDown("ImmuneSkill")) _inputCount += 1;
            if (_inputCount >= 25) Exorcised();
            
            // print(_inputCount);
        }
        
        private void DieState()
        {
            _isImmune = true;
            Time.timeScale = 0.2f;
        }
        
        #endregion
        
        #region StatesLogic
        
        private void SetState(State newState)
        {
            _state = newState;
        }
        
        private IEnumerator LockStateForSeconds(float duration)
        {
            _canChangeState = false;
            yield return new WaitForSeconds(duration);
            _canChangeState = true;
        }
        
        private void CheckIfGrounded()
        {
            RaycastHit2D ray = Physics2D.Raycast(
                _body.transform.position, 
                Vector2.down, 
                1.1f, 
                groundLayer);
            

            if (ray.collider) // El raycast toca el suelo
            {
                Debug.DrawRay(_body.transform.position, Vector2.down * 1.1f, Color.red);
                _isOnFloor = true;
                _coyoteCounter = data.coyoteTime; // Reseteamos el tiempo de coyote
                _body.gravityScale = data.regularGravity;
                if (_dashUnlocked && _dashTimer <= 0)
                {
                    _canDash = true;
                }
                if (_doubleJumpUnlocked) _canDoubleJump = true;
            }
            else // Si el raycast no toca el suelo...
            {
                if (_isOnFloor)
                {
                    // Acabamos de dejar el suelo
                    _coyoteCounter = data.coyoteTime;
                    if (_dashUnlocked) _canDash = true;
                } 

                _isOnFloor = false;
                _coyoteCounter -= Time.deltaTime;
                Debug.DrawRay(_body.transform.position, Vector2.down * 1.1f, Color.green);
            }
        }
        
        private void Attack()
        {
            if (_isAttacking || _isPossessed) return;
            _body.linearVelocityX *= 0.5f;
            _isAttacking = true;
            Invoke(nameof(EndAttack), data.attackSpeed);
            _attackCooldown = data.attackCooldown;
            _weapon.FlipAttackPosition();
            AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerMissedAttack, true);
            _animator.SetBool(Attacking, true);
        }
        
        private void EndAttack()
        {
            _isAttacking = false;
            _animator.SetBool(Attacking, false);
        }

        private void Move()
        {
            if (_isAttacking && _isOnFloor) _body.linearVelocity = Vector2.Lerp(_body.linearVelocity, Vector2.zero, 0.01f);
            else _body.linearVelocity = new Vector2(_xInput * data.moveSpeed, _body.linearVelocity.y);
        }
        
        public void PlayStepSound()
        {
            AudioManager.Instance.PlayStepSound();
        }

        private bool JumpPressed()
        {
            return Input.GetButtonDown("Jump");
        }

        private bool JumpReleased()
        {
            return Input.GetButtonUp("Jump");
        }

        private void Jump()
        {
            if (_state == State.Possessed) return;
            
            _isOnFloor = false;
            _body.gravityScale = data.regularGravity;
            SetState(State.Jump);
            
            _body.linearVelocity = new Vector2(_body.linearVelocity.x, 0f);
            
            _body.AddForceY(data.jumpForce, ForceMode2D.Impulse);
        }

        private void PlayJumpSound()
        {
            if (Time.timeScale == 1)
                AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerJump, true);
        }

        private void CheckIfIsFalling()
        {
            if (_body.linearVelocityY < 0 && !_isOnFloor)
            {
                SetState(State.Fall);
            }
        }
        
        private void GetHit(int damage)
        {
            if (_isImmune) return;
            
            _hp -= damage;

            if (!_isPossessed)
            {
                // Frena el impulso del player (por si venia de un salto o dash)
                _body.linearVelocity = Vector2.zero;
                
                // Reproducir animacion al ser golpeado
                float direction = visuals.GetSpriteRenderer.flipX ? 1 : -1;
                _body.linearVelocityX = direction * data.pushForce / 2;
                _body.linearVelocityY = data.pushForce;
            }
            
            GameManager.Instance.GetHpAmountChanged?.Invoke(_hp);

            if (_hp <= 0)
            {
                _animator.SetTrigger("Died");
                SetState(State.Die);
                if (GetPlayerLives >= 1) _cam.DOShakePosition(0.4f, 0.5f);
                AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerDeath);
                if (!GameManager.Instance.GetGameOver) Invoke(nameof(Respawn), data.deathAnimationTime);
            }
            else
            {
                SetState(State.GetHit);
                _animator.SetBool(GotHit, true);
                _body.gravityScale = data.regularGravity;
                AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerHit, true);
            }
        }
        
        private void PlayerPossessed()
        {
            _animator.SetBool(Possessed, true);
            GetRandomDirection();
            SetState(State.Possessed);
            _isPossessed = true;
            // data.immunityTime = 0.5f;
        }

        private void GetRandomDirection()
        {
            _randomPossessedDirection = Random.value < 0.5f ? -1f : 1f;
        }

        public void Exorcised()
        {
            _animator.SetBool(Possessed, false);
            _onPlayerExorcised.Invoke();
            _inputCount = 0;
            _isPossessed = false;
            data.immunityTime = data.baseImmunityTime;
            SetState(State.Idle);
        }

        #endregion

        #region CheckInput

        private void CheckJumpInput()
        {
            if (_isAttacking || _isPossessed) return;
            
            if (JumpPressed())
                _lastJumpPressedTime = Time.time;
            
            // Si el jugador está en el piso o dentro de la ventana del coyote time -> puede saltar
            if ((_isOnFloor || _coyoteCounter > 0f) && Time.time - _lastJumpPressedTime <= data.jumpBufferTime)
            {
                Jump();
                _lastJumpPressedTime = -Mathf.Infinity;
                if (_doubleJumpUnlocked) _canDoubleJump = true;
                _animator.SetBool(Jumping, true);
                PlayJumpSound();
            }
            // Sino, si el jugador esta en el aire y puede hacer doble salto -> saltar en el aire
            else if ((!_isOnFloor && _canDoubleJump) && Time.time - _lastJumpPressedTime <= data.jumpBufferTime)
            {
                Jump();
                _jumpKeyTimePressed = 0f;
                _canDoubleJump = false;
                _lastJumpPressedTime = -Mathf.Infinity;
                _animator.SetBool(Falling, false);
                _animator.SetBool(DoubleJumping, true);
                jumpParticles.Play();
            }
        }

        private void CheckMovementInput()
        {
            if (_isPossessed) return;
            _xInput = Input.GetAxis("Horizontal");
        }

        private void CheckAttackInput()
        {
            // if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.Z) && !_isPossessed && _attackCooldown <= 0)
            if (Input.GetButtonDown("Attack") && !_isPossessed && _attackCooldown <= 0)
            {
                Attack();
                SetState(State.Attack);
            }
        }
        
        private void CheckDashInput()
        {
            if (Input.GetButtonDown("Dash") && !_isAttacking && !_isPossessed && _canDash)
            {
                Dash();
            }
        }
        
        private void CheckImmunitySkillInput()
        {
            if (!_immuneSkillUnlocked) return;
            if (Input.GetButtonDown("ImmuneSkill") && !_isPossessed)
            {
                ActivateImmunity();
                _onPlayerImmunity.Invoke();
            }
        }

        #endregion

        #region Skills
        private void UnlockDoubleJump()
        {
            _doubleJumpUnlocked = true;
        }

        private void UnlockDash()
        {
            _dashUnlocked = true;
        }
        
        private void UnlockImmunity()
        {
            _immuneSkillUnlocked = true;
        }
        private void Dash()
        {
            _dashTimer = data.dashCooldown;
            _animator.SetBool(Falling, false);
            _animator.SetBool(Dashing, true);
            AudioManager.Instance.PlayClip(AudioManager.AudioList.Dash);
            _body.linearVelocity = Vector2.zero;
            float direction = visuals.GetSpriteRenderer.flipX ? -1 : 1;
            _body.linearVelocityX = direction * data.dashSpeed;
            _canDash = false;
            _jumpKeyTimePressed = 0f;
            Invoke(nameof(EndDash), data.dashDuration);
            SetState(State.Dash);
        }

        private void EndDash()
        {
            _animator.SetBool(Dashing, false);
            _body.gravityScale = data.regularGravity;
            _body.linearVelocity = new Vector2(0f, _body.linearVelocity.y);
            SetState(_isOnFloor ? State.Idle : State.Jump);
        }

        private void ActivateImmunity()
        {
            if (_cooldown > 0f) return;
            _isImmune = true;
            shieldVisuals.gameObject.SetActive(true);
            CancelInvoke(nameof(EndImmunityTime));
            Invoke(nameof(EndImmunityTime), data.shieldDuration);
            _cooldown = data.shieldCooldown;
        }
        
        private void EndImmunityTime()
        {
            _isImmune = false;
            shieldVisuals.gameObject.SetActive(false);
        }
        #endregion
        
        #region RespawnLogic
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
            Exorcised();
            SetState(State.Idle);
        }

        private void RestartGame()
        {
            SetRespawnPosition(Vector3.zero);
            transform.position = _respawnPosition;
            _hp = data.baseHp;
            data.shieldCooldown = data.baseShieldCooldown;
            _cooldown = data.shieldCooldown;
            _body.linearVelocity = new Vector2(0, 0);
            EndImmunityTime();
            _cooldown = 0f;
            CancelInvoke();
            SetState(State.Idle);
            _dashUnlocked = false;
            _canDash = false;
            _doubleJumpUnlocked = false;
            _canDoubleJump = false;
            _immuneSkillUnlocked = false;
            EndAttack();
            ResetPlayerStats();
        }
        
        private void Revive()
        {
            transform.position = _mainRespawnPosition;
            _hp = data.baseHp;
            _body.linearVelocity = new Vector2(0, 0);
            _body.gravityScale = data.regularGravity;
            EndImmunityTime();
            _cooldown = 0f;
            CancelInvoke();
            StopCoroutine(LockStateForSeconds(data.deathAnimationTime));
            EndAttack();
            Exorcised();
            SetState(State.Idle);
        }
        
        public void SetRespawnPosition(Vector3 newPosition)
        {
            _respawnPosition = newPosition;
            SavePlayerStats();
        }

        public void SetMainRespawnPosition(Vector3 newPosition)
        {
            _mainRespawnPosition = newPosition;
            SavePlayerStats();
        }
        #endregion
        
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
        
        public void Heal()
        {
            _hp = data.baseHp;
            _playerHealth.healthBar.SetHealth(_hp);
        }
        
        public void SetPlayerMaterial(PhysicsMaterial2D material)
        {
            _body.sharedMaterial = material;
        }

        private void GameWon()
        {
            _body.linearVelocity = new Vector2(_body.linearVelocity.x / 2, _body.linearVelocity.y / 2);
            _body.gravityScale = data.dashGravity;
            _isImmune = true;
            Invoke(nameof(EndGame), 3f);
        }

        private void EndGame()
        {
            GameManager.Instance.gameWon = false;
            _animator.SetTrigger("TouchedFloor");
            _body.gravityScale = data.regularGravity;
            transform.position = _endingPosition;
            _isImmune = false;
        }

        public void StopInputs()
        {
            _canChangeState = false;
        }

        #region GetProperties
        
        public State GetState => _state;

        public float GetMovementInput => _xInput;
        
        public Vector3 GetPlayerPosition => transform.position;
        
        public Vector3 GetPlayerTarget => playerTarget.position;
        
        public int GetPlayerLives => PlayerPrefs.GetInt("Lives");
        
        public float GetCooldown => _cooldown;

        public bool GetIsAttacking => _isAttacking;

        public PlayerData GetPlayerData => data;

        public PlayerVisuals GetPlayerVisuals => visuals;
        
        public bool GetIsPossessed => _isPossessed;
        
        public bool GetIsImmune => _isImmune;
        
        public bool GetDashUnlocked => _dashUnlocked;
        
        public bool GetDoubleJumpUnlocked => _doubleJumpUnlocked;

        #endregion

        #region PlayerPrefs

        public void SavePlayerStats()
        {
            PlayerPrefs.SetFloat("XPosition", _respawnPosition.x);
            PlayerPrefs.SetFloat("YPosition", _respawnPosition.y);
            
            // Guardamos la posicion del respawn de inicio del nivel.
            PlayerPrefs.SetFloat("MainXPosition", _mainRespawnPosition.x);
            PlayerPrefs.SetFloat("MainYPosition", _mainRespawnPosition.y);
            
            PlayerPrefs.SetInt("Lives", _playerHealth.GetPlayerLives);
            PlayerPrefs.SetInt("BaseLives", _playerHealth.GetPlayerBaseLives);
        }

        private void ResetPlayerStats()
        {
            PlayerPrefs.SetFloat("XPosition", 0);
            PlayerPrefs.SetFloat("YPosition", 0);
            
            PlayerPrefs.SetFloat("MainXPosition", 0);
            PlayerPrefs.SetFloat("MainYPosition", 0);
            
            PlayerPrefs.SetInt("Lives", data.startingLives);
            PlayerPrefs.SetInt("BaseLives", data.startingLives);
        }

        #endregion

        #region GetEvents

        public UnityEvent<int> GetPlayerHitEvent => _onPlayerHit;

        public UnityEvent GetPlayerPossessedEvent => _onPlayerPossessed;
        
        public UnityEvent GetPlayerExorcisedEvent => _onPlayerExorcised;
        
        public UnityEvent GetPlayerRevived => _onPlayerRevived;
        
        public UnityEvent GetPlayerImmunity => _onPlayerImmunity;

        #endregion
    }
}
