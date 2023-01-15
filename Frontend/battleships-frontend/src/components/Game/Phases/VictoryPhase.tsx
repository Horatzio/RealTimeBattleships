import { Heading } from "@chakra-ui/react";
import EndPhase from "./EndPhase";

function VictoryPhase() {
    return <EndPhase content={
    <Heading color={'green'}>You have won!</Heading>
    }/>
}

export default VictoryPhase;