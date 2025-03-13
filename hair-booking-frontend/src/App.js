import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout/Layout';
import Login from './components/Login/Login';
import Register from './components/Register/Register';
import Home from './components/Home/Home';
import Profile from './components/Profile/Profile';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      setIsAuthenticated(true);
    }
  }, []);

  const Placeholder = ({ title }) => (
    <div className="home-content">
      <h1>{title}</h1>
      <p>This is a placeholder page.</p>
    </div>
  );

  return (
    <Router>
      <Layout isAuthenticated={isAuthenticated} setIsAuthenticated={setIsAuthenticated}>
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
            path="/home/features" 
            element={isAuthenticated ? <Placeholder title="Features" /> : <Navigate to="/login" />}
          />
          <Route 
            path="/home/benefits" 
            element={isAuthenticated ? <Placeholder title="Benefits" /> : <Navigate to="/login" />}
          />
          <Route 
            path="/profile" 
            element={isAuthenticated ? <Profile /> : <Navigate to="/login" />}
          />
          <Route 
            path="/profile/edit" 
            element={isAuthenticated ? <Placeholder title="Edit Profile" /> : <Navigate to="/login" />}
          />
          <Route 
            path="/profile/view" 
            element={isAuthenticated ? <Placeholder title="View Profile" /> : <Navigate to="/login" />}
          />
          <Route 
            path="/settings" 
            element={isAuthenticated ? <Placeholder title="Settings" /> : <Navigate to="/login" />}
          />
          <Route 
            path="/settings/account" 
            element={isAuthenticated ? <Placeholder title="Account Settings" /> : <Navigate to="/login" />}
          />
          <Route 
            path="/settings/privacy" 
            element={isAuthenticated ? <Placeholder title="Privacy Settings" /> : <Navigate to="/login" />}
          />
          <Route 
            path="/store" 
            element={isAuthenticated ? <Placeholder title="Store" /> : <Navigate to="/login" />}
          />
          <Route 
            path="/search" 
            element={isAuthenticated ? <Placeholder title="Search" /> : <Navigate to="/login" />}
          />
          <Route 
            path="/contact" 
            element={<Placeholder title="Contact" />}
          />
          <Route 
            path="/try" 
            element={<Placeholder title="Try My App" />}
          />
          <Route 
            path="/" 
            element={<Navigate to="/login" />}
          />
        </Routes>
      </Layout>
    </Router>
  );
}

export default App;