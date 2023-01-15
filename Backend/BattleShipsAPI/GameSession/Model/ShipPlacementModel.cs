namespace BattleShipsAPI.GameSession
{
	public class ShipPlacementModel
	{
		public string SessionId { get; set; }
		public string PlayerId { get; set; }
		public ShipPositions[] Ships { get; set; }
	}

	public class ShipPositions
	{
		public int[] Positions { get; set; }
		public int Length { get; set; }
	}
}
