import { useEffect, useState, type Dispatch, type SetStateAction, type MouseEvent } from "react";
import '../styles/ReadUsers.css';
import api from "../api";
import { type UserType } from "../types";
import { useAuth } from "../contexts/AuthContext";
import { useNavigate } from "react-router-dom";

function ReadUsers(){
    const [users, setUsers] : [users: Array<UserType>, setUsers: Dispatch<SetStateAction<Array<UserType>>>] = useState<Array<UserType>>([]);
    const [isLoading, setIsLoading] = useState(true); // NEW: loading state
    const {isAdmin} = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        const fetchUsers = async() => {
            try{
                setIsLoading(true);
                const res = await api.get('api/Admin/users');

                if (res.status == 200){
                    setUsers(res.data);
                } else {
                    console.log(res.status);
                }
            } catch(error){
                console.log(error);
            } finally {
                setIsLoading(false);
            }
        };

        if (!isAdmin){
            navigate('/Dashboard');
            return;
        } else {
            fetchUsers();
        }
    }, [isAdmin, navigate]);

    const handleDelete = async (
        e: MouseEvent<HTMLButtonElement>,
        userId: string
    ) => {
        e.preventDefault();
        e.stopPropagation(); // Prevent event bubbling

        const confirmed = window.confirm(
            "Are you sure you want to delete this user? This action cannot be undone."
        );

        if (!confirmed) return;

        try {
            await api.delete(`api/Account/profile/${userId}`);
            setUsers(prev => prev.filter(user => user.userId !== userId));
        } catch (error) {
            console.log(error);
        }
    };

    // Get initials from username for avatar
    const getInitials = (username: string) => {
        return username.charAt(0).toUpperCase();
    };

    if (isLoading) {
        return (
            <main className="users-page">
                <h2>User Management</h2>
                <div className="users-grid">
                    {[1, 2, 3, 4].map((i) => (
                        <div key={i} className="user-card loading-skeleton">
                            <div className="user-info">
                                <div className="user-avatar">...</div>
                                <div className="user-details">
                                    <h3 className="user-name">Loading...</h3>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            </main>
        );
    }

    return (
        <main className="users-page">
            <h2>User Management</h2>
            <div className="users-grid">
                {users.map((user) => (
                    <div key={user.userId} className="user-card">
                        <div className="user-info">
                            <div className="user-avatar">
                                {getInitials(user.userName)}
                            </div>
                            <div className="user-details">
                                <h3 className="user-name">{user.userName}</h3>
                                <p className="user-email">{user.email}</p>
                            </div>
                        </div>
                        <div className="user-meta">
                            <button 
                                className="delete-user-btn"
                                onClick={(e) => handleDelete(e, user.userId)}
                            >
                                🗑️ Delete
                            </button>
                        </div>
                    </div>
                ))}
            </div>
        </main>
    );
}

export default ReadUsers;