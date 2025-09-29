using UnityEngine;

namespace EnemiesScripts
{
    public class FlyingObjectScript : EnemyController
    {
        protected bool Flying;
        
        protected override void Update()
        {
            CalculateDirection();
            if (!Flying)
                GetPlayerPosition();
            
            UpdateState();
        }
        
        protected override void ResetEnemy()
        {
            base.ResetEnemy();
            attackHitbox.SetActive(true);
        }
        
        protected override void GetPlayerPosition()
        {
            PlayerPos = Player.GetPlayerTarget;
        }

        protected override void ChaseState()
        {
            base.ChaseState();
            Flying = true;
        }

        protected override void EndAttack()
        {
            base.EndAttack();
            Body.AddForce(Direction * data.stepAwayForce, ForceMode2D.Impulse);
            SetState(State.Idle);
            StartCoroutine(LockStateForSeconds(data.attackCooldown));
            Flying = false;
        }
    }
}
