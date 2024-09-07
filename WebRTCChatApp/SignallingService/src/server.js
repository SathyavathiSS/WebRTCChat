// server.js
const express = require('express');
const io = require('socket.io')(express());

const app = express();
const port = 3000;

io.on('connection', (socket) => {
  console.log('New client connected');

  socket.on('join', (room) => {
    socket.join(room);
    console.log(`Client joined room ${room}`);
  });

  socket.on('offer', (offer, room) => {
    io.to(room).emit('offer', offer);
  });

  socket.on('answer', (answer, room) => {
    io.to(room).emit('answer', answer);
  });

  socket.on('candidate', (candidate, room) => {
    io.to(room).emit('candidate', candidate);
  });

  socket.on('disconnect', () => {
    console.log('Client disconnected');
  });
});

app.use(express.static('public'));

app.listen(port, () => {
  console.log(`Server running on port ${port}`);
});
