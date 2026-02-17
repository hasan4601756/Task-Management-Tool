import React, {
  useState,
  useContext,
  useEffect,
  type ReactNode,
} from "react";
import { jwtDecode } from "jwt-decode";
import api from "../api";
import { ACCESS_TOKEN, REFRESH_TOKEN } from "../constants";

type JwtPayload = {
  exp?: number;
};

type AuthContextType = {
  isAuthenticated: boolean;
  isAdmin: boolean | null;
  isLoading: boolean;
  login: (access: string, refresh: string, isAdmin?: boolean) => void;
  logout: () => void;
};

const AuthContext = React.createContext<AuthContextType | undefined>(
  undefined
);

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return context;
}

type AuthProviderProps = {
  children: ReactNode;
};

export function AuthProvider({ children }: AuthProviderProps) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isAdmin, setIsAdmin] = useState<boolean | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const logout = () => {
    localStorage.removeItem(ACCESS_TOKEN);
    localStorage.removeItem(REFRESH_TOKEN);
    setIsAuthenticated(false);
    setIsAdmin(null);
  };

  const login = (
    access: string,
    refresh: string,
    adminStatus?: boolean
  ) => {
    localStorage.setItem(ACCESS_TOKEN, access);
    localStorage.setItem(REFRESH_TOKEN, refresh);
    setIsAuthenticated(true);
    if (adminStatus !== undefined) {
      setIsAdmin(adminStatus);
    }
  };

  const refreshToken = async () => {
    const refresh = localStorage.getItem(REFRESH_TOKEN);
    if (!refresh) {
      logout();
      return;
    }

    try {
      const res = await api.post("/api/token/refresh/", {
        refresh,
      });

      if (res.status === 200) {
        localStorage.setItem(ACCESS_TOKEN, res.data.token);
        localStorage.setItem(REFRESH_TOKEN, res.data.refreshToken);
        setIsAuthenticated(true);
      } else {
        logout();
      }
    } catch (error) {
      console.error("Refresh failed", error);
      logout();
    }
  };

  const authenticate = async () => {
    const token = localStorage.getItem(ACCESS_TOKEN);

    if (!token) {
      logout();
      setIsLoading(false);
      return;
    }

    try {
      const decoded = jwtDecode<JwtPayload>(token);
      const now = Date.now() / 1000;

      if (!decoded.exp || decoded.exp <= now) {
        await refreshToken();
      } else {
        setIsAuthenticated(true);
      }
    } catch (error) {
      console.error("Invalid token", error);
      logout();
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    authenticate();
  }, []);

  const value: AuthContextType = {
    isAuthenticated,
    isAdmin,
    isLoading,
    login,
    logout,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}