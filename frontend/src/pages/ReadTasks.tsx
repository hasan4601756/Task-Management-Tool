import { useEffect, useState, type Dispatch, type SetStateAction } from "react";
import '../styles/ReadTasks.css';
import api from "../api";
import { useNavigate, NavLink } from "react-router-dom";
import { type TaskType } from '../types';

export function getStatusLabel(status: number) {
    switch (status) {
        case 1:
            return "Pending";
        case 2:
            return "Completed";
        case 3:
            return "In Progress";
        default:
            return "Unknown";
    }
}

export function getTaskPriority(priority: number) {
    switch (priority) {
        case 0:
            return "Low";
        case 1:
            return "Medium";
        case 2:
            return "High";
        case 3:
            return "Critical";
        default:
            return "Unknown";
    }
}

function ReadTasks(){
    const [tasks, setTasks] : [tasks : Array<TaskType>, setTasks : Dispatch<SetStateAction<Array<TaskType>>>] = useState<Array<TaskType>>([]);
    const [searchTerm, setSearchTerm] = useState(""); // NEW: search state
    const [isLoading, setIsLoading] = useState(true); // NEW: loading state
    const navigate = useNavigate();

    useEffect(() => {
        const fetchTasks = async() => {
            try{
                setIsLoading(true);

                // if (isAdmin)
                // const res = await api.get('api/Admin/tasks');

                const res = await api.get('api/Task/tasks');
                if (res.status == 200){
                    setTasks(res.data);
                } else {
                    console.log(res.status);
                }
            } catch(error){
                console.log(error);
            } finally {
                setIsLoading(false);
            }
        }

        fetchTasks();
    }, []);

    // NEW: filter tasks based on search term
    const filteredTasks = tasks.filter(task => 
        task.title.toLowerCase().includes(searchTerm.toLowerCase())
    );

    if (isLoading) {
        return (
            <main className="tasks-page">
                <div className="tasks-grid">
                    {[1, 2, 3, 4].map((i) => (
                        <div key={i} className="task-card loading-skeleton">
                            <div className="task-header">
                                <h3 className="task-title">Loading...</h3>
                            </div>
                        </div>
                    ))}
                </div>
            </main>
        );
    }

    return (
        <main className="tasks-page">
            <div className="tasks-header">
                <h2>Tasks</h2>
                <div className="search-bar">
                    <input
                        type="text"
                        placeholder="Search tasks by title or description..."
                        className="search-input"
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)} // NEW: update search term
                    />
                    <NavLink to="/tasks/add" className="add-task-btn">
                        + New Task
                    </NavLink>
                </div>
            </div>

            {filteredTasks.length === 0 ? (
                <div className="empty-state">
                    {searchTerm ? "No tasks match your search" : "No tasks yet. Create your first task!"}
                </div>
            ) : (
                <div className="tasks-grid">
                    {filteredTasks.map((task) => (
                        <div 
                            onClick={() => navigate(`/tasks/task/${task.id}`)} 
                            className="task-card" 
                            key={task.id}
                        >
                            <div className="task-header">
                                <h3 className="task-title">{task.title}</h3>
                                <span className={`task-badge priority-${getTaskPriority(task.priority).toLowerCase()}`}>
                                    {getTaskPriority(task.priority)}
                                </span>
                                <span className={`task-badge status-${getStatusLabel(task.taskStatus).toLowerCase().replace(' ', '')}`}>
                                    {getStatusLabel(task.taskStatus)}
                                </span>
                                {task.userName && (
                                    <span className="task-user">
                                        👤 {task.userName}
                                    </span>
                                )}
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </main>
    );
}

export default ReadTasks;