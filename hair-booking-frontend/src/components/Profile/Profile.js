import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './Profile.css';

function Profile() {
  const navigate = useNavigate();
  const [user, setUser] = useState({
    name: 'John Doe',
    email: 'john.doe@example.com',
    avatar: 'https://via.placeholder.com/150',
    bio: 'This is a sample bio about the user.',
  });
  const [isEditing, setIsEditing] = useState(false);
  const [editedUser, setEditedUser] = useState({ ...user });

  useEffect(() => {
    // Giả lập lấy thông tin người dùng từ API
    // Trong thực tế, bạn sẽ gọi API ở đây
    const token = localStorage.getItem('token');
    if (!token) {
      navigate('/login');
    }
  }, [navigate]);

  const handleEdit = () => {
    setIsEditing(true);
  };

  const handleSave = () => {
    setUser({ ...editedUser });
    setIsEditing(false);
    // Gửi dữ liệu lên server ở đây (fetch API)
    console.log('Saved:', editedUser);
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setEditedUser((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleAvatarChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        setEditedUser((prev) => ({
          ...prev,
          avatar: reader.result,
        }));
      };
      reader.readAsDataURL(file);
    }
  };

  return (
    <div className="profile-container">
      <h1>Profile</h1>
      <div className="profile-content">
        <div className="profile-avatar">
          <img src={isEditing ? editedUser.avatar : user.avatar} alt="Avatar" />
          {isEditing && (
            <input
              type="file"
              accept="image/*"
              onChange={handleAvatarChange}
              className="avatar-upload"
            />
          )}
        </div>
        <div className="profile-details">
          {isEditing ? (
            <>
              <div className="form-group">
                <label>Name:</label>
                <input
                  type="text"
                  name="name"
                  value={editedUser.name}
                  onChange={handleChange}
                />
              </div>
              <div className="form-group">
                <label>Email:</label>
                <input
                  type="email"
                  name="email"
                  value={editedUser.email}
                  onChange={handleChange}
                />
              </div>
              <div className="form-group">
                <label>Bio:</label>
                <textarea
                  name="bio"
                  value={editedUser.bio}
                  onChange={handleChange}
                />
              </div>
              <button onClick={handleSave} className="save-btn">
                Save
              </button>
            </>
          ) : (
            <>
              <p><strong>Name:</strong> {user.name}</p>
              <p><strong>Email:</strong> {user.email}</p>
              <p><strong>Bio:</strong> {user.bio}</p>
              <button onClick={handleEdit} className="edit-btn">
                Edit Profile
              </button>
            </>
          )}
        </div>
      </div>
    </div>
  );
}

export default Profile;