public interface IDamagable
{
    public void TakeDamage(int damage);
    public EventableValue<int> CurrentHP { get; }
}
