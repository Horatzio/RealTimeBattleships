using BattleShipsAPI.GameSession;
using JsonFlatFileDataStore;

namespace BattleShipsAPI
{
    public class GameManager
    {
        private readonly IDocumentCollection<GameSessionModel> sessionCollection;
        private readonly IDocumentCollection<FiredShotsModel> firedShotsCollection;
        private readonly IDocumentCollection<ShipPlacementModel> shipPlacementsCollection;
        private readonly IShipFactory shipFactory;
        private readonly Random random;

        public GameManager(DataStore dataStore, IShipFactory shipFactory)
        {
            sessionCollection = dataStore.GetCollection<GameSessionModel>();
            firedShotsCollection = dataStore.GetCollection<FiredShotsModel>();
            shipPlacementsCollection = dataStore.GetCollection<ShipPlacementModel>();
            this.shipFactory = shipFactory;
            random = new Random();
        }

        public GameSessionDto GetSession(string playerId)
        {
            var session = sessionCollection.AsQueryable()
                   .FirstOrDefault(s => s.PlayerId == playerId);

            if (session == null)
            {
                return null;
            }

            var shipPlacement = shipPlacementsCollection.AsQueryable()
                    .FirstOrDefault(s => s.SessionId == session.Id && s.PlayerId == playerId);

            var playerShipPositions = shipPlacement.Ships.Aggregate(new List<int>(), (list, s) => {
                list.AddRange(s.Positions);
                return list;
            });

            var playerShots = firedShotsCollection.AsQueryable()
                .FirstOrDefault(s => s.SessionId == session.Id && s.PlayerId == playerId);
            var enemyShots = firedShotsCollection.AsQueryable()
                .FirstOrDefault(s => s.SessionId == session.Id && s.PlayerId == session.RobotId);

            var enemyPlacement = shipPlacementsCollection.AsQueryable()
                .FirstOrDefault(s => s.SessionId == session.Id && s.PlayerId == session.RobotId);

            var revealedEnemyPositions = enemyPlacement.Ships.Aggregate(new List<int>(), (list, s) => {
               if (s.Positions.Any(p => playerShots.Positions.Contains(p)))
                {
                    list.AddRange(s.Positions);
                }
                return list;
            });

            var sessionDto = new GameSessionDto()
            {
                Id = session.Id,
                BoardSize = session.BoardSize,
                PlayerId = session.PlayerId,
                CurrentPhase = session.CurrentPhase,
                PlayerShipLengths = shipPlacement.Ships.Select(s => s.Length).ToArray(),
                PlayerShipPositions = playerShipPositions.ToArray(),
                PlayerShotPositions = playerShots.Positions.ToArray(),
                EnemyShotPositions = enemyShots.Positions.ToArray(),
                RevealedEnemyPositions = revealedEnemyPositions.ToArray()
            };

            return sessionDto;
        }

