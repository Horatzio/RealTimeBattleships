import * as signalR from "@microsoft/signalr";
import { appConfig } from "./config";

const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${appConfig.baseUrl}/battleships-hub`)
    .withAutomaticReconnect()
    .build();

export const battleShipsHub = {
    async connect(playerId: string) {
        await connection.start();
        await connection.send("registerConnection", playerId)
    },
    async startGame(playerId: string, enemyPlayerId: string) {
        await connection.send("startGame", playerId, enemyPlayerId)
    },
    onGameSessionUpdate(callback: any) {
        connection.on("GameSessionUpdate", callback)
    }
}