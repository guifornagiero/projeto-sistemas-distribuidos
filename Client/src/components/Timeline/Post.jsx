import { formatDate } from "../../util/util";

function Post({ post }) {
    return (
        <div className="w-full border rounded-2xl shadow-md p-4 bg-white space-y-2">
            <h3 className="text-lg font-bold text-gray-800">{post.titulo}</h3>
            <h4 className="text-sm text-gray-600">
                <span className="underline">{post.criadorNome}</span> • {formatDate(post.dataCriacao)}
            </h4>
            <p className="text-sm text-gray-700">{post.conteudo}</p>
        </div>
    );
}

export default Post;
