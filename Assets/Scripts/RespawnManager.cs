using PlayerScripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class RespawnManager : MonoBehaviour
{
    // private enum Level { Level1, Level2, Level3, }
    
    // [SerializeField] private Level currentLevel;
    
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

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        // _respawnPos = mainRespawn.transform.position;
        // print(_respawnPos);
        /*if (currentLevel == Level.Level1)
        {
            globalLight.intensity = 0.6f;
        }
        else if (currentLevel == Level.Level2)
        {
            globalLight.intensity = 0.4f;
            // _respawnPos = new Vector3(182.46f, -29.39f, 0f);
        }
        else if (currentLevel == Level.Level3) globalLight.intensity = 0.2f;*/
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
