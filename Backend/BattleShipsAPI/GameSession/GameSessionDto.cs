namespace BattleShipsAPI.GameSession
{
    public class GameSessionDto
    {
        public string Id { get; set; }
        public string PlayerId { get; set; }
        public int BoardSize { get; set; }
        public GamePhase CurrentPhase { get; set; }

        public int[] PlayerShipLengths { get; set; }
        public int[] PlayerShipPositions { get; set; }
        public int[] PlayerShotPositions { get; set; }
        public int[] EnemyShotPositions { get; set; }
        public int[] RevealedEnemyPositions { get; set; }
    }
}
