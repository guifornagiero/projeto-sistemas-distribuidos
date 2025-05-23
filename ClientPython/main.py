import sys
import json
from datetime import datetime
from PyQt5.QtWidgets import (QApplication, QMainWindow, QWidget, QVBoxLayout, QHBoxLayout, 
                            QLabel, QLineEdit, QPushButton, QTextEdit, QScrollArea, 
                            QFrame, QTabWidget, QListWidget, QListWidgetItem, QSplitter)
from PyQt5.QtCore import Qt, QSize, QTimer, pyqtSignal
from PyQt5.QtGui import QFont, QColor

import requests

class User:
    def __init__(self, login, nome, id=None, seguindo=None):
        self.login = login
        self.nome = nome
        self.id = id
        self.seguindo = seguindo or []

class ApiService:
    BASE_URL = "http://localhost:5001"
    
    @staticmethod
    def get_user_by_login(login):
        try:
            response = requests.get(f"{ApiService.BASE_URL}/Usuario/Login/{login}")
            if response.status_code == 200:
                return response.json()
            return None
        except Exception as e:
            print(f"Erro ao buscar usuário: {e}")
            return None
    
    @staticmethod
    def get_timeline():
        try:
            response = requests.get(f"{ApiService.BASE_URL}/Postagem/Timeline")
            if response.status_code == 200:
                return response.json()
            return []
        except Exception as e:
            print(f"Erro ao carregar timeline: {e}")
            return []
    
    @staticmethod
    def create_post(post_data):
        try:
            response = requests.post(f"{ApiService.BASE_URL}/Postagem", json=post_data)
            return response.status_code == 200
        except Exception as e:
            print(f"Erro ao criar post: {e}")
            return False
    
    @staticmethod
    def get_notifications(login):
        try:
            response = requests.get(f"{ApiService.BASE_URL}/Usuario/Notificacoes/{login}")
            if response.status_code == 200:
                return response.json()
            return []
        except Exception as e:
            print(f"Erro ao buscar notificações: {e}")
            return []
    
    @staticmethod
    def follow_user(logged_user, user_to_follow):
        try:
            data = {
                "loginQuerSeguir": logged_user,
                "loginParaSeguir": user_to_follow
            }
            response = requests.post(f"{ApiService.BASE_URL}/Usuario/Seguir/", json=data)
            return response.status_code == 200
        except Exception as e:
            print(f"Erro ao seguir usuário: {e}")
            return False
            
    @staticmethod
    def get_chats_by_user(login):
        try:
            response = requests.get(f"{ApiService.BASE_URL}/Chat/GetChatsByUser/{login}")
            if response.status_code == 200:
                return response.json()
            return []
        except Exception as e:
            print(f"Erro ao buscar chats: {e}")
            return []
    
    @staticmethod
    def get_chat(usuario1, usuario2):
        try:
            response = requests.get(f"{ApiService.BASE_URL}/Chat/Chat/{usuario1}&{usuario2}")
            if response.status_code == 200:
                return response.json()
            return None
        except Exception as e:
            print(f"Erro ao buscar chat: {e}")
            return None
    
    @staticmethod
    def send_message(enviando, recebendo, mensagem):
        try:
            response = requests.post(
                f"{ApiService.BASE_URL}/Chat/EnviarMensagem/{enviando}&{recebendo}", 
                json=mensagem
            )
            if response.status_code == 200:
                return response.json()
            return None
        except Exception as e:
            print(f"Erro ao enviar mensagem: {e}")
            return None
            
    @staticmethod
    def get_all_users():
        try:
            response = requests.get(f"{ApiService.BASE_URL}/Usuario/GetAll")
            if response.status_code == 200:
                return response.json()
            return []
        except Exception as e:
            print(f"Erro ao buscar todos os usuários: {e}")
            return []

