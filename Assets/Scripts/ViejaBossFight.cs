using System.Collections;
using EnemiesScripts;
using GameplayElements;
using Managers;
using UnityEngine;

public class ViejaBossFight : EnemyController
{
    [SerializeField] private Transform[] positions;
    [SerializeField] private FinalBossScript finalBoss;
    private Transform _newPos;
    private Transform _actualPos;

    protected override void Start()
    {
        Body = GetComponent<Rigidbody2D>();
        Visuals = GetComponentInChildren<SpriteRenderer>();
            
        AddListeners(true);
    }

    protected override void Update()
    {
        
    }

    public override void GetHit()
    {
        if (Hp == 10) finalBoss.StartFight();
        Body.AddForce(Direction * data.pushForce, ForceMode2D.Impulse);
            
        AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerAttack, true, 0.8f);

        PlayHitFeedback();
            
        Hp -= 1;
        if (Hp <= 0) 
            print("ded, u win");
        else
            Invoke(nameof(ChangePosition), 0.05f);
    }
    
    protected override IEnumerator EndFeedback()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        Visuals.color = new Color(100, 100, 100);
        Time.timeScale = 1;
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
    }
}
