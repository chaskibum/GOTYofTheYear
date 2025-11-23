using Managers;
using PlayerScripts;
using UnityEngine;

namespace EnemiesScripts
{
    public class PossessedScript : MonoBehaviour
    {
        private PlayerController _player;

        private void Start()
        {
            _player = GameManager.Instance.GetPlayer;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _player.GetPlayerPossessedEvent?.Invoke();
        }
    }
}
