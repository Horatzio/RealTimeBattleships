namespace BattleShipsAPI.GameSession.Robot
{
	public class RudimentaryRobotBrain : ICheaterRobotBrain
	{
		private readonly Random random;
		public RudimentaryRobotBrain()
		{
			random = new Random();
		}

		public string GetRobotId()
		{
			var robotId = Guid.NewGuid().ToString();
			return $"RUDROBO-{robotId}";
		}

		public ShipPlacementModel CalculateShipPlacement(ShipPlacementModel playerShipPlacementModel)
		{
			var shipPlacement = new ShipPlacementModel();
			shipPlacement.Ships = playerShipPlacementModel.Ships.Select((s, i) =>
			{
				s.Positions = playerShipPlacementModel.Ships[i].Positions;
				return s;
			}).ToArray();

			return shipPlacement;
		}

		public int CalculateNextShot(int boardSize, FiredShotsModel enemyShots)
		{
			var maxPosition = boardSize * boardSize;
			int nextEnemyShot;

			do
			{
				nextEnemyShot = random.Next(maxPosition);
			}
			while (enemyShots.Positions.Contains(nextEnemyShot));
			return nextEnemyShot;
		}
	}
}
