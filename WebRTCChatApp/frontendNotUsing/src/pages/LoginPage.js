import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate  } from 'react-router-dom';

const LoginPage = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const history = useNavigate ();

  const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post('https://refactored-disco-r79rx4x5gwrfxvjr-8080.app.github.dev/api/auth/login', {
        email,
        password,
      });
      localStorage.setItem('token', response.data.token);
      history.push('/chatrooms');
    } catch (error) {
      console.error('Login failed', error);
    }
  };

  return (
    <div>
      <h2>Login</h2>
      <form onSubmit={handleLogin}>
        <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
        <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Password" />
        <button type="submit">Login</button>
      </form>
    </div>
  );
};

export default LoginPage;
