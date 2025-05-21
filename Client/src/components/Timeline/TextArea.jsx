import { useState } from "react";
import TimelineService from "../../services/TimelineService";

function TextArea({ logedUser }) {
  const [title, setTitle] = useState("");
  const [text, setText] = useState("");

  const handlePublish = async () => {
    const postagem = {
      titulo: title,
      conteudo: text,
      criadorLogin: logedUser
    }

    await TimelineService.post(postagem);
  };

  return (
    <div className="w-full p-4 border rounded-2xl shadow-md space-y-4 bg-white">
      <h2 className="font-bold text-xl text-gray-800">Escreva uma postagem</h2>
      <input
        type="text"
        className="w-full p-2 border border-gray-300 text-black bg-white rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 shadow-md"
        placeholder="Título da postagem"
        value={title}
        onChange={(e) => setTitle(e.target.value)}
      />
      <textarea
        className="w-full h-28 p-2 border border-gray-300 text-black rounded-lg bg-white resize-none focus:outline-none focus:ring-2 focus:ring-blue-500 shadow-md"
        placeholder="Escreva sua postagem..."
        value={text}
        onChange={(e) => setText(e.target.value)}
      />
      <div className="flex justify-end">
        <button
          className="bg-blue-600 text-white py-2 px-4 rounded-lg hover:bg-blue-700 transition"
          onClick={handlePublish}
        >
          Publicar
        </button>
      </div>
    </div>
  );
}

export default TextArea;
