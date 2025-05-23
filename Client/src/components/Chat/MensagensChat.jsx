import { useEffect, useState } from "react";
import ChatService from "../../services/ChatService";

function MensagensChat({ chat, usuario }) {
    const [mensagens, setMensagens] = useState([]);

    useEffect(() => {
        // const fetchMensagens = async () => {
        //     try {
        //         const msgs = await ChatService.getMensagens(chat.id);
        //         setMensagens(msgs);
        //     } catch (error) {
        //         console.error("Erro ao carregar mensagens:", error);
        //     }
        // };

        // fetchMensagens();
        setMensagens(chat.mensagens)
    }, [chat]);

    return (
        <div className="bg-white h-full p-4 rounded-2xl shadow-lg flex flex-col gap-2 overflow-y-auto">
            <h3 className="text-lg font-semibold text-gray-700 mb-3">
                Conversa com {chat.usuario1 === usuario.login ? chat.usuario2 : chat.usuario1}
            </h3>

            {mensagens.length === 0 ? (
                <p className="text-gray-400">Nenhuma mensagem ainda.</p>
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
    );
}

export default MensagensChat;
