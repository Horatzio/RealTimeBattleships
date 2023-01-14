import { useState } from "react";
import { Card, CardBody, Button, Spinner } from "@chakra-ui/react";

interface GameStarterProps {
    onStartGame: () => any
}

function GameStarter ({ onStartGame }: GameStarterProps) {
    const [isLoading, setIsLoading] = useState(false);

    if (isLoading) {
        return <Spinner size={'lg'} colorScheme='white' />
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