namespace BattleShipsAPI.GameSession.Robot
{
    public interface ICheaterRobotBrain : IRobotBrain
    {
        ShipPlacementModel CalculateShipPlacement(ShipPlacementModel playerShipPlacementModel);
    }
}
