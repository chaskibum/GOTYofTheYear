namespace EnemiesScripts
{
    public class SneakyBushScript : EnemyController
    {
        protected override void Attack()
        {
            if (!CanAttack) return;
            AudioManager.Instance.PlayClip(AudioManager.AudioList.BushAttack);
            base.Attack();
        }

        public override void GetHit()
        {
            base.GetHit();
            if (Hp <= 0) AudioManager.Instance.PlayClip(AudioManager.AudioList.BushDeath);
        }
    }
}
