using System;
using UnityEngine;

namespace EnemiesScripts
{
    public class SneakyBushScript : EnemyController
    {
        protected override void Attack()
        {
            if (!CanAttack) return;
            AudioManager.Instance.PlayClip(AudioManager.AudioList.BushAttack);
            Animator?.SetBool("Attacking", true);
            base.Attack();
        }

        protected override void ResetEnemy()
        {
            base.ResetEnemy();
            Animator?.SetBool("Attacking", false);
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
            {
                Animator?.SetTrigger("Attack");
                SetState(State.Attack);
            }
        }

        protected override void EndAttack()
        {
            base.EndAttack();
            Animator?.SetBool("Attacking", false);
        }
    }
}
