using System.Collections.Generic;
using UnityEngine;

public class ParallaxBehaviour : MonoBehaviour
{
    [SerializeField] private List<ParallaxLayer> layers;
    private Transform _camera;
    private Vector3 _lastCameraPosition;
    // private float _lastCameraPositionX;

    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layer;
        [Range(0f, 1f)] public float stillness;
        public bool lockY = true;
    }

    private void Start()
    {
        if (Camera.main) _camera = Camera.main.transform;
        _lastCameraPosition = _camera.position;
        // _lastCameraPositionX = _camera.position.x;
    }

    private void LateUpdate()
    {
        Vector3 cameraDelta = _camera.position - _lastCameraPosition;
        
        foreach (var layer in layers)
        {
            var moveX = cameraDelta.x * layer.stillness;
            var moveY = cameraDelta.y * layer.stillness;
            
            if (!layer.lockY) layer.layer.position += new Vector3(moveX, moveY, 0);
            else layer.layer.position += new Vector3(moveX, 0, 0);
        }
        _lastCameraPosition = _camera.position;
    }

/*
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
*/
}
