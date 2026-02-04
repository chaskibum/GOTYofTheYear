using System.Collections;
using Managers;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameplayElements
{
    public abstract class RespawnManager : MonoBehaviour
    {
        [SerializeField] protected GameObject mainRespawn;
        [SerializeField] protected Light2D globalLight;
        [SerializeField] protected GameObject levelEnemies;
        private PlayerController _player;
        protected Vector3 _respawnPos;

        private void Start()
        {
            _player = GameManager.Instance.GetPlayer;
            _player.GetPlayerRevived?.AddListener(Revive);
            // GameManager.Instance.GetPlayerRespawn?.AddListener(DeactivateEnemies);
            GameManager.Instance.GetGameRestarted?.AddListener(ReloadLevel); 
            // DeactivateEnemies();
        }

        private void ReloadLevel()
        {
            // DeactivateEnemies();
            globalLight.intensity = 0.5f;
        }

        private void ActivateEnemies()
        {
            StartCoroutine(SpawnEnemies(true));
        }
    
        private void DeactivateEnemies()
        {
            if (PlayerPrefs.GetString("Respawn") == name) return;
            StartCoroutine(SpawnEnemies(false));
        }

        private void OnDestroy()
        {
            _player.GetPlayerRevived?.RemoveListener(Revive);
            GameManager.Instance.GetGameRestarted?.RemoveListener(ReloadLevel);
            GameManager.Instance.GetPlayerRespawn?.RemoveListener(DeactivateEnemies);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            // ActivateEnemies();
        }

        protected IEnumerator SpawnEnemies(bool spawn)
        {
            if (spawn) levelEnemies.gameObject.SetActive(true);
            foreach (Transform enemy in levelEnemies.transform)
            {
                enemy.gameObject.SetActive(spawn);
                yield return new WaitForSecondsRealtime(0.1f);
            }
            if (!spawn && PlayerPrefs.GetString("Respawn") != name) levelEnemies.gameObject.SetActive(false);
        }
    
        private void OnTriggerExit2D(Collider2D other)
        {
            PlayerPrefs.SetString("Respawn", "");
            // StartCoroutine(SpawnEnemies(false));
        }

        private void Revive()
        {
            if (PlayerPrefs.GetString("Respawn") == name)
            {
                _player.SetRespawnPosition(_respawnPos);
                _player.SetMainRespawnPosition(_respawnPos);
            }
        }
    }
}
