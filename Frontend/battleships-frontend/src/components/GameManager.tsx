import { useState, useEffect } from "react";
import UserLogin from "./UserLogin";
import { battleShipsApi } from "../api/battleships-api";
import { Card, Spinner, Text } from "@chakra-ui/react";
import { GameSession } from "../api/game-session";
import GameDisplay from "./GameDisplay";
import GameStarter from "./GameStarter";
import { battleShipsHub } from "../api/battleships-hub";

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

      Promise.allSettled([
        GetUserIdFromLocalStorage(),
        GetUsernameFromLocalStorage()
      ]).then(() => setIsLoading(false))

    }, [])

    useEffect(() => {
      if (!userId) return;

      async function GetGameSession() {
        const gameSesion = await battleShipsApi.getGameSession(userId!);
        setGameSession(gameSesion);
      }

      async function ConnectToHub() {
        await battleShipsHub.connect(userId!)
        battleShipsHub.onGameSessionUpdate((gameSession: GameSession) => setGameSession(gameSession))
      }

      ConnectToHub().then(() => GetGameSession())

    }, [userId])

    async function onLogin(username: string) {
      const userId = await battleShipsApi.login(username);
      if (!userId) return;

      localStorage.setItem(USER_ID, userId);
      localStorage.setItem(USERNAME, username);

      setUserId(userId);
      setUsername(username);
    }

    if (isLoading) {
      return <Spinner size={'lg'} colorScheme='white' />
    }

    if (!userId || !username) {
      return <UserLogin onLogin={onLogin} />
    }

    if (!gameSession) {
      return <>
        <Card>
          <Text>{username}</Text>
        </Card>
        <GameStarter userId={userId}/>
      </>
    }

    return <GameDisplay username={username} gameSession={gameSession}/>
}

export default GameManager;