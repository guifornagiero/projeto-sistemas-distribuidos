import { useEffect, useRef, useState } from "react";
import Post from "./Post";

function Timeline({ posts }) {
    const endRef = useRef(null);
    const [firstLoad, setFirstLoad] = useState(true);

    useEffect(() => {
        if (!firstLoad) {
            endRef.current?.scrollIntoView({ behavior: "smooth" });
        } else {
            setFirstLoad(false);
        }
    }, [posts]);

    return (
        <div className="w-full p-4 space-y-4 border rounded-2xl shadow-md bg-white overflow-y-auto no-scrollbar min-h-[64%]">
            <h2 className="font-bold text-xl text-black">Timeline</h2>
            {posts.map((post) => (
                <Post key={post.id} post={post} />
            ))}
            <div ref={endRef} />
        </div>
    );
}

export default Timeline;

// const posts = [
//     {
//         id: 1,
//         titulo: "postagem 1",
//         conteudo: "POSTAGEM 1",
//         criadorId: 1,
//         criadorNome: "Guilherme",
//         criadorLogin: "guifornagiero",
//         dataCriacao: "2025-05-15T21:27:45.6441302-03:00"
//     },
//     {
//         id: 2,
//         titulo: "postagem 2",
//         conteudo: "POSTAGEM 2",
//         criadorId: 2,
//         criadorNome: "Paulo",
//         criadorLogin: "paulobrito",
//         dataCriacao: "2025-05-15T21:27:54.1804196-03:00"
//     },
//     {
//         id: 3,
//         titulo: "postagem 3",
//         conteudo: "POSTAGEM 3",
//         criadorId: 4,
//         criadorNome: "Gian",
//         criadorLogin: "gianluca",
//         dataCriacao: "2025-05-15T21:27:59.8177136-03:00"
//     },
//     {
//         id: 5,
//         titulo: "postagem 4",
//         conteudo: "POSTAGEM 4",
//         criadorId: 3,
//         criadorNome: "Pedro",
//         criadorLogin: "pedrobento",
//         dataCriacao: "2025-05-15T21:28:07.6581613-03:00"
//     },
//     {
//         id: 6,
//         titulo: "postagem 4",
//         conteudo: "POSTAGEM 4",
//         criadorId: 3,
//         criadorNome: "Pedro",
//         criadorLogin: "pedrobento",
//         dataCriacao: "2025-05-15T21:28:07.6581613-03:00"
//     }
// ];