class LoginWindow(QWidget):
    login_successful = pyqtSignal(User)
    
    def __init__(self):
        super().__init__()
        self.init_ui()
        
    def init_ui(self):
        self.setWindowTitle("Login - Sistema Distribuído")
        self.setGeometry(300, 300, 400, 200)
        
        layout = QVBoxLayout()
        
        title = QLabel("Login")
        title.setFont(QFont("Arial", 18, QFont.Bold))
        title.setStyleSheet("color: black;")
        title.setAlignment(Qt.AlignCenter)
        
        self.username_input = QLineEdit()
        self.username_input.setPlaceholderText("Digite seu login")
        self.username_input.setFixedHeight(40)
        
        self.time_input = QLineEdit()
        self.time_input.setPlaceholderText("Digite o tempo")
        self.time_input.setFixedHeight(40)
        self.time_input.setText("0")
        
        self.login_button = QPushButton("Entrar")
        self.login_button.setFixedHeight(40)
        self.login_button.clicked.connect(self.handle_login)
        
        self.error_label = QLabel("")
        self.error_label.setStyleSheet("color: red; background-color: transparent;")
        self.error_label.setAlignment(Qt.AlignCenter)
        
        layout.addWidget(title)
        layout.addWidget(QLabel("Login:"))
        layout.addWidget(self.username_input)
        layout.addWidget(QLabel("Tempo:"))
        layout.addWidget(self.time_input)
        layout.addWidget(self.login_button)
        layout.addWidget(self.error_label)
        
        self.setLayout(layout)
    
    def handle_login(self):
        username = self.username_input.text().strip()
        time = self.time_input.text().strip() or "0"
        
        valid_users = ["guifornagiero", "gianluca", "paulobrito", "pedrobento"]
        
        if username in valid_users:
            user_data = ApiService.get_user_by_login(username)
            if user_data:
                user = User(
                    login=user_data.get("login"),
                    nome=user_data.get("nome"),
                    id=user_data.get("id"),
                    seguindo=user_data.get("seguindo", [])
                )
                self.login_successful.emit(user)
            else:
                self.error_label.setText("Usuário inválido no sistema!")
        else:
            self.error_label.setText("Usuário não encontrado!")

class PostWidget(QFrame):
    def __init__(self, post_data):
        super().__init__()
        self.post_data = post_data
        self.init_ui()
        
    def init_ui(self):
        self.setFrameShape(QFrame.StyledPanel)
        self.setStyleSheet("background-color: white; border-radius: 10px; padding: 10px; color: black;")
        
        layout = QVBoxLayout()
        
        # Título do post
        title = QLabel(self.post_data.get("titulo", "Sem título"))
        title.setFont(QFont("Arial", 12, QFont.Bold))
        
        # Informações do autor e data
        creator_date = QLabel(f"{self.post_data.get('criadorNome', 'Anônimo')} • {self.format_date(self.post_data.get('dataCriacao', ''))}")
        creator_date.setStyleSheet("color: #555555;")
        
        # Conteúdo do post
        content = QLabel(self.post_data.get("conteudo", ""))
        content.setWordWrap(True)
        
        layout.addWidget(title)
        layout.addWidget(creator_date)
        layout.addWidget(content)
        
        self.setLayout(layout)
    
    def format_date(self, date_str):
        if not date_str:
            return ""
        
        try:
            date_obj = datetime.fromisoformat(date_str.replace('Z', '+00:00'))
            return date_obj.strftime("%d/%m/%Y %H:%M:%S")
        except Exception:
            return date_str

class NotificationWidget(QFrame):
    def __init__(self, notification_data):
        super().__init__()
        self.notification_data = notification_data
        self.init_ui()
        
    def init_ui(self):
        self.setFrameShape(QFrame.StyledPanel)
        self.setStyleSheet("background-color: white; border-radius: 10px; padding: 10px;")
        
        layout = QVBoxLayout()
        
        # Descrição da notificação
        description = QLabel(self.notification_data.get("descricao", ""))
        description.setWordWrap(True)
        
        # Detalhes da postagem
        details = QLabel(f"<b>Título:</b> {self.notification_data.get('postagemTitulo', '')}<br>"
                         f"<b>Resumo:</b> {self.notification_data.get('postagemDescricao', '')}<br>"
                         f"<b>Data:</b> {self.format_date(self.notification_data.get('criadaEm', ''))}")
        details.setWordWrap(True)
        details.setStyleSheet("color: #555555; font-size: 10px;")
        
        layout.addWidget(description)
        layout.addWidget(details)
        
        self.setLayout(layout)
    
    def format_date(self, date_str):
        if not date_str:
            return ""
        
        try:
            date_obj = datetime.fromisoformat(date_str.replace('Z', '+00:00'))
            return date_obj.strftime("%d/%m/%Y %H:%M:%S")
        except Exception:
            return date_str

