using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
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
    }
}
