using UnityEngine;

public class Level1Respawn : RespawnManager
{
    protected override void OnTriggerStay2D(Collider2D other)
    {
        _respawnPos = new Vector3(41f, -9.35f, 0f);
        _currentRespawn = "Level1Respawn";
        globalLight.intensity = 0.5f;
    }
}
