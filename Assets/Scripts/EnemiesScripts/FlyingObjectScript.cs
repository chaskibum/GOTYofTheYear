using UnityEngine;

namespace EnemiesScripts
{
    public class FlyingObjectScript : EnemyController
    {
        protected override void GetPlayerPosition()
        {
            PlayerPos = Player.GetPlayerTarget;
        }

        protected override void EndAttack()
        {
            base.EndAttack();
            Body.AddForce(Direction * data.stepAwayForce, ForceMode2D.Impulse);
            SetState(State.Idle);
            StartCoroutine(DelayChangeState());
        }
    }
}
