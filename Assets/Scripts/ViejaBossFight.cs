using System.Collections;
using DG.Tweening;
using EnemiesScripts;
using Managers;
using UnityEngine;

public class ViejaBossFight : EnemyController
{
    [Header("Audio")]
    [SerializeField] private AudioSource lastHitSound;
    [SerializeField] private AudioSource possessedSound;
    
    [SerializeField] private Transform[] positions;
    [SerializeField] private FinalBossScript finalBoss;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private GameObject viejaEnding;
    private Transform _newPos;
    private Transform _actualPos;

    protected override void Start()
    {
        Body = GetComponent<Rigidbody2D>();
        Visuals = GetComponentInChildren<SpriteRenderer>();
        
        possessedSound.volume = 0.4f;
            
        AddListeners(true);
    }

    public override void GetHit()
    {
        if (Hp == data.baseHp)
        {
            finalBoss.StartFight();
            possessedSound.volume = 1f;
        }
        Body.AddForce(Direction * data.pushForce, ForceMode2D.Impulse);
        
        AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerAttack, true, 0.8f);

        PlayHitFeedback();
            
        Hp -= 1;
        if (Hp <= 0)
        {
            finalBoss.GetAnimator.SetTrigger("Die");
            deathParticles.Play();
            GameManager.Instance.GetGameWonEvent.Invoke();
            AudioManager.Instance.PlayClip(AudioManager.AudioList.CalderoDeath);
            Camera.main.DOShakePosition(1.8f, 1.2f);
            possessedSound.volume = 0f;
            AudioManager.Instance.finalBossMusic.volume = 0f;
            lastHitSound.Play();
            Invoke(nameof(EndGame), 3f);
            finalBoss.speed = 0f;
        }
        else
            Invoke(nameof(ChangePosition), 0.05f);
    }

    private void EndGame()
    {
        viejaEnding.SetActive(true);
        finalBoss.DeactivateCoroutine();
        Time.timeScale = 1f;
    }
    
    protected override IEnumerator EndFeedback()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        Visuals.color = new Color(100, 100, 100);
        if (Hp <= 0)
            Time.timeScale = 0.3f;
        else
            Time.timeScale = 1;
    }

    protected override void GetPlayerPosition()
    {
        
    }

    private void ChangePosition()
    {
        if (Hp == 1)
            _newPos = positions[^1];
        else 
            _newPos = positions[Random.Range(0, positions.Length - 1)];
        
        if (_newPos != _actualPos)
        {
            _actualPos = _newPos;
            transform.position = _actualPos.position;
            Visuals.color = Color.white;
        }
        else
            ChangePosition();
    }

    protected override void Respawn()
    {
        transform.position = StartingPosition;
        Hp = data.baseHp;
        Body.linearVelocity = Vector2.zero;
        Visuals.color = Color.white;
        possessedSound.volume = 0.4f;
    }
}
