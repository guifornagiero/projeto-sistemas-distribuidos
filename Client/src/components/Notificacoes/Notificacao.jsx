import { formatDate } from "../../util/util";

function Notificacao({ notificacao }) {
  return (
    <div className="w-full border rounded-2xl shadow-md p-4 bg-white space-y-2">
      <p className="text-sm text-gray-700">{notificacao.descricao}</p>
      <div className="text-xs text-gray-500">
        <strong>Título:</strong> {notificacao.postagemTitulo} <br />
        <strong>Resumo:</strong> {notificacao.postagemDescricao} <br />
        <span><strong>Data:</strong> {formatDate(notificacao.criadaEm)}</span>
      </div>
    </div>
  );
}

export default Notificacao;
