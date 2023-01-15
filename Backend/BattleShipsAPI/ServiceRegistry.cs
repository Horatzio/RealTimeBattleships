using BattleShipsAPI.GameSession;
using BattleShipsAPI.GameSession.Dao;
using BattleShipsAPI.GameSession.Robot;
using BattleShipsAPI.User;

namespace BattleShipsAPI
{
    public static class ServiceRegistry
    {
        public static IServiceCollection AddBattleshipServices(this IServiceCollection services)
        {
            services.AddSingleton((_) => BattleShipsDataStoreFactory.Create());

            services.AddSingleton<IDao<UserModel>, JsonUserDao>();
            services.AddSingleton<UserManager>();

            services.AddSingleton<IDao<GameSessionModel>, JsonGameSessionDao>();
            services.AddSingleton<IDao<ShipPlacementModel>, JsonShipPlacementDao>();
            services.AddSingleton<IDao<FiredShotsModel>, JsonFiredShotsDao>();
            services.AddSingleton<IShipFactory, DefaultShipLineupFactory>();
            services.AddSingleton<IGameSessionDtoFactory, GameSessionDtoFactory>();
            services.AddSingleton<ICheaterRobotBrain, RudimentaryRobotBrain>();
            services.AddSingleton<GameManager>();

            return services;
        }
    }
}
