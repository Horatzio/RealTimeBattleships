using BattleShipsAPI.GameSession;
using BattleShipsAPI.User;
using JsonFlatFileDataStore;

namespace BattleShipsAPI
{
    public class GameManager
    {
        private readonly IDocumentCollection<GameSessionModel> sessionCollection;
        private readonly IDocumentCollection<FiredShotsModel> firedShotsCollection;
        private readonly IDocumentCollection<ShipPositionsModel> shipPositionsCollection;
        private readonly UserManager userManager;

        public GameManager(DataStore dataStore, UserManager userManager)
        {
            sessionCollection = dataStore.GetCollection<GameSessionModel>();
            firedShotsCollection = dataStore.GetCollection<FiredShotsModel>();
            shipPositionsCollection = dataStore.GetCollection<ShipPositionsModel>();
            this.userManager = userManager;
        }

        public GameSessionDto GetSession(string playerId)
        {
            var session = sessionCollection.AsQueryable()
                   .FirstOrDefault(s => s.Player1Id == playerId || s.Player2Id == playerId);

            if (session == null)
            {
                return null;
            }

            var sessionDto = new GameSessionDto()
            {
                Id = session.Id,
                BoardSize = session.BoardSize,
            };

            if (session.GameState == GameSessionState.Setup)
            {
                sessionDto.GameState = ClientGameSessionState.Setup;
            }

            if (session.Player1Id == playerId) {
                return CurrentPlayerIsPlayer1(sessionDto, session);
            } else {
                return CurrentPlayerIsPlayer2(sessionDto, session);
            }
        }

        private GameSessionDto CurrentPlayerIsPlayer1(GameSessionDto sessionDto, GameSessionModel session)
        {
            sessionDto.CurrentPlayerId = session.Player1Id;
            sessionDto.EnemyPlayerId = session.Player2Id;

            if (session.GameState == GameSessionState.Player1Shot)
            {
                sessionDto.GameState = ClientGameSessionState.CurrentPlayerShot;
            }
            else if (session.GameState == GameSessionState.Player2Shot)
            {
                sessionDto.GameState = ClientGameSessionState.EnemyPlayerShot;
            }
            else if (session.GameState == GameSessionState.Player1Victory)
            {
                sessionDto.GameState = ClientGameSessionState.CurrentPlayerVictory;
            } else if (session.GameState == GameSessionState.Player2Victory) {
                sessionDto.GameState = ClientGameSessionState.EnemyPlayerVictory;
            }

            return sessionDto;
        }

        private GameSessionDto CurrentPlayerIsPlayer2(GameSessionDto sessionDto, GameSessionModel session)
        {
            sessionDto.CurrentPlayerId = session.Player1Id;
            sessionDto.EnemyPlayerId = session.Player2Id;

            if (session.GameState == GameSessionState.Player1Shot)
            {
                sessionDto.GameState = ClientGameSessionState.EnemyPlayerShot;
            }
            else if (session.GameState == GameSessionState.Player2Shot)
            {
                sessionDto.GameState = ClientGameSessionState.CurrentPlayerShot;
            }
            else if (session.GameState == GameSessionState.Player1Victory)
            {
                sessionDto.GameState = ClientGameSessionState.EnemyPlayerVictory;
            }
            else if (session.GameState == GameSessionState.Player2Victory)
            {
                sessionDto.GameState = ClientGameSessionState.CurrentPlayerVictory;
            }

            return sessionDto;
        }

        public IEnumerable<UserModel> GetAvailablePlayers()
        {
            var usersInGameIds = sessionCollection.AsQueryable()
                   .Aggregate(new List<string>(), (list, session) =>
                   {
                       list.Add(session.Player1Id);
                       list.Add(session.Player2Id);
                       return list;
                   })
                   .Distinct();

            return userManager.GetAllUsers()
                    .Where(u => !usersInGameIds.Contains(u.Id));
        }

        public async Task StartGame(string player1Id, string player2Id)
        {
            await sessionCollection.InsertOneAsync(new GameSessionModel()
            {
                Player1Id = player1Id,
                Player2Id = player2Id
            });

            await firedShotsCollection.InsertManyAsync(new [] {
                new FiredShotsModel()
                {
                    PlayerId = player1Id
                },
                new FiredShotsModel()
                {
                    PlayerId = player2Id
                },
            });

            await shipPositionsCollection.InsertManyAsync(new[] {
                new ShipPositionsModel()
                {
                    PlayerId = player1Id
                },
                new ShipPositionsModel()
                {
                    PlayerId = player2Id
                },
            });
        }
    }
}
