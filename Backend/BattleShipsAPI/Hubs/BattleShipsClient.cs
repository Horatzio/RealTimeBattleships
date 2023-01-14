using BattleShipsAPI.GameSession;

namespace BattleShipsAPI.Hubs
{
    public interface IBattleShipsClient
    {
        Task GameSessionUpdate(GameSessionDto gameSessionDto);
    }
}
