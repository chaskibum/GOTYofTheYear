using System.Collections;
using PlayerScripts;
using TMPro;
using UnityEngine;

namespace EnemiesScripts
{
    public abstract class EnemyController : MonoBehaviour
    {
        [SerializeField] protected EnemyData data;
        [SerializeField] protected GameObject attackHitbox;
        [SerializeField] protected TMP_Text stateText;
        
        public enum State { Idle, Chase, Attack, GetHit, Die, }
        protected State ActualState = State.Idle;

        [Header("Properties")] 
        protected int Hp;
        protected Vector3 StartingPosition;
        protected Vector2 Direction;
        protected float AttackTimer;
        protected bool CanAttack = true;
        protected bool CanChangeState = true;
        
        protected Rigidbody2D Body;
        protected SpriteRenderer Visuals;
        protected Animator Animator;
        
        protected PlayerController Player;
        protected Vector3 PlayerPos;
        protected bool PlayerToTheRight;

        protected void Awake()
        {
            StartingPosition = transform.position;
            AttackTimer = data.attackAnimationTime;
            Hp = data.baseHp;
        }

        protected virtual void Start()
        {
            Body = GetComponent<Rigidbody2D>();
            Visuals = GetComponentInChildren<SpriteRenderer>();
            TryGetComponent(out Animator);
            Player = GameManager.Instance.GetPlayer;
            
            GameManager.Instance.GetGameRestarted.AddListener(ResetEnemy);
        }
        
        protected void Update()
        {
            CalculateDirection();
            GetPlayerPosition();
            UpdateState();
        }

        protected void FixedUpdate()
        {
            UpdatePhysicsState();
        }

        protected virtual void ResetEnemy()
        {
            ActualState = State.Idle;
            transform.position = StartingPosition;
            Hp = data.baseHp;
            Body.linearVelocity = Vector2.zero;
            CanAttack = true;
            Animator?.SetBool("Attacking", false);
            attackHitbox.SetActive(false);
            gameObject.SetActive(true);
            
            // PARA BORRAR DESPUÉS (se va a hacer mediante animaciones)
            var color = Visuals.color;
            color.a = 1;
            Visuals.color = color;
        }
        
        protected virtual void GetPlayerPosition()
        // Por defecto obtiene la posicion de los pies del player.
        {
            PlayerPos = Player.GetPlayerPosition;
        }

        // Calculamos la dirección hacia donde tendríamos que estar mirando.
        protected void CalculateDirection()
        {
            PlayerToTheRight = PlayerPos.x > transform.position.x;
            Direction = PlayerToTheRight ? Vector2.left : Vector2.right;
        }

        protected virtual void IdleState()
        {
            Body.linearVelocity = Vector2.Lerp(Body.linearVelocity, Vector2.zero, Time.deltaTime);
            if (!CanChangeState) return;

            if (Vector3.Distance(Body.position, PlayerPos) < data.detectionRange)
                SetState(State.Chase);
        }
        
        protected virtual void ChaseState()
        {
            Visuals.flipX = PlayerToTheRight;
            
            Vector3 newPosition = Vector3.MoveTowards(
                Body.position,
                PlayerPos,
                data.moveSpeed * Time.deltaTime);
                
            Body.MovePosition(newPosition);
            
            if (Vector3.Distance(Body.position, PlayerPos) > data.detectionRange) 
                SetState(State.Idle);
            
            if (Vector3.Distance(Body.position, PlayerPos) < data.attackRange && CanAttack) 
                SetState(State.Attack);
        }
        
        protected virtual void AttackState()
        {
            // Si el tiempo de la animación de ataque termino, desactivamos la hitbox
            AttackTimer -= Time.deltaTime;
            if (AttackTimer <= 0f)
            {
                EndAttack();
                AttackTimer = data.attackAnimationTime;
                SetState(State.Chase);
            }
            // Si no la activamos y hacemos que no se pueda volver a atacar hasta en cierto tiempo
            else
                Attack();
        }
        
        protected virtual void GetHitState()
        {
            Body.linearVelocity = Vector2.Lerp(Body.linearVelocity, Vector2.zero, Time.deltaTime);
            StartCoroutine(DelayChaseState());
        }
        
        protected IEnumerator DelayChaseState()
        {
            yield return new WaitForSeconds(data.pushTime);
            Body.linearVelocity = Vector2.zero;
            SetState(State.Chase);
        }
        
        protected virtual void DieState()
        {
            attackHitbox.SetActive(false);
            Body.AddForce(Vector2.up * 0.1f, ForceMode2D.Impulse);
            StartCoroutine(nameof(Disappear));
        }
        
        protected virtual void Attack()
        {
            Animator?.SetBool("Attacking", true);
            attackHitbox.SetActive(true);
            CanAttack = false;
            StartCoroutine(nameof(CanAttackAgain));
        }
        
        protected IEnumerator CanAttackAgain()
        {
            yield return new WaitForSeconds(data.attackCooldown);
            CanAttack = true;
        }

        protected virtual void EndAttack()
        {
            Animator?.SetBool("Attacking", false);
            attackHitbox.SetActive(false);
        }
        
        public virtual void GetHit()
        {
            Body.AddForce(Direction * data.pushForce, ForceMode2D.Impulse);

            Hp -= 1;
            SetState(Hp <= 0 ? State.Die : State.GetHit);
        }
        
        // PARA BORRAR DESPUÉS (cuando tengamos la animación de muerte)
        protected IEnumerator Disappear()
        {
            while (Visuals.color.a > 0)
            {
                var color = Visuals.color;
                color.a -= 0.02f;
                Visuals.color = color;
                yield return new WaitForSeconds(0.05f);
            }

            gameObject.SetActive(false);
        }
        
        protected IEnumerator DelayChangeState()
        {
            CanChangeState = false;
            yield return new WaitForSeconds(data.attackCooldown);
            CanChangeState = true;
        }
        
        protected void UpdateState()
        {
            switch (ActualState)
            {
                case State.Idle: IdleState(); break;
                case State.Attack: AttackState(); break;
            }
        }

        protected void UpdatePhysicsState()
        {
            switch (ActualState)
            {
                case State.Chase: ChaseState(); break;
                case State.GetHit: GetHitState(); break;
                case State.Die: DieState(); break;
            }
        }

        public virtual void SetState(State newState)
        {
            if (newState == ActualState || ActualState == State.Die) return;
            print("Previous State: " + ActualState + ". New State: " + newState);
            ActualState = newState;
            stateText.text = ActualState.ToString();
        }
        
        public State GetState => ActualState;

        public bool GetCanAttack => CanAttack;
    }
}
