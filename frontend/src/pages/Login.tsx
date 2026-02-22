import { useReducer, useState, type Dispatch, type SetStateAction, type SyntheticEvent } from "react";
import '../styles/Login.css';
import api from "../api";
import { useAuth } from "../contexts/AuthContext";
import { useNavigate, Link } from "react-router-dom";

type Login = {
    email: string;
    password: string;
    rememberMe: boolean;
};

function Login(){
    const { login } = useAuth();
    const navigate = useNavigate();

    const [state, dispatch] = useReducer((state, action) => {
        switch(action.type){
            case "SET_EMAIL":
                return {...state, email: action.payload};
            case "SET_PASSWORD":
                return {...state, password: action.payload};
            case "SET_REMEMBER_ME":
                return {...state, rememberMe: action.payload};
            default:
                return state;
        }
    }, {
        email: '',
        password: '',
        rememberMe: false,
    });
    const [isLoading, setIsLoading] : [isLoading : boolean , setIsLoading : Dispatch<SetStateAction<boolean>>] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    const handleSubmit = async (e: SyntheticEvent<HTMLFormElement>) => {
        e.preventDefault();
        setIsLoading(true);
        setError(null);

        const request_data: Login = {
            email: state.email,
            password: state.password,
            rememberMe: state.rememberMe
        };

        try {
            const res = await api.post('api/account/login', request_data);

            if (res.status === 200) {
                let isAdmin : boolean|undefined = undefined;

                const response = await api.get('api/account/roles', {
                    headers: {
                        Authorization: `Bearer ${res.data.token}`
                    }
                    });

                if (response.status == 200){
                    if (response.data.includes("Admin")) isAdmin = true;
                    else isAdmin = false;
                } else {
                    console.log("Error getting user role.");
                }

                login(res.data.token, res.data.refreshToken, isAdmin);
                dispatch({ type: "SET_EMAIL", payload: '' });
                dispatch({ type: "SET_PASSWORD", payload: '' });
                navigate("/dashboard", { replace: true });
            } else {
                console.log("Error logging in.");
            }
        } catch (error) {
            console.error(error);
            setError("Invalid email or password!");
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <main className="auth-container">
            <div className="auth-card">
                <div className="auth-header">
                    <h1>Welcome Back</h1>
                    <p>Sign in to your account</p>
                </div>

                <form
                    className="auth-form"
                    onSubmit={handleSubmit}
                >
                    <div className="form-group">
                        <label htmlFor="email">Email</label>
                        <input
                            id="email"
                            type="email"
                            placeholder="Enter your email"
                            value={state.email}
                            onChange={(e) => dispatch({type: "SET_EMAIL", payload: e.target.value})}
                            disabled={isLoading}
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="password">Password</label>
                        <input
                            id="password"
                            type="password"
                            placeholder="Enter your password"
                            value={state.password}
                            onChange={(e) => dispatch({type: "SET_PASSWORD", payload: e.target.value})}
                            disabled={isLoading}
                            required
                        />
                    </div>

                    <div className="form-group checkbox-group">
                        <label className="checkbox-label">
                            <input
                                type="checkbox"
                                checked={state.rememberMe}
                                onChange={(e) =>
                                    dispatch({
                                        type: "SET_REMEMBER_ME",
                                        payload: e.target.checked
                                    })
                                }
                                disabled={isLoading}
                            />
                            Remember me
                        </label>
                    </div>

                    {error && <p className="error-text">{error}</p>}

                    <button
                        className="auth-button"
                        type="submit"
                        disabled={isLoading}
                    >
                        {isLoading ? <span className="spinner"></span> : "Sign In"}
                    </button>
                </form>

                <div className="auth-footer">
                    Don't have an account?
                    <Link to="/register">Sign up</Link>
                </div>
            </div>
        </main>
    );
}


export default Login;
