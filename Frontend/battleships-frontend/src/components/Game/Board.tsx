import { Box, SimpleGrid } from "@chakra-ui/react";
import { useCallback } from "react";
interface BoardProps {
    size: number,
    onBoxClick?: (position: number) => {},
    shipColor: string,
    shipPostions?: number[],
    shotPostions?: number[]
}

function Board({ size, shipPostions, shotPostions, shipColor, onBoxClick }: BoardProps) {
    const gridBoxes = Array.from(Array(size * size).keys())

    const getColor = useCallback((i: number) => {
        if (shotPostions?.includes(i)) {
            return 'red.400';
        }
        else if (shipPostions?.includes(i)) {
            return shipColor;
        }
        return 'gray.200';
    },
    [shipColor, shipPostions, shotPostions])

    return (
        <SimpleGrid columns={size} gridGap={'2px'} cursor={"pointer"}>
            {gridBoxes.map(i => (
                <Box key={i} bg={getColor(i)} borderRadius={'md'} height={'40px'} width={'40px'} borderWidth={'5px'} 
                onClick={() => onBoxClick ? onBoxClick(i) : undefined}
                _hover={onBoxClick ? { background: 'red.500'} : {}}
                />
            ))}
        </SimpleGrid>
    )
}

export default Board;