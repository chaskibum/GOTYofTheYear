using Unity.VisualScripting;
using UnityEngine;

namespace EnemiesScripts
{
    public class EnemyHurtboxZone : MonoBehaviour
    {
        [SerializeField] private EnemyData data;

        private int _hp;
        
        private GameObject _parent;

        private void Start()
        {
            _hp = data.baseHp;

            _parent = transform.parent.gameObject;
            GameManager.Instance.GetGameRestarted.AddListener(() => Restart());
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            GetHit();
        }

        private void GetHit()
        {
            _hp -= 1;
            
            if (_hp <= 0) _parent.SetActive(false);
            // else _parent.GetComponent<EnemyController>().SetState(EnemyController.State.GetHit);
            else if (_parent.GetComponent<EnemyController>().GetState == EnemyController.State.GetHit) return;
            
            _parent.GetComponent<EnemyController>().GetHit();
            print("GOT HIT");
        }

        private void Restart()
        {
            _parent.SetActive(true);
            _hp = data.baseHp;
        }
    }
}
