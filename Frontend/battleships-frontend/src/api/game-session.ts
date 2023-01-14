export interface GameSession {
    id: string,
    currentPlayerId: string,
    enemyPlayerId: string,
    boardSize: number,
}