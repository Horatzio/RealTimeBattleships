import { ReactNode, createContext, useEffect, useState } from "react"
import { battleShipsApi } from "../api/battleships-api";

interface UserState {
    username: string | null,
    userId: string | null,
    onLogin: (username: string) => any
}

const defaultUserState = {
    username: null,
    userId: null,
    onLogin: () => { throw new Error('UserContext not initialized') }
}

const UserContext = createContext<UserState>(defaultUserState)

const USER_ID = 'BATTLESHIPS_USER_ID';
const USERNAME = 'BATTLESHIPS_USERNAME';

interface UserContextProviderProps {
    children: ReactNode
}

function UserContextProvider({ children }: UserContextProviderProps) {
    const [userId, setUserId] = useState<string | null>(null);
    const [username, setUsername] = useState('');

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
  
        GetUserIdFromLocalStorage();
        GetUsernameFromLocalStorage();
      }, [])

    async function onLogin(username: string) {
        const userId = await battleShipsApi.login(username);
        if (!userId) return;

        localStorage.setItem(USER_ID, userId);
        localStorage.setItem(USERNAME, username);

        setUserId(userId);
        setUsername(username);
    }

    return (<UserContext.Provider value={{
        userId,
        username,
        onLogin
    }}>
        {children}
    </UserContext.Provider>)
}

export { UserContext, UserContextProvider }