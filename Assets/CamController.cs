using DG.Tweening;
using UnityEngine;

public class CamController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Camera.main.DOOrthoSize(9, 1.5f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Camera.main.DOOrthoSize(7, 1f);
    }
}
