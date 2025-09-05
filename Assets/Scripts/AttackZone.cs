using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public bool isAttacking;
    private SpriteRenderer _spriteRenderer;
    private PolygonCollider2D _collider;
    
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<PolygonCollider2D>();
        
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
    }

    public void Attack()
    {
        if (isAttacking) return;
        isAttacking = true;
        _spriteRenderer.enabled = true;
        _collider.enabled = true;
        Invoke("StopAttacking", 0.5f);
    }

    private void StopAttacking()
    {
        isAttacking = false;
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
    }
}
