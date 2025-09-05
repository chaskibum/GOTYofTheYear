using UnityEngine;

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
        
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidbody2D;

        private float _xInput;
        
        private void Start()
        {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
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

            if (ray.collider != null)
            {
                if (!_isOnFloor)
                    data.canUseCoyoteTime = false;
        
                _isOnFloor = true;
                _rigidbody2D.gravityScale = data.regularGravity;
            }
            else
            {
                if (_isOnFloor)
                {
                    data.canUseCoyoteTime = true;
                    Invoke("EndCoyoteTime", data.coyoteTime);
                }

                _isOnFloor = false;
            }
        }
        
        private void CheckJumpInput()
        {
            if (Input.GetButtonDown("Jump"))
                data.lastJumpPressedTime = Time.time;
            
            if ((_isOnFloor || data.canUseCoyoteTime) && Time.time - data.lastJumpPressedTime <= data.jumpBufferTime)
            {
                Jump();
                SetState(State.Jump);
                data.lastJumpPressedTime = -Mathf.Infinity;
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
            
            // _spriteRenderer.color = Color.white;
            // Here we play the Idle animation
        }
        
        private void MoveState()
        {
            CheckIfIsFalling();
            
            if (_xInput == 0) SetState(State.Idle);
            
            // _spriteRenderer.color = Color.red;
            Move();
            // Here we play the animations and sounds for when the character moves
        }

        private void Move()
        {
            _rigidbody2D.linearVelocity = new Vector2(_xInput * data.moveSpeed, _rigidbody2D.linearVelocity.y);
        }

        private void JumpState()
        {
            // _spriteRenderer.color = Color.black;
            if (_xInput != 0) Move();
            
            CheckIfIsFalling();
        }
        
        private void Jump()
        {
            // _rigidbody2D.gravityScale = data.regularGravity;
            _isOnFloor = false;
            data.canUseCoyoteTime = false;
            
            _rigidbody2D.gravityScale = data.regularGravity;
            
            _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, 0f);
            _rigidbody2D.AddForce(Vector2.up * data.jumpForce, ForceMode2D.Impulse);
            // Here we play the animations and sounds for when the character jumps
        }

        private void FallState()
        {
            // _spriteRenderer.color = Color.yellow;
            if (_xInput != 0) Move();
            
            if (_isOnFloor) SetState(State.Idle);
        }

        private void CheckIfIsFalling()
        {
            if (_rigidbody2D.linearVelocityY < 0)
            {
                SetState(State.Fall);
                _rigidbody2D.gravityScale *= data.fallGravity;
            }
        }
        
        private void AttackState()
        {
            Move();
            if (!playerWeapon.isAttacking) SetState(State.Idle);
        }

        private void EndCoyoteTime()
        {
            data.canUseCoyoteTime = false;
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
                /*case State.GetHit: GetHitState(); break;
                case State.Die: DieState(); break;*/
            }
        }

        private void SetState(State newState)
        {
            _state = newState;
        }
        
        #region PaDespues
        
        private void GetHitState()
        {
            
        }
        
        private void DieState()
        {
            
        }
        

        #endregion
        
    }
}
