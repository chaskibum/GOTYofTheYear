using System;
using System.Collections;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerHealth : MonoBehaviour
    {
        private PlayerController _player;
        [SerializeField] private SpriteRenderer visuals;
        private PlayerData _data;
        public HealthBar healthBar;
        
        private int _hp;
        private int _lives;
        
        private void Start()
        {
            _player = GameManager.Instance.GetPlayer;
            _data = _player.GetPlayerData;
            
            _hp = _data.baseHp;
            _lives = PlayerPrefs.GetInt("Lives");
            healthBar.SetMaxHealth(_hp);
            
            AddListeners(true);
        }

        private void OnDisable()
        {
            AddListeners(false);
        }
        
        private void AddListeners(bool add)
        {
            var player = GameManager.Instance.GetPlayer;

            if (add)
            {
                GameManager.Instance.GetHpAmountChanged?.AddListener(TakeDamage);
                GameManager.Instance.GetCollectablePicked?.AddListener(AddExtraLife);
                GameManager.Instance.GetPlayerRespawn?.AddListener(Respawn);
                GameManager.Instance.GetGameRestarted?.AddListener(RestartGame);
                player.GetPlayerRevived?.AddListener(Revive);
            }
            else
            {
                GameManager.Instance.GetHpAmountChanged?.RemoveListener(TakeDamage);
                GameManager.Instance.GetCollectablePicked?.RemoveListener(AddExtraLife);
                GameManager.Instance.GetPlayerRespawn?.RemoveListener(Respawn);
                GameManager.Instance.GetGameRestarted?.RemoveListener(RestartGame);
                player.GetPlayerRevived?.RemoveListener(Revive);
            }
        }

        private void Respawn()
        {
            _hp = _data.baseHp;
            healthBar.SetMaxHealth(_hp);
            healthBar.SetHealth(_hp);
        }

        private void RestartGame()
        {
            _hp = _data.baseHp;
            healthBar.SetMaxHealth(_hp);
            _lives = _data.startingLives;
            _data.baseLives = _data.startingLives;
            GameManager.Instance.GetLifeAmountChanged?.Invoke(_lives);
        }

        private void Revive()
        {
            _hp = _data.baseHp;
            healthBar.SetMaxHealth(_hp);
            _lives = _data.baseLives;
            GameManager.Instance.GetLifeAmountChanged?.Invoke(_lives);
        }

        private void TakeDamage(int hp)
        {
            healthBar.SetHealth(hp);

            if (hp <= 0)
            {
                _lives -= 1;
                GameManager.Instance.GetLifeAmountChanged?.Invoke(_lives);
                GameManager.Instance.GetPlayer.SavePlayerStats();
            }
            else
            {
                PlayHitFeedback();
            }
        }
        
        private void PlayHitFeedback()
        {
            visuals.color = new Color(5, 5, 5);
            Time.timeScale = 0;
            StartCoroutine(nameof(EndFeedback));
        }

        private IEnumerator EndFeedback()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            visuals.color = Color.white;
            Time.timeScale = 1;
        }

        private void AddExtraLife()
        {
            _lives += 1;
            _data.baseLives += 1;
            GameManager.Instance.GetLifeAmountChanged?.Invoke(_lives);
            GameManager.Instance.GetPlayer.SavePlayerStats();
        }
        
        public int GetPlayerLives => _lives;
        
        public int GetPlayerBaseLives => _data.baseLives;
    }
}
