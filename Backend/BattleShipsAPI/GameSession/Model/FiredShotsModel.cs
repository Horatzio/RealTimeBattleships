namespace BattleShipsAPI.GameSession
{
	public class FiredShotsModel
	{
		public string SessionId { get; set; }
		public string PlayerId { get; set; }
		public List<int> Positions { get; set; } = new List<int>();
	}
}
