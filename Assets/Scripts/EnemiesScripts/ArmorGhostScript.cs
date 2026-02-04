using Ami.BroAudio;
using UnityEngine;

namespace EnemiesScripts
{
    public class ArmorGhostScript : EnemyController
    {
        [SerializeField] private GameObject attack;
        private float _rotation;

        [Header("Audio")]
        [SerializeField] private SoundID attackSound;
        [SerializeField] private SoundID hurtSound;
        [SerializeField] private SoundID dieSound;
        
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
            if (Hp > 0) BroAudio.Play(hurtSound);// AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorHit, true);
            else {
                BroAudio.Stop(attackSound);
                BroAudio.Play(dieSound);//AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorDeath);
            }
        }

        protected override void Attack()
        {
            if (!CanAttack) return;
            Animator?.SetBool("Attacking", true);
            // AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorAttack, false, 0.8f);
            BroAudio.Play(attackSound);
            base.Attack();
        }

        protected override void EndAttack()
        {
            base.EndAttack();
            Animator?.SetBool("Attacking", false);
        }
    }
}
