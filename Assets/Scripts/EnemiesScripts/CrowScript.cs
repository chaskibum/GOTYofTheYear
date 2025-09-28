namespace EnemiesScripts
{
    public class CrowScript : EnemyController
    {
        protected ApplyMovement MovementScript;
        
        protected override void Start()
        {
            base.Start();
            TryGetComponent(out MovementScript);
        }

        protected override void ResetEnemy()
        {
            base.ResetEnemy();
            attackHitbox.SetActive(true);
        }

        protected override void IdleState()
        {
            base.IdleState();
            if (!MovementScript) Visuals.flipX = !PlayerToTheRight;
        }
    }
}
