import { useEffect, useState } from "react";
import UsuarioService from "../../services/UsuarioService";
import { UsuarioContext } from "../../contexts/UsuarioContext";

function LoginProvider({ children }) {
  const users = ["guifornagiero", "gianluca", "paulobrito", "pedrobento"];
  const [username, setUsername] = useState("");
  const [usuario, setUsuario] = useState(null);

  const handleLogin = async () => {
    if (users.includes(username)) {
      try {
        const userData = await UsuarioService.getByLogin(username);
        if (userData) {
          sessionStorage.setItem("usuario", JSON.stringify(userData));
          setUsuario(userData);
        } else {
          alert("Usuário inválido no sistema!");
        }
      } catch (error) {
        console.error("Erro ao buscar usuário:", error);
      }
    } else {
      alert("Usuário não encontrado!");
    }
  };

  useEffect(() => {
    const stored = sessionStorage.getItem("usuario");
    if (stored) {
      try {
        setUsuario(JSON.parse(stored));
      } catch (e) {
        console.error("Erro ao converter usuário:", e);
      }
    }
  }, []);

  if (!usuario) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-600">
        <div className="bg-gray-900 p-6 rounded-2xl shadow-md w-full max-w-sm">
          <h2 className="text-2xl font-bold mb-4 text-center text-white">Login</h2>
          <input
            type="text"
            placeholder="Digite seu login"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            className="w-full px-4 py-2 mb-4 border rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          <button
            onClick={handleLogin}
            className="w-full bg-blue-600 text-white py-2 rounded-xl hover:bg-blue-700 transition"
          >
            Entrar
          </button>
        </div>
      </div>
    );
  }

  return (
    <UsuarioContext.Provider value={{usuario, setUsuario}}>
      {children}
    </UsuarioContext.Provider>
  );
}

export default LoginProvider;
