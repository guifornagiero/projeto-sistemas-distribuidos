#!/bin/bash

echo "Iniciando servidores..."


echo "Iniciando servidor 1 na porta 5001..."
osascript -e 'tell app "Terminal" to do script "cd '$(pwd)' && dotnet run --project SistemasDistribuidosServer/SistemasDistribuidosServer.csproj --urls=http://localhost:5001"'

sleep 2

echo "Iniciando servidor 2 na porta 5002..."
osascript -e 'tell app "Terminal" to do script "cd '$(pwd)' && dotnet run --project SistemasDistribuidosServer/SistemasDistribuidosServer.csproj --urls=http://localhost:5002"'

sleep 2

echo "Iniciando servidor 3 na porta 5003..."
osascript -e 'tell app "Terminal" to do script "cd '$(pwd)' && dotnet run --project SistemasDistribuidosServer/SistemasDistribuidosServer.csproj --urls=http://localhost:5003"'

echo "Todos os servidores iniciados!"
echo "Servidor 1: http://localhost:5001"
echo "Servidor 2: http://localhost:5002"
echo "Servidor 3: http://localhost:5003"
echo "Nginx (Load Balancer): http://localhost:8080" 