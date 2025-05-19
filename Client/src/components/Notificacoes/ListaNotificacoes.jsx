import { useEffect, useRef, useState } from "react";
import Notificacao from "./Notificacao";

function ListaNotificacoes({ notifications }) {
    const endRef = useRef(null);
    const [firstLoad, setFirstLoad] = useState(true);

    useEffect(() => {
        if (!firstLoad) {
            endRef.current?.scrollIntoView({ behavior: "smooth" });
        } else {
            setFirstLoad(false);
        }
    }, [notifications]);

    return (
        <div className="w-full h-[60vh] overflow-auto p-4 space-y-4 bg-white rounded-2xl no-scrollbar">
            <h2 className="font-bold text-xl text-black">Notificações</h2>
            {!!notifications && notifications.map((n) => (
                <Notificacao key={n.id} notificacao={n} />
            ))}
            <div ref={endRef} />
        </div>
    );
}

export default ListaNotificacoes;