class FollowWidget(QFrame):
    def __init__(self, user_login, logged_user, refresh_callback=None):
        super().__init__()
        self.user_login = user_login
        self.logged_user = logged_user
        self.is_following = any(s.get('login') == user_login for s in logged_user.seguindo)
        self.refresh_callback = refresh_callback
        self.init_ui()
        
    def init_ui(self):
        self.setFrameShape(QFrame.StyledPanel)
        self.setStyleSheet("background-color: white; border-radius: 10px; padding: 10px;")
        
        layout = QHBoxLayout()
        
        # Nome do usuário
        user_label = QLabel(self.user_login)
        user_label.setFont(QFont("Arial", 10))
        
        # Botão de seguir
        self.follow_button = QPushButton("Seguindo" if self.is_following else "Seguir")
        self.follow_button.setStyleSheet(
            "background-color: #CCCCCC; color: #333333;" if self.is_following else
            "background-color: #0066CC; color: white;"
        )
        self.follow_button.clicked.connect(self.toggle_follow)
        
        layout.addWidget(user_label)
        layout.addStretch()
        layout.addWidget(self.follow_button)
        
        self.setLayout(layout)
    
    def toggle_follow(self):
        if not self.is_following:
            success = ApiService.follow_user(self.logged_user.login, self.user_login)
            if success:
                self.is_following = True
                self.follow_button.setText("Seguindo")
                self.follow_button.setStyleSheet("background-color: #CCCCCC; color: #333333;")
                
                # Atualizar a lista de seguindo do usuário logado
                user_data = ApiService.get_user_by_login(self.logged_user.login)
                if user_data:
                    self.logged_user.seguindo = user_data.get("seguindo", [])
                
                if self.refresh_callback:
                    self.refresh_callback()

class MessageWidget(QFrame):
    def __init__(self, message_data, is_from_current_user):
        super().__init__()
        self.message_data = message_data
        self.is_from_current_user = is_from_current_user
        self.init_ui()
        
    def init_ui(self):
        self.setFrameShape(QFrame.StyledPanel)
        bg_color = "#E6F2FF" if self.is_from_current_user else "#F5F5F5"
        self.setStyleSheet(f"background-color: {bg_color}; border-radius: 10px; padding: 10px; color: black;")
        
        layout = QVBoxLayout()
        
        # Cabeçalho (nome e data)
        header_layout = QHBoxLayout()
        
        sender_name = QLabel(self.message_data.get("remetente", {}).get("nome", "Anônimo"))
        sender_name.setFont(QFont("Arial", 10, QFont.Bold))
        
        time_label = QLabel(self.format_date(self.message_data.get("enviadaEm", "")))
        time_label.setStyleSheet("color: #555555; font-size: 9px;")
        
        header_layout.addWidget(sender_name)
        header_layout.addStretch()
        header_layout.addWidget(time_label)
        
        # Conteúdo da mensagem
        content = QLabel(self.message_data.get("texto", ""))
        content.setWordWrap(True)
        
        layout.addLayout(header_layout)
        layout.addWidget(content)
        
        self.setLayout(layout)
        
    def format_date(self, date_str):
        if not date_str:
            return ""
        
        try:
            date_obj = datetime.fromisoformat(date_str.replace('Z', '+00:00'))
            return date_obj.strftime("%d/%m/%Y %H:%M:%S")
        except Exception:
            return date_str

class UserChatButton(QPushButton):
    def __init__(self, user_login, is_active=False, parent=None):
        super().__init__(parent)
        self.user_login = user_login
        self.setText(user_login)
        self.setMinimumHeight(40)
        self.setCursor(Qt.PointingHandCursor)
        self.update_style(is_active)
        
    def update_style(self, is_active):
        if is_active:
            self.setStyleSheet("""
                QPushButton {
                    background-color: #E6F2FF;
                    border-radius: 10px;
                    padding: 8px;
                    text-align: left;
                    font-weight: bold;
                    border: 2px solid #4A86E8;
                }
            """)
        else:
            self.setStyleSheet("""
                QPushButton {
                    background-color: #F5F5F5;
                    border-radius: 10px;
                    padding: 8px;
                    text-align: left;
                }
                QPushButton:hover {
                    background-color: #E6F2FF;
                }
            """)

