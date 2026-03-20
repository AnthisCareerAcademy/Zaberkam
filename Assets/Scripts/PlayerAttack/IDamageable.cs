namespace Interfaces
{
    public interface IDamageable
    {
        public bool IsPlayer { get; }
        public void TakeDamage(float damage);
    }
}
