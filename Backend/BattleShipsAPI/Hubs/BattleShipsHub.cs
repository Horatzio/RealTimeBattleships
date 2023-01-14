using Microsoft.AspNetCore.SignalR;

namespace BattleShipsAPI.Hubs
{
    public class BattleShipsHub : Hub<IBattleShipsClient>
    {
        private readonly GameManager gameManager;

        public BattleShipsHub(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public IDictionary<string, IList<string>> UserClientIdAssociations { get; set; } = new Dictionary<string, IList<string>>();

        private IEnumerable<string> GetConnectionIdsForPlayerId(string playerId)
        {
            if (!UserClientIdAssociations.TryGetValue(playerId, out var connectionIds))
            {
                return Enumerable.Empty<string>();
            }
            return connectionIds;
        }

        private async Task UpdatePlayer(string playerId)
        {
            var connectionIds = GetConnectionIdsForPlayerId(playerId);
            var gameSession = gameManager.GetSession(playerId);
            
            await Task.WhenAll(connectionIds.Select(connectionId => UpdatePlayer(connectionId)));
        }

        public void RegisterConnection(string playerId)
        {
            if (!UserClientIdAssociations.TryGetValue(playerId, out var connectionIds))
            {
                UserClientIdAssociations.Add(playerId, new List<string>());
            }
            else
            {
                connectionIds.Add(Context.ConnectionId);
            }
        }

        public async Task StartGame(string player1Id, string player2Id)
        {
            await gameManager.StartGame(player1Id, player2Id);
            await Task.WhenAll(UpdatePlayer(player1Id), UpdatePlayer(player2Id));
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);

            await Parallel.ForEachAsync(UserClientIdAssociations, (kvp, cancelToken) =>
            {
                var connectionIds = kvp.Value;

                var index = connectionIds.IndexOf(Context.ConnectionId);
                if (index >= 0)
                {
                    connectionIds.RemoveAt(index);
                }
                return ValueTask.CompletedTask;
            });
        }
    }
}
