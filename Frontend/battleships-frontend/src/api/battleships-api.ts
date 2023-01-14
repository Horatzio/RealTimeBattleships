import axios from 'axios';
import { GameSession } from './game-session';
import { User } from './user';
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

    async getName(playerId: string) {
        const response = await axios.get<string>(`${this.baseUrl}/get-name?playerId=${playerId}`);
        return response.data;
    }

    async getAvailablePlayers() {
        const response = await axios.get<User[]>(`${this.baseUrl}/available-players`);
        return response.data;
    }
}

export const battleShipsApi = new Api(appConfig.baseUrl);