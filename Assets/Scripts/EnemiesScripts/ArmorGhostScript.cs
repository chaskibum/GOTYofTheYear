using UnityEngine;

namespace EnemiesScripts
{
    public class ArmorGhostScript : EnemyController
    {
        protected override void ChaseState()
        {
            if (Vector3.Distance(Body.position, PlayerPos) < 1f)
                Body.AddForce(Direction * data.stepAwayForce, ForceMode2D.Impulse);
            else base.ChaseState();
        }
    }
}
