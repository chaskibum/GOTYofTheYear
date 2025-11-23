using UnityEngine;

namespace GameplayElements
{
    public class Level2Respawn : RespawnManager
    {
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            _respawnPos = mainRespawn.transform.position;
            PlayerPrefs.SetString("Respawn", "Level2RespawnZone");
            globalLight.intensity = 0.3f;
        }
    }
}
