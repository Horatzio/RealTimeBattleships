using JsonFlatFileDataStore;

namespace BattleShipsAPI.User
{
    public class JsonUserDao : JsonDao<UserModel>
    {
        public JsonUserDao(DataStore dataStore) : base(dataStore)
        { }
    }
}
