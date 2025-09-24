using UnityEngine;

namespace EnemiesScripts
{
    public class EnemyHurtboxZone : MonoBehaviour
    {
        [SerializeField] private EnemyData data;

        private int _hp;
        
        [SerializeField] private EnemyController enemy;

        private void Start()
        {
            _hp = data.baseHp;
            
            GameManager.Instance.GetGameRestarted.AddListener(Restart);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            GetHit();
        }

        private void GetHit()
        {
            _hp -= 1;
            
            if (_hp <= 0) enemy.SetState(EnemyController.State.Die);
            else if (enemy.GetState != EnemyController.State.Attack) enemy.GetHit();
        }

        private void Restart()
        {
            enemy.gameObject.SetActive(true);
            _hp = data.baseHp;
        }
    }
}
