#!/bin/bash

echo "Script de teste de eleição por bullying"
echo "======================================="

# Função para verificar status do servidor
check_server() {
    local porta=$1
    local response=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:$porta/health)
    if [ "$response" == "200" ]; then
        echo "Servidor na porta $porta está ONLINE"
        return 0
    else
        echo "Servidor na porta $porta está OFFLINE"
        return 1
    fi
}

# Função para verificar qual servidor é o principal
check_leader() {
    for porta in 5001 5002 5003; do
        if check_server $porta; then
            local info=$(curl -s http://localhost:$porta/health)
            echo "Informações do servidor $porta: $info"
        fi
    done
}

# Iniciar os servidores
start_servers() {
    echo "Iniciando os servidores..."
    bash ./start-servers.sh
    sleep 10  # Esperar que os servidores inicializem completamente
}

# Derrubar um servidor específico
kill_server() {
    local porta=$1
    echo "Derrubando servidor na porta $porta..."
    local pid=$(lsof -ti :$porta)
    if [ -n "$pid" ]; then
        kill -9 $pid
        echo "Servidor na porta $porta foi derrubado (PID: $pid)"
    else
        echo "Não foi encontrado processo na porta $porta"
    fi
}

# Iniciar uma eleição manualmente
force_election() {
    local porta=$1
    echo "Forçando eleição no servidor $porta..."
    curl -s -X POST http://localhost:$porta/health/eleicao
}

# Menu principal
while true; do
    echo ""
    echo "Menu de Teste:"
    echo "1. Iniciar servidores"
    echo "2. Verificar status dos servidores"
    echo "3. Derrubar servidor principal (porta 5001)"
    echo "4. Derrubar servidor secundário (porta 5002)"
    echo "5. Derrubar servidor secundário (porta 5003)"
    echo "6. Forçar eleição em um servidor específico"
    echo "0. Sair"
    echo ""
    read -p "Escolha uma opção: " opcao

    case $opcao in
        1)
            start_servers
            ;;
        2)
            check_leader
            ;;
        3)
            kill_server 5001
            sleep 5
            check_leader
            ;;
        4)
            kill_server 5002
            sleep 5
            check_leader
            ;;
        5)
            kill_server 5003
            sleep 5
            check_leader
            ;;
        6)
            read -p "Digite a porta do servidor para forçar eleição (5001, 5002 ou 5003): " porta_eleicao
            force_election $porta_eleicao
            sleep 5
            check_leader
            ;;
        0)
            echo "Saindo..."
            exit 0
            ;;
        *)
            echo "Opção inválida!"
            ;;
    esac
done