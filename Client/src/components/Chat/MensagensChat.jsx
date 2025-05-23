import { useEffect, useState, useRef } from "react";
import ChatService from "../../services/ChatService";

function MensagensChat({ chat, usuario, onMessageSent }) {
    const [mensagens, setMensagens] = useState([]);
    const [novaMensagem, setNovaMensagem] = useState("");
    const [loading, setLoading] = useState(false);
    const mensagensContainerRef = useRef(null);
    const [chatCarregando, setChatCarregando] = useState(true);
    const intervalRef = useRef(null);

    const fetchMensagens = async () => {
        try {
            if (chat) {
                setChatCarregando(true);
                const chatAtualizado = await ChatService.getChat(
                    chat.usuario1, 
                    chat.usuario2
                );
                if (chatAtualizado && chatAtualizado.mensagens) {
                    // Verificar se as mensagens são diferentes das atuais antes de atualizar o estado
                    if (JSON.stringify(chatAtualizado.mensagens) !== JSON.stringify(mensagens)) {
                        setMensagens(chatAtualizado.mensagens);
                    }
                }
                setChatCarregando(false);
            }
        } catch (error) {
            console.error("Erro ao carregar mensagens:", error);
            setChatCarregando(false);
        }
    };

    useEffect(() => {
        fetchMensagens();
        
        // Limpar intervalo anterior se existir
        if (intervalRef.current) {
            clearInterval(intervalRef.current);
        }
        
        // Atualiza as mensagens a cada 3 segundos
        intervalRef.current = setInterval(() => {
            fetchMensagens();
        }, 3000);
        
        return () => {
            if (intervalRef.current) {
                clearInterval(intervalRef.current);
            }
        };
    }, [chat]);

    useEffect(() => {
        // Rola para o final das mensagens quando novas são adicionadas
        if (mensagensContainerRef.current) {
            mensagensContainerRef.current.scrollTop = mensagensContainerRef.current.scrollHeight;
        }
    }, [mensagens]);

    const handleEnviarMensagem = async (e) => {
        e.preventDefault();
        
        if (!novaMensagem.trim()) return;
        
        setLoading(true);
        try {
            const destinatario = chat.usuario1 === usuario.login ? chat.usuario2 : chat.usuario1;
            const resultado = await ChatService.sendMessage(usuario.login, destinatario, novaMensagem);
            setNovaMensagem("");
            
            if (resultado) {
                // Recarrega as mensagens após enviar
                await fetchMensagens();
                
                // Notifica o componente pai para atualizar a lista de chats
                if (onMessageSent) onMessageSent();
                
                // Garante que o foco volte para o campo de mensagem
                document.getElementById('mensagem-input').focus();
            }
        } catch (error) {
            console.error("Erro ao enviar mensagem:", error);
        } finally {
            setLoading(false);
        }
    };

    const destinatario = chat.usuario1 === usuario.login ? chat.usuario2 : chat.usuario1;

    return (
        <div className="flex flex-col h-full">
            <h3 className="text-lg font-semibold text-gray-700 mb-3">
                Conversa com {destinatario}
            </h3>

            <div 
                ref={mensagensContainerRef}
                className="flex-grow p-4 rounded-2xl shadow-inner bg-gray-50 flex flex-col gap-2 overflow-y-auto mb-4"
            >
                {chatCarregando ? (
                    <p className="text-gray-400 text-center my-auto">Carregando mensagens...</p>
                ) : mensagens.length === 0 ? (
                    <p className="text-gray-400 text-center my-auto">
                        Nenhuma mensagem ainda. Comece a conversar com {destinatario}!
                    </p>
                ) : (
                    mensagens.map((msg, index) => (
                        <div
                            key={index}
                            className={`px-4 py-2 rounded-xl max-w-xs ${
                                msg.remetente.login === usuario.login
                                    ? "bg-blue-500 text-white self-end"
                                    : "bg-gray-200 text-gray-800 self-start"
                            }`}
                        >
                            {msg.texto}
                        </div>
                    ))
                )}
            </div>

            <form onSubmit={handleEnviarMensagem} className="flex gap-2">
                <input
                    id="mensagem-input"
                    type="text"
                    value={novaMensagem}
                    onChange={(e) => setNovaMensagem(e.target.value)}
                    placeholder={`Escreva uma mensagem para ${destinatario}...`}
                    className="flex-grow border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                    disabled={loading || chatCarregando}
                    autoFocus
                />
                <button
                    type="submit"
                    className="bg-blue-500 text-white rounded-lg px-4 py-2 hover:bg-blue-600 transition-colors disabled:bg-blue-300"
                    disabled={loading || !novaMensagem.trim() || chatCarregando}
                >
                    {loading ? "Enviando..." : "Enviar"}
                </button>
            </form>
        </div>
    );
}

export default MensagensChat;
