using EnemiesScripts.ArmorGhost;
using UnityEngine;

namespace EnemiesScripts
{
    public class DetectionCollider : MonoBehaviour
    {
        [SerializeField] private ArmorGhostBehaviour armorGhost;
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            print("CHASING");
            armorGhost.SetState(ArmorGhostBehaviour.State.Chase);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            print("IDLE");
            armorGhost.SetState(ArmorGhostBehaviour.State.Idle);
        }
    }
}
