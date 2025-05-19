import { useContext } from "react";
import { UsuarioContext } from "../../contexts/UsuarioContext";
import Seguir from "./Seguir";

function Sugestoes() {
  const { usuario } = useContext(UsuarioContext);
  const usuarios = ["guifornagiero", "gianluca", "paulobrito", "pedrobento"];

  return (
    <div className="w-full h-83 mt-3 p-4 space-y-3 border rounded-2xl shadow-md bg-white overflow-y-auto">
      <h2 className="font-bold text-xl text-gray-800">Quem seguir</h2>
      {usuarios
        .filter((u) => u !== usuario.login)
        .map((u) => (
          <Seguir key={u} usuarioLogado={usuario} follow={u} />
        ))}
    </div>
  );
}

export default Sugestoes;
