using UnityEngine;

public class Collectable : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private BoxCollider2D _collider;
    
    [SerializeField] private GameObject text;

    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _sprite.enabled = false;
        _collider.enabled = false;
        text.SetActive(true);
        Invoke("HideText", 1f);
    }

    private void HideText()
    {
        text.SetActive(false);
    }
}
