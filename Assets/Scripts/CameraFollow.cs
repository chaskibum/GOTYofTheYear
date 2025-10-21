using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private Transform target;
    [SerializeField] private float yOffset = 3f;
    [SerializeField] private Collider2D limits;
    
    private Camera _camera;
    private Mouse _mouse;

    private float _boundHeight;
    private float _boundWidth;


    private void Awake()
    {
        _camera = Camera.main;
        _mouse = Mouse.current;
    }

    private void Start()
    {
        _boundHeight = _camera.orthographicSize;
        _boundWidth = _boundHeight * _camera.aspect;
    }

    private void LateUpdate()
    {
        if (!target)
        {
            CheckLimitsAndMoveBasedOnMousePosition();
        }
        else
        {
            Vector3 newPos = new Vector3(target.position.x, target.position.y + yOffset, -10f);
            transform.position = Vector3.Lerp(transform.position, newPos, followSpeed * Time.deltaTime);
        }
        
        MoveCamera();
    }

    private void CheckLimitsAndMoveBasedOnMousePosition()
    {
        Vector3 newTarget = new Vector3(_mouse.position.ReadValue().x, _mouse.position.ReadValue().y, -10f);
        Vector3 worldPosition = _camera.ScreenToWorldPoint(newTarget);
        worldPosition.z = -10;
        
        Bounds bounds = limits.bounds;
        
        var minX = bounds.min.x + _boundWidth;
        var maxX = bounds.max.x - _boundWidth;
                
        var minY = bounds.min.y + _boundHeight;
        var maxY = bounds.max.y - _boundHeight;
        
        worldPosition.x = Mathf.Clamp(worldPosition.x, minX, maxX);
        worldPosition.y = Mathf.Clamp(worldPosition.y, minY, maxY);
        
        transform.position = Vector3.Lerp(transform.position, worldPosition, followSpeed * Time.deltaTime);
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
