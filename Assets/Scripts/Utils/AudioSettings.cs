using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Utils
{
    public class AudioSettings : MonoBehaviour
    {
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider musicSlider;

        private bool _isLoading = true;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    
        void Start()
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
            StartCoroutine(EnableSoundAfterLoading());
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
    
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _isLoading = true;
            StartCoroutine(EnableSoundAfterLoading());
        }
    
        private IEnumerator EnableSoundAfterLoading()
        {
            yield return new WaitForSeconds(0.2f);
            _isLoading = false;
        }

        public void SetPlayerSfxPref()
        {
            PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        }
        
        public void SetPlayerMusicPref()
        {
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        }
    
        public bool GetIsLoading => _isLoading;
    }
}
