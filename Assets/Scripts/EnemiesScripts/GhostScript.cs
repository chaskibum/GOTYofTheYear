namespace EnemiesScripts
{
    
    public class GhostScript : EnemyController
    {
        protected override void Start()
        {
            base.Start();
            Player.GetPlayerExorcisedEvent?.AddListener(GetHit);
        }

        protected override void ResetEnemy()
        {
            base.ResetEnemy();
            attackHitbox.SetActive(true);
        }

        protected override void GetPlayerPosition()
        {
            PlayerPos = Player.GetPlayerTarget;
        }

        protected override void GetHitState()
        {
            base.GetHitState();
            attackHitbox.SetActive(false);
        }
    }
}
