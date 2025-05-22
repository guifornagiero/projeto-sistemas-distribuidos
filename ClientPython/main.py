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
    BASE_URL = "http://localhost:8080"
    
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

class ChatPanel(QWidget):
    def __init__(self, current_user):
        super().__init__()
        self.current_user = current_user
        self.messages = self.get_mock_messages()
        self.init_ui()
        
    def init_ui(self):
        layout = QVBoxLayout()
        
        # Título do chat
        chat_title = QLabel(f"Chat entre {self.current_user.login} e gianluca")
        chat_title.setFont(QFont("Arial", 12, QFont.Bold))
        chat_title.setAlignment(Qt.AlignCenter)
        
        # Área de mensagens
        self.messages_area = QWidget()
        self.messages_layout = QVBoxLayout(self.messages_area)
        self.messages_layout.setSpacing(10)
        self.messages_layout.setAlignment(Qt.AlignTop)
        
        # Adicionar mensagens
        for message in self.messages:
            is_from_current_user = message.get("remetente", {}).get("login") == self.current_user.login
            self.messages_layout.addWidget(MessageWidget(message, is_from_current_user))
        
        # Área de rolagem para as mensagens
        scroll_area = QScrollArea()
        scroll_area.setWidgetResizable(True)
        scroll_area.setWidget(self.messages_area)
        
        # Campo de texto e botão para enviar
        input_layout = QHBoxLayout()
        
        self.message_input = QLineEdit()
        self.message_input.setPlaceholderText("Digite sua mensagem...")
        self.message_input.returnPressed.connect(self.send_message)
        
        send_button = QPushButton("Enviar")
        send_button.clicked.connect(self.send_message)
        
        input_layout.addWidget(self.message_input)
        input_layout.addWidget(send_button)
        
        layout.addWidget(chat_title)
        layout.addWidget(scroll_area)
        layout.addLayout(input_layout)
        
        self.setLayout(layout)
        
        # Rolar para a última mensagem
        QTimer.singleShot(100, lambda: scroll_area.verticalScrollBar().setValue(
            scroll_area.verticalScrollBar().maximum()))
    
    def send_message(self):
        text = self.message_input.text().strip()
        if not text:
            return
        
        new_message = {
            "id": len(self.messages) + 1,
            "remetente": {
                "id": 999,
                "nome": self.current_user.nome,
                "login": self.current_user.login
            },
            "texto": text,
            "enviadaEm": datetime.now().isoformat()
        }
        
        self.messages.append(new_message)
        self.messages_layout.addWidget(MessageWidget(new_message, True))
        self.message_input.clear()
        
        # Rolar para a última mensagem
        QTimer.singleShot(100, lambda: self.parent().parent().verticalScrollBar().setValue(
            self.parent().parent().verticalScrollBar().maximum()))
    
    def get_mock_messages(self):
        return [
            {
                "id": 1,
                "remetente": {
                    "id": 1,
                    "nome": "Guilherme",
                    "login": "guifornagiero"
                },
                "texto": "asdijajsidajisdjiasjid",
                "enviadaEm": "2025-05-15T22:12:51.5916872-03:00"
            },
            {
                "id": 2,
                "remetente": {
                    "id": 4,
                    "nome": "Gian",
                    "login": "gianluca"
                },
                "texto": "123912u893nkakdsads-a-sdaijdsasudhi",
                "enviadaEm": "2025-05-15T22:13:00.3975898-03:00"
            },
            {
                "id": 3,
                "remetente": {
                    "id": 1,
                    "nome": "Guilherme",
                    "login": "guifornagiero"
                },
                "texto": "asdijajsidajisdjiasjid",
                "enviadaEm": "2025-05-15T22:12:51.5916872-03:00"
            },
            {
                "id": 4,
                "remetente": {
                    "id": 4,
                    "nome": "Gian",
                    "login": "gianluca"
                },
                "texto": "123912u893nkakdsads-a-sdaijdsasudhi",
                "enviadaEm": "2025-05-15T22:13:00.3975898-03:00"
            },
            {
                "id": 5,
                "remetente": {
                    "id": 1,
                    "nome": "Guilherme",
                    "login": "guifornagiero"
                },
                "texto": "asdijajsidajisdjiasjid",
                "enviadaEm": "2025-05-15T22:12:51.5916872-03:00"
            },
            {
                "id": 6,
                "remetente": {
                    "id": 4,
                    "nome": "Gian",
                    "login": "gianluca"
                },
                "texto": "123912u893nkakdsads-a-sdaijdsasudhi",
                "enviadaEm": "2025-05-15T22:13:00.3975898-03:00"
            }
        ]

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
        chat_widget = QScrollArea()
        chat_widget.setWidgetResizable(True)
        chat_widget.setWidget(ChatPanel(self.user))
        
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