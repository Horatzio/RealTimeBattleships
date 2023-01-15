using BattleShipsAPI.GameSession;
using BattleShipsAPI.GameSession.Robot;

namespace BattleShipsAPI
{
    public class GameManager
    {
        private readonly IDao<GameSessionModel> gameSessionDao;
        private readonly IDao<FiredShotsModel> firedShotsDao;
        private readonly IDao<ShipPlacementModel> shipPlacementDao;
        private readonly IShipFactory shipFactory;
        private readonly UserManager userManager;
        private readonly IGameSessionDtoFactory gameSessionDtoFactory;
        private readonly ICheaterRobotBrain robotBrain;

        public GameManager(IDao<GameSessionModel> gameSessionDao, IDao<FiredShotsModel> firedShotsDao, IDao<ShipPlacementModel> shipPlacementDao,
            IShipFactory shipFactory, UserManager userManager, IGameSessionDtoFactory gameSessionDtoFactory, ICheaterRobotBrain robotBrain)
        {
            this.gameSessionDao = gameSessionDao;
            this.firedShotsDao = firedShotsDao;
            this.shipPlacementDao = shipPlacementDao;
            this.userManager = userManager;
            this.shipFactory = shipFactory;
            this.gameSessionDtoFactory = gameSessionDtoFactory;
            this.robotBrain = robotBrain;
        }

        public async Task<GameSessionDto> GetSessionAsync(string playerId)
        {
            var session = gameSessionDao.GetAll()
                   .FirstOrDefault(s => s.PlayerId == playerId);

            if (session == null)
            {
                return null;
            }

            var getPlayerShipPlacement = Task.Run(() => shipPlacementDao.GetAll()
                    .FirstOrDefault(s => s.SessionId == session.Id && s.PlayerId == playerId));
            var getPlayerShots = Task.Run(() => firedShotsDao.GetAll()
                .FirstOrDefault(s => s.SessionId == session.Id && s.PlayerId == playerId));
            var getEnemyShots = Task.Run(() => firedShotsDao.GetAll()
                .FirstOrDefault(s => s.SessionId == session.Id && s.PlayerId == session.RobotId));
            var getEnemyPlacement = Task.Run(() => shipPlacementDao.GetAll()
                .FirstOrDefault(s => s.SessionId == session.Id && s.PlayerId == session.RobotId));

            await Task.WhenAll(getPlayerShipPlacement, getPlayerShots, getEnemyShots, getEnemyPlacement);

            var playerShipPlacement = await getPlayerShipPlacement;
            var playerShots = await getPlayerShots;
            var enemyShots = await getEnemyShots;
            var enemyPlacement = await getEnemyPlacement;

            return gameSessionDtoFactory.Create(session, playerShipPlacement, playerShots, enemyShots, enemyPlacement);
        }

        public async Task StartGame(string playerId)
        {
            var robotId = robotBrain.GetRobotId();
            var sessionId = Guid.NewGuid().ToString();

            await Task.WhenAll(new[] {
                gameSessionDao.InsertOneAsync(new GameSessionModel()
                {
                    Id = sessionId,
                    PlayerId = playerId,
                    RobotId = robotId,
                    CurrentPhase = GamePhase.Setup
                }),
                firedShotsDao.InsertManyAsync(new[] {
                new FiredShotsModel()
                {
                    SessionId = sessionId,
                    PlayerId = playerId
                },
                new FiredShotsModel()
                {
                    SessionId = sessionId,
                    PlayerId = robotId
                },
            }),
                CreateShipPositions(sessionId, playerId, robotId)
            });
        }

        private async Task CreateShipPositions(string sessionId, string playerId, string robotId)
        {
            await shipPlacementDao.InsertManyAsync(new[] {
                new ShipPlacementModel()
                {
                    SessionId = sessionId,
                    PlayerId = playerId,
                    Ships = shipFactory.Create()
                },
                new ShipPlacementModel()
                {
                    SessionId = sessionId,
                    PlayerId = robotId,
                    Ships = shipFactory.Create()
                },
            });
        }

        public async Task SubmitPlayerPositions(string playerId, string sessionId, ShipPositions[] playerPositions)
        {
            if (!playerPositions.Any(p => p.Positions.Length > 0)) return;

            var session = gameSessionDao.GetAll()
                .FirstOrDefault(s => s.PlayerId == playerId);

            var shipPlacement = shipPlacementDao.GetAll()
                .FirstOrDefault(s => s.SessionId == session.Id && s.PlayerId == playerId);

            shipPlacement.Ships = shipPlacement.Ships.Select((s, i) =>
            {
                s.Positions = playerPositions[i].Positions;
                return s;
            }).ToArray();
            await shipPlacementDao.UpdateOneAsync(s => s.SessionId == session.Id && s.PlayerId == playerId, shipPlacement);


            var enemyShipPlacement = robotBrain.CalculateShipPlacement(shipPlacement);
            enemyShipPlacement.SessionId = session.Id;
            enemyShipPlacement.PlayerId = session.RobotId;
            await shipPlacementDao.UpdateOneAsync(s => s.SessionId == session.Id && s.PlayerId == session.RobotId, enemyShipPlacement);


            session.CurrentPhase = GamePhase.Fight;
            await gameSessionDao.UpdateOneAsync(sessionId, session);
        }

        public async Task PlayerShot(string playerId, string sessionId, int position)
        {
            var session = gameSessionDao.GetAll()
                .FirstOrDefault(s => s.PlayerId == playerId);

            FiredShotsModel playerShots = await UpdatePlayerShots(playerId, sessionId, position, session);

            var enemyShots = firedShotsDao.GetAll()
                .FirstOrDefault(s => s.SessionId == session.Id && s.PlayerId == session.RobotId);

            int nextEnemyShot = robotBrain.CalculateNextShot(session.BoardSize, enemyShots);

            enemyShots = await UpdateEnemyShots(sessionId, session.RobotId, nextEnemyShot, enemyShots);

            bool hasPlayerWon = VerifyWinCondition(playerId, sessionId, playerShots);

            if (hasPlayerWon)
            {
                session.CurrentPhase = GamePhase.Victory;
                await gameSessionDao.UpdateOneAsync(sessionId, session);
                return;
            }

            bool hasRobotWon = VerifyRobotWinCondition(playerId, sessionId, enemyShots);

            if (hasRobotWon)
            {
                session.CurrentPhase = GamePhase.Loss;
                await gameSessionDao.UpdateOneAsync(sessionId, session);
            }
        }

        private bool VerifyRobotWinCondition(string playerId, string sessionId, FiredShotsModel enemyShots)
        {
            var playerShipPlacement = shipPlacementDao.GetAll()
                .FirstOrDefault(s => s.SessionId == sessionId && s.PlayerId == playerId);

            var allPlayerPositions = playerShipPlacement.Ships.Aggregate(new List<int>(), (list, s) =>
            {
                list.AddRange(s.Positions);
                return list;
            });

            bool hasRobotWon = !allPlayerPositions.Except(enemyShots.Positions).Any();
            return hasRobotWon;
        }

        private bool VerifyWinCondition(string playerId, string sessionId, FiredShotsModel playerShots)
        {
            var robotShipPlacement = shipPlacementDao.GetAll()
                .FirstOrDefault(s => s.SessionId == sessionId && s.PlayerId == playerId);

            var allRobotPositions = robotShipPlacement.Ships.Aggregate(new List<int>(), (list, s) =>
            {
                list.AddRange(s.Positions);
                return list;
            });

            var hasPlayerWon = !allRobotPositions.Except(playerShots.Positions).Any();
            return hasPlayerWon;
        }

        private async Task<FiredShotsModel> UpdatePlayerShots(string playerId, string sessionId, int position, GameSessionModel session)
        {
            var playerShots = firedShotsDao.GetAll()
                .FirstOrDefault(s => s.SessionId == session.Id && s.PlayerId == playerId);
            playerShots.Positions.Add(position);

            await firedShotsDao.UpdateOneAsync((s) => s.SessionId == sessionId && s.PlayerId == playerId, playerShots);
            return playerShots;
        }

        private async Task<FiredShotsModel> UpdateEnemyShots(string sessionId, string robotId, int nextEnemyShot, FiredShotsModel enemyShots)
        {
            enemyShots.Positions.Add(nextEnemyShot);

            await firedShotsDao.UpdateOneAsync((s) => s.SessionId == sessionId && s.PlayerId == robotId, enemyShots);
            return enemyShots;
        }

        public async Task EndSession(string sessionId)
        {
            var session = gameSessionDao.GetAll()
                .FirstOrDefault(s => s.Id == sessionId);

            await Task.WhenAll(new[] {
                firedShotsDao.DeleteManyAsync(s => s.SessionId == session.Id),
                shipPlacementDao.DeleteManyAsync(s => s.SessionId == session.Id),
                userManager.DeleteUserAsync(session.RobotId)
                });

            await gameSessionDao.DeleteOneAsync(session.Id);
        }
    }
}