class ChatPanel(QWidget):
    def __init__(self, current_user):
        super().__init__()
        self.current_user = current_user
        self.chats = []
        self.all_users = []
        self.current_chat = None
        self.current_chat_partner = None
        self.messages = []
        self.showing_all_users = False
        self.init_ui()
        self.load_chats()
        self.load_all_users()
        
    def init_ui(self):
        self.main_layout = QHBoxLayout(self)
        self.main_layout.setContentsMargins(0, 0, 0, 0)
        
        # Painel da esquerda (lista de chats)
        self.left_panel = QWidget()
        self.left_panel.setFixedWidth(250)
        self.left_panel.setStyleSheet("background-color: white; border-radius: 10px;")
        left_layout = QVBoxLayout(self.left_panel)
        
        # Cabeçalho da lista de chats
        header_layout = QHBoxLayout()
        chats_title = QLabel("Conversas")
        chats_title.setFont(QFont("Arial", 14, QFont.Bold))
        
        self.new_chat_btn = QPushButton("Nova")
        self.new_chat_btn.setFixedWidth(60)
        self.new_chat_btn.setCursor(Qt.PointingHandCursor)
        self.new_chat_btn.setStyleSheet("""
            QPushButton {
                background-color: #4A86E8;
                color: white;
                border-radius: 5px;
                padding: 5px;
            }
            QPushButton:hover {
                background-color: #3B78E7;
            }
        """)
        self.new_chat_btn.clicked.connect(self.toggle_users_list)
        
        header_layout.addWidget(chats_title)
        header_layout.addWidget(self.new_chat_btn)
        
        # Container para a lista de chats
        self.chats_container = QWidget()
        self.chats_layout = QVBoxLayout(self.chats_container)
        self.chats_layout.setAlignment(Qt.AlignTop)
        self.chats_layout.setSpacing(8)
        
        # Scroll para a lista de chats
        chats_scroll = QScrollArea()
        chats_scroll.setWidgetResizable(True)
        chats_scroll.setWidget(self.chats_container)
        chats_scroll.setHorizontalScrollBarPolicy(Qt.ScrollBarAlwaysOff)
        chats_scroll.setStyleSheet("border: none;")
        
        left_layout.addLayout(header_layout)
        left_layout.addWidget(chats_scroll)
        
        # Painel da direita (mensagens)
        self.right_panel = QWidget()
        self.right_panel.setStyleSheet("background-color: white; border-radius: 10px;")
        self.right_layout = QVBoxLayout(self.right_panel)
        
        # Mensagem inicial
        self.placeholder = QLabel("Selecione um chat para visualizar as mensagens")
        self.placeholder.setAlignment(Qt.AlignCenter)
        self.placeholder.setStyleSheet("color: #666; font-size: 14px;")
        self.right_layout.addWidget(self.placeholder)
        
        # Adicionar painéis ao layout principal
        self.main_layout.addWidget(self.left_panel)
        self.main_layout.addWidget(self.right_panel, 1)
        
        # Timer para atualização dos chats
        self.update_timer = QTimer(self)
        self.update_timer.timeout.connect(self.refresh_chats)
        self.update_timer.start(3000)  # Atualiza a cada 3 segundos
    
    def load_chats(self):
        # Limpar chats existentes
        self.clear_chats_list()
        
        # Buscar chats do usuário
        chats = ApiService.get_chats_by_user(self.current_user.login)
        
        # Filtrar apenas chats que possuem pelo menos uma mensagem
        self.chats = [chat for chat in chats if chat.get("mensagens") and len(chat.get("mensagens")) > 0]
        
        if not self.chats:
            no_chats = QLabel("Nenhuma conversa ativa encontrada")
            no_chats.setAlignment(Qt.AlignCenter)
            no_chats.setStyleSheet("color: #666; font-size: 12px; padding: 10px;")
            self.chats_layout.addWidget(no_chats)
            return
        
        # Adicionar chats à lista
        for chat in self.chats:
            chat_partner = chat["usuario1"] if chat["usuario1"] != self.current_user.login else chat["usuario2"]
            is_active = self.current_chat and self.current_chat.get("id") == chat["id"]
            
            chat_btn = UserChatButton(chat_partner, is_active)
            chat_btn.clicked.connect(lambda checked, c=chat: self.select_chat(c))
            self.chats_layout.addWidget(chat_btn)
    
    def load_all_users(self):
        self.all_users = ApiService.get_all_users()
    
    def toggle_users_list(self):
        self.showing_all_users = not self.showing_all_users
        self.new_chat_btn.setText("Voltar" if self.showing_all_users else "Nova")
        
        self.clear_chats_list()
        
        if self.showing_all_users:
            # Mostrar todos os usuários disponíveis
            other_users = [user for user in self.all_users if user["login"] != self.current_user.login]
            
            if not other_users:
                no_users = QLabel("Nenhum outro usuário encontrado")
                no_users.setAlignment(Qt.AlignCenter)
                no_users.setStyleSheet("color: #666; font-size: 12px; padding: 10px;")
                self.chats_layout.addWidget(no_users)
                return
            
            users_title = QLabel("Selecione um usuário:")
            users_title.setStyleSheet("font-weight: bold; margin-top: 5px;")
            self.chats_layout.addWidget(users_title)
            
            for user in other_users:
                user_btn = UserChatButton(user["login"])
                user_btn.clicked.connect(lambda checked, u=user: self.start_new_chat(u["login"]))
                self.chats_layout.addWidget(user_btn)
        else:
            # Voltar para a lista de chats
            self.load_chats()
    
    def clear_chats_list(self):
        # Limpar todos os widgets do container de chats
        for i in reversed(range(self.chats_layout.count())):
            widget = self.chats_layout.itemAt(i).widget()
            if widget:
                widget.deleteLater()
    
    def select_chat(self, chat):
        self.current_chat = chat
        self.current_chat_partner = chat["usuario1"] if chat["usuario1"] != self.current_user.login else chat["usuario2"]
        
        # Atualizar a interface destacando o chat selecionado
        self.load_chats()
        
        # Limpar o painel direito
        self.clear_right_panel()
        
        # Criar a interface de chat
        self.setup_chat_interface()
        
        # Carregar mensagens
        self.load_messages()
    
    def start_new_chat(self, user_login):
        # Verificar se já existe um chat com este usuário
        existing_chat = next((chat for chat in self.chats if 
            chat["usuario1"] == user_login or chat["usuario2"] == user_login), None)
        
        if existing_chat:
            self.select_chat(existing_chat)
        else:
            # Enviar uma mensagem inicial para criar o chat
            mensagem_enviada = ApiService.send_message(self.current_user.login, user_login, "Olá!")
            
            if mensagem_enviada:
                # Aguardar brevemente para o servidor processar
                QTimer.singleShot(500, lambda: self.refresh_new_chat(user_login))
            else:
                # Falha ao enviar mensagem
                QMessageBox.warning(self, "Erro", "Não foi possível iniciar a conversa.")
        
        # Voltar para a lista de chats
        self.showing_all_users = False
        self.new_chat_btn.setText("Nova")
    
    def clear_right_panel(self):
        # Remover todos os widgets do painel direito
        while self.right_layout.count():
            item = self.right_layout.takeAt(0)
            widget = item.widget()
            if widget:
                widget.deleteLater()
    
    def setup_chat_interface(self):
        # Título do chat
        chat_title = QLabel(f"Conversa com {self.current_chat_partner}")
        chat_title.setFont(QFont("Arial", 12, QFont.Bold))
        chat_title.setStyleSheet("margin-bottom: 10px;")
        
        # Área de mensagens
        self.messages_area = QWidget()
        self.messages_layout = QVBoxLayout(self.messages_area)
        self.messages_layout.setSpacing(10)
        self.messages_layout.setAlignment(Qt.AlignTop)
        
        # Área de rolagem para as mensagens
        self.scroll_area = QScrollArea()
        self.scroll_area.setWidgetResizable(True)
        self.scroll_area.setWidget(self.messages_area)
        self.scroll_area.setStyleSheet("border: none; background-color: #F5F5F5;")
        
        # Campo de texto e botão para enviar
        input_layout = QHBoxLayout()
        
        self.message_input = QLineEdit()
        self.message_input.setPlaceholderText(f"Escreva uma mensagem para {self.current_chat_partner}...")
        self.message_input.setStyleSheet("border: 1px solid #DDD; border-radius: 5px; padding: 8px;")
        self.message_input.returnPressed.connect(self.send_message)
        
        send_button = QPushButton("Enviar")
        send_button.setCursor(Qt.PointingHandCursor)
        send_button.setStyleSheet("""
            QPushButton {
                background-color: #4A86E8;
                color: white;
                border-radius: 5px;
                padding: 8px 15px;
            }
            QPushButton:hover {
                background-color: #3B78E7;
            }
        """)
        send_button.clicked.connect(self.send_message)
        
        input_layout.addWidget(self.message_input)
        input_layout.addWidget(send_button)
        
        # Adicionar widgets ao layout do painel direito
        self.right_layout.addWidget(chat_title)
        self.right_layout.addWidget(self.scroll_area, 1)
        self.right_layout.addLayout(input_layout)
    
    def load_messages(self):
        # Limpar mensagens existentes
        for i in reversed(range(self.messages_layout.count())):
            widget = self.messages_layout.itemAt(i).widget()
            if widget:
                widget.deleteLater()
        
        # Buscar mensagens atualizadas
        updated_chat = ApiService.get_chat(self.current_user.login, self.current_chat_partner)
        if updated_chat and "mensagens" in updated_chat:
            self.messages = updated_chat["mensagens"]
            
            if not self.messages:
                no_messages = QLabel("Nenhuma mensagem ainda. Comece a conversar!")
                no_messages.setAlignment(Qt.AlignCenter)
                no_messages.setStyleSheet("color: #666; font-size: 14px; padding: 20px;")
                self.messages_layout.addWidget(no_messages)
            else:
                # Adicionar mensagens à interface
                for message in self.messages:
                    is_from_current_user = message.get("remetente", {}).get("login") == self.current_user.login
                    self.messages_layout.addWidget(MessageWidget(message, is_from_current_user))
                
                # Rolar para a última mensagem
                QTimer.singleShot(100, lambda: self.scroll_area.verticalScrollBar().setValue(
                    self.scroll_area.verticalScrollBar().maximum()))
    
    def send_message(self):
        if not self.current_chat_partner:
            return
            
        text = self.message_input.text().strip()
        if not text:
            return
        
        # Enviar mensagem para a API
        success = ApiService.send_message(self.current_user.login, self.current_chat_partner, text)
        
        if success:
            # Limpar campo de texto
            self.message_input.clear()
            
            # Recarregar mensagens
            self.load_messages()
    
    def refresh_new_chat(self, user_login):
        # Buscar o chat recém-criado
        new_chat = ApiService.get_chat(self.current_user.login, user_login)
        if new_chat and new_chat.get("mensagens") and len(new_chat.get("mensagens")) > 0:
            self.current_chat = new_chat
            self.current_chat_partner = user_login
            
            # Limpar o painel direito
            self.clear_right_panel()
            
            # Criar a interface de chat
            self.setup_chat_interface()
            
            # Carregar mensagens
            self.load_messages()
            
            # Atualizar a lista de chats
            self.load_chats()
    
    def refresh_chats(self):
        # Salvar o chat atual
        current_chat_id = self.current_chat["id"] if self.current_chat else None
        
        # Atualizar lista de chats (filtragem acontece no load_chats)
        chats = ApiService.get_chats_by_user(self.current_user.login)
        
        # Se estiver mostrando a lista de chats, atualizar a interface
        if not self.showing_all_users:
            self.load_chats()
        
        # Se houver um chat selecionado, atualizar as mensagens
        if self.current_chat:
            self.load_messages()

