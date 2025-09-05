using UnityEngine;

public class EnemyHitboxZone : MonoBehaviour
{
    private int _hp = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        GetHit();
    }

    private void GetHit()
    {
        _hp -= 1;
        print(_hp);
        
        if(_hp <= 0) gameObject.SetActive(false);
    }
}
