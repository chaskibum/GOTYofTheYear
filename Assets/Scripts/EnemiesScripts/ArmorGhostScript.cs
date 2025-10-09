using UnityEngine;

namespace EnemiesScripts
{
    public class ArmorGhostScript : EnemyController
    {
        [SerializeField] private GameObject attack;
        private float _rotation;
        
        protected override void ChaseState()
        {
            if (Vector3.Distance(Body.position, PlayerPos) < 1f)
                Body.AddForce(Direction * data.stepAwayForce, ForceMode2D.Impulse);
            else base.ChaseState();
        }

        public override void GetHit()
        {
            base.GetHit();
            if (Hp > 0) AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorHit, true);
            else AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorDeath);
        }

        protected override void Attack()
        {
            if (!CanAttack) return;
            AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorAttack, false, 0.8f);
            base.Attack();
        }
    }
}
