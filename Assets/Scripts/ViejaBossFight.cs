using System.Collections;
using EnemiesScripts;
using Managers;
using UnityEngine;

public class ViejaBossFight : EnemyController
{
    [SerializeField] private Transform[] positions;
    private Transform _newPos;
    private Transform _actualPos;

    protected override void Start()
    {
        foreach (var pos in positions)
        {
            print(pos.position);
        }
        
        Body = GetComponent<Rigidbody2D>();
        Visuals = GetComponentInChildren<SpriteRenderer>();
        // TryGetComponent(out Animator);
        // 1Player = GameManager.Instance.GetPlayer;
        // if (wallsCollider) wallsCollider.enabled = true;
            
        AddListeners(true);
    }

    protected override void Update()
    {
        
    }

    public override void GetHit()
    {
        Body.AddForce(Direction * data.pushForce, ForceMode2D.Impulse);
            
        AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerAttack, true, 0.8f);

        PlayHitFeedback();
            
        Hp -= 1;
        if (Hp <= 0) print("ded, u win");
        else
        {
            Invoke(nameof(ChangePosition), 0.05f);
        }
    }
    
    protected override IEnumerator EndFeedback()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        Visuals.color = new Color(50, 50, 50);
        Time.timeScale = 1;
    }

    private void ChangePosition()
    {
        _newPos = positions[Random.Range(0, positions.Length)];
        if (_newPos != _actualPos)
        {
            _actualPos = _newPos;
            transform.position = _actualPos.position;
            Visuals.color = Color.white;
        }
        else
        {
            print("justo la misma...");
            ChangePosition();
        }
    }
}
