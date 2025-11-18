using UnityEngine;

public class Level2Respawn : RespawnManager
{
    protected override void OnTriggerStay2D(Collider2D other)
    {
        //_respawnPos = mainRespawn.transform.position;
        _respawnPos = new Vector3(182.46f, -29.39f, 0f);
        _currentRespawn = "Level2Respawn";
        globalLight.intensity = 0.3f;
    }
}
