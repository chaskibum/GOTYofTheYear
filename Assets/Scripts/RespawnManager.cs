using PlayerScripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RespawnManager : MonoBehaviour
{
    /*private enum Level { Level1, Level2, Level3, }
    
    [SerializeField] private Level currentLevel;*/
    
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
        // globalLight.color = Color.green;
    }

    private void Revive()
    {
        _player.transform.position = _respawnPos;
    }
}
