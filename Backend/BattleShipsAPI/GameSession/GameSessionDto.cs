namespace BattleShipsAPI.GameSession
{
    public class GameSessionDto
    {
        public string Id { get; set; }
        public string CurrentPlayerId { get; set; }
        public string EnemyPlayerId { get; set; }
        public int BoardSize { get; set; }

        public ClientGameSessionState GameState;
    }

    public enum ClientGameSessionState
    {
        Setup = 0,
        CurrentPlayerShot = 1,
        EnemyPlayerShot = 2,
        CurrentPlayerVictory = 3,
        EnemyPlayerVictory = 4
    }
}
