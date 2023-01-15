using JsonFlatFileDataStore;

namespace BattleShipsAPI
{
	public class BattleShipsDataStoreFactory
	{
		private const string DataDirectory = "Data";
		private const string DataFileName = "battleships-data.json";

		public static DataStore Create()
		{
			var currentDirectory = Directory.GetCurrentDirectory();
			return new DataStore(Path.Combine(currentDirectory, DataDirectory, DataFileName));
		}
	}
}
