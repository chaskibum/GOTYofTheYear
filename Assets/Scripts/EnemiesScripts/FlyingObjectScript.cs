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
        
        protected override void ResetEnemy()
        {
            base.ResetEnemy();
            attackHitbox.SetActive(true);
            _forceAdded = false;
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
    }
}
