namespace Core
{
    public abstract class CreateModel<T, TKey> where T : DomainEntity<TKey>
    {
        public abstract T ToDomainModel();
    }
}