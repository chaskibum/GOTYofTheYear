using UnityEngine;

namespace EnemiesScripts
{
    public class GhostScript : EnemyController
    {
        protected float StateTimer = 0;
        
        protected override void Start()
        {
            base.Start();
            Player.GetPlayerExorcisedEvent?.AddListener(GetHit);
        }

        protected override void Update()
        {
            if (StateTimer > 0)
                StateTimer -= Time.deltaTime;
            
            base.Update();
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

        public override void GetHit()
        {
            Body.AddForce(Direction * data.pushForce, ForceMode2D.Impulse);

            StateTimer = data.pushTime;
            
            AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerHit);
            
            Hp -= 1;
            SetState(Hp <= 0 ? State.Die : State.GetHit);
        }

        protected override void GetHitState()
        {
            if (Body.linearVelocity.x == 0) Body.AddForce(Direction * data.pushForce / 2, ForceMode2D.Impulse);
            base.GetHitState();
            attackHitbox.SetActive(false);
        }

        public override void SetState(State newState)
        {
            if (StateTimer > 0 && newState != State.GetHit && newState != State.Die) return;
            CurrentState = newState;
            stateText.text = CurrentState.ToString();
        }
    }
}
