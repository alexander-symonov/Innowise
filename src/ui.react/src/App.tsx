import { BrowserRouter as Router, Routes, Route, NavLink } from 'react-router-dom';
import { AuthProvider, useAuth } from './components/auth/AuthContext';
import Login from './components/auth/Login';
import HealthIncidentReport from './components/reports/HealthIncidentReport';
import FlatIncidentReport from './components/reports/FlatIncidentReport';
import CarIncidentReport from './components/reports/CarIncidentReport';
import Charts from './components/Charts';
import About from './components/About';
import Users from './components/Users';
import './App.css';

function App() {
  console.log("render App");
  return (
    <AuthProvider>
      <Router>
        <MainApp />
      </Router>
    </AuthProvider>
  );
}

function MainApp() {
  const { isAuthenticated, login, logout } = useAuth();

  return (
    <>
      <header className="header">
        <nav className="nav">
          <NavLink to="/" end>Home</NavLink>
          <NavLink to="/about">About</NavLink>
          <NavLink to="/reports">Reports</NavLink>
          <NavLink to="/users">Users</NavLink>
          <NavLink to="/charts">Charts</NavLink>
        </nav>
        {isAuthenticated ? (
          <button onClick={logout} className="logout-button">
            Logout
          </button>
        ) : (
          <NavLink to="/login" className="login-button">
            Login
          </NavLink>
        )}
      </header>
      <Routes>
        <Route path="/" element={<h1>Welcome to the App</h1>} />
        <Route path="/about" element={<About />} />
        <Route path="/reports/*" element={<Reports />} />
        <Route path="/users" element={<Users />} />
        <Route path="/charts" element={<Charts />} />
        <Route path="/login" element={<Login />} />
      </Routes>
    </>
  );
}

function Reports() {
  return (
    <div>
      <h1>Reports</h1>
      <nav className="reports-nav">
        <NavLink to="/reports/health-incident">Health Incident</NavLink>
        <NavLink to="/reports/flat-incident">Flat Incident</NavLink>
        <NavLink to="/reports/car-incident">Car Incident</NavLink>
      </nav>
      <Routes>
        <Route path="/health-incident" element={<HealthIncidentReport />} />
        <Route path="/flat-incident" element={<FlatIncidentReport />} />
        <Route path="/car-incident" element={<CarIncidentReport />} />
        <Route path="*" element={<h1>Reports header</h1>} />
      </Routes>
    </div>
  );
}

export default App;
