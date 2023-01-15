using JsonFlatFileDataStore;

namespace BattleShipsAPI.GameSession.Dao
{
    public class JsonFiredShotsDao : JsonDao<FiredShotsModel>
    {
        public JsonFiredShotsDao(DataStore dataStore) : base(dataStore)
        { }
    }
}
