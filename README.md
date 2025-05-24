# Sistema de Eleição por Bullying

## Visão Geral

Este projeto implementa um sistema distribuído resiliente com um mecanismo de eleição por bullying (Bully Algorithm) para determinar o servidor principal em caso de falha. A eleição por bullying é um algoritmo de eleição distribuída que seleciona um coordenador baseado no maior (ou menor) identificador entre os processos ativos no sistema.

## Como Funciona

### Clientes

#### Cliente em JavaScript
Para nosso cliente em JavaScript, utilizamos do framework React.js para fazer uma página Web funcional que atende a todos os requisitos propostos. Ele se comunica com nosso servidor através de requisições HTTP, e atualiza os estados dos componentes em tela a partir das responses que chegam.

##### Para rodar o cliente JavaScript
É necessário possuir o 20.15.0 ou versões mais atuais instalado.
1. npm install
2. npm run dev

#### Cliente em Python
Para nosso cliente em Python, utilizamos as libs 'PyQt5' para a visualização de uma interface interativa e 'requests' para a comunicação HTTP com o servidor.

##### Para rodar o cliente em Python
É necessário possuir o compilador Python instalado em sua máquina. Ao rodar o comando abaixo, o gerenciador se responsabiliza por baixar as dependências de bibliotecas, caso já não estejam instaladas.
1. python run.py

### Servidor
Nosso servidor é construído em .NET 8.0, e é utilizado no modelo de API REST. Nele, possuímos todo o processamento e armazenamento de dados (tendo em vista que trabalhamos com eles em memória - arrays e listas). Nossa arquitetura está orientada a uma separação em camadas, e disponibiliza serviços de Usuários, Timeline, Chat e Notificações - além da eleição por bullying explicada abaixo.

##### Para rodar o servidor .NET
Para rodar o servidor, é necessário possuir o Redis em sua máquina, como explicado abaixo - pode ser subido em um container Docker. Nota-se que para rodar o server, passamos a url/porta que queremos subí-lo, para que possam existir diferente servidores (em portas diferentes) da mesma aplicação, que são orquestrados por um Proxy Nginx. Para rodá-lo, utilize o comando ou o script PowerShell 'start-servers.sh'
1. dotnet run --no-build --project SistemasDistribuidosServer/SistemasDistribuidosServer.csproj --urls=http://localhost:5001””
2. ./start-servers.sh

### Proxy
Nosso proxy Nginx, que também roda com um container Docker, se responsabiliza por orquestrar as requisições nos diferentes servidores (portas 5001, 5002 e 5003).

para executá-lo no terminal rode:

```
cd Proxy
docker-compose up -d
```

--------------------------------------------------------------------------------------------

### Algoritmo de Eleição por Bullying

1. **Inicialização**: O servidor com a menor porta (5001) é definido como o servidor principal.

2. **Detecção de Falha**: Os servidores verificam periodicamente se o servidor principal está ativo. Se o servidor principal não responder, qualquer servidor que detecte a falha pode iniciar uma eleição.

3. **Processo de Eleição**:
   - Um servidor inicia uma eleição enviando uma mensagem "ELEIÇÃO" para todos os outros servidores.
   - Se um servidor recebe uma mensagem "ELEIÇÃO" de um servidor com ID menor que o seu, ele responde com uma mensagem "OK".
   - Se um servidor recebe uma mensagem "ELEIÇÃO" de um servidor com ID maior que o seu, ele inicia sua própria eleição.
   - Se um servidor não recebe nenhuma resposta, ele se declara vencedor e envia uma mensagem "COORDENADOR" para todos os outros servidores.

4. **Novo Coordenador**: O servidor vencedor se torna o novo servidor principal e todos os outros servidores reconhecem o novo líder.

#### Canais de Comunicação Redis

- **eleicao**: Usado para anunciar o início de uma eleição.
- **vitoria**: Usado para anunciar o vencedor da eleição.
- **heartbeat**: Usado para verificar se o servidor principal está ativo.
- **voto**: Usado para enviar votos durante o processo de eleição.
- **servidor_ativo**: Usado para manter um registro dos servidores ativos.

## Configuração e Uso

#### Pré-requisitos
- .NET 8.0
- Redis (porta 6379) para comunicação entre os servidores
- Bash (para executar os scripts)

#### Configuração do Redis

O Redis é essencial para a comunicação entre os servidores neste sistema de eleição. Para configurar o Redis:

1. Execute o script de configuração automática:
   ```bash
   chmod +x setup-redis.sh
   ./setup-redis.sh
   ```

   Este script irá:
   - Verificar se o Redis está instalado e instalá-lo se necessário
   - Iniciar o servidor Redis se ele não estiver rodando
   - Verificar as configurações básicas do Redis

2. Ou configure manualmente:
   - Instale o Redis: 
     - macOS: `brew install redis`
     - Ubuntu: `sudo apt install redis-server`
   - Inicie o Redis:
     - macOS: `brew services start redis`
     - Ubuntu: `sudo systemctl start redis-server`
   - Verifique se o Redis está rodando: `redis-cli ping` (deve retornar "PONG")

### Executando o Sistema

1. Iniciando os servidores:
   ```bash
   chmod +x start-servers.sh
   ./start-servers.sh
   ```

2. Testando o sistema de eleição:
   ```bash
   chmod +x test-election.sh
   ./test-election.sh
   ```

O script oferece as seguintes opções:
1. Iniciar todos os servidores
2. Verificar o status dos servidores
3. Derrubar o servidor principal (porta 5001)
4. Derrubar um servidor secundário (porta 5002)
5. Derrubar outro servidor secundário (porta 5003)
6. Forçar eleição em um servidor específico

## Endpoints para Monitoramento

- **GET /health**: Verifica o status do servidor e retorna informações sobre o servidor principal.
- **GET /health/cluster**: Retorna o status de todos os servidores no cluster.
- **POST /health/eleicao**: Força o início de uma eleição no servidor.

## Componentes Principais

- **BullyEleicoesService**: Implementa a lógica do algoritmo de eleição por bullying.
- **EleicoesStartupService**: Inicializa o serviço de eleição quando o servidor é iniciado.
- **HealthController**: Fornece endpoints para monitorar o status do cluster e forçar eleições.

## Logs

O sistema registra eventos importantes relacionados à eleição, incluindo:
- Início de eleições
- Detecção de falhas
- Mudanças no servidor principal
- Status dos servidores ativos

Estes logs ajudam a entender o processo de eleição e a resolver problemas.

## Solução de Problemas

Se encontrar problemas ao executar o sistema, verifique:

1. **Conexão com o Redis**: 
   - Certifique-se de que o Redis está rodando: `redis-cli ping`
   - Verifique a configuração em `appsettings.json` para garantir que o endereço do Redis esteja correto

2. **Portas ocupadas**:
   - Verifique se as portas 5001, 5002 e 5003 estão disponíveis
   - Use `lsof -i :PORTA` para verificar se algum processo já está usando a porta

3. **Problemas de permissão**:
   - Certifique-se de que os scripts têm permissão de execução: `chmod +x *.sh`
