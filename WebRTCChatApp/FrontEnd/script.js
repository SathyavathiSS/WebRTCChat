const API_URL = 'http://webrtc:8080/api';
console.log('API URL:', API_URL);

class ChatApp {
    constructor() {
        this.socket = null;
        this.roomId = '';
        this.userId = '';

        this.setupEventListeners();
        this.loadUser();
        this.connectWebSocket();
    }

    // connectWebSocket() {
    //     const socketUrl = 'wss://' + window.location.host + '/ws';
    //     console.log('Attempting to connect to WebSocket:', socketUrl);
        
    //     this.socket = new WebSocket(socketUrl);
        
    //     this.socket.onopen = () => {
    //         console.log('Successfully connected to WebSocket');
    //     };
    
    //     this.socket.onerror = (error) => {
    //         console.error('WebSocket Error:', error);
    //         console.error('WebSocket URL:', socketUrl);
    //         console.error('Current host:', window.location.host);
    //         console.error('Detailed error message:', error.message);
    //         console.error('Error stack trace:', error.stack);
    //         // Try to reconnect
    //         this.reconnectWebSocket();
    //     };
    
    //     this.socket.onclose = () => {
    //         console.log('Disconnected from WebSocket');
    //         console.log('Close reason:', this.socket.closeReason);
    //         // Attempt to reconnect
    //         this.reconnectWebSocket();
    //     };
    // }

    connectWebSocket() {
        const MAX_RETRIES = 5;
        let retries = 0;
        const initialDelay = 1000; // 1 second
        const maxDelay = 30000; // 30 seconds
    
        function attemptConnection() {
            if (retries >= MAX_RETRIES) {
                console.error('Max retries reached. Giving up.');
                return;
            }
    
            const socketUrl = 'wss://refactored-disco-r79rx4x5gwrfxvjr-8082.app.github.dev/ws';
            console.log('Attempting to connect to WebSocket:', socketUrl);
            
            this.socket = new WebSocket(socketUrl);
            
            this.socket.onopen = () => {
                console.log('Successfully connected to WebSocket');
            };
    
            this.socket.onerror = (error) => {
                console.error('WebSocket Error:', error);
                retries++;
                console.log(`Retry attempt ${retries}:`);
                const delay = Math.min(initialDelay * Math.pow(2, retries), maxDelay);
                setTimeout(attemptConnection.bind(this), delay);
            };
    
            this.socket.onclose = () => {
                console.log('Disconnected from WebSocket');
                retries++;
                console.log(`Retry attempt ${retries}:`);
                const delay = Math.min(initialDelay * Math.pow(2, retries), maxDelay);
                setTimeout(attemptConnection.bind(this), delay);
            };
        }
    
        attemptConnection.call(this);
    }    

    reconnectWebSocket() {
        setTimeout(() => {
            console.log('Attempting to reconnect...');
            this.connectWebSocket();
        }, 5000); // Wait for 5 seconds before attempting to reconnect
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

    setupEventListeners() {
        document.getElementById('message-form').addEventListener('submit', this.sendMessage.bind(this));
        document.getElementById('create-room-button').addEventListener('click', this.createRoom.bind(this));
        document.getElementById('join-room-button').addEventListener('click', this.joinRoom.bind(this));
    }

    loadUser() {
        this.userId = 'user123'; // Replace with actual user ID
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
        this.socket.send(JSON.stringify({ type: 'joinRoom', roomId }));
    }

    refreshRoomList() {
        // Implement logic to refresh room list
    }
}

new ChatApp();
