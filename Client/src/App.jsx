import { useContext, useEffect, useState } from "react";
import Chat from "./components/Chat/Chat";
import TextArea from "./components/Timeline/TextArea";
import Timeline from "./components/Timeline/Timeline";
import TimelineService from "./services/TimelineService";
import { UsuarioContext } from "./contexts/UsuarioContext";
import NotificationService from "./services/NotificationService";
import ListaNotificacoes from "./components/Notificacoes/ListaNotificacoes";
import Sugestoes from "./components/Notificacoes/Sugestoes";

export default function App() {
    const { usuario } = useContext(UsuarioContext);
    const [posts, setPosts] = useState([]);
    const [notifications, setNotifications] = useState([])

    useEffect(() => {
        const getNotifications = async () => {
            try {
                const notificacoes = await NotificationService.getNotifications(usuario.login);
                setNotifications(notificacoes);
            } catch (error) {
                console.error("Erro ao carregar notificações:", error);
            }
        };

        getNotifications();
        setInterval(() => {
            getNotifications();
        }, 3000);
    }, []);

    useEffect(() => {
        const getPosts = async () => {
            try {
                const timelinePosts = await TimelineService.getTimeline();
                setPosts(timelinePosts);
            } catch (error) {
                console.error("Erro ao carregar posts da timeline:", error);
            }
        };

        getPosts();
        setInterval(() => {
            getPosts();
        }, 3000);
    }, []);

    return (
        <div className="h-screen max-w-full mx-auto flex flex-row p-3 gap-3">
            {/* 1. Notificações */}
            <div className="flex-1 border border-gray-300 gap-3 rounded-2xl max-h-full overflow-auto">
                <ListaNotificacoes notifications={notifications}/>
                <Sugestoes usuario={usuario}/>
            </div>

            {/* 2. Área do meio: TextArea + Timeline */}
            <div className="flex-1 flex flex-col gap-3 max-h-full overflow-hidden">
                <TextArea logedUser={usuario.login} />
                <Timeline posts={posts} />
            </div>

            {/* 3. Área do chat */}
            <div className="flex-1 border border-gray-300 rounded-2xl max-h-full overflow-auto">
                <Chat usuario={usuario} />
            </div>
        </div>
    );
}
