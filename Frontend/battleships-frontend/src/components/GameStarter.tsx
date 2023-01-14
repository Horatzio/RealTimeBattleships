import { useCallback, useEffect, useState } from "react";
import { battleShipsApi } from "../api/battleships-api";
import { User } from "../api/user";
import { Card, CardBody, CardHeader, Stack, StackDivider, Text, Button, HStack, Spinner } from "@chakra-ui/react";
import { battleShipsHub } from "../api/battleships-hub";

interface GameStarterProps {
    userId: string
}

function GameStarter ({ userId }: GameStarterProps) {
    const [players, setPlayers] = useState<User[]>([]);
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
      async function GetAvailablePlayers() {
        const availablePlayers = await battleShipsApi.getAvailablePlayers();

        setPlayers(availablePlayers.filter(p => p.id !== userId))
      }

      setIsLoading(true);
      GetAvailablePlayers().then(() => setIsLoading(false));
    }, [userId])
    
    async function FightPlayer(userId: string, enemy: User) {
        await battleShipsHub.startGame(userId, enemy.id);
    }

    const onFightPlayer = useCallback((player: User) => {
        setIsLoading(true);
        FightPlayer(userId, player).then(() => window.location.reload())
    }, [userId])

    if (isLoading) {
        return <Spinner size={'lg'} colorScheme='white' />
    }

    return (
        <Card height={'fit-content'}>
            <CardHeader>
                Choose a player to fight!
            </CardHeader>
            <CardBody>
                <Stack divider={<StackDivider/>}>
                    {players.map((p: User) =>(
                            <Card>
                                <CardBody>
                                    <HStack spacing={"20px"}>
                                        <Text fontWeight={900}>
                                            {p.name}
                                        </Text>
                                        <Button colorScheme={'red'} onClick={() => onFightPlayer(p)}>
                                            Fight
                                        </Button>
                                    </HStack>
                                </CardBody>
                            </Card>
                        ))}
                </Stack>
            </CardBody>
        </Card>
    );
}

export default GameStarter;