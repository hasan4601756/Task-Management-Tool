import { useEffect, useState } from "react";
import '../styles/Dashboard.css';
import api from "../api";
import { type DashboardType } from "../types";

const initialState : DashboardType = {
    completedTasks: 0,
    inProgressTasks: 0,
    pendingTasks: 0,
};

function Dashboard(){
    const [state, setState] = useState<DashboardType>(initialState);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchDashboard = async () => {
            try {
                setIsLoading(true);
                const res = await api.get("api/Task/dashboard");

                if (res.status === 200) {
                    setState(res.data);
                } else {
                    setError("Failed to fetch dashboard");
                }
            } catch (err) {
                setError("Something went wrong");
            } finally {
                setIsLoading(false);
            }
        };

        fetchDashboard();
    }, []);

    if (isLoading) {
        return (
            <main className="dashboard-page">
                <div className="dashboard-grid">
                    {[1, 2, 3].map((i) => (
                        <div key={i} className="dashboard-card loading-skeleton">
                            <div className="card-header">Loading...</div>
                            <div className="card-value">---</div>
                        </div>
                    ))}
                </div>
            </main>
        );
    }

    if (error) {
        return (
            <main className="dashboard-page">
                <div className="error-message">{error}</div>
            </main>
        );
    }

    return (
        <main className="dashboard-page">
            <div className="dashboard-grid">
                <div className="dashboard-card">
                    <div className="card-header" data-status="completed">
                        <span>Completed</span>
                        <span>✓</span>
                    </div>
                    <div className="card-value">{state.completedTasks}</div>
                    <div className="card-footer">Tasks successfully completed</div>
                </div>

                <div className="dashboard-card">
                    <div className="card-header" data-status="pending">
                        <span>Pending</span>
                        <span>⏳</span>
                    </div>
                    <div className="card-value">{state.pendingTasks}</div>
                    <div className="card-footer">Tasks awaiting action</div>
                </div>

                <div className="dashboard-card">
                    <div className="card-header" data-status="in-progress">
                        <span>In Progress</span>
                        <span>🔄</span>
                    </div>
                    <div className="card-value">{state.inProgressTasks}</div>
                    <div className="card-footer">Tasks currently being worked on</div>
                </div>
            </div>
        </main>
    );
}

export default Dashboard;