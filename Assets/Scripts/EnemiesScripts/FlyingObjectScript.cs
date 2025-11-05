using System.Collections;
using UnityEngine;

namespace EnemiesScripts
{
    public class FlyingObjectScript : EnemyController
    {
        private bool _flying;
        private Vector3 _playerDirection;
        private bool _forceAdded = false;
        
        protected override void Update()
        {
            CalculateDirection();
            if (!_flying)
                GetPlayerPosition();
            
            UpdateState();
        }

        protected override void IdleState()
        {
            Animator.SetTrigger("BackToIdle");
            particles.Stop();
            Body.linearVelocity = Vector2.Lerp(Body.linearVelocity, Vector2.zero, Time.deltaTime);
            //if (!CanChangeState) return;

            if (Vector3.Distance(Body.position, PlayerPos) < data.detectionRange)
            {
                Animator.SetTrigger("StartAttack");
                particles.Play();
            }
        }

        protected override void ResetEnemy()
        {
            base.ResetEnemy();
            particles.Stop();
            attackHitbox.SetActive(true);
            _forceAdded = false;
            StopAllCoroutines();
            _flying = false;
            CanChangeState = true;
            Visuals.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        
        protected override void GetPlayerPosition()
        {
            PlayerPos = Player.GetPlayerTarget;
            _playerDirection = (PlayerPos - transform.position).normalized;
        }

        protected override void ChaseState()
        {
            Visuals.flipX = !PlayerToTheRight;
            
            if (!_forceAdded)
            {
                Body.AddForce(_playerDirection * data.stepAwayForce, ForceMode2D.Impulse);
                _forceAdded = true;
            }
            
            if (Vector3.Distance(Body.position, PlayerPos) > data.detectionRange) 
                SetState(State.Idle);

            if (Vector3.Distance(Body.position, PlayerPos) < data.attackRange && CanAttack)
            {
                SetState(State.Attack);
            }

            var actualDistance = Vector3.Distance(Body.position, PlayerPos);
            StartCoroutine(CheckIfStuck(actualDistance));
            
            _flying = true;
            Visuals.transform.eulerAngles += new Vector3(0, 0, 1000) * Time.deltaTime;
        }

        protected override void EndAttack()
        {
            base.EndAttack();
            SetState(State.Idle);
            StartCoroutine(LockStateForSeconds(data.attackCooldown));
            _flying = false;
            _forceAdded = false;
            attackHitbox.SetActive(true);
        }

        private IEnumerator CheckIfStuck(float distance)
        {
            yield return new WaitForSeconds(1);
            if (distance == Vector3.Distance(Body.position, PlayerPos))
            {
                print("bugeao");
                EndAttack();
                StartCoroutine(LockStateForSeconds(1));
            }
        }
    }
}
