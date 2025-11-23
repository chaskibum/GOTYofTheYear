using GameplayElements;
using Managers;
using UnityEngine;

namespace EnemiesScripts
{
    public class GhostScript : EnemyController
    {
        protected float StateTimer = 0;
        private ApplyMovement _movementScript;

        protected override void Start()
        {
            base.Start();
            Player.GetPlayerExorcisedEvent?.AddListener(GetHit);
            TryGetComponent(out _movementScript);
        }

        private void OnDestroy()
        {
            Player.GetPlayerExorcisedEvent?.RemoveListener(GetHit);
        }

        protected override void Update()
        {
            if (StateTimer > 0)
                StateTimer -= Time.deltaTime;
            
            base.Update();
        }

        protected override void Attack()
        {
            base.Attack();
            attackHitbox.SetActive(true);
        }

        protected override void ResetEnemy()
        {
            base.ResetEnemy();
            attackHitbox.SetActive(true);
            if (_movementScript) _movementScript.enabled = true;
        }

        protected override void GetPlayerPosition()
        {
            PlayerPos = Player.GetPlayerTarget;
        }

        public override void GetHit()
        {
            if (Vector3.Distance(transform.position, PlayerPos) > data.detectionRange + 3) return;
            Body.AddForce(Direction * data.pushForce, ForceMode2D.Impulse);

            StateTimer = data.pushTime;
            
            AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerAttack);
            
            Hp -= 1;
            SetState(Hp <= 0 ? State.Die : State.GetHit);
        }

        protected override void GetHitState()
        {
            if (Body.linearVelocity.x == 0) Body.AddForce(Direction * data.pushForce / 2, ForceMode2D.Impulse);
            base.GetHitState();
            attackHitbox.SetActive(false);
        }

        protected override void ChaseState()
        {
            if (Vector3.Distance(Body.position, PlayerPos) > data.detectionRange && Player.GetIsPossessed)
            {
                Player.Exorcised();
            }
            base.ChaseState();
        }

        public override void SetState(State newState)
        {
            if (StateTimer > 0 && newState != State.GetHit && newState != State.Die) return;
            CurrentState = newState;
            stateText.text = CurrentState.ToString();
        }

        protected override void DieState()
        {
            base.DieState();
            StopAllCoroutines();
        }
    }
}
