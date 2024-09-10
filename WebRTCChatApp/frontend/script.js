const API_URL = 'http://webrtc:8080/api'; // Change this as per your backend URL

class ChatApp {
    constructor() {
        this.socket = null;
        this.roomId = '';
        this.userId = '';

        this.setupEventListeners();
        this.loadUser();
        this.connectWebSocket();
    }

    connectWebSocket() {
        const MAX_RETRIES = 5;
        let retries = 0;
        const initialDelay = 1000; // 1 second
        const maxDelay = 30000; // 30 seconds

        const attemptConnection = () => {
            if (retries >= MAX_RETRIES) {
                console.error('Max retries reached. Giving up.');
                return;
            }

            const socketUrl = 'wss://refactored-disco-r79rx4x5gwrfxvjr-8082.app.github.dev/ws'; // Use your actual WebSocket URL
            console.log('Attempting to connect to WebSocket:', socketUrl);

            this.socket = new WebSocket(socketUrl);

            this.socket.onopen = () => {
                console.log('Successfully connected to WebSocket');
            };

            this.socket.onmessage = (event) => {
                const data = JSON.parse(event.data);
                this.handleIncomingMessage(data);
            };

            this.socket.onerror = (error) => {
                console.error('WebSocket Error:', error);
                retries++;
                console.log(`Retry attempt ${retries}:`);
                const delay = Math.min(initialDelay * Math.pow(2, retries), maxDelay);
                setTimeout(attemptConnection, delay);
            };

            this.socket.onclose = () => {
                console.log('Disconnected from WebSocket');
                retries++;
                const delay = Math.min(initialDelay * Math.pow(2, retries), maxDelay);
                setTimeout(attemptConnection, delay);
            };
        };

        attemptConnection();
    }

    sendMessage(e) {
        e.preventDefault();
        const input = document.getElementById('message-input');
        const message = input.value.trim();

        if (message.length > 0) {
            this.socket.send(JSON.stringify({ type: 'sendMessage', roomId: this.roomId, message }));
            input.value = '';
        }
    }

    handleIncomingMessage(data) {
        if (data.type === 'receiveMessage') {
            this.displayMessage(data.message);
        }
    }

    displayMessage(message) {
        const messageContainer = document.getElementById('messages');
        const messageElement = document.createElement('div');
        messageElement.textContent = message;
        messageContainer.appendChild(messageElement);
    }

    setupEventListeners() {
        document.getElementById('message-form').addEventListener('submit', this.sendMessage.bind(this));
        document.getElementById('create-room-button').addEventListener('click', this.createRoom.bind(this));
        document.getElementById('join-room-button').addEventListener('click', this.handleRoomSelection.bind(this));
    }

    loadUser() {
        this.userId = 'user123'; // Replace with actual user ID logic
    }

    async createRoom() {
        const roomName = document.getElementById('room-name-input').value.trim();
        console.log('Creating room:', roomName);

        if (roomName.length > 0) {
            try {
                const response = await fetch(`${API_URL}/chat/create-room`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ name: roomName })
                });

                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }

                const data = await response.json();
                console.log('API Response:', data);

                if (data.success && data.id) {
                    console.log('Room created successfully');
                    this.roomId = data.id;
                    this.joinRoom(data.id);
                    this.refreshRoomList();
                } else {
                    throw new Error('Failed to create room');
                }
            } catch (error) {
                console.error('Error creating room:', error);
                alert(`An error occurred while creating the room:\n${error.message}\nPlease try again.`);
            }
        } else {
            console.log('Room name is empty');
        }
    }

    joinRoom(roomId) {
        console.log('Attempting to join room:', roomId);
        this.roomId = roomId;
        this.socket.send(JSON.stringify({ type: 'joinRoom', roomId }));
    }

    handleRoomSelection() {
        const selectedRoomId = document.getElementById('room-select').value;
        this.joinRoom(selectedRoomId);
    }

    async refreshRoomList() {
        try {
            const response = await fetch(`${API_URL}/chat/rooms`);
            const rooms = await response.json();

            const roomSelect = document.getElementById('room-select');
            roomSelect.innerHTML = ''; // Clear current options

            rooms.forEach(room => {
                const option = document.createElement('option');
                option.value = room.id;
                option.textContent = room.name;
                roomSelect.appendChild(option);
            });
        } catch (error) {
            console.error('Error fetching room list:', error);
        }
    }
}

new ChatApp();
