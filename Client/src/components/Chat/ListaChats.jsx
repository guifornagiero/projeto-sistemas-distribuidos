import { useContext, useEffect, useState } from "react";
import { UsuarioContext } from "../../contexts/UsuarioContext";
import ChatService from "../../services/ChatService";
import MensagensChat from "./MensagensChat";

function ListaChats({chatss}) {
    const { usuario } = useContext(UsuarioContext);
    const [chats, setChats] = useState([]);
    const [chatSelecionado, setChatSelecionado] = useState(null);

    useEffect(() => {
        const getChats = async () => {
            try {
                const chats = await ChatService.getChatsByUser(usuario.login);
                setChats(chats);
            } catch (error) {
                console.error("Erro ao carregar chats:", error);
            }
        };

        getChats();
        setInterval(() => {
            getChats();
        }, 4000);
    }, []);

    return (
        <div className="w-full max-w-6xl mx-auto mt-4 h-[80vh] flex gap-6">
            {/* Coluna da esquerda - chats */}
            <div className="w-1/3 bg-white rounded-2xl shadow-lg flex flex-col h-full p-4">
                <h2 className="text-xl font-semibold text-gray-800 mb-2">Conversas</h2>

                {/* Espaço reservado para crescimento das mensagens ou outro conteúdo se quiser */}
                <div className="flex-grow"></div>

                {/* Botões fixos na parte inferior */}
                <div className="space-y-2 mt-4 overflow-y-auto max-h-[60%]">
                    {chats.map(chat => (
                        <button
                            key={chat.id}
                            onClick={() => setChatSelecionado(chat)}
                            className={`w-full text-left px-4 py-3 rounded-xl transition-all duration-200
                                ${chatSelecionado?.id === chat.id
                                    ? "bg-blue-200 shadow-md"
                                    : "bg-gray-100 hover:bg-blue-100 hover:shadow-md"}
                                text-gray-800 cursor-pointer`}
                        >
                            {chat.usuario1 === usuario.login ? chat.usuario2 : chat.usuario1}
                        </button>
                    ))}
                </div>
            </div>

            {/* Coluna da direita - mensagens */}
            <div className="w-2/3 bg-white rounded-2xl shadow-lg p-4 flex flex-col h-full">
                <div className="flex-grow overflow-y-auto">
                    {chatSelecionado ? (
                        <MensagensChat chat={chatSelecionado} usuario={usuario} />
                    ) : (
                        <div className="h-full text-gray-500 flex items-center justify-center">
                            Selecione um chat para visualizar as mensagens
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

export default ListaChats;
