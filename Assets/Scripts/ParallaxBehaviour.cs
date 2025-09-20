using UnityEngine;

public class ParallaxBehaviour : MonoBehaviour
{
    [SerializeField] float parallaxStillness;
    [SerializeField] bool lockY;

    private Transform _camera;
    private Vector3 _lastCameraPosition;
    private float _lastCameraPositionX;

    private void Start()
    {
        if (Camera.main) _camera = Camera.main.transform;
        _lastCameraPosition = _camera.position;
        _lastCameraPositionX = _camera.position.x;
    }

    private void Update()
    {
        if (!lockY)
        {
            Vector3 newPosition = _camera.position - _lastCameraPosition;
            transform.position += newPosition * parallaxStillness;
            _lastCameraPosition = _camera.position;
        }
        else
        {
            float newPositionX = _camera.position.x - _lastCameraPositionX;
            
            var vector3 = transform.position;
            vector3.x = vector3.x + newPositionX * parallaxStillness;
            transform.position = vector3;
            
            _lastCameraPositionX = _camera.position.x;
        }
    }
}