class MainWindow(QMainWindow):
    def __init__(self, user):
        super().__init__()
        self.user = user
        self.init_ui()
        self.load_data()
        self.setup_timers()
        
    def init_ui(self):
        self.setWindowTitle(f"Sistema Distribuído - {self.user.nome}")
        self.setGeometry(100, 100, 1200, 800)
        
        # Layout principal
        main_widget = QWidget()
        main_layout = QHBoxLayout(main_widget)
        
        # 1. Painel de notificações
        self.notifications_widget = QWidget()
        self.notifications_layout = QVBoxLayout(self.notifications_widget)
        
        notifications_title = QLabel("Notificações")
        notifications_title.setFont(QFont("Arial", 14, QFont.Bold))
        notifications_title.setStyleSheet("color: black;")
        
        self.notifications_container = QWidget()
        self.notifications_container_layout = QVBoxLayout(self.notifications_container)
        self.notifications_container_layout.setAlignment(Qt.AlignTop)
        self.notifications_container_layout.setSpacing(10)
        
        notifications_scroll = QScrollArea()
        notifications_scroll.setWidgetResizable(True)
        notifications_scroll.setWidget(self.notifications_container)
        
        # Seção de sugestões
        suggestions_title = QLabel("Quem seguir")
        suggestions_title.setFont(QFont("Arial", 14, QFont.Bold))
        suggestions_title.setStyleSheet("color: black;")
        
        self.suggestions_container = QWidget()
        self.suggestions_layout = QVBoxLayout(self.suggestions_container)
        self.suggestions_layout.setAlignment(Qt.AlignTop)
        self.suggestions_layout.setSpacing(10)
        
        self.notifications_layout.addWidget(notifications_title)
        self.notifications_layout.addWidget(notifications_scroll)
        self.notifications_layout.addWidget(suggestions_title)
        self.notifications_layout.addWidget(self.suggestions_container)
        
        # 2. Painel central (TextArea + Timeline)
        center_widget = QWidget()
        center_layout = QVBoxLayout(center_widget)
        
        # TextArea para postagem
        post_widget = QWidget()
        post_layout = QVBoxLayout(post_widget)
        
        post_title = QLabel("Escreva uma postagem")
        post_title.setFont(QFont("Arial", 14, QFont.Bold))
        post_title.setStyleSheet("color: black;")
        
        self.post_title_input = QLineEdit()
        self.post_title_input.setPlaceholderText("Título da postagem")
        
        self.post_content_input = QTextEdit()
        self.post_content_input.setPlaceholderText("Escreva sua postagem...")
        
        post_button = QPushButton("Publicar")
        post_button.clicked.connect(self.publish_post)
        
        post_button_layout = QHBoxLayout()
        post_button_layout.addStretch()
        post_button_layout.addWidget(post_button)
        
        post_layout.addWidget(post_title)
        post_layout.addWidget(self.post_title_input)
        post_layout.addWidget(self.post_content_input)
        post_layout.addLayout(post_button_layout)
        
        post_widget.setStyleSheet("background-color: white; border-radius: 10px; padding: 10px; color: black;")
        
        # Timeline
        timeline_widget = QWidget()
        timeline_layout = QVBoxLayout(timeline_widget)
        
        timeline_title = QLabel("Timeline")
        timeline_title.setFont(QFont("Arial", 14, QFont.Bold))
        timeline_title.setStyleSheet("color: black;")
        
        self.posts_container = QWidget()
        self.posts_layout = QVBoxLayout(self.posts_container)
        self.posts_layout.setAlignment(Qt.AlignTop)
        self.posts_layout.setSpacing(10)
        
        timeline_scroll = QScrollArea()
        timeline_scroll.setWidgetResizable(True)
        timeline_scroll.setWidget(self.posts_container)
        
        timeline_layout.addWidget(timeline_title)
        timeline_layout.addWidget(timeline_scroll)
        
        timeline_widget.setStyleSheet("background-color: white; border-radius: 10px; padding: 10px; color: black;")
        
        center_layout.addWidget(post_widget, 1)
        center_layout.addWidget(timeline_widget, 3)
        
        # 3. Painel de chat
        self.chat_panel = ChatPanel(self.user)
        chat_widget = QScrollArea()
        chat_widget.setWidgetResizable(True)
        chat_widget.setWidget(self.chat_panel)
        
        # Adicionar os três painéis ao layout principal
        main_layout.addWidget(self.notifications_widget, 1)
        main_layout.addWidget(center_widget, 1)
        main_layout.addWidget(chat_widget, 1)
        
        self.setCentralWidget(main_widget)
    
    def load_data(self):
        # Carregar timeline
        self.load_timeline()
        
        # Carregar notificações
        self.load_notifications()
        
        # Carregar sugestões
        self.load_suggestions()
    
    def load_timeline(self):
        # Limpar posts existentes
        for i in reversed(range(self.posts_layout.count())):
            widget = self.posts_layout.itemAt(i).widget()
            if widget:
                widget.deleteLater()
        
        # Buscar posts da timeline
        posts = ApiService.get_timeline()
        
        # Adicionar posts à interface
        for post in posts:
            self.posts_layout.addWidget(PostWidget(post))
    
    def load_notifications(self):
        # Limpar notificações existentes
        for i in reversed(range(self.notifications_container_layout.count())):
            widget = self.notifications_container_layout.itemAt(i).widget()
            if widget:
                widget.deleteLater()
        
        # Buscar notificações
        notifications = ApiService.get_notifications(self.user.login)
        
        # Adicionar notificações à interface
        for notification in notifications:
            self.notifications_container_layout.addWidget(NotificationWidget(notification))
    
    def load_suggestions(self):
        # Limpar sugestões existentes
        for i in reversed(range(self.suggestions_layout.count())):
            widget = self.suggestions_layout.itemAt(i).widget()
            if widget:
                widget.deleteLater()
        
        # Lista de usuários disponíveis
        available_users = ["guifornagiero", "gianluca", "paulobrito", "pedrobento"]
        
        # Filtrar usuários que não são o usuário logado
        suggestions = [user for user in available_users if user != self.user.login]
        
        # Adicionar sugestões à interface
        for suggestion in suggestions:
            self.suggestions_layout.addWidget(
                FollowWidget(suggestion, self.user, self.refresh_user_data)
            )
    
    def publish_post(self):
        title = self.post_title_input.text().strip()
        content = self.post_content_input.toPlainText().strip()
        
        if not title or not content:
            return
        
        post_data = {
            "titulo": title,
            "conteudo": content,
            "criadorLogin": self.user.login
        }
        
        success = ApiService.create_post(post_data)
        if success:
            self.post_title_input.clear()
            self.post_content_input.clear()
            self.load_timeline()
    
    def refresh_user_data(self):
        user_data = ApiService.get_user_by_login(self.user.login)
        if user_data:
            self.user.seguindo = user_data.get("seguindo", [])
    
    def setup_timers(self):
        # Atualizar timeline e notificações a cada 3 segundos
        self.timeline_timer = QTimer(self)
        self.timeline_timer.timeout.connect(self.load_timeline)
        self.timeline_timer.start(3000)
        
        self.notifications_timer = QTimer(self)
        self.notifications_timer.timeout.connect(self.load_notifications)
        self.notifications_timer.start(3000)

def main():
    app = QApplication(sys.argv)
    app.setStyle('Fusion')
    
    # Aplicar estilos - garantindo que o texto seja preto para legibilidade
    app.setStyleSheet("""
        QMainWindow, QWidget {
            background-color: #f0f2f5;
            color: black;
        }
        QScrollArea {
            border: none;
        }
        QPushButton {
            padding: 8px 16px;
            border-radius: 4px;
            background-color: #0066CC;
            color: white;
            font-weight: bold;
        }
        QPushButton:hover {
            background-color: #0055AA;
        }
        QLineEdit, QTextEdit {
            padding: 8px;
            border: 1px solid #ddd;
            border-radius: 4px;
            color: black;
        }
    """)
    
    # Iniciar com a janela de login
    login_window = LoginWindow()
    
    def on_login_successful(user):
        login_window.hide()
        main_window = MainWindow(user)
        main_window.show()
    
    login_window.login_successful.connect(on_login_successful)
    login_window.show()
    
    sys.exit(app.exec_())

if __name__ == "__main__":
    main()