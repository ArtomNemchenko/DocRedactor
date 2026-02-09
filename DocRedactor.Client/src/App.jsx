import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import PrivateRoute from './components/PrivateRoute';
import Login from './pages/Login';
import Register from './pages/Register';
import Documents from './pages/Documents';
import CreateDocument from './pages/CreateDocument';
import ViewDocument from './pages/ViewDocument';
import SplitViewDocument from './pages/SplitViewDocument';
import './App.css';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route
            path="/documents"
            element={
              <PrivateRoute>
                <Documents />
              </PrivateRoute>
            }
          />
          <Route
            path="/documents/create"
            element={
              <PrivateRoute>
                <CreateDocument />
              </PrivateRoute>
            }
          />
          <Route
            path="/documents/:id"
            element={
              <PrivateRoute>
                <ViewDocument />
              </PrivateRoute>
            }
          />
          <Route
            path="/documents/:id/split-view"
            element={
              <PrivateRoute>
                <SplitViewDocument />
              </PrivateRoute>
            }
          />
          <Route path="/" element={<Navigate to="/documents" />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
