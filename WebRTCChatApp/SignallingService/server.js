const express = require('express');
const http = require('http');
const { Server } = require('socket.io');

const app = express();
const server = http.createServer(app);
const io = new Server(server);

// Setup SignalR hub
io.on('connection', (socket) => {
    console.log('A user connected:', socket.id);

    // Listen for signaling messages from the client
    socket.on('SendMessage', (roomId, message) => {
        // Broadcast the message to the specified room
        socket.to(roomId).emit('ReceiveMessage', message);
    });

    socket.on('SendOffer', (peerId, offer) => {
        socket.to(peerId).emit('ReceiveOffer', offer, socket.id);
    });

    socket.on('SendAnswer', (answer, peerId) => {
        socket.to(peerId).emit('ReceiveAnswer', answer, socket.id);
    });

    socket.on('SendICECandidate', (candidate, peerId) => {
        socket.to(peerId).emit('ReceiveICECandidate', candidate, socket.id);
    });

    socket.on('JoinRoom', (roomId) => {
        socket.join(roomId);
        console.log(`${socket.id} joined room: ${roomId}`);
    });

    socket.on('disconnect', () => {
        console.log('User disconnected:', socket.id);
    });
});

const PORT = 8080;
server.listen(PORT, () => {
    console.log(`Signaling server running on port ${PORT}`);
});
