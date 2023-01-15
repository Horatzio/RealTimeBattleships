import { Heading } from "@chakra-ui/react";
import EndPhase from "./EndPhase";

function LossPhase() {
    return <EndPhase content={
        <Heading color={'red'}>You have lost.</Heading>
    }/>
}

export default LossPhase;