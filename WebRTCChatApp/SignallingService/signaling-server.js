// signaling-server.js
const express = require('express');
const http = require('http');
const socketIo = require('socket.io');

const app = express();
const server = http.createServer(app);
const io = socketIo(server);

io.on('connection', (socket) => {
    console.log('A user connected:', socket.id);

    socket.on('signal', (data) => {
        // Forward the signal to the target peer
        socket.to(data.target).emit('signal', {
            sender: socket.id,
            message: data.message,
        });
    });

    socket.on('disconnect', () => {
        console.log('User disconnected:', socket.id);
    });
});

const PORT = 5000; // Port number
server.listen(PORT, () => {
    console.log(`Signaling server running on port ${PORT}`);
});
