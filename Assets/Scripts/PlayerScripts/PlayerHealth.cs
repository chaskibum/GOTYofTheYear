using System;
using System.Collections;
using GameplayElements;
using Managers;
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

        private GameManager _gm;
        
        private void Start()
        {
            _gm = GameManager.Instance;
            _player = _gm.GetPlayer;
            _data = _player.GetPlayerData;
            
            _hp = _data.baseHp;
            _lives = PlayerPrefs.GetInt("Lives");
            healthBar.SetMaxHealth(_hp);
            
            AddListeners(true);
        }

        private void OnDestroy()
        {
            AddListeners(false);
        }
        
        private void AddListeners(bool add)
        {
            var player = _gm.GetPlayer;

            if (add)
            {
                _gm.GetHpAmountChanged?.AddListener(TakeDamage);
                _gm.GetCollectablePicked?.AddListener(AddExtraLife);
                _gm.GetPlayerRespawn?.AddListener(Respawn);
                _gm.GetGameRestarted?.AddListener(RestartGame);
                player.GetPlayerRevived?.AddListener(Revive);
            }
            else
            {
                _gm.GetHpAmountChanged?.RemoveListener(TakeDamage);
                _gm.GetCollectablePicked?.RemoveListener(AddExtraLife);
                _gm.GetPlayerRespawn?.RemoveListener(Respawn);
                _gm.GetGameRestarted?.RemoveListener(RestartGame);
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
            _gm.GetLifeAmountChanged?.Invoke(_lives);
        }

        private void Revive()
        {
            _hp = _data.baseHp;
            healthBar.SetMaxHealth(_hp);
            _lives = _data.baseLives;
            _gm.GetLifeAmountChanged?.Invoke(_lives);
        }

        private void TakeDamage(int hp)
        {
            healthBar.SetHealth(hp);

            if (hp <= 0)
            {
                _lives -= 1;
                _gm.GetLifeAmountChanged?.Invoke(_lives);
                _gm.GetPlayer.SavePlayerStats();
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
            _gm.GetLifeAmountChanged?.Invoke(_lives);
            _gm.GetPlayer.SavePlayerStats();
        }
        
        public int GetPlayerLives => _lives;
        
        public int GetPlayerBaseLives => _data.baseLives;
    }
}
