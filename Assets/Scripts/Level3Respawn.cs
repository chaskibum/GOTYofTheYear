using System;
using UnityEngine;

public class Level3Respawn : RespawnManager
{
    protected override void OnTriggerStay2D(Collider2D other)
    {
        _respawnPos = mainRespawn.transform.position;
        _currentRespawn = "Level3Respawn";
        globalLight.intensity = 0.2f;
    }
}
