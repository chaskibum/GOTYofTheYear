using UnityEngine;

public class HealingStatue : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameManager.Instance.GetPlayer.Heal();
    }
}
