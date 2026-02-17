import { Link } from "react-router-dom";
import '../styles/NotFound.css';

function NotFound(){
    return (
        <main className="not-found-page">
            <div className="not-found-content">
                <h1>404</h1>
                <h2>Page Not Found</h2>
                <p>The page you're looking for doesn't exist or has been moved.</p>
                <Link to="/dashboard" className="back-home-btn">
                    Back to Dashboard
                </Link>
            </div>
        </main>
    );
}

export default NotFound;