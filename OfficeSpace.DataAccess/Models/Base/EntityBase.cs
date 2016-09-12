namespace OfficeSpace.DataAccess.Models
{
    public abstract class EntityBase : IEntity
    {
        public int Id { get; set; }
    }

    public abstract class EntityBase<TKey> : EntityBase, IEntity<TKey>
    {
        public new TKey Id { get; set; }
    }
}
