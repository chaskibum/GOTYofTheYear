using System.Collections.Generic;
using UnityEngine;

public class ApplyMovement : MonoBehaviour
{
    [SerializeField] private List<Transform> positions;
    [SerializeField] private float speed;
    [SerializeField] private bool isPlatform;

    private int _targetIndex;
    
    private SpriteRenderer _sprite;
    private Rigidbody2D _body;
    

    private void Start()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (isPlatform)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                positions[_targetIndex].position,
                (speed * Time.deltaTime));

            if (Vector2.Distance(transform.position, positions[_targetIndex].position) < 0.1f)
            {
                _targetIndex = (_targetIndex + 1) % positions.Count;
            }
        }
        else
        {
            Vector2 newPosition = Vector2.MoveTowards(
                _body.position,
                positions[_targetIndex].position,
                speed * Time.deltaTime);
            _body.MovePosition(newPosition);

            
            if (Vector2.Distance(_body.position, positions[_targetIndex].position) < 0.01f)
            {
                if (positions.Count == 1) Destroy(this);
                else
                {
                    // Guardamos la x de la posicion actual
                    float currentIndexX = positions[_targetIndex].position.x;
                    _targetIndex = (_targetIndex + 1) % positions.Count;
                    _sprite.flipX = positions[_targetIndex].position.x < currentIndexX;
                }
            }
        }
    }
}
