using PlayerScripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RespawnManager : MonoBehaviour
{
    private enum Level { Level1, Level2, Level3, }
    
    [SerializeField] private Level currentLevel;
    
    [SerializeField] private GameObject mainRespawn;
    [SerializeField] private Light2D globalLight;
    private PlayerController _player;
    private Vector3 _respawnPos;

    private void Start()
    {
        _player = GameManager.Instance.GetPlayer;
        _player.GetPlayerRevived?.AddListener(Revive);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        _respawnPos = mainRespawn.transform.position;
        print(_respawnPos);
        if (currentLevel == Level.Level1) globalLight.intensity = 0.6f;
        else if (currentLevel == Level.Level2)
        {
            globalLight.intensity = 0.4f;
            // _respawnPos = new Vector3(182.46f, -29.39f, 0f);
        }
        else if (currentLevel == Level.Level3) globalLight.intensity = 0.2f;
    }

    private void Revive()
    {
        _player.SetRespawnPosition(_respawnPos);
        _player.SetMainRespawnPosition(_respawnPos);
    }
}
