import React, { useEffect, useState } from 'react';
import axios from 'axios';

import ChatComponent from '../components/ChatComponent';
<ChatComponent />

const ChatRoomsPage = () => {
  const [rooms, setRooms] = useState([]);

  useEffect(() => {
    const fetchRooms = async () => {
      try {
        const response = await axios.get('https://refactored-disco-r79rx4x5gwrfxvjr-8082.app.github.dev/api/chat/get-rooms', {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`,
          },
        });
        setRooms(response.data);
      } catch (error) {
        console.error('Failed to fetch chat rooms', error);
      }
    };
    fetchRooms();
  }, []);

  return (
    <div>
      <h2>Chat Rooms</h2>
      <ul>
        {rooms.map((room) => (
          <li key={room.id}>{room.name}</li>
        ))}
      </ul>
    </div>
  );
};

export default ChatRoomsPage;
