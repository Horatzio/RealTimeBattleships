import { Card, CardHeader, CardBody } from "@chakra-ui/react";
import { battleShipsApi } from "../../../api/battleships-api";
import { ShipPositions } from "../../../api/game-session";
import PlacementBoard from "./PlacementBoard";
import { GameContext } from "../GameContext";
import { useContext } from "react";
import Loading from "../../Loading";
import { UserContext } from "../../UserContext";

function PlacementPhase () {
    const { username } = useContext(UserContext);
    const { gameSession, refreshGameSession } = useContext(GameContext);

    if (!gameSession) {
        return <Loading />
    }

    const { id, playerId, boardSize, playerShipLengths } = gameSession;
    const sessionId = id;

    async function onFinishedPlacing(shipPostions: number[][]) {
        const ships = Array.from(Array(playerShipLengths.length).keys()).map(() => ({} as ShipPositions));
        for(let i = 0 ; i < ships.length; i++) {
            ships[i].positions = shipPostions[i];
        }
        await battleShipsApi.submitPlayerPositions(playerId, sessionId, ships);
        refreshGameSession();
    }

    return (
        <Card height={'fit-content'}>
            <CardHeader>
                {username}'s Board (Yours) - Place your ships
            </CardHeader>
            <CardBody>
                <PlacementBoard boardSize={boardSize} shipLengths={playerShipLengths} onFinishedPlacing={onFinishedPlacing} />
            </CardBody>
        </Card>)
}

export default PlacementPhase;