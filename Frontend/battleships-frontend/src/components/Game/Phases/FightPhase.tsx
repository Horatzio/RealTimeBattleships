import { HStack, Card, CardHeader, CardBody } from "@chakra-ui/react";
import { battleShipsApi } from "../../../api/battleships-api";
import Board from "../Board";
import { UserContext } from "../../UserContext";
import { GameContext } from "../GameContext";
import { useContext } from "react";
import Loading from "../../Loading";

function FightPhase() {
    const { username } = useContext(UserContext);
    const { gameSession, refreshGameSession } = useContext(GameContext)

    if (!gameSession) {
        return <Loading />
    }

    const { playerId, id, boardSize } = gameSession;
    const sessionId = id; 
    const { playerShotPositions, playerShipPositions, enemyShotPositions, revealedEnemyPositions } = gameSession;

    async function onEnemyBoardClick(position: number) {
        await battleShipsApi.playerShot(playerId, sessionId, position)
        refreshGameSession();
    }

    return (
        <>
            <HStack spacing={"50px"}>
                <Card>
                    <CardHeader>
                    {username}'s Board (Yours)
                    </CardHeader>
                    <CardBody>
                        <Board shipColor={'green.400'} size={boardSize} 
                        shotPostions={enemyShotPositions} shipPostions={playerShipPositions}/>
                    </CardBody>
                </Card>
                <Card>
                    <CardHeader>
                    Enemy's Board
                    </CardHeader>
                    <CardBody>
                        <Board shipColor={'red.200'} size={boardSize} onBoxClick={onEnemyBoardClick}
                        shotPostions={playerShotPositions} shipPostions={revealedEnemyPositions}/>
                    </CardBody>
                </Card>
            </HStack>
        </>
    );
}

export default FightPhase;