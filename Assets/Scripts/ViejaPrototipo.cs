using UnityEngine;

public class ViejaPrototipo : MonoBehaviour
{
    [SerializeField] private GameObject grannyDialog;
    
    private void OnTriggerStay2D(Collider2D other)
    {
        grannyDialog.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        grannyDialog.SetActive(false);
    }
}
