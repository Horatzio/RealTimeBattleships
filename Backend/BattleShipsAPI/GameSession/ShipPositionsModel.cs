﻿namespace BattleShipsAPI.GameSession
{
    public class ShipPositionsModel
    {
        public string SessionId { get; set; }
        public string PlayerId { get; set; }
        public List<(int, int)> Positions { get; set; } = new List<(int, int)>();
    }
}
