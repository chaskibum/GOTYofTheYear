using System;
using System.Collections.Generic;
using UnityEngine;

public class ApplyMovement : MonoBehaviour
{
    [SerializeField] private List<Transform> positions;

    [SerializeField] private float speed;

    private int _targetIndex = 0;
    
    private SpriteRenderer _sprite;

    private void Start()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            positions[_targetIndex].position,
            (speed * Time.deltaTime));

        if (Vector2.Distance(transform.position, positions[_targetIndex].position) < 0.1f)
        {
            _sprite.flipX = !_sprite.flipX;
            _targetIndex = (_targetIndex + 1) % positions.Count;
            print(_targetIndex);
        }
    }
}
