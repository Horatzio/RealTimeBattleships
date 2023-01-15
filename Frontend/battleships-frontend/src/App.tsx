import { useContext } from 'react';
import './App.css';
import GameManager from './components/Game/GameManager';
import { ChakraProvider } from '@chakra-ui/react';
import { UserContext, UserContextProvider } from './components/UserContext';
import UserLogin from './components/UserLogin';
import { withBasicProvider } from './helper/withBasicProvider';

function App() {
  const { userId, username, onLogin } = useContext(UserContext);

  return (
    <ChakraProvider>
      <div className="App">
        { 
        (userId && username) ? 
          <GameManager /> : 
          <UserLogin onLogin={onLogin}/>
        }
      </div>
    </ChakraProvider>
  );
}

export default withBasicProvider(UserContextProvider)(App);
