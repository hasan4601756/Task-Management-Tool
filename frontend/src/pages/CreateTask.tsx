import { useEffect, useReducer, useState, type Dispatch, type SetStateAction, type SyntheticEvent } from "react";
import '../styles/CreateTask.css';
import api from "../api";
import { useNavigate } from "react-router-dom";
import { type CreateTaskType, type CategoryType } from "../types";

export const taskPriority : Record<string, number> = {
    "Low": 0,
    "Medium": 1,
    "High": 2,
    "Critical": 3
};

type Action =
    | { type: "SET_TITLE"; payload: string }
    | { type: "SET_DESCRIPTION"; payload: string }
    | { type: "SET_DUEDATE"; payload: string }
    | { type: "SET_CATEGORY"; payload: number }
    | { type: "SET_PRIORITY"; payload: number }
    | { type: "RESET" };

const initialState: CreateTaskType = {
    title: "",
    description: "",
    due_date: "",
    categoryId: 0,
    priority: 0
};

function reducer(state: CreateTaskType, action: Action): CreateTaskType {
    switch (action.type) {
        case "SET_TITLE":
            return { ...state, title: action.payload };
        case "SET_DESCRIPTION":
            return { ...state, description: action.payload };
        case "SET_DUEDATE":
            return { ...state, due_date: action.payload };
        case "SET_CATEGORY":
            return { ...state, categoryId: action.payload };
        case "SET_PRIORITY":
            return { ...state, priority: action.payload };
        case "RESET":
            return initialState;
        default:
            return state;
    }
}

function CreateTask() {
    const [state, dispatch] = useReducer(reducer, initialState);
    const [categories, setCategories] : [categories: Array<CategoryType>, setCategories: Dispatch<SetStateAction<Array<CategoryType>>>] = useState<Array<CategoryType>>([]);
    const [isSubmitting, setIsSubmitting] = useState(false); // NEW: loading state for form submission

    const navigate = useNavigate();

    useEffect(() => {
        getCategories();
    }, []);

    const getCategories = async() => {
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

    async function handleSubmit(e: SyntheticEvent<HTMLFormElement>) {
        e.preventDefault();
        setIsSubmitting(true); // NEW: set loading state

        const newTask : CreateTaskType = {
            title: state.title,
            description: state.description,
            due_date: state.due_date ? new Date(state.due_date).toISOString() : "",
            categoryId: state.categoryId,
            priority: state.priority
        };
        if (newTask.categoryId == 0) {
            alert("Please select a valid category.");
            setIsSubmitting(false);
            return;
        }

        console.log(newTask);

        try{
            const res = await api.post("api/Task/add", newTask);

            if (res.status == 200) {
                dispatch({ type: "RESET" });
                navigate("/tasks");
            }
        } catch(error){
            console.log(error);
        } finally {
            setIsSubmitting(false); // NEW: reset loading state
        }
    }

    return (
        <main className="create-task-container">
            <div className="create-task-header">
                <h2>Create New Task</h2>
            </div>

            <form className="create-task-form" onSubmit={handleSubmit}>
                <div className="form-row">
                    <div className="form-group">
                        <label htmlFor="title">Title</label>
                        <input
                            id="title"
                            type="text"
                            placeholder="Enter task title"
                            value={state.title}
                            onChange={(e) =>
                                dispatch({ type: "SET_TITLE", payload: e.target.value })
                            }
                            required
                            disabled={isSubmitting}
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="priority">Priority</label>
                        <select
                            id="priority"
                            className="priority-select"
                            value={state.priority}
                            onChange={(e) =>
                                dispatch({
                                    type: "SET_PRIORITY",
                                    payload: Number(e.target.value)
                                })
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
                    <input
                        id="description"
                        type="text"
                        placeholder="Enter task description (optional)"
                        value={state.description}
                        onChange={(e) =>
                            dispatch({
                                type: "SET_DESCRIPTION",
                                payload: e.target.value
                            })
                        }
                        disabled={isSubmitting}
                    />
                </div>

                <div className="form-row">
                    <div className="form-group">
                        <label htmlFor="dueDate">Due Date</label>
                        <input
                            id="dueDate"
                            type="date"
                            value={state.due_date}
                            onChange={(e) =>
                                dispatch({
                                    type: "SET_DUEDATE",
                                    payload: e.target.value
                                })
                            }
                            required
                            disabled={isSubmitting}
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="category">Category</label>
                        <select
                            id="category"
                            className="category-select"
                            value={state.categoryId}
                            onChange={(e) =>
                                dispatch({
                                    type: "SET_CATEGORY",
                                    payload: Number(e.target.value)
                                })
                            }
                            required
                            disabled={isSubmitting}
                        >
                            <option value="0" disabled>Select a category</option>
                            {categories.map((category) => (
                                <option key={category.taskCategoryId} value={category.taskCategoryId}>
                                    {category.name} {state.categoryId}
                                </option>
                            ))}
                        </select>
                    </div>
                </div>

                <button 
                    type="submit" 
                    className="submit-button"
                    disabled={isSubmitting}
                >
                    {isSubmitting ? "Creating..." : "Create Task"}
                </button>
            </form>
        </main>
    );
}

export default CreateTask;

