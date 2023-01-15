namespace BattleShipsAPI.GameSession.Robot
{
	public interface IRobotBrain
	{
		string GetRobotId();
		int CalculateNextShot(int boardSize, FiredShotsModel enemyShots);
	}
}
