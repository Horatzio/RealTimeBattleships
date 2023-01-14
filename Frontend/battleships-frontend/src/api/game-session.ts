export interface GameSession {
    id: string,
    playerId: string,
    boardSize: number,
    currentPhase: GamePhase,

    playerShipLengths: number[]
    playerShipPositions: number[]
    playerShotPositions: number[]
    enemyShotPositions: number[]
    revealedEnemyPositions: number[]
}

export interface ShipPlacement {
    ships: ShipPositions[]
}

export interface ShipPositions {
    positions: number[],
    length: number
}

export enum GamePhase {
    Setup = 0,
    Fight = 1,
    Victory = 2,
    Loss = 3
}