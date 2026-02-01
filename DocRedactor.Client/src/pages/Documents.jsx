import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { documentService } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import './Documents.css';

function Documents() {
  const [documents, setDocuments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const { logout, user } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    loadDocuments();
  }, []);

  const loadDocuments = async () => {
    try {
      const data = await documentService.getAll();
      setDocuments(data);
    } catch (err) {
      setError('Failed to load documents');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this document?')) {
      return;
    }

    try {
      await documentService.delete(id);
      setDocuments(documents.filter((doc) => doc.id !== id));
    } catch (err) {
      alert('Failed to delete document');
      console.error(err);
    }
  };

  if (loading) {
    return <div className="loading">Loading documents...</div>;
  }

  return (
    <div className="documents-page">
      <header className="documents-header">
        <h1>DocRedactor</h1>
        <div className="header-actions">
          <span className="user-info">Welcome, {user?.userName}!</span>
          <button onClick={() => navigate('/documents/create')} className="btn-create">
            Create New Document
          </button>
          <button onClick={handleLogout} className="btn-logout">
            Logout
          </button>
        </div>
      </header>

      <main className="documents-content">
        {error && <div className="error-message">{error}</div>}

        {documents.length === 0 ? (
          <div className="empty-state">
            <h2>No documents yet</h2>
            <p>Create your first document to get started!</p>
            <button onClick={() => navigate('/documents/create')} className="btn-create">
              Create Document
            </button>
          </div>
        ) : (
          <div className="documents-grid">
            {documents.map((doc) => (
              <div key={doc.id} className="document-card">
                <h3>{doc.title}</h3>
                <p className="document-preview">
                  {doc.content.substring(0, 150)}
                  {doc.content.length > 150 ? '...' : ''}
                </p>
                <p className="document-date">
                  Created: {new Date(doc.createdAt).toLocaleDateString()}
                </p>
                <div className="document-actions">
                  <button
                    onClick={() => navigate(`/documents/${doc.id}`)}
                    className="btn-view"
                  >
                    View
                  </button>
                  <button
                    onClick={() => handleDelete(doc.id)}
                    className="btn-delete"
                  >
                    Delete
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </main>
    </div>
  );
}

export default Documents;
