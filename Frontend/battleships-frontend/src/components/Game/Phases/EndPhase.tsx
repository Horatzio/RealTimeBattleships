import { Button, Card, CardBody, CardFooter } from "@chakra-ui/react";
import { battleShipsApi } from "../../../api/battleships-api";
import { GameContext } from "../GameContext";
import { ReactElement, useContext } from "react";
import Loading from "../../Loading";

interface EndPhaseProps {
    content: ReactElement
}

function EndPhase({ content }: EndPhaseProps) {
    const { gameSession, refreshGameSession } = useContext(GameContext);

    if (!gameSession) {
        return <Loading />
    }

    async function onEndSession() {
        await battleShipsApi.endSession(gameSession!.id)
        refreshGameSession();
    }
    
    return (<Card height={'fit-content'}>
            <CardBody>
                {content}
            </CardBody>
            <CardFooter placeContent={'center'}>
                <Button onClick={onEndSession}>
                    End Session
                </Button>
            </CardFooter>
        </Card>)
}

export default EndPhase;