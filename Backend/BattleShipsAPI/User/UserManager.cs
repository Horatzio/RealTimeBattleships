using BattleShipsAPI.User;
using JsonFlatFileDataStore;

namespace BattleShipsAPI
{
    public class UserManager
    {
        private readonly IDocumentCollection<UserModel> userCollection;

        public UserManager(DataStore dataStore)
        {
           userCollection = dataStore.GetCollection<UserModel>();
        }

        public async Task<string> CreateOrGetId(string username)
        {
            var existingUser = userCollection.AsQueryable()
                .Where(u => u.Name == username)
                .FirstOrDefault();

            if (existingUser == null)
            {
                var newUser = new UserModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = username
                };

                await userCollection.InsertOneAsync(newUser);
                return newUser.Id.ToString();
            }

            return existingUser.Id.ToString();
        }

        private const string DefaultUsername = "Player";

        public async Task<string> GetUsername(string userId)
        {
            var user = userCollection.AsQueryable()
                .FirstOrDefault(u => u.Id == userId);

            return user?.Name ?? DefaultUsername;
        }

        public IEnumerable<UserModel> GetAllUsers() => userCollection.AsQueryable();
    }
}
