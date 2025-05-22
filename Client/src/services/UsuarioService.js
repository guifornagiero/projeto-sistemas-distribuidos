import axios from "axios";

export default class UsuarioService {
    static async getByLogin(login) {
        try {
            const response = await axios.get(`http://localhost:8080/Usuario/Login/${login}`);
            return response.data;
        } catch (error) {
            console.error('Erro ao buscar usuário:', error);
            throw error;
        }
    }

    static async seguir(usuarioLogado, usuarioASerSeguido) {
        try {
            const response = await axios.post(`http://localhost:8080/Usuario/Seguir/`, { loginQuerSeguir: usuarioLogado, loginParaSeguir: usuarioASerSeguido });
            return response.data;
        } catch (error) {
            console.error('Erro ao seguir usuário:', error);
            throw error;
        }
    }
}