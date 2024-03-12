namespace AS.Core.Primitives
{
    public abstract class BaseEntity<T>
    {
        public T Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
