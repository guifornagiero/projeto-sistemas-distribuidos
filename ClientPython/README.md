# Cliente Python para Sistema Distribuído

Este é um cliente alternativo para o sistema distribuído, desenvolvido em Python utilizando PyQt5 para a interface gráfica. A interface foi projetada com foco na legibilidade, utilizando texto preto sobre fundo claro.

## Requisitos

- Python 3.7+
- PyQt5
- Requests
- python-dateutil

## Instalação

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/projeto-sistemas-distribuidos.git
cd projeto-sistemas-distribuidos/ClientPython
```

2. Instale as dependências:
```bash
pip install -r requirements.txt
```

## Execução

Para iniciar a aplicação, execute:

```bash
python main.py
```

## Funcionalidades

- **Login**: Acesse o sistema utilizando seu nome de usuário
- **Timeline**: Visualize e crie posts na timeline
- **Notificações**: Receba notificações sobre atividades relacionadas a você
- **Sugestões de Usuários**: Veja sugestões de usuários para seguir
- **Chat**: Comunique-se com outros usuários através do chat

Todas as interfaces são projetadas com cores contrastantes para melhor legibilidade e usabilidade.

## Estrutura do Projeto

- `main.py`: Arquivo principal da aplicação
- `requirements.txt`: Lista de dependências
- `README.md`: Documentação do projeto

## Comparação com o Cliente React

Este cliente foi desenvolvido como uma alternativa ao cliente React original, mantendo as mesmas funcionalidades:

1. Autenticação de usuários
2. Visualização e postagem na timeline
3. Sistema de notificações
4. Sugestões para seguir outros usuários
5. Chat entre usuários

Diferenças principais:
- **Tecnologia**: Enquanto o cliente original usa React e executa no navegador, este cliente utiliza Python com PyQt5 e executa como uma aplicação desktop.
- **Interface**: O cliente Python utiliza texto preto em fundos claros para melhor legibilidade, e controles nativos do sistema operacional para melhor integração.
- **Performance**: Sendo uma aplicação nativa, oferece potencialmente melhor desempenho em sistemas com recursos limitados.

## API

A aplicação se comunica com o mesmo backend que o cliente React, utilizando as seguintes endpoints:

- `/Usuario/Login/{login}`: Obter informações do usuário
- `/Postagem/Timeline`: Obter posts da timeline
- `/Postagem`: Criar um novo post
- `/Usuario/Notificacoes/{login}`: Obter notificações do usuário
- `/Usuario/Seguir/`: Seguir um usuário