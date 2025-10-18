using UnityEngine;
using System.Collections;

namespace EnemiesScripts
{
    public class ArmorGhostBossScript : EnemyController
    {
        [SerializeField] private GameObject attack;
        private float _rotation;
        
        protected override void ChaseState()
        {
            if (Vector3.Distance(Body.position, PlayerPos) < 1f) Attack();
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
            Body.linearVelocityX = 0f;
            AudioManager.Instance.PlayClip(AudioManager.AudioList.ArmorAttack, false, 0.8f);
            base.Attack();
        }
        
        protected override IEnumerator EndFeedback()
        {
            yield return new WaitForSecondsRealtime(0.05f);
            Visuals.color = new Color(0.5f, 1, 0.5f);
            Time.timeScale = 1;
        }
    }
}
