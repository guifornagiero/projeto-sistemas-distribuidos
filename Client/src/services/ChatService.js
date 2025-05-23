import axios from "axios";

export default class ChatService {
    static async getChatsByUser(login) {
        try {
            const response = await axios.get(`http://localhost:8080/Chat/GetChatsByUser/${login}`);
            return response.data || [];
        } catch (error) {
            console.error('Erro ao buscar conversas:', error);
            throw error;
        }
    }
}