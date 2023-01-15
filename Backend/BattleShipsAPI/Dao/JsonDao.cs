using JsonFlatFileDataStore;

namespace BattleShipsAPI
{
    public abstract class JsonDao<T> : IDao<T> where T : class
    {
        private readonly IDocumentCollection<T> collection;

        public JsonDao(DataStore dataStore)
        {
            collection = dataStore.GetCollection<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return collection.AsQueryable();
        }

        public async Task InsertOneAsync(T entity)
        {
            await collection.InsertOneAsync(entity);
        }
        public async Task InsertManyAsync(IEnumerable<T> entities)
        {
            await collection.InsertManyAsync(entities);
        }

        public async Task UpdateOneAsync(Predicate<T> predicate, T entity)
        {
            await collection.UpdateOneAsync(predicate, entity);
        }
        public async Task UpdateOneAsync(dynamic id, T entity)
        {
            await collection.UpdateOneAsync(id, entity);
        }

        public async Task DeleteOneAsync(dynamic id)
        {
            await collection.DeleteOneAsync(id);
        }
        public async Task DeleteOneAsync(Predicate<T> predicate)
        {
            await collection.DeleteOneAsync(predicate);
        }
        public async Task DeleteManyAsync(Predicate<T> predicate)
        {
            await collection.DeleteManyAsync(predicate);
        }
    }
}
