namespace BattleShipsAPI.GameSession
{
    public class DefaultShipFactory : IShipFactory
    {
        private const int DestroyerLength = 4;
        private const int BattleshipLength = 5;

        public ShipPositions[] Create()
        {
            var firstDestroyer = new ShipPositions()
            {
                Length = DestroyerLength,
                Positions = new int[DestroyerLength]
            };

            var secondDestroyer = new ShipPositions()
            {
                Length = DestroyerLength,
                Positions = new int[DestroyerLength]
            };

            var battleship = new ShipPositions()
            {
                Length = BattleshipLength,
                Positions = new int[BattleshipLength]
            };

            return new ShipPositions[]
            {
                firstDestroyer,
                secondDestroyer,
                battleship
            };
        }
    }
}
