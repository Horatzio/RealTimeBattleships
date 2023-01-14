import { useEffect, useState } from "react";
import { GameSession } from "../api/game-session";
import { battleShipsApi } from "../api/battleships-api";
import { HStack, Card, CardHeader, CardBody } from "@chakra-ui/react";
import Board from "./Board";

interface GameDisplayProps {
    username: string,
    gameSession: GameSession
}


function GameDisplay({ username, gameSession }: GameDisplayProps) {
    const [enemyName, setEnemyName] = useState('');
    const columns = gameSession.boardSize;

    useEffect(() => {
        async function GetEnemyName() {
            const name = await battleShipsApi.getName(gameSession.enemyPlayerId);
            setEnemyName(name);
        }
        GetEnemyName();
    }, [gameSession])

    return (
        <HStack spacing={"50px"}>
            <Card>
                <CardHeader>
                {username}'s Board (Yours)
                </CardHeader>
                <CardBody>
                    <Board columns={columns}/>
                </CardBody>
            </Card>
            <Card>
                <CardHeader>
                {enemyName}'s Board
                </CardHeader>
                <CardBody>
                    <Board columns={columns}/>
                </CardBody>
            </Card>
        </HStack>
    );
}

export default GameDisplay;