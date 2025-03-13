import React, { useState } from 'react';
import { useNavigate, NavLink } from 'react-router-dom';
import './Layout.css';

function Layout({ children, isAuthenticated, setIsAuthenticated }) {
  const navigate = useNavigate();
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  const handleLogout = () => {
    localStorage.removeItem('token');
    setIsAuthenticated(false);
    navigate('/login');
  };

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
  };

  return (
    <div className="layout-container">
      <header className="header">
        <div className="header-content">
          <NavLink to="/" className="logo">HAIR</NavLink>
          
          <button className="menu-toggle" onClick={toggleMenu}>
            ☰
          </button>

          <nav className={`nav-menu ${isMenuOpen ? 'open' : ''}`}>
            <div className="nav-main">
              <div className="dropdown">
                <NavLink 
                  to="/home" 
                  className={({ isActive }) => isActive ? 'nav-item active' : 'nav-item'}
                  onClick={() => setIsMenuOpen(false)}
                >
                  Home <span className="dropdown-arrow">▼</span>
                </NavLink>
                <div className="dropdown-content">
                  <NavLink to="/home/features">Features</NavLink>
                  <NavLink to="/home/benefits">Benefits</NavLink>
                </div>
              </div>
              <div className="dropdown">
                <NavLink 
                  to="/profile" 
                  className={({ isActive }) => isActive ? 'nav-item active' : 'nav-item'}
                  onClick={() => setIsMenuOpen(false)}
                >
                  Profile <span className="dropdown-arrow">▼</span>
                </NavLink>
                <div className="dropdown-content">
                  <NavLink to="/profile/edit">Edit Profile</NavLink>
                  <NavLink to="/profile/view">View Profile</NavLink>
                </div>
              </div>
              <div className="dropdown">
                <NavLink 
                  to="/settings" 
                  className={({ isActive }) => isActive ? 'nav-item active' : 'nav-item'}
                  onClick={() => setIsMenuOpen(false)}
                >
                  Settings <span className="dropdown-arrow">▼</span>
                </NavLink>
                <div className="dropdown-content">
                  <NavLink to="/settings/account">Account</NavLink>
                  <NavLink to="/settings/privacy">Privacy</NavLink>
                </div>
              </div>
              <NavLink 
                to="/store" 
                className={({ isActive }) => isActive ? 'nav-item active' : 'nav-item'}
                onClick={() => setIsMenuOpen(false)}
              >
                Store
              </NavLink>
            </div>

            <div className="nav-secondary">
              {isAuthenticated ? (
                <>
                  <NavLink 
                    to="/search" 
                    className="nav-secondary-item"
                    onClick={() => setIsMenuOpen(false)}
                  >
                    <span className="search-icon">🔍</span>
                  </NavLink>
                  <NavLink 
                    to="/contact" 
                    className="nav-secondary-item"
                    onClick={() => setIsMenuOpen(false)}
                  >
                    Contact
                  </NavLink>
                  <button onClick={handleLogout} className="cta-btn">
                    Logout
                  </button>
                </>
              ) : (
                <>
                  <NavLink 
                    to="/search" 
                    className="nav-secondary-item"
                    onClick={() => setIsMenuOpen(false)}
                  >
                    <span className="search-icon">🔍</span>
                  </NavLink>
                  <NavLink 
                    to="/contact" 
                    className="nav-secondary-item"
                    onClick={() => setIsMenuOpen(false)}
                  >
                    Book a meeting
                  </NavLink>
                  <NavLink to="/try" className="cta-btn" onClick={() => setIsMenuOpen(false)}>
                    Try HAIR
                  </NavLink>
                </>
              )}
            </div>
          </nav>
        </div>
      </header>
      
      <main className="body">
        {children}
      </main>
      
      <footer className="footer">
        <p>© 2025 HAIR. All rights reserved.</p>
      </footer>
    </div>
  );
}

export default Layout;