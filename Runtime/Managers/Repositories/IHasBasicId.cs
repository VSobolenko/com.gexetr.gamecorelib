namespace Game.Repositories
{
public interface IHasBasicId<T>
{
    T Id { get; set; }
}
}