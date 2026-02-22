import { useReducer, useState, type Dispatch, type SetStateAction, type SyntheticEvent } from "react";
import '../styles/Register.css';
import api from "../api";
import { useNavigate, Link } from "react-router-dom";

type Register = {
    email: string,
    userName: string,
    fullName: string,
    password: string,
    confirmPassword: string
};

type RegisterAction =
  | { type: "SET_USERNAME"; payload: string }
  | { type: "SET_EMAIL"; payload: string }
  | { type: "SET_PASSWORD"; payload: string }
  | { type: "SET_FULLNAME"; payload: string }
  | { type: "SET_CONFIRM_PASSWORD"; payload: string }
  | { type: "RESET"};

const initialState : Register = {
    email : "",
    userName: "",
    fullName: "",
    password: "",
    confirmPassword: "",
}

function Register(){
    const [state, dispatch] = useReducer((state : Register, action : RegisterAction) => {
        switch(action.type){
            case "SET_EMAIL":
                return {...state, email: action.payload};
            case "SET_USERNAME":
                return {...state, userName: action.payload};
            case "SET_FULLNAME":
                return {...state, fullName: action.payload};
            case "SET_PASSWORD":
                return {...state, password: action.payload};
            case "SET_CONFIRM_PASSWORD":
                return {...state, confirmPassword: action.payload};
            case "RESET":
                return initialState;
            default:
                return state; 
        }
    }, initialState);

    const [isLoading, setIsLoading] : [isLoading : boolean , setIsLoading : Dispatch<SetStateAction<boolean>>] = useState<boolean>(false);
    const [exception, setException] : [exception : string, setException : Dispatch<SetStateAction<string>>] = useState<string>('');
    const navigate = useNavigate();

    const handleSubmit = async (e: SyntheticEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (state.password !== state.confirmPassword) {
            setException('Passwords do not match');
            return;
        }

        setException('');
        setIsLoading(true);

        try {
            const res = await api.post('/api/Account/register', state);
            if (res.status === 200) {
                navigate('/users');
            }
        } catch (error) {
            setException('Registration failed. Please try again.');
        } finally {
            setIsLoading(false);
            dispatch({ type: "RESET" });
        }
    };

    return (
        <main className="auth-container">
            <div className="auth-card">
                <div className="auth-header">
                    <h1>Create Account</h1>
                    <p>Sign up to get started</p>
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
                        <label htmlFor="username">Username</label>
                        <input
                            id="username"
                            type="text"
                            placeholder="Choose a username"
                            value={state.userName}
                            onChange={(e) => dispatch({type: "SET_USERNAME", payload: e.target.value})}
                            disabled={isLoading}
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="fullname">Full Name</label>
                        <input
                            id="fullname"
                            type="text"
                            placeholder="Enter your full name"
                            value={state.fullName}
                            onChange={(e) => dispatch({type: "SET_FULLNAME", payload: e.target.value})}
                            disabled={isLoading}
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="password">Password</label>
                        <input
                            id="password"
                            type="password"
                            placeholder="Create a password"
                            value={state.password}
                            onChange={(e) => dispatch({type: "SET_PASSWORD", payload: e.target.value})}
                            disabled={isLoading}
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="confirm-password">Confirm Password</label>
                        <input
                            id="confirm-password"
                            type="password"
                            placeholder="Confirm your password"
                            value={state.confirmPassword}
                            onChange={(e) => dispatch({type: "SET_CONFIRM_PASSWORD", payload: e.target.value})}
                            disabled={isLoading}
                            required
                        />
                    </div>

                    {exception && <p className="error-text">{exception}</p>}

                    <button
                        className="auth-button"
                        type="submit"
                        disabled={isLoading}
                    >
                        {isLoading ? "Creating account..." : "Create Account"}
                    </button>
                </form>

                <div className="auth-footer">
                    Already have an account?
                    <Link to="/login">Sign in</Link>
                </div>
            </div>
        </main>
    );
}


export default Register;
