import { Navigate, Route, Routes } from 'react-router-dom';
import './App.css';
import Login from './pages/Login';
import Register from './pages/Register';
import ReadTasks from './pages/ReadTasks';
import CreateTask from './pages/CreateTask';
import ReadUsers from './pages/ReadUsers';
import Dashboard from './pages/Dashboard';
import UserProfile from './pages/UserProfile';
import Navbar from './components/Navbar';
import NotFound from './pages/NotFound';
import TaskDetail from './pages/TaskDetail';
import ProtectedRoute from './components/ProtectedRoute';
import Logout from './pages/Logout';

function App() {
  
  return (
    <div className='app-container'>
      <Navbar/>
      <Routes>
        <Route path='/login' element={<Login/>}/>
        <Route path='/register' element={<ProtectedRoute><Register/></ProtectedRoute>}/>
        <Route path='/tasks' element={<ProtectedRoute><ReadTasks/></ProtectedRoute>}/>
        <Route path='/tasks/add' element={<ProtectedRoute><CreateTask/></ProtectedRoute>}/>
        <Route path='/users' element={<ProtectedRoute><ReadUsers/></ProtectedRoute>}/>
        <Route path='/dashboard' element={<ProtectedRoute><Dashboard/></ProtectedRoute>}/>
        <Route path='/profile' element={<ProtectedRoute><UserProfile/></ProtectedRoute>}/>
        <Route path='/logout' element={<ProtectedRoute><Logout/></ProtectedRoute>}/>
        <Route path='/tasks/task/:taskId' element={<ProtectedRoute><TaskDetail/></ProtectedRoute>}/>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path='*' element={<NotFound />} />
      </Routes>
      <footer></footer>
    </div>
  );
}

export default App;