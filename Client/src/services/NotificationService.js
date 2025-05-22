import axios from "axios";

export default class NotificationService {
    static async getNotifications(login) {
        try {
            const response = await axios.get(`http://localhost:8080/Usuario/Notificacoes/${login}`);
            return response.data || [];
        } catch (error) {
            console.error('Erro ao buscar notificações:', error);
            throw error;
        }
    }
}