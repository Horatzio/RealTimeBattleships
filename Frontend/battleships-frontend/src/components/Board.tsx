import { Box, SimpleGrid } from "@chakra-ui/react";
import { Position } from "../api/position";

interface BoardProps {
    columns: number,
    onBoxClick: (position: Position) => {},
    positions: Position[]
}

function Board({ columns, positions, onBoxClick }: BoardProps) {
    return (
        <SimpleGrid columns={columns}>
            {positions.map(p => (
                <Box border={'gray'} height={'40px'} width={'40px'} borderWidth={'5px'} onClick={() => onBoxClick(p)}/>
            ))}
        </SimpleGrid>
    )
}

export default Board;