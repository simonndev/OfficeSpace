namespace OfficeSpace.DataAccess.Models
{
    public interface IEntity
    {
        int Id { get; }
    }

    interface IEntity<TKey> : IEntity
    {
        new TKey Id { get; }
    }
}
