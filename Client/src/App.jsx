import { useEffect, useState } from "react";
import Chat from "./components/Chat/Chat";
import TextArea from "./components/Timeline/TextArea";
import Timeline from "./components/Timeline/Timeline";
import TimelineService from "./services/TimelineService";

export default function App() {
    const logedUser = "guifornagiero";
    const [posts, setPosts] = useState([]);

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
        setInterval(() => { getPosts() }, 3000)
    }, []);

    return (
        <div className="h-screen max-w-full mx-auto flex flex-row p-3 gap-3">
            {/* 1. Área vazia */}
            <div className="flex-1 border border-gray-300 rounded-2xl" />

            {/* 2. Área do meio: TextArea + Timeline */}
            <div className="flex-1 flex flex-col gap-3 max-h-full overflow-hidden">
                <TextArea logedUser={logedUser}/>
                <Timeline posts={posts} />
            </div>

            {/* 3. Área do chat */}
            <div className="flex-1 border border-gray-300 rounded-2xl max-h-full overflow-auto">
                {/* <Chat remetente={"guifornagiero"}/> */}
            </div>
        </div>
    );
}
