import { GamePhase } from "../../api/game-session";
import PlacementPhase from "./Phases/PlacementPhase";
import FightPhase from "./Phases/FightPhase";
import { GameContext } from "./GameContext";
import { useContext } from "react";
import VictoryPhase from "./Phases/VictoryPhase";
import LossPhase from "./Phases/LossPhase";
import Loading from "../Loading";

function GameDisplay() {
    const { gameSession } = useContext(GameContext);

    if (!gameSession) {
        return <Loading />
    }

    switch (gameSession.currentPhase) {
        case (GamePhase.Setup): {
            return <PlacementPhase />
        }
        case (GamePhase.Fight): {
            return <FightPhase />
        }
        case (GamePhase.Victory): {
            return <VictoryPhase />
        }
        case (GamePhase.Loss): {
            return <LossPhase />
        }
    }
}

export default GameDisplay;