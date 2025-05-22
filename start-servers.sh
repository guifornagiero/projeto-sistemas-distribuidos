#!/bin/bash

dotnet build SistemasDistribuidosServer/SistemasDistribuidosServer.csproj

if [ $? -ne 0 ]; then
  echo "Abort."
  exit 1
fi



PROJECT_PATH=$(pwd)

start_server() {
  local port=$1
  echo "🔹 Iniciando servidor na porta $port..."
  osascript -e "tell application \"Terminal\" to do script \"cd $PROJECT_PATH && dotnet run --no-build --project SistemasDistribuidosServer/SistemasDistribuidosServer.csproj --urls=http://localhost:$port\""
  sleep 2
}

start_server 5001
start_server 5002
start_server 5003

echo "✅ Todos os servidores iniciados!"
echo "Servidor 1: http://localhost:5001"
echo "Servidor 2: http://localhost:5002"
echo "Servidor 3: http://localhost:5003"
echo "Nginx (Load Balancer): http://localhost:8080"