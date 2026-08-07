using UnityEngine;
using Utils;

namespace EnemiesScripts
{
    public class ArmorGhostScript : EnemyController
    {
        [SerializeField] private GameObject attack;
        private float _rotation;

        [Header("Audio")]
        [SerializeField] private AudioSource attackSound;
        [SerializeField] private AudioSource hurtSound;
        [SerializeField] private AudioSource dieSound;

        protected override void ResetEnemy()
        {
            base.ResetEnemy();
            Animator?.SetBool("Attacking", false);
        }

        protected override void RestartEnemy()
        {
            base.RestartEnemy();
            Animator?.SetBool("Attacking", false);
        }

        protected override void ChaseState()
        {
            if (Vector3.Distance(Body.position, PlayerPos) < 1f)
                Body.AddForce(Direction * data.stepAwayForce, ForceMode2D.Impulse);
            else base.ChaseState();
        }

        public override void GetHit()
        {
            base.GetHit();
            if (Hp > 0)
            {
                hurtSound.Play();
                GamepadVibration.Instance.Rumble(0.2f, 0.35f, 0.15f);
            }
            else
            {
                attackSound.Stop();
                dieSound.Play();
            }
        }

        protected override void Attack()
        {
            if (!CanAttack) return;
            Animator?.SetBool("Attacking", true);
            attackSound.Play();
            base.Attack();
        }

        protected override void EndAttack()
        {
            base.EndAttack();
            Animator?.SetBool("Attacking", false);
        }
    }
}
