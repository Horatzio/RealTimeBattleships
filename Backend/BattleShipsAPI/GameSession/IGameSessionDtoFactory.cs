namespace BattleShipsAPI.GameSession
{
	public interface IGameSessionDtoFactory
	{
		GameSessionDto Create(GameSessionModel session, ShipPlacementModel playerShipPlacement, FiredShotsModel playerShots, FiredShotsModel enemyShots, ShipPlacementModel enemyPlacement);
	}

	public class GameSessionDtoFactory : IGameSessionDtoFactory
	{
		public GameSessionDto Create(GameSessionModel session, ShipPlacementModel playerShipPlacement, FiredShotsModel playerShots, FiredShotsModel enemyShots, ShipPlacementModel enemyPlacement)
		{
			var playerShipPositions = playerShipPlacement.Ships.Aggregate(new List<int>(), (list, s) =>
			{
				list.AddRange(s.Positions);
				return list;
			});

			var revealedEnemyPositions = enemyPlacement.Ships.Aggregate(new List<int>(), (list, s) =>
			{
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
				PlayerShipLengths = playerShipPlacement.Ships.Select(s => s.Length).ToArray(),
				PlayerShipPositions = playerShipPositions.ToArray(),
				PlayerShotPositions = playerShots.Positions.ToArray(),
				EnemyShotPositions = enemyShots.Positions.ToArray(),
				RevealedEnemyPositions = revealedEnemyPositions.ToArray()
			};

			return sessionDto;
		}
	}
}
