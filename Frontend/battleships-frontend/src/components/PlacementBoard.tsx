import { Box, Button, Center, HStack, IconButton, SimpleGrid } from "@chakra-ui/react";
import { CloseIcon, RepeatIcon } from "@chakra-ui/icons";
import { useCallback, useEffect, useRef, useState } from "react";

interface PlacementBoardProps {
    boardSize: number,
    shipLengths: number[],
    onFinishedPlacing: (shipPositions: number[][]) => {}
}

type Direction = Horizontal | Vertical;
type Horizontal = 0;
type Vertical = 1;
const horizontal: Horizontal = 0;
const vertical: Vertical = 1;

function PlacementBoard({ boardSize, shipLengths, onFinishedPlacing }: PlacementBoardProps) {
    const gridBoxes = Array.from(Array(boardSize * boardSize).keys())

    const [direction, setDirection] = useState<Direction>(vertical)
    const [hoverCenter, setHoverCenter] = useState<number | null>(null);
    const [placedPoints, setPlacedPoints] = useState<number[]>([]);
    const [placingPoints, setPlacingPoints] = useState<number[]>([]);

    const [currentShipIndex, setCurrentShipIndex] = useState(0);
    const shipPositions = useRef<number[][]>(shipLengths.map(() => []))

    const [shipLength, setShipLength] = useState(4)

    useEffect(() => {
        setShipLength(shipLengths[currentShipIndex])
    },
    [currentShipIndex, shipLengths])

    async function onPointerEnter(i: number) {
        setHoverCenter(i)
    }

    async function switchDirection() {
        setDirection((direction) => direction === vertical ? horizontal : vertical)
    }

    const getVerticalPoints = useCallback((i: number) => {
        return Array.from(Array(shipLength).keys()).map((j) => i + boardSize * j)
        .filter(p => p >= 0 && p < boardSize * boardSize)
        .filter(p => !placedPoints.includes(p));
    }, [boardSize, placedPoints, shipLength])

    const getHorizontalPoints = useCallback((i: number) => {
        return Array.from(Array(shipLength).keys()).map((j) => i + j)
        .filter(p => p < boardSize * (Math.floor(i / boardSize) + 1))
        .filter(p => !placedPoints.includes(p));
    }, [boardSize, placedPoints, shipLength])

    useEffect(() => {
        if (hoverCenter === null) return;

        if (direction === vertical) {
            setPlacingPoints(getVerticalPoints(hoverCenter));
        }
        if (direction === horizontal) {
            setPlacingPoints(getHorizontalPoints(hoverCenter));
        }
    }, 
    [hoverCenter, direction, boardSize, getVerticalPoints, getHorizontalPoints])

    const placeShip = useCallback(() => {
        if (placingPoints.length === shipLength) {
            setPlacedPoints([...placedPoints, ...placingPoints])
            shipPositions.current[currentShipIndex] = [...placingPoints]
            setCurrentShipIndex(currentShipIndex + 1);
        }
    }, [placingPoints, shipLength, placedPoints, currentShipIndex]);

    const resetShips = () => {
        setPlacedPoints([])
        setCurrentShipIndex(0)
        shipPositions.current = shipLengths.map(() => []);
    }

    const getBoxColor = useCallback((i: number) => {
        const isPlacing = placingPoints.includes(i);
        const isPlaced = placedPoints.includes(i);
        const hasEnoughPoints = placingPoints.length === shipLength;

        if (isPlacing) {
            if (hasEnoughPoints) {
                return 'green.200';
            } else {
                return 'red.200';
            }
        }
        if (isPlaced) return 'green.400';

        return 'gray.200';
    }, [placingPoints, placedPoints, shipLength])

    return (
        <>
        <SimpleGrid cursor={'pointer'} columns={boardSize} gridGap={'2px'}>
            {gridBoxes.map(i => (
                <Box as="div" key={i} onPointerEnter={() => onPointerEnter(i)}
                bg={getBoxColor(i)}
                borderRadius={'md'} height={'40px'} width={'40px'} borderWidth={'5px'}
                onDoubleClick={placeShip}
                />
            ))}
        </SimpleGrid>
        <Center marginTop={"10px"}>
            <HStack>
                <Button onClick={() => onFinishedPlacing(shipPositions.current)}>
                    Finish
                </Button>
                <IconButton icon={<CloseIcon />} aria-label={""} onClick={resetShips}/>
                <IconButton icon={<RepeatIcon />} aria-label={""} onClick={switchDirection}/> 
            </HStack>
        </Center>
        </>
    )
}

export default PlacementBoard;