import { useContext, useEffect, useState } from "react";
import UsuarioService from "../../services/UsuarioService";
import { UsuarioContext } from "../../contexts/UsuarioContext";

function Seguir({ follow, usuarioLogado }) {
  const { setUsuario } = useContext(UsuarioContext);
  const [seguindo, setSeguindo] = useState(false);

  useEffect(() => {
    if (usuarioLogado.seguindo.some(s => s.login == follow))
      setSeguindo(true) 
  }, [])

  const handleToggleSeguir = async () => {
    try {
      await UsuarioService.seguir(usuarioLogado.login, follow);

      // Buscar o usuário atualizado
      const usuarioAtualizado = await UsuarioService.getByLogin(usuarioLogado.login);
      setUsuario(usuarioAtualizado);

      setSeguindo(true); // opcional: ou use info vinda da resposta
    } catch (error) {
      console.error("Erro ao seguir usuário:", error);
    }
  };

  return (
    <div className="flex items-center justify-between border p-3 rounded-lg shadow-sm bg-white">
      <span className="text-gray-800 font-medium">{follow}</span>
      <button
        onClick={handleToggleSeguir}
        className={`px-4 py-1 rounded-md text-sm font-semibold transition ${
          seguindo
            ? "bg-gray-300 text-gray-800 hover:bg-gray-400"
            : "bg-blue-600 text-white hover:bg-blue-700"
        }`}
      >
        {seguindo ? "Seguindo" : "Seguir"}
      </button>
    </div>
  );
}

export default Seguir;
