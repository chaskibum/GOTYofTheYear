using System;
using UnityEngine;

namespace EnemiesScripts
{
    public class SneakyBushScript : EnemyController
    {
        protected override void Attack()
        {
            if (!CanAttack) return;
            Animator?.SetBool("Attacking", true);
            CanAttack = false;
            StartCoroutine(nameof(CanAttackAgain));
            SetState(State.Attack);
        }

        public void PlayAttackSound()
        {
            AudioManager.Instance.PlayClip(AudioManager.AudioList.BushAttack);
        }

        protected override void ResetEnemy()
        {
            base.ResetEnemy();
            Animator?.SetBool("Attacking", false);
        }

        public override void GetHit()
        {
            base.GetHit();
            if (Hp <= 0)
            {
                AudioManager.Instance.PlayClip(AudioManager.AudioList.BushDeath);
                Animator?.SetBool("Attacking", false);
            }
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
