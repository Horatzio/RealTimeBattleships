namespace BattleShipsAPI.GameSession
{
    public class GameSessionModel
    {
        public string Id { get; set; }
        public string Player1Id { get; set; }
        public string Player2Id { get; set; }
        public int BoardSize { get; set; } = DefaultColumnsNumber;

        public const int DefaultColumnsNumber = 10;

        public GameSessionState GameState;
    }

    public enum GameSessionState
    {
        Setup,
        Player1Shot,
        Player2Shot,
        Player1Victory,
        Player2Victory
    }
}
