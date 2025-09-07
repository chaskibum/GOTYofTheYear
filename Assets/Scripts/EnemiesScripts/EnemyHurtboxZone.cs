using UnityEngine;

namespace EnemiesScripts
{
    public class EnemyHurtboxZone : MonoBehaviour
    {
        [SerializeField] private EnemyData data;

        private int _hp;
        private float _moveSpeed;

        private void Start()
        {
            _hp = data.baseHp;
            _moveSpeed = data.moveSpeed;
            
            GameManager.Instance.GetGameRestarted.AddListener(() => Restart());
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            GetHit();
        }

        private void GetHit()
        {
            _hp -= 1;
            
            if(_hp <= 0) gameObject.SetActive(false);
        }

        private void Restart()
        {
            gameObject.SetActive(true);
            _hp = data.baseHp;
        }
    }
}
