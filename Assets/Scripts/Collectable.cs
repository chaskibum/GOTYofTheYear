using TMPro;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private BoxCollider2D _collider;

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject text;
    
    // PARA BORRAR DESPUÉS
    [SerializeField] private bool isGameWonCollectable = false;

    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isGameWonCollectable)
        {
            GameManager.Instance.canWin = true;
            text.GetComponent<TextMeshProUGUI>().text = "Item final conseguido!";
            panel.SetActive(true);
        }
            
        _sprite.enabled = false;
        _collider.enabled = false;
        panel.SetActive(true);
        Invoke(nameof(HideText), 1f);
    }

    private void HideText()
    {
        panel.SetActive(false);
    }
}
