using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private Transform target;
    [SerializeField] private float yOffset = 3f;
    [SerializeField] private GameObject limits;
    
    private Camera _camera;
    private Mouse _mouse;

    private void Start()
    {
        _camera = Camera.main;
        _mouse = Mouse.current;
    }

    private void LateUpdate()
    {
        if (!target)
        {
            MoveBasedOnMousePosition();
            CheckLimits();
        }
        else
        {
            Vector3 newPos = new Vector3(target.position.x, target.position.y + yOffset, -10f);
            transform.position = Vector3.Lerp(transform.position, newPos, followSpeed * Time.deltaTime);
        }
        
        MoveCamera();
    }

    private void MoveBasedOnMousePosition()
    {
        Vector3 newTarget = new Vector3(_mouse.position.ReadValue().x, _mouse.position.ReadValue().y, -10f);
        Vector3 worldPosition = _camera.ScreenToWorldPoint(newTarget);
        transform.position = Vector3.Lerp(transform.position, worldPosition, followSpeed * Time.deltaTime);
    }

    private void CheckLimits()
    {
        
    }

    private void MoveCamera()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 newPos = new Vector3(target.position.x, target.position.y + 5.5f, -10f);
            transform.position = Vector3.Lerp(transform.position, newPos, followSpeed * 2 * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 newPos = new Vector3(target.position.x, target.position.y - 4.5f, -10f);
            transform.position = Vector3.Lerp(transform.position, newPos, followSpeed * 2 * Time.deltaTime);
        }
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 newPos = new Vector3(target.position.x - 5.5f, target.position.y + yOffset, -10f);
            transform.position = Vector3.Lerp(transform.position, newPos, followSpeed * 2 * Time.deltaTime);
        }
        
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 newPos = new Vector3(target.position.x + 5.5f, target.position.y + yOffset, -10f);
            transform.position = Vector3.Lerp(transform.position, newPos, followSpeed * 2 * Time.deltaTime);
        }
    }
}
