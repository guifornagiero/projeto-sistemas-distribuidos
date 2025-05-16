import axios from "axios";

export default class TimelineService {
    static async getTimeline() {
        try {
            const response = await axios.get("https://localhost:5001/Postagem/Timeline");
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar timeline:', error);
            throw error;
        }
    }

    static async post(postagem) {
        try {
            const response = await axios.post("https://localhost:5001/Postagem", postagem);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar post:', error);
            throw error;
        }
    }
}