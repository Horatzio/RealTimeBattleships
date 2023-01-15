import axios from 'axios';
import { GameSession, ShipPositions } from './game-session';
import { appConfig } from './config';

export class Api {
    constructor(public baseUrl: string) {}

    async login(username: string) {
        const response = await axios.post<string>(`${this.baseUrl}/login-user?username=${username}`);
        return response.data;
    }

    async getGameSession(playerId: string) {
        const response = await axios.get<GameSession>(`${this.baseUrl}/get-session?playerId=${playerId}`);
        return response.data;
    }

    async startGame(playerId: string) {
        const response = await axios.post<string>(`${this.baseUrl}/start-game?playerId=${playerId}`);
        return response.data;
    }

    async submitPlayerPositions(playerId: string, sessionId: string, positions: ShipPositions[]) {
        await axios.post(`${this.baseUrl}/submit-player-positions?playerId=${playerId}&sessionId=${sessionId}`, positions)
    }
    
    async playerShot(playerId: string, sessionId: string, position: number) {
        await axios.post(`${this.baseUrl}/player-shot?playerId=${playerId}&sessionId=${sessionId}`, { position: position })
    }

    async endSession(sessionId: string) {
        await axios.post(`${this.baseUrl}/end-session?sessionId=${sessionId}`);
    }
}

export const battleShipsApi = new Api(appConfig.baseUrl);