import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import SignupPage from './pages/SignupPage';
import ChatRoomsPage from './pages/ChatRoomsPage'; // Assuming you have this page

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/signup" element={<SignupPage />} />
        <Route path="/chatrooms" element={<ChatRoomsPage />} />
      </Routes>
    </Router>
  );
}

export default App;
