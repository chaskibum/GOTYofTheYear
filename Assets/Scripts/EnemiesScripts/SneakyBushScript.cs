using UnityEngine;

namespace EnemiesScripts
{
    public class SneakyBushScript : EnemyController
    {
        protected override void Attack()
        {
            if (!CanAttack) return;
            Animator?.SetBool("Attacking", true);
            AudioManager.Instance.PlayClip(AudioManager.AudioList.BushAttack);
            base.Attack();
        }

        public override void GetHit()
        {
            base.GetHit();
            if (Hp <= 0) AudioManager.Instance.PlayClip(AudioManager.AudioList.BushDeath);
        }

        protected override void ChaseState()
        {
            Visuals.flipX = !PlayerToTheRight;
            
            if (Vector2.Distance(Body.position, PlayerPos) > data.detectionRange) 
                SetState(State.Idle);
            
            if (Vector2.Distance(Body.position, PlayerPos) < data.attackRange && CanAttack) 
                SetState(State.Attack);
        }

        protected override void EndAttack()
        {
            base.EndAttack();
            Animator?.SetBool("Attacking", false);
        }
    }
}
