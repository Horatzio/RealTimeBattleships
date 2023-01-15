using JsonFlatFileDataStore;

namespace BattleShipsAPI.GameSession.Dao
{
    public class JsonGameSessionDao : JsonDao<GameSessionModel>
    {
        public JsonGameSessionDao(DataStore dataStore) : base(dataStore)
        { }
    }
}
