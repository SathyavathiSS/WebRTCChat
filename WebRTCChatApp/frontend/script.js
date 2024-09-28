const API_URL = 'http://webrtc:8080/api';
const SIGNALR_URL = `http://signaling-service:8080/hub`;

class ChatApp {
    constructor() {
        this.connection = null;
        this.roomId = '';
        this.userId = '';
        this.localStream = null; // Store local media stream
        this.webrtc = null; // Initialize SimpleWebRTC instance
        this.setupEventListeners();
        this.loadUser();
        this.connectSignalR();
        this.initWebRTC(); // Initialize SimpleWebRTC
    }

    initWebRTC() {
        // Initialize SimpleWebRTC
        this.webrtc = new SimpleWebRTC({
            // Options for SimpleWebRTC
            localVideoEl: 'localVideo', // ID of local video element
            remoteVideosEl: 'remoteVideos', // ID of remote video container
            autoRequestMedia: true // Automatically request access to media
        });

        // Handle remote video added event
        this.webrtc.on('videoAdded', (video, peer) => {
            document.getElementById('remoteVideos').appendChild(video);
        });

        // Join a room when initialized
        this.webrtc.on('readyToCall', () => {
            if (this.roomId) {
                this.webrtc.joinRoom(this.roomId);
            }
        });

        // Send ICE candidates to the signaling server
        this.webrtc.on('ice', (candidate) => {
            this.connection.invoke('SendICECandidate', candidate);
        });
    }

    connectSignalR() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(SIGNALR_URL)
            .configureLogging(signalR.LogLevel.Information)
            .build();

        this.connection.start()
            .then(() => console.log('SignalR connected'))
            .catch(err => console.error('SignalR connection error:', err));

        this.connection.on('ReceiveMessage', (message) => {
            this.displayMessage(message);
        });

        this.connection.on('ReceiveOffer', (offer, peerId) => {
            this.handleOffer(offer, peerId);
        });

        this.connection.on('ReceiveAnswer', (answer, peerId) => {
            this.handleAnswer(answer, peerId);
        });

        this.connection.on('ReceiveICECandidate', (candidate, peerId) => {
            this.handleNewICECandidate(candidate, peerId);
        });
    }

    async handleOffer(offer, peerId) {
        // Use SimpleWebRTC to handle the offer
        this.webrtc.createPeer({ id: peerId, room: this.roomId });
        await this.webrtc.getLocalStream().getTracks().forEach(track => {
            this.webrtc.addTrack(track, peerId);
        });
        
        // Send the offer back to the signaling server
        this.connection.invoke('SendOffer', peerId, offer);
    }

    async sendOffer(peerId, offer) {
        this.connection.invoke('SendOffer', peerId, offer);
    }

    async handleAnswer(answer, peerId) {
        const peerConnection = this.webrtc.getPeer(peerId);
        await peerConnection.setRemoteDescription(new RTCSessionDescription(answer));
    }

    async handleNewICECandidate(candidate, peerId) {
        const peerConnection = this.webrtc.getPeer(peerId);
        await peerConnection.addIceCandidate(new RTCIceCandidate(candidate));
    }

    setupEventListeners() {
        document.getElementById('message-form').addEventListener('submit', this.sendMessage.bind(this));
        document.getElementById('create-room-button').addEventListener('click', this.createRoom.bind(this));
        document.getElementById('join-room-button').addEventListener('click', this.handleRoomSelection.bind(this));
    }

    loadUser() {
        this.userId = '2'; // Example user ID
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
        this.connection.invoke('JoinRoom', roomId);
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

    sendMessage(e) {
        e.preventDefault();
        const input = document.getElementById('message-input');
        const message = input.value.trim();

        if (message.length > 0) {
            // Send the message using SignalR
            this.connection.invoke('SendMessage', this.roomId, message);
            input.value = '';
        }
    }

    displayMessage(message) {
        const messageContainer = document.getElementById('messages');
        const messageElement = document.createElement('div');
        messageElement.textContent = message;
        messageContainer.appendChild(messageElement);
    }
}

new ChatApp();
