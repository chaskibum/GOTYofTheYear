using UnityEngine;

public class Collectable : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private BoxCollider2D _collider;
    
    [SerializeField] private GameObject text;
    
    // PARA BORRAR DESPUÉS
    [SerializeField] private bool isGameWonCollectable = false;
    [SerializeField] private GameObject gameWonScreen;
    [SerializeField] private GameObject restartButton;

    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isGameWonCollectable)
        {
            gameWonScreen.SetActive(true);
            restartButton.SetActive(false);
            Time.timeScale = 0;
        }
            
        _sprite.enabled = false;
        _collider.enabled = false;
        text.SetActive(true);
        Invoke(nameof(HideText), 1f);
    }

    private void HideText()
    {
        text.SetActive(false);
    }
}