        public async Task StartGame(string playerId)
        {
            var robotId = Guid.NewGuid().ToString();
            var sessionId = Guid.NewGuid().ToString();

            await Task.WhenAll(new[] {
                sessionCollection.InsertOneAsync(new GameSessionModel()
                {
                    Id = sessionId,
                    PlayerId = playerId,
                    RobotId = robotId,
                    CurrentPhase = GamePhase.Setup
                }),
                firedShotsCollection.InsertManyAsync(new[] {
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
            await shipPlacementsCollection.InsertManyAsync(new[] {
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
            // await SetupRobotShipPositions(robotId);
        }

        //private async Task SetupRobotShipPositions(string robotId)
        //{

        //}

        public async Task SubmitPlayerPositions(string playerId, string sessionId, ShipPositions[] playerPositions)
        {
            if (!playerPositions.Any(p => p.Positions.Length > 0)) return;

            var session = sessionCollection.AsQueryable()
                .FirstOrDefault(s => s.PlayerId == playerId);

            var shipPlacement = shipPlacementsCollection.AsQueryable()
                .FirstOrDefault(s => s.SessionId == session.Id && s.PlayerId == playerId);

            shipPlacement.Ships = shipPlacement.Ships.Select((s, i) =>
            {
                s.Positions = playerPositions[i].Positions;
                return s;
            }).ToArray();

            await shipPlacementsCollection.UpdateOneAsync(s => s.SessionId == session.Id && s.PlayerId == playerId, shipPlacement);

            await CopyRobotPositionsFromPlayer(session.Id, session.RobotId, playerPositions);

            session.CurrentPhase = GamePhase.Fight;
            await sessionCollection.UpdateOneAsync(sessionId, session);
        }

        private async Task CopyRobotPositionsFromPlayer(string sessionId, string robotId, ShipPositions[] playerPositions)
        {
            var shipPlacement = shipPlacementsCollection.AsQueryable()
                .FirstOrDefault(s => s.SessionId == sessionId && s.PlayerId == robotId);

            shipPlacement.Ships = shipPlacement.Ships.Select((s, i) =>
            {
                s.Positions = playerPositions[i].Positions;
                return s;
            }).ToArray();
            await shipPlacementsCollection.UpdateOneAsync(s => s.SessionId == sessionId && s.PlayerId == robotId, shipPlacement);
        }

        public async Task PlayerShot(string playerId, string sessionId, int position)
        {
            var session = sessionCollection.AsQueryable()
                .FirstOrDefault(s => s.PlayerId == playerId);

            FiredShotsModel playerShots = await UpdatePlayerShots(playerId, sessionId, position, session);

            var enemyShots = firedShotsCollection.AsQueryable()
                .FirstOrDefault(s => s.SessionId == session.Id && s.PlayerId == session.RobotId);

            int nextEnemyShot = GetNextEnemyShot(session, enemyShots);

            enemyShots = await UpdateEnemyShots(sessionId, session.RobotId, nextEnemyShot, enemyShots);

            bool hasPlayerWon = VerifyWinCondition(playerId, sessionId, playerShots);

            if (hasPlayerWon)
            {
                session.CurrentPhase = GamePhase.Victory;
                await sessionCollection.UpdateOneAsync(sessionId, session);
                return;
            }

            bool hasRobotWon = VerifyRobotWinCondition(playerId, sessionId, enemyShots);

            if (hasRobotWon)
            {
                session.CurrentPhase = GamePhase.Loss;
                await sessionCollection.UpdateOneAsync(sessionId, session);
            }
        }

        private bool VerifyRobotWinCondition(string playerId, string sessionId, FiredShotsModel enemyShots)
        {
            var playerShipPlacement = shipPlacementsCollection.AsQueryable()
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
            var robotShipPlacement = shipPlacementsCollection.AsQueryable()
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
            var playerShots = firedShotsCollection.AsQueryable()
                .FirstOrDefault(s => s.SessionId == session.Id && s.PlayerId == playerId);
            playerShots.Positions.Add(position);

            await firedShotsCollection.UpdateOneAsync((s) => s.SessionId == sessionId && s.PlayerId == playerId, playerShots);
            return playerShots;
        }
        private int GetNextEnemyShot(GameSessionModel session, FiredShotsModel enemyShots)
        {
            var maxPosition = session.BoardSize * session.BoardSize;
            int nextEnemyShot;

            do
            {
                nextEnemyShot = random.Next(maxPosition);
            }
            while (enemyShots.Positions.Contains(nextEnemyShot));
            return nextEnemyShot;
        }

        private async Task<FiredShotsModel> UpdateEnemyShots(string sessionId, string robotId, int nextEnemyShot, FiredShotsModel enemyShots)
        {
            enemyShots.Positions.Add(nextEnemyShot);

            await firedShotsCollection.UpdateOneAsync((s) => s.SessionId == sessionId && s.PlayerId == robotId, enemyShots);
            return enemyShots;
        }
    }
}
