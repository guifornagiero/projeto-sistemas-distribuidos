#!/usr/bin/env python3
"""
Script de inicialização para o Cliente Python do Sistema Distribuído.
Este script verifica se as dependências estão instaladas e inicia a aplicação.
"""

import sys
import subprocess
import importlib.util

def check_dependency(module_name):
    """Verifica se um módulo está instalado."""
    return importlib.util.find_spec(module_name) is not None

def install_dependencies():
    """Instala as dependências necessárias."""
    print("Verificando e instalando dependências...")
    try:
        subprocess.check_call([sys.executable, "-m", "pip", "install", "-r", "requirements.txt"])
        print("Dependências instaladas com sucesso!")
        return True
    except subprocess.CalledProcessError:
        print("Erro ao instalar dependências. Verifique sua conexão com a internet.")
        return False

def main():
    """Função principal."""
    # Verificar dependências principais
    dependencies = ["PyQt5", "requests"]
    missing_deps = [dep for dep in dependencies if not check_dependency(dep)]
    
    if missing_deps:
        print(f"Dependências ausentes: {', '.join(missing_deps)}")
        if not install_dependencies():
            return
    
    # Iniciar a aplicação
    try:
        from main import main as run_app
        run_app()
    except ImportError as e:
        print(f"Erro ao importar o módulo principal: {e}")
    except Exception as e:
        print(f"Erro ao executar a aplicação: {e}")

if __name__ == "__main__":
    main()