using BattleShipsAPI.User;

namespace BattleShipsAPI
{
    public class UserManager
    {
        private readonly IDao<UserModel> userDao;

        public UserManager(IDao<UserModel> userDao)
        {
            this.userDao = userDao;
        }

        public async Task<string> CreateOrGetIdAsync(string username)
        {
            var existingUser = userDao.GetAll()
                .Where(u => u.Name == username)
                .FirstOrDefault();

            if (existingUser == null)
            {
                var newUser = new UserModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = username
                };

                await userDao.InsertOneAsync(newUser);
                return newUser.Id.ToString();
            }

            return existingUser.Id.ToString();
        }

        public async Task DeleteUserAsync(string userId)
        {
            await userDao.DeleteOneAsync(userId);
        }
    }
}
