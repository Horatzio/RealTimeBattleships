import { useState, useEffect, useCallback } from "react";
import UserLogin from "./UserLogin";
import { battleShipsApi } from "../api/battleships-api";
import { Spinner } from "@chakra-ui/react";
import { GameSession } from "../api/game-session";
import GameDisplay from "./GameDisplay";
import GameStarter from "./GameStarter";

const USER_ID = 'BATTLESHIPS_USER_ID';
const USERNAME = 'BATTLESHIPS_USERNAME';

function GameManager() {
    const [userId, setUserId] = useState<string | null>(null);
    const [username, setUsername] = useState('');
    const [isLoading, setIsLoading] = useState(true);
    
    const [gameSession, setGameSession] = useState<GameSession | null>(null);
    
    useEffect(() => {
      async function GetUserIdFromLocalStorage() {
        const localStorageUserId = localStorage.getItem(USER_ID);
        if (localStorageUserId) {
          setUserId(localStorageUserId)
        }
      }
  
      async function GetUsernameFromLocalStorage() {
        const localStorageUsername = localStorage.getItem(USERNAME);
        if (localStorageUsername) {
          setUsername(localStorageUsername);
        }
      }

      setIsLoading(true);
      Promise.allSettled([
        GetUserIdFromLocalStorage(),
        GetUsernameFromLocalStorage()
      ]).then(() => setIsLoading(false))
    }, [])

    const getGameSession = useCallback(() => {
      if (!userId) return;

      async function GetGameSession() {
        const gameSesion = await battleShipsApi.getGameSession(userId!);
        setGameSession(gameSesion);
      }

      setIsLoading(true)
      GetGameSession()
        .then(() => setIsLoading(false))
    }, [userId])

    useEffect(() => {
      getGameSession()
    }, [userId, getGameSession])

    async function onLogin(username: string) {
      const userId = await battleShipsApi.login(username);
      if (!userId) return;

      localStorage.setItem(USER_ID, userId);
      localStorage.setItem(USERNAME, username);

      setUserId(userId);
      setUsername(username);
    }

    const onStartGame = useCallback(() => {
      if (!userId) return userId;
      setIsLoading(true);
      battleShipsApi.startGame(userId)
        .then(getGameSession)
    }, [userId, getGameSession])

    if (isLoading) {
      return <Spinner size={'lg'} colorScheme='white' />
    }

    if (!userId || !username) {
      return <UserLogin onLogin={onLogin} />
    }

    if (!gameSession) {
      return <GameStarter onStartGame={onStartGame}/>
    }

    return <GameDisplay username={username} gameSession={gameSession} refreshGameSession={() => getGameSession()}/>
}

export default GameManager;