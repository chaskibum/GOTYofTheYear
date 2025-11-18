using System.Collections;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class RespawnManager : MonoBehaviour
{
    [SerializeField] protected GameObject mainRespawn;
    [SerializeField] protected Light2D globalLight;
    [SerializeField] protected GameObject levelEnemies;
    private PlayerController _player;
    protected Vector3 _respawnPos;
    protected string _currentRespawn;

    private void Start()
    {
        _player = GameManager.Instance.GetPlayer;
        _player.GetPlayerRevived?.AddListener(Revive);
        GameManager.Instance.GetGameRestarted?.AddListener(DeactivateEnemies);
        DeactivateEnemies();
    }

    protected void DeactivateEnemies()
    {
        StartCoroutine(SpawnEnemies(false));
    }

    private void OnDisable()
    {
        _player.GetPlayerRevived?.RemoveListener(Revive);
        GameManager.Instance.GetGameRestarted?.RemoveListener(DeactivateEnemies);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(SpawnEnemies(true));
    }

    protected IEnumerator SpawnEnemies(bool spawn)
    {
        foreach (Transform enemy in levelEnemies.transform)
        {
            if (spawn)
            {
                enemy.gameObject.SetActive(true);
                yield return new WaitForSecondsRealtime(0.1f);
            }
            else
            {
                enemy.gameObject.SetActive(false);
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        _currentRespawn = "";
        StartCoroutine(SpawnEnemies(false));
    }

    private void Revive()
    {
        if (_currentRespawn == name)
        {
            _player.SetRespawnPosition(_respawnPos);
            _player.SetMainRespawnPosition(_respawnPos);
        }
    }
}
