namespace BattleShipsAPI
{
    public interface IDao<T>
    {
        IEnumerable<T> GetAll();
        Task InsertOneAsync(T entity);
        Task InsertManyAsync(IEnumerable<T> entities);

        Task UpdateOneAsync(Predicate<T> predicate, T entity);
        Task UpdateOneAsync(dynamic id, T entity);

        Task DeleteOneAsync(dynamic id);
        Task DeleteOneAsync(Predicate<T> predicate);
        Task DeleteManyAsync(Predicate<T> predicate);
    }
}
