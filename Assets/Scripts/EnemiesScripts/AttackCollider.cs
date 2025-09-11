using EnemiesScripts.ArmorGhost;
using UnityEngine;

namespace EnemiesScripts
{
    public class AttackCollider : MonoBehaviour
    {
        [SerializeField] private ArmorGhostBehaviour armorGhost;

        private void OnTriggerEnter2D(Collider2D other)
        {
            armorGhost.SetState(ArmorGhostBehaviour.State.Attack);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            armorGhost.SetState(ArmorGhostBehaviour.State.Chase);
        }
    }
}
