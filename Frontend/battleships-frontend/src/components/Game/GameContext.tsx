import { ReactNode, createContext, useCallback, useContext, useEffect, useState } from "react";
import { GameSession } from "../../api/game-session";
import { battleShipsApi } from "../../api/battleships-api";
import { UserContext } from "../UserContext";

interface GameState {
    username: string | null,
    gameSession: GameSession | null,
    refreshGameSession: () => any
}

const defaultGameState = {
    username: null,
    gameSession: null,
    refreshGameSession: () => { throw new Error('GameContext not intialized')}
}

const GameContext = createContext<GameState>(defaultGameState);

interface GameContextProviderProps {
    children: ReactNode
}

function GameContextProvider({ children }: GameContextProviderProps) {
    const { userId, username } = useContext(UserContext);
    const [gameSession, setGameSession] = useState<GameSession | null>(null)

    const getGameSession = useCallback(() => {
        if (!userId) return;

        async function GetGameSession() {
            const gameSesion = await battleShipsApi.getGameSession(userId!);
            setGameSession(gameSesion);
        }

        GetGameSession()
    }, [userId])
  
    useEffect(() => {
        getGameSession()
    }, [userId, getGameSession])

    return <GameContext.Provider value={{
        username,
        gameSession,
        refreshGameSession: () => getGameSession()
    }}>
        {children}
    </GameContext.Provider>
}

export { GameContext, GameContextProvider }