using GameplayElements;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace EnemiesScripts
{
    public class CrowScript : EnemyController
    {
        protected ApplyMovement MovementScript;
        [SerializeField] private Light2D crowLight;
        
        protected override void Start()
        {
            base.Start();
            TryGetComponent(out MovementScript);
        }

        protected override void ResetEnemy()
        {
            base.ResetEnemy();
            attackHitbox.SetActive(true);
            crowLight.enabled = true;
        }

        protected override void RestartEnemy()
        {
            base.RestartEnemy();
            attackHitbox.SetActive(true);
            crowLight.enabled = true;
        }

        protected override void IdleState()
        {
            base.IdleState();
            if (!MovementScript) Visuals.flipX = !PlayerToTheRight;
        }

        protected override void DieState()
        {
            base.DieState();
            crowLight.enabled = false;
        }
    }
}
