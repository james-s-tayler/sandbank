namespace Core
{
    public abstract class ViewModel<T, TKey> where T : DomainEntity<TKey>
    {
        public TKey Id { get; }
        
        public ViewModel(T entity)
        {
            Id = entity.Id;
        }
    }
}