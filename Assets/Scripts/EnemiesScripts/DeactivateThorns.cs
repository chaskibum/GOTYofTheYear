using System;
using PlayerScripts;
using UnityEngine;

namespace EnemiesScripts
{
    public class DeactivateThorns : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D boxCollider;
        private PlayerController _player;

        private void Start()
        {
            _player = GameManager.Instance.GetPlayer;
            _player.GetPlayerImmunity.AddListener(DeactivateColliders);
        }

        private void OnDestroy()
        {
            _player.GetPlayerImmunity.RemoveListener(DeactivateColliders);
        }

        private void DeactivateColliders()
        {
            boxCollider.enabled = false; 
            Invoke(nameof(ActivateColliders), _player.GetPlayerData.shieldDuration);
        }
        
        private void ActivateColliders()
        {
            boxCollider.enabled = true;
        }
    }
}
