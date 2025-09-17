using PlayerScripts;
using UnityEngine;

public class PlayerMaterialController : MonoBehaviour
{
    [SerializeField] private PhysicsMaterial2D playerMaterial;
    [SerializeField] private PhysicsMaterial2D slopeMaterial;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().SetPlayerMaterial(slopeMaterial);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().SetPlayerMaterial(playerMaterial);
        }
    }
}
