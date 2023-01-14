import './App.css';
import GameManager from './components/GameManager';
import { ChakraProvider } from '@chakra-ui/react';

function App() {
  return (
    <ChakraProvider>
      <div className="App">
        <GameManager />
      </div>
    </ChakraProvider>
  );
}

export default App;
