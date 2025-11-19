using UnityEngine;

public class Level3Respawn : RespawnManager
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        _respawnPos = mainRespawn.transform.position;
        PlayerPrefs.SetString("Respawn", "Level3RespawnZone");
        globalLight.intensity = 0.1f;
    }
}
