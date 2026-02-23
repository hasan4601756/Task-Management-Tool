export type CreateTaskType = {
    title: string;
    description: string;
    dueDate: string;
    categoryId: number;
    priority: number;
};

export type TaskType = {
    id : number,
    title : string,
    taskStatus : number,
    priority: number,
    userName: string|null
};

export type CategoryType = {
    taskCategoryId: number;
    name: string;
    description: string;
};

export type TaskDetailType = {
    id : number,
    title : string,
    description : string,
    dueDate : string,
    creationDate : string,
    taskStatus : number,
    categoryId: number,
    categoryName : string,
    categoryDescription : string,
    priority: number
};

export type TaskUpdateType = {
    id : number,
    title : string,
    description : string,
    dueDate : string,
    status : number,
    categoryId: number,
    priority: number
};

export type taskRouteParamsType = {taskId : string};

export type DashboardType = {
    completedTasks: number,
    inProgressTasks: number,
    pendingTasks: number
};

export type UserType = {
  userId: string;
  userName: string;
  email: string;
};

export type ProfileType = {
  userName: string;
  email: string;
  fullName: string;
  phoneNumber: string;
};

export type NavigationType = {
  name: string,
  link: string

};

