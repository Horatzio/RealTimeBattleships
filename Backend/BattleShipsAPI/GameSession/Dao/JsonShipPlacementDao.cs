using JsonFlatFileDataStore;

namespace BattleShipsAPI.GameSession.Dao
{
	public class JsonShipPlacementDao : JsonDao<ShipPlacementModel>
	{
		public JsonShipPlacementDao(DataStore dataStore) : base(dataStore)
		{ }
	}
}
