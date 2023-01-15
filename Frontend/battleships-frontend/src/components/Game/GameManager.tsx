import { useContext } from "react";
import GameDisplay from "./GameDisplay";
import GameStarter from "./GameStarter";
import { GameContext, GameContextProvider } from "./GameContext";
import { withBasicProvider } from "../../helper/withBasicProvider";

function GameManager() {
    const { gameSession } = useContext(GameContext);

    if (!gameSession) {
      return <GameStarter />
    }

    return <GameDisplay />
}

export default withBasicProvider(GameContextProvider)(GameManager);