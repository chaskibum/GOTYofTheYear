using UnityEngine;

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
            // if (Animator.GetBool("Attacking")) return;
            base.GetHit();
            if (Hp > 0) hurtSound.Play();// AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorHit, true);
            else {
                attackSound.Stop();
                dieSound.Play();//AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorDeath);
            }
        }

        protected override void Attack()
        {
            if (!CanAttack) return;
            Animator?.SetBool("Attacking", true);
            // AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorAttack, false, 0.8f);
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
