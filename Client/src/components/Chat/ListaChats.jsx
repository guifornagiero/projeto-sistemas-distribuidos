import { useContext, useEffect, useState } from "react";
import { UsuarioContext } from "../../contexts/UsuarioContext";
import ChatService from "../../services/ChatService";
import UsuarioService from "../../services/UsuarioService";
import MensagensChat from "./MensagensChat";

function ListaChats() {
    const { usuario } = useContext(UsuarioContext);
    const [chats, setChats] = useState([]);
    const [chatSelecionado, setChatSelecionado] = useState(null);
    const [loading, setLoading] = useState(false);
    const [todosUsuarios, setTodosUsuarios] = useState([]);
    const [mostrarTodosUsuarios, setMostrarTodosUsuarios] = useState(false);

    const getChats = async () => {
        if (!usuario?.login) return;
        
        setLoading(true);
        try {
            const chats = await ChatService.getChatsByUser(usuario.login);
            // Filtrar apenas chats que possuem pelo menos uma mensagem
            const chatsAtivos = chats.filter(chat => chat.mensagens && chat.mensagens.length > 0);
            setChats(chatsAtivos);
            
            // Manter o chat selecionado após atualização
            if (chatSelecionado) {
                const chatAtualizado = chatsAtivos.find(c => c.id === chatSelecionado.id);
                if (chatAtualizado) {
                    setChatSelecionado(chatAtualizado);
                }
            }
        } catch (error) {
            console.error("Erro ao carregar chats:", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        getChats();
        carregarTodosUsuarios();
        
        const interval = setInterval(() => {
            getChats();
        }, 4000);
        
        return () => clearInterval(interval);
    }, [usuario]);

    const carregarTodosUsuarios = async () => {
        try {
            const usuarios = await UsuarioService.getAllUsers();
            // Filtrar para não mostrar o usuário atual
            const outrosUsuarios = usuarios.filter(u => u.login !== usuario.login);
            setTodosUsuarios(outrosUsuarios);
        } catch (error) {
            console.error("Erro ao carregar usuários:", error);
        }
    };

    const iniciarNovoChat = async (outroUsuario) => {
        try {
            // Verificar se já existe um chat com este usuário
            const chatExistente = chats.find(c => 
                c.usuario1 === outroUsuario.login || c.usuario2 === outroUsuario.login
            );
            
            if (chatExistente) {
                setChatSelecionado(chatExistente);
            } else {
                try {
                    // Cria um novo chat enviando uma mensagem inicial
                    await ChatService.sendMessage(usuario.login, outroUsuario.login, "Olá!");
                    
                    // Encontra o chat recém-criado e o seleciona
                    const chatAtualizado = await ChatService.getChat(usuario.login, outroUsuario.login);
                    
                    if (chatAtualizado && chatAtualizado.mensagens && chatAtualizado.mensagens.length > 0) {
                        // Recarrega a lista de chats para incluir o novo chat
                        await getChats();
                        // Seleciona o novo chat
                        setChatSelecionado(chatAtualizado);
                    }
                } catch (error) {
                    console.error("Erro ao iniciar novo chat:", error);
                    alert("Não foi possível iniciar a conversa. Tente novamente.");
                }
            }
            
            setMostrarTodosUsuarios(false);
        } catch (error) {
            console.error("Erro ao iniciar novo chat:", error);
        }
    };

    return (
        <div className="w-full max-w-6xl mx-auto mt-4 h-[80vh] flex gap-6">
            {/* Coluna da esquerda - chats */}
            <div className="w-1/3 bg-white rounded-2xl shadow-lg flex flex-col h-full p-4">
                <div className="flex justify-between items-center mb-2">
                    <h2 className="text-xl font-semibold text-gray-800">Conversas</h2>
                    <button 
                        onClick={() => setMostrarTodosUsuarios(!mostrarTodosUsuarios)}
                        className="px-3 py-1 bg-blue-500 text-white rounded-lg hover:bg-blue-600 text-sm"
                    >
                        {mostrarTodosUsuarios ? "Ver Conversas" : "Nova Conversa"}
                    </button>
                </div>

                {/* Espaço reservado para crescimento das mensagens ou outro conteúdo se quiser */}
                <div className="flex-grow"></div>

                {/* Botões fixos na parte inferior */}
                <div className="space-y-2 mt-4 overflow-y-auto max-h-[60%]">
                    {mostrarTodosUsuarios ? (
                        todosUsuarios.length === 0 ? (
                            <p className="text-center text-gray-500">Nenhum outro usuário encontrado</p>
                        ) : (
                            <>
                                <h3 className="font-medium text-gray-700 mb-1">Selecione um usuário:</h3>
                                {todosUsuarios.map(user => (
                                    <button
                                        key={user.id}
                                        onClick={() => iniciarNovoChat(user)}
                                        className="w-full text-left px-4 py-3 rounded-xl transition-all duration-200
                                            bg-gray-100 hover:bg-blue-100 hover:shadow-md
                                            text-gray-800 cursor-pointer flex items-center"
                                    >
                                        <span className="w-8 h-8 rounded-full bg-gray-300 flex items-center justify-center text-gray-700 font-bold mr-2">
                                            {user.login.charAt(0).toUpperCase()}
                                        </span>
                                        {user.login}
                                    </button>
                                ))}
                            </>
                        )
                    ) : loading && chats.length === 0 ? (
                        <p className="text-center text-gray-500">Carregando conversas...</p>
                    ) : chats.length === 0 ? (
                        <p className="text-center text-gray-500">
                            Nenhuma conversa ativa encontrada.<br/>
                            Clique em "Nova Conversa" para iniciar um chat.
                        </p>
                    ) : (
                        chats.map(chat => (
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
                        ))
                    )}
                </div>
            </div>

            {/* Coluna da direita - mensagens */}
            <div className="w-2/3 bg-white rounded-2xl shadow-lg p-4 flex flex-col h-full">
                {chatSelecionado ? (
                    <MensagensChat 
                        chat={chatSelecionado} 
                        usuario={usuario} 
                        onMessageSent={getChats} 
                    />
                ) : (
                    <div className="h-full text-gray-500 flex items-center justify-center">
                        Selecione um chat para visualizar as mensagens
                    </div>
                )}
            </div>
        </div>
    );
}

export default ListaChats;
