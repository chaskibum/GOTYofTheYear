using UnityEngine;

public class Level1Respawn : RespawnManager
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        _respawnPos = mainRespawn.transform.position;
        _currentRespawn = "Level1Respawn";
        globalLight.intensity = 0.5f;
        //levelEnemies.SetActive(true);
    }
}
