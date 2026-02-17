import { NavLink } from "react-router-dom";
import '../styles/Navbar.css';
import { useAuth } from "../contexts/AuthContext";
import { type NavigationType } from "../types";

const Navbar = () => {
  const { isAuthenticated, isAdmin } = useAuth();

  const baseLinks: NavigationType[] = [
    { name: "Dashboard", link: "/dashboard" },
    { name: "Tasks", link: "/tasks" },
    { name: "Add Tasks", link: "/tasks/add" },
    { name: "Profile", link: "/profile" },
    { name: "Logout", link: "/logout"}
  ];

  const navigations = isAdmin
    ? [
        ...baseLinks,
        { name: "Users", link: "/users" },
        { name: "Register", link: "/register" },
      ]
    : baseLinks;

  if (isAuthenticated === false) {
    return (
      <header>
        <nav className="navbar"></nav>
      </header>
    );
  }

  return (
    <header>
      <nav className="navbar">
        <ul className="nav-links">
          {navigations.map((navigation) => (
            <li key={navigation.link}>
              <NavLink 
                to={navigation.link} 
                end
                className={({ isActive }) => isActive ? "active" : ""}
              >
                {navigation.name}
              </NavLink>
            </li>
          ))}
        </ul>
      </nav>
    </header>
  );
};

export default Navbar;