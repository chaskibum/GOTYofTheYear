using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
        Dialog,
        ExtraLifeGrabbed,
        PlayerWalk,
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

    public void PlayStepSound(AudioList clip)
    {
        StopClip();
        sfxSource.pitch = Random.Range(0.9f, 1.1f);
        sfxSource.PlayOneShot(audioClips[(int)clip]);
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
