using UnityEngine;

namespace PlayerScripts
{
    public class PlayerHealth : MonoBehaviour
    {
        private PlayerController _player;
        private PlayerData _data;
        [SerializeField] private HealthBar healthBar;
        
        private int _hp;
        private int _lives;
        
        private void Start()
        {
            _player = GameManager.Instance.GetPlayer;
            _data = _player.GetPlayerData;
            
            _hp = _data.baseHp;
            _lives = _data.baseLives;
            healthBar.SetMaxHealth(_hp);
            
            GameManager.Instance.GetHpAmountChanged?.AddListener(TakeDamage);
            GameManager.Instance.GetPlayerRespawn?.AddListener(Respawn);
            GameManager.Instance.GetGameRestarted?.AddListener(Restart);
        }

        private void Respawn()
        {
            _hp = _data.baseHp;
            healthBar.SetMaxHealth(_hp);
            healthBar.SetHealth(_hp);
        }

        private void Restart()
        {
            _hp = _data.baseHp;
            healthBar.SetMaxHealth(_hp);
            _lives = _data.baseLives;
        }

        private void TakeDamage(int hp)
        {
            healthBar.SetHealth(hp);

            if (hp <= 0)
            {
                _lives -= 1;
                GameManager.Instance.GetLifeAmountChanged?.Invoke(_lives);
            }
        }
    }
}
