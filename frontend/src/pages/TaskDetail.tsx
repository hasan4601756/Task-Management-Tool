import { useEffect, useState, type Dispatch, type SetStateAction, type SyntheticEvent, type MouseEvent } from "react";
import '../styles/TaskDetail.css';
import api from "../api";
import { useNavigate, useParams } from "react-router-dom";
import axios from "axios";
import { taskPriority } from "./CreateTask";
import type { taskRouteParamsType, TaskUpdateType, TaskDetailType, CategoryType, UserType } from "../types";
import { useAuth } from "../contexts/AuthContext";

const taskStatus : Record<string, number> = {
    "Unknown": 0,
    "Pending": 1,
    "Completed": 2,
    "In Progress": 3
};

function TaskDetail(){
    const { taskId } = useParams<taskRouteParamsType>();
    const navigate = useNavigate();
    const { isAdmin } = useAuth();
    const [users, setUsers] : [users: Array<UserType>, setUsers: Dispatch<SetStateAction<Array<UserType>>>] = useState<Array<UserType>>([]);
    const [taskUser, setTaskUser] = useState<UserType>();
    const [selectedUserId, setSelectedUserId] = useState<string>();
    const [isLoading, setIsLoading] = useState(true); // NEW: loading state
    const [isSubmitting, setIsSubmitting] = useState(false); // NEW: form submission state
    
    const [taskDetail, setTaskDetail] : [taskDetail : TaskDetailType, setTaskDetail : Dispatch<SetStateAction<TaskDetailType>>] = useState<TaskDetailType>({
        id : -1,
        title : "",
        description : "",
        dueDate : "",
        creationDate : "",
        taskStatus : 0,
        categoryId: 0,
        categoryName : "",
        categoryDescription : "",
        priority: 0
    });

    const [categories, setCategories] : [categories: Array<CategoryType>, setCategories: Dispatch<SetStateAction<Array<CategoryType>>>] = useState<Array<CategoryType>>([]);

    useEffect(() => {
        if (!taskId) return;
        fetchTask();
        fetchCategories();

        if (isAdmin){
            fetchUserByTaskId(taskId);
            fetchUsers();
        } 
    }, [taskId, isAdmin]);

    useEffect(() => {
        if (taskUser) {
            setSelectedUserId(taskUser.userId);
        }
    }, [taskUser]);

    const fetchTask = async() => {
        try{
            setIsLoading(true);
            const numericTaskId = Number(taskId);
            if (isNaN(numericTaskId)) return;

            const res = await api.get(`api/Task/tasks/${taskId}`);

            if (res.status == 200){
                setTaskDetail({
                    ...res.data,
                    dueDate: res.data.dueDate.split("T")[0]
                });
                console.log(res.data);
            } else{
                console.log(res.status);
            }
        }catch(error){
            console.log(error);
        } finally {
            setIsLoading(false);
        }
    }

    const fetchCategories = async() => {
        try {
            const res = await api.get("/api/TaskCategory/categories");

            if (res.status == 200){
                setCategories(res.data);
                console.log(res.data);
            } else{
                console.log("Could not fetch the categories");
            }
        } catch(error){
            console.log(error);
        }
    }

    const fetchUserByTaskId = async(taskId:string) => {
        const numericTaskId = Number(taskId);
        if (isNaN(numericTaskId)) return;
        try{
            const res = await api.get(`api/Admin/${taskId}/user`);

            if (res.status == 200){
                setTaskUser(res.data);
            } else {
                console.log(res.status);
            }
        } catch(error){
            console.log(error);
        } 
    }

    const fetchUsers = async() => {
        try{
            const res = await api.get('api/Admin/users');

            if (res.status == 200){
                setUsers(res.data);
            } else {
                console.log(res.status);
            }
        } catch(error){
            console.log(error);
        } 
    };

    async function handleAssignation(e: SyntheticEvent<HTMLFormElement>) {
        e.preventDefault();
        setIsSubmitting(true);

        if (selectedUserId == taskUser?.userId) {
            setIsSubmitting(false);
            return;
        }
        
        try{
            console.log(`api/Admin/assigntask/${taskId}/${selectedUserId}`);
            const res = await api.put(`api/Admin/assigntask/${taskId}/${selectedUserId}`);

            if (res.status == 200) {
                navigate("/tasks");
                console.log("Success");
            } else{
                console.log("Failure");
            }
        } catch (error: any) { 
            if (axios.isAxiosError(error)) { 
                console.log("Axios error:", error.response?.status, error.response?.data); 
            } else { 
                console.log("Unexpected error:", error); 
            }
        } finally {
            setIsSubmitting(false);
        }
    }

    async function handleDelete(e: MouseEvent<HTMLButtonElement>) {
        e.preventDefault();

        if (!taskId) return;

        const confirmed = window.confirm(
            "Are you sure you want to delete this task? This action cannot be undone."
        );

        if (!confirmed) return;

        setIsSubmitting(true);

        try{
            console.log(`api/Task/delete/${taskId}`);
            const res = await api.delete(`api/Task/delete/${taskId}`);

            if (res.status == 200) {
                navigate("/tasks");
                console.log("Success");
            } else{
                console.log("Failure");
            }
        } catch (error: any) { 
            if (axios.isAxiosError(error)) { 
                console.log("Axios error:", error.response?.status, error.response?.data); 
            } else { 
                console.log("Unexpected error:", error); 
            }
        } finally {
            setIsSubmitting(false);
        }
    }

    async function handleSubmit(e: SyntheticEvent<HTMLFormElement>) {
        e.preventDefault();
        setIsSubmitting(true);

        const updatedTask : TaskUpdateType = {
            title: taskDetail.title,
            description: taskDetail.description,
            dueDate: new Date(taskDetail.dueDate).toISOString().split("T")[0],
            categoryId: taskDetail.categoryId,
            status: taskDetail.taskStatus,
            id: taskDetail.id,
            priority: taskDetail.priority
        };

        try{
            console.log(`api/Task/update/${taskDetail.id}`);
            const res = await api.put(`api/Task/update/${taskDetail.id}`, updatedTask);

            if (res.status == 200) {
                navigate("/tasks");
                console.log("Success");
            } else{
                console.log("Failure");
            }
        } catch (error: any) { 
            if (axios.isAxiosError(error)) { 
                console.log("Axios error:", error.response?.status, error.response?.data); 
            } else { 
                console.log("Unexpected error:", error); 
            }
        } finally {
            setIsSubmitting(false);
        }
    }

    if (isLoading) {
        return (
            <main className="task-detail-page">
                <div className="task-detail-card loading-skeleton">
                    <div className="task-detail-header">
                        <h2>Loading task details...</h2>
                    </div>
                </div>
            </main>
        );
    }

    return (
        <main className="task-detail-page">
            <div className="task-detail-card">
                <div className="task-detail-header">
                    <h2>Task Details</h2>
                    <div className="badge-group">
                        <span className={`task-badge priority-${getTaskPriority(taskDetail.priority).toLowerCase()}`}>
                            {getTaskPriority(taskDetail.priority)}
                        </span>
                        <span className={`task-badge status-${getStatusLabel(taskDetail.taskStatus).toLowerCase().replace(' ', '')}`}>
                            {getStatusLabel(taskDetail.taskStatus)}
                        </span>
                    </div>
                </div>

                <form className="task-detail-form" onSubmit={handleSubmit}>
                    <div className="form-section">
                        <h3 className="form-section-title">📋 Basic Information</h3>
                        <div className="form-row">
                            <div className="form-group">
                                <label htmlFor="title">Title</label>
                                <input
                                    id="title"
                                    type="text"
                                    placeholder="Task title"
                                    value={taskDetail.title}
                                    onChange={(e) =>
                                        setTaskDetail({...taskDetail, title: e.target.value})
                                    }
                                    required
                                    disabled={isSubmitting}
                                />
                            </div>

                            <div className="form-group">
                                <label htmlFor="priority">Priority</label>
                                <select
                                    id="priority"
                                    value={taskDetail.priority}
                                    onChange={(e) =>
                                        setTaskDetail({...taskDetail, priority: Number(e.target.value)})
                                    }
                                    required
                                    disabled={isSubmitting}
                                >
                                    <option value="" disabled>Select priority</option>
                                    {Object.entries(taskPriority).map(([key, value]) => (
                                        <option key={key} value={value}>{key}</option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        <div className="form-group">
                            <label htmlFor="description">Description</label>
                            <textarea
                                id="description"
                                placeholder="Task description"
                                value={taskDetail.description}
                                onChange={(e) =>
                                    setTaskDetail({...taskDetail, description: e.target.value})
                                }
                                disabled={isSubmitting}
                                rows={3}
                            />
                        </div>
                    </div>

                    <div className="form-section">
                        <h3 className="form-section-title">⚙️ Status & Dates</h3>
                        <div className="form-row">
                            <div className="form-group">
                                <label htmlFor="status">Status</label>
                                <select
                                    id="status"
                                    value={taskDetail.taskStatus}
                                    onChange={(e) =>
                                        setTaskDetail({...taskDetail, taskStatus: Number(e.target.value)})
                                    }
                                    required
                                    disabled={isSubmitting}
                                >
                                    <option value="" disabled>Select status</option>
                                    {Object.entries(taskStatus).map(([key, value]) => (
                                        <option key={key} value={value}>{key}</option>
                                    ))}
                                </select>
                            </div>

                            <div className="form-group">
                                <label htmlFor="dueDate">Due Date</label>
                                <input
                                    id="dueDate"
                                    type="date"
                                    value={taskDetail.dueDate}
                                    onChange={(e) =>
                                        setTaskDetail({...taskDetail, dueDate: e.target.value})
                                    }
                                    required
                                    disabled={isSubmitting}
                                />
                            </div>
                        </div>

                        <div className="form-row">
                            <div className="form-group">
                                <label htmlFor="category">Category</label>
                                <select
                                    id="category"
                                    value={taskDetail.categoryId}
                                    onChange={(e) =>
                                        setTaskDetail({...taskDetail, categoryId: Number(e.target.value)})
                                    }
                                    required
                                    disabled={isSubmitting}
                                >
                                    <option value="" disabled>Select a category</option>
                                    {categories.map((category) => (
                                        <option key={category.taskCategoryId} value={category.taskCategoryId}>
                                            {category.name}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div className="form-group">
                                <label>Created On</label>
                                <input
                                    type="text"
                                    value={new Date(taskDetail.creationDate).toLocaleDateString()}
                                    disabled
                                    readOnly
                                />
                            </div>
                        </div>
                    </div>

                    <div className="action-buttons">
                        <button 
                            type="submit" 
                            className="update-btn"
                            disabled={isSubmitting}
                        >
                            {isSubmitting ? "Updating..." : "Update Task"}
                        </button>
                        <button 
                            className="delete-btn"
                            onClick={handleDelete}
                            disabled={isSubmitting}
                        >
                            {isSubmitting ? "Deleting..." : "Delete Task"}
                        </button>
                    </div>
                </form>

                {isAdmin && (
                    <div className="assign-section">
                        <form className="assign-form" onSubmit={handleAssignation}>
                            <div className="form-group">
                                <label htmlFor="userSelect">Assign to User</label>
                                <select 
                                    id="userSelect"
                                    className="user-select" 
                                    value={selectedUserId} 
                                    onChange={(e) => setSelectedUserId(e.target.value)}
                                    required
                                    disabled={isSubmitting}
                                >
                                    <option value="" disabled>Select user</option>
                                    {users.map(user => (
                                        <option key={user.userId} value={user.userId}>
                                            {user.userName} ({user.email})
                                        </option>
                                    ))}
                                </select>
                            </div>
                            <button 
                                type="submit" 
                                className="assign-btn"
                                disabled={isSubmitting}
                            >
                                {isSubmitting ? "Assigning..." : "Assign User"}
                            </button>
                        </form>
                    </div>
                )}
            </div>
        </main>
    );
}

// Helper functions (needed for TaskDetail)
function getStatusLabel(status: number) {
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

function getTaskPriority(priority: number) {
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

export default TaskDetail;