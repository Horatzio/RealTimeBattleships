namespace BattleShipsAPI.GameSession
{
	public class GameSessionModel
	{
		public string Id { get; set; }
		public string PlayerId { get; set; }
		public string RobotId { get; set; }
		public int BoardSize { get; set; } = DefaultColumnsNumber;

		public const int DefaultColumnsNumber = 10;
		public GamePhase CurrentPhase { get; set; }
	}

	public enum GamePhase
	{
		Setup = 0,
		Fight = 1,
		Victory = 2,
		Loss = 3,
	}
}
