import axios from "axios";

export default class ChatService {
    static async getChatsByUser(login) {
        try {
            const response = await axios.get(`http://localhost:5001/Chat/GetChatsByUser/${login}`);
            return response.data || [];
        } catch (error) {
            console.error('Erro ao buscar conversas:', error);
            throw error;
        }
    }

    static async sendMessage(enviando, recebendo, mensagem) {
        try {
            const response = await axios.post(`http://localhost:5001/Chat/EnviarMensagem/${enviando}&${recebendo}`, JSON.stringify(mensagem), {
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            return response.data;
        } catch (error) {
            console.error('Erro ao enviar mensagem:', error);
            throw error;
        }
    }

    static async getChat(usuario1, usuario2) {
        try {
            const response = await axios.get(`http://localhost:5001/Chat/Chat/${usuario1}&${usuario2}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar chat:', error);
            throw error;
        }
    }
}