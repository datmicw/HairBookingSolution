import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './components/Login/Login';
import Register from './components/Register/Register';
import Home from './components/Home/Home';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  return (
    <Router>
      <Routes>
        <Route 
          path="/login" 
          element={<Login setIsAuthenticated={setIsAuthenticated} />} 
        />
        <Route 
          path="/register" 
          element={<Register />} 
        />
        <Route 
          path="/home" 
          element={isAuthenticated ? <Home /> : <Navigate to="/login" />}
        />
        <Route 
          path="/" 
          element={<Navigate to="/login" />}
        />
      </Routes>
    </Router>
  );
}

export default App;