using PlayerScripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class RespawnManager : MonoBehaviour
{
    [SerializeField] protected GameObject mainRespawn;
    [SerializeField] protected Light2D globalLight;
    private PlayerController _player;
    protected Vector3 _respawnPos;
    protected string _currentRespawn;

    private void Start()
    {
        _player = GameManager.Instance.GetPlayer;
        _player.GetPlayerRevived?.AddListener(Revive);
    }

    private void OnDisable()
    {
        _player.GetPlayerRevived?.RemoveListener(Revive);
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        _currentRespawn = "";
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
