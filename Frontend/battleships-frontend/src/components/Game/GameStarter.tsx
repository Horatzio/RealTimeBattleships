import { useCallback, useContext, useState } from "react";
import { Card, CardBody, Button } from "@chakra-ui/react";
import { UserContext } from "../UserContext";
import { GameContext } from "./GameContext";
import { battleShipsApi } from "../../api/battleships-api";
import Loading from "../Loading";

function GameStarter () {
    const { userId } = useContext(UserContext);
    const { refreshGameSession } = useContext(GameContext);

    const [isLoading, setIsLoading] = useState(false);

    const onStartGame = useCallback(() => {
        if (!userId) return userId;
        setIsLoading(true);
        battleShipsApi.startGame(userId)
          .then(refreshGameSession)
      }, [userId, refreshGameSession])

    if (isLoading) {
        return <Loading />
    }

    const onClick = () => {
        setIsLoading(true)
        onStartGame()
    }

    return (
        <Card height={'fit-content'}>
            <CardBody>
                <Button colorScheme={'red'} onClick={onClick}>
                    Fight
                </Button>
            </CardBody>
        </Card>
    );
}

export default GameStarter;