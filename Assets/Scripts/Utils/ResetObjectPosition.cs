using Managers;
using UnityEngine;

namespace Utils
{
    public class ResetObjectPosition : MonoBehaviour
    {
        private Vector3 _originalPosition;
        [SerializeField] private SpriteRenderer sprite;
        
        private GameManager _gm;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _gm = GameManager.Instance;
            _originalPosition = transform.position;
            AddListeners(true);
        }

        private void OnDestroy()
        {
            AddListeners(false);
        }

        private void AddListeners(bool add)
        {
            if (add)
            {
                _gm.GetGameRestarted?.AddListener(ResetPosition);
                _gm.GetPlayerRespawn?.AddListener(ResetPosition);
                _gm.GetPlayer.GetPlayerRevived?.AddListener(ResetPosition);
                _gm.GetResetObjects?.AddListener(ResetPosition);
            }
            else
            {
                _gm.GetGameRestarted?.RemoveListener(ResetPosition);
                _gm.GetPlayerRespawn?.RemoveListener(ResetPosition);
                _gm.GetPlayer.GetPlayerRevived?.RemoveListener(ResetPosition);
                _gm.GetResetObjects?.RemoveListener(ResetPosition);
            }
        }

        private void Update()
        {
            sprite.transform.eulerAngles += new Vector3(0, 0, 0.5f);
        }

        private void ResetPosition()
        {
            transform.position = _originalPosition;
        }
    }
}
