using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using AudioSettings = Utils.AudioSettings;
using Random = UnityEngine.Random;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;

        [SerializeField] private AudioSettings settings;

        // [SerializeField] private GameObject mainMenuOptionsPanel;
    
        public enum AudioList
        {
            MainCheckpointActivated,
            FireCheckpointActivated,
            Dialog1,
            Dialog2,
            ExtraLifeGrabbed,
            PlayerWalk1,
            PlayerWalk2,
            PlayerJump,
            PlayerHit,
            PlayerDeath,
            PlayerAttack,
            PlayerMissedAttack,
            Dash,
            ArmorAttack,
            ArmorHit,
            ArmorDeath,
            BushAttack,
            BushDeath,
            CrowScream,
            BushAlert,
        }
    
        [SerializeField] List<AudioClip> audioClips;

    
        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void PlayClip(AudioList clip, bool changePitch = false, float volume = 1f, bool oneShot = true)
        {
            sfxSource.volume = volume;
            sfxSource.pitch = changePitch ? Random.Range(0.9f, 1.1f) : 1f;
            if (oneShot)
                sfxSource.PlayOneShot(audioClips[(int)clip]);
            else
            {
                sfxSource.clip = audioClips[(int)clip];
                sfxSource.Play();
            }
            sfxSource.loop = oneShot;
        }

        public void PlayStepSound()
        {
            var clip = RandomStep();
            sfxSource.pitch = Random.Range(0.9f, 1.1f);
            sfxSource.PlayOneShot(audioClips[clip]);
        }

        private int RandomStep()
        {
            return Random.Range(5, 7);
        }
    
        public void PlayDialogSound()
        {
            var clip = RandomDialog();
            sfxSource.pitch = Random.Range(0.9f, 1.1f);
            sfxSource.PlayOneShot(audioClips[clip]);
        }

        private int RandomDialog()
        {
            return Random.Range(2, 4);
        }

        public void StopClip()
        {
            sfxSource.Stop();
            sfxSource.loop = false;
        }

        public void ChangeMusicVolume()
        {
            if (sfxSlider.value < -39f) audioMixer.SetFloat("MusicVolume", -60f);
            else audioMixer.SetFloat("MusicVolume", sfxSlider.value);
        }

        public void ChangeSfxVolume()
        {
            if (sfxSlider.value < -39f) audioMixer.SetFloat("SFXVolume", -60f);
            else audioMixer.SetFloat("SFXVolume", sfxSlider.value);

            if (!settings.GetIsLoading) PlayClip(AudioList.PlayerAttack, true, 1f, false);
        }
    }
}
