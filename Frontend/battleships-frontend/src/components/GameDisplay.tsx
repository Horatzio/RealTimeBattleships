import { GamePhase, GameSession, ShipPositions } from "../api/game-session";
import { HStack, Card, CardHeader, CardBody } from "@chakra-ui/react";
import Board from "./Board";
import PlacementBoard from "./PlacementBoard";
import { battleShipsApi } from "../api/battleships-api";

interface GameDisplayProps {
    username: string,
    gameSession: GameSession,
    refreshGameSession: () => any
}


function GameDisplay({ username, gameSession, refreshGameSession }: GameDisplayProps) {

    const shipLengths = gameSession.playerShipLengths;

    async function onFinishedPlacing(shipPostions: number[][]) {
        const ships = Array.from(Array(gameSession.playerShipLengths.length).keys()).map(() => ({} as ShipPositions));
        for(let i = 0 ; i < ships.length; i++) {
            ships[i].positions = shipPostions[i];
        }
        await battleShipsApi.submitPlayerPositions(gameSession.playerId, gameSession.id, ships);
        refreshGameSession();
    }

    if (gameSession.currentPhase === GamePhase.Setup) {
        return (
        <Card height={'fit-content'}>
            <CardHeader>
                {username}'s Board (Yours) - Place your ships
            </CardHeader>
            <CardBody>
                <PlacementBoard boardSize={gameSession.boardSize} shipLengths={shipLengths} onFinishedPlacing={onFinishedPlacing}/>
            </CardBody>
        </Card>)
    }

    async function onEnemyBoardClick(position: number) {
        await battleShipsApi.playerShot(gameSession.playerId, gameSession.id, position)
        refreshGameSession();
    }

    return (
        <HStack spacing={"50px"}>
            <Card>
                <CardHeader>
                {username}'s Board (Yours)
                </CardHeader>
                <CardBody>
                    <Board shipColor={'green.400'} size={gameSession.boardSize} 
                    shotPostions={gameSession.enemyShotPositions} shipPostions={gameSession.playerShipPositions}/>
                </CardBody>
            </Card>
            <Card>
                <CardHeader>
                Enemy's Board
                </CardHeader>
                <CardBody>
                    <Board shipColor={'red.200'} size={gameSession.boardSize} onBoxClick={onEnemyBoardClick}
                    shotPostions={gameSession.playerShotPositions} shipPostions={gameSession.revealedEnemyPositions}/>
                </CardBody>
            </Card>
        </HStack>
    );
}

export default GameDisplay;