import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate  } from 'react-router-dom';

const SignupPage = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const history = useNavigate ();

  const handleSignup = async (e) => {
    e.preventDefault();
    try {
      await axios.post('https://refactored-disco-r79rx4x5gwrfxvjr-8080.app.github.dev/api/auth/register', {
        email,
        password,
      });
      history.push('/login');
    } catch (error) {
      console.error('Signup failed', error);
    }
  };

  return (
    <div>
      <h2>Signup</h2>
      <form onSubmit={handleSignup}>
        <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
        <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Password" />
        <button type="submit">Sign Up</button>
      </form>
    </div>
  );
};

export default SignupPage;
