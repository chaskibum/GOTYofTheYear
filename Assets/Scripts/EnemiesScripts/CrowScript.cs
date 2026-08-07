using GameplayElements;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils;

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

            if (MovementScript)
                MovementScript.enabled = true;
        }

        protected override void RestartEnemy()
        {
            base.RestartEnemy();
            attackHitbox.SetActive(true);
            crowLight.enabled = true;

            if (MovementScript)
                MovementScript.enabled = true;
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

            if (MovementScript)
                MovementScript.enabled = false;
        }

        public override void GetHit()
        {
            base.GetHit();
            GamepadVibration.Instance.Rumble(0.1f, 0.25f, 0.10f);
        }
    }
}
