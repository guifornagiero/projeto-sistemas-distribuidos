import { useEffect, useRef, useState } from "react";
import { formatDate } from "../../util/util";

function Chat({ remetente }) {
    const [mensagens, setMensagens] = useState(chat.mensagens);
    const [novaMensagem, setNovaMensagem] = useState("");
    const messagesEndRef = useRef(null);
    const [firstLoad, setFirstLoad] = useState(true);

    useEffect(() => {
        if (!firstLoad) {
            messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
        } else {
            setFirstLoad(false);
        }
    }, [mensagens]);

    const handleEnviar = () => {
        const textoTrim = novaMensagem.trim();
        if (!textoTrim) return;

        const novaMsgObj = {
            id: mensagens.length + 1,
            remetente: {
                id: 999,
                nome: remetente,
                login: remetente
            },
            texto: textoTrim,
            enviadaEm: new Date().toISOString()
        };

        setMensagens([...mensagens, novaMsgObj]);
        setNovaMensagem("");
    };

    return (
        <div className="w-full max-w-md mx-auto p-4 border rounded-2xl shadow-md bg-white flex flex-col space-y-4 max-h-[60vh]">
            <h2 className="text-xl font-bold text-gray-800">
                Chat entre {chat.usuario1} e {chat.usuario2}
            </h2>

            {/* Lista de mensagens com scroll */}
            <div className="flex-grow overflow-y-auto space-y-4 no-scrollbar">
                {mensagens.map((mensagem) => (
                    <div
                        key={mensagem.id}
                        className={`p-3 border rounded-lg shadow-sm ${
                            mensagem.remetente.login === remetente
                                ? "bg-blue-50 border-blue-300 self-end"
                                : "bg-gray-50 border-gray-300"
                        }`}
                    >
                        <div className="flex justify-between items-center mb-1">
                            <span
                                className={`font-semibold ${
                                    mensagem.remetente.login === remetente
                                        ? "text-blue-700"
                                        : "text-gray-700"
                                }`}
                            >
                                {mensagem.remetente.nome}
                            </span>
                            <span className="text-xs text-gray-500">
                                {formatDate(mensagem.enviadaEm)}
                            </span>
                        </div>
                        <p className="text-gray-800">{mensagem.texto}</p>
                    </div>
                ))}
                <div ref={messagesEndRef} />
            </div>

            {/* Campo para nova mensagem + botão */}
            <div className="flex gap-2 mt-2">
                <input
                    type="text"
                    value={novaMensagem}
                    onChange={(e) => setNovaMensagem(e.target.value)}
                    placeholder="Digite sua mensagem..."
                    className="flex-grow p-2 border text-black border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    onKeyDown={(e) => {
                        if (e.key === "Enter") {
                            e.preventDefault();
                            handleEnviar();
                        }
                    }}
                />
                <button
                    onClick={handleEnviar}
                    className="bg-blue-600 text-white px-4 rounded-lg hover:bg-blue-700 transition"
                >
                    Enviar
                </button>
            </div>
        </div>
    );
}

export default Chat;

const chat = {
    id: 1,
    usuario1: "guifornagiero",
    usuario2: "gianluca",
    mensagens: [
        {
            id: 1,
            remetente: {
                id: 1,
                nome: "Guilherme",
                login: "guifornagiero"
            },
            texto: "asdijajsidajisdjiasjid",
            enviadaEm: "2025-05-15T22:12:51.5916872-03:00"
        },
        {
            id: 2,
            remetente: {
                id: 4,
                nome: "Gian",
                login: "gianluca"
            },
            texto: "123912u893nkakdsads-a-sdaijdsasudhi",
            enviadaEm: "2025-05-15T22:13:00.3975898-03:00"
        },
        {
            id: 3,
            remetente: {
                id: 1,
                nome: "Guilherme",
                login: "guifornagiero"
            },
            texto: "asdijajsidajisdjiasjid",
            enviadaEm: "2025-05-15T22:12:51.5916872-03:00"
        },
        {
            id: 4,
            remetente: {
                id: 4,
                nome: "Gian",
                login: "gianluca"
            },
            texto: "123912u893nkakdsads-a-sdaijdsasudhi",
            enviadaEm: "2025-05-15T22:13:00.3975898-03:00"
        },
        {
            id: 5,
            remetente: {
                id: 1,
                nome: "Guilherme",
                login: "guifornagiero"
            },
            texto: "asdijajsidajisdjiasjid",
            enviadaEm: "2025-05-15T22:12:51.5916872-03:00"
        },
        {
            id: 6,
            remetente: {
                id: 4,
                nome: "Gian",
                login: "gianluca"
            },
            texto: "123912u893nkakdsads-a-sdaijdsasudhi",
            enviadaEm: "2025-05-15T22:13:00.3975898-03:00"
        }
    ]
};
