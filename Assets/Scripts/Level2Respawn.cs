using UnityEngine;

public class Level2Respawn : RespawnManager
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        _respawnPos = mainRespawn.transform.position;
        _currentRespawn = "Level2Respawn";
        globalLight.intensity = 0.3f;
        //levelEnemies.SetActive(true);
    }
}
