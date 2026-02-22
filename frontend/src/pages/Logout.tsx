import { useEffect } from "react";
import api from "../api";
import { useNavigate } from "react-router-dom";
import { REFRESH_TOKEN } from "../constants";
import { useAuth } from "../contexts/AuthContext";

const Logout = () => {
    const navigate = useNavigate();
    const {logout} = useAuth();

    useEffect(() => {
        const handleLogout = async () => {
            try {
                if (localStorage.getItem(REFRESH_TOKEN) != null){
                    await api.post("api/Account/logout");
                }
            } catch (error) {
                console.error("Logout failed:", error);
            } finally {
                logout();
                navigate("/login", { replace: true });
            }
        };

        handleLogout();
    }, [navigate]);

    return null;
};

export default Logout;

