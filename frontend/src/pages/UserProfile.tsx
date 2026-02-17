import { useEffect, useState, type SyntheticEvent } from "react";
import '../styles/UserProfile.css';
import api from "../api";
import { type ProfileType } from "../types";

function UserProfile() {
  const [profile, setProfile] = useState<ProfileType>({
    userName: "",
    email: "",
    fullName: "",
    phoneNumber: ""
  });
  const [isLoading, setIsLoading] = useState(true); // NEW: loading state
  const [isSaving, setIsSaving] = useState(false); // NEW: save state
  const [message, setMessage] = useState<{type: 'success' | 'error', text: string} | null>(null); // NEW: feedback message

  useEffect(() => {
    const fetchUserProfile = async() => {
      try{
        setIsLoading(true);
        const res = await api.get("api/Account/profile");

        if (res.status == 200){
          setProfile(res.data);
        } else {
          console.log(res.status);
        }
      }catch(error){
        console.log(error);
      } finally {
        setIsLoading(false);
      }
    }

    fetchUserProfile();
  }, []);

  const handleSubmit = async(e : SyntheticEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsSaving(true);
    setMessage(null);

    try{
      const res = await api.put("api/Account/profile/update", profile);

      if (res.status == 200){
        setMessage({type: 'success', text: 'Profile updated successfully!'});
        console.log("Success");
      } else {
        setMessage({type: 'error', text: 'Failed to update profile'});
        console.log("Failure");
      }
    } catch(error){
      console.log(error);
      setMessage({type: 'error', text: 'An error occurred while updating'});
    } finally {
      setIsSaving(false);
    }
  }

  const getInitials = () => {
    if (profile.fullName) {
      return profile.fullName.split(' ').map(n => n[0]).join('').toUpperCase().substring(0, 2);
    }
    return profile.userName.substring(0, 2).toUpperCase();
  };

  if (isLoading) {
    return (
      <main className="profile-page">
        <div className="profile-card loading-skeleton">
          <div className="profile-header">
            <div className="profile-avatar-large">...</div>
            <div className="profile-title">
              <h2>Loading profile...</h2>
            </div>
          </div>
        </div>
      </main>
    );
  }

  return (
    <main className="profile-page">
      <div className="profile-card">
        <div className="profile-header">
          <div className="profile-avatar-large">
            {getInitials()}
          </div>
          <div className="profile-title">
            <h2>{profile.fullName || profile.userName}</h2>
            <p>{profile.email}</p>
          </div>
        </div>

        <form className="profile-form" onSubmit={handleSubmit}>
          <div className="profile-field">
            <label htmlFor="username">Username</label>
            <input
              id="username"
              type="text"
              value={profile.userName}
              onChange={(e) =>
                setProfile({
                  ...profile,
                  userName: e.target.value
                })
              }
              disabled={isSaving}
              required
            />
          </div>

          <div className="profile-field">
            <label htmlFor="email">Email</label>
            <input
              id="email"
              type="email"
              value={profile.email}
              onChange={(e) =>
                setProfile({
                  ...profile,
                  email: e.target.value
                })
              }
              disabled={isSaving}
              required
            />
          </div>

          <div className="profile-field">
            <label htmlFor="fullName">Full Name</label>
            <input
              id="fullName"
              type="text"
              value={profile.fullName}
              onChange={(e) =>
                setProfile({
                  ...profile,
                  fullName: e.target.value
                })
              }
              disabled={isSaving}
            />
          </div>

          <div className="profile-field">
            <label htmlFor="phoneNumber">Phone Number</label>
            <input
              id="phoneNumber"
              type="tel"
              value={profile.phoneNumber}
              onChange={(e) =>
                setProfile({
                  ...profile,
                  phoneNumber: e.target.value
                })
              }
              disabled={isSaving}
              placeholder="Optional"
            />
          </div>

          {message && (
            <div className={`message ${message.type === 'success' ? 'success-message' : 'error-message'}`}>
              {message.text}
            </div>
          )}

          <button 
            type="submit" 
            className="profile-save-btn"
            disabled={isSaving}
          >
            {isSaving ? "Saving..." : "Save Changes"}
          </button>
        </form>
      </div>
    </main>
  );
}

export default UserProfile;