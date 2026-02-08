import { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { documentService } from '../services/api';
import StarRating from '../components/StarRating';
import TextFormatToolbar from '../components/TextFormatToolbar';
import './SplitViewDocument.css';

function SplitViewDocument() {
  const { id } = useParams();
  const navigate = useNavigate();
  
  // Original document (left panel)
  const [originalDocument, setOriginalDocument] = useState(null);
  const [versions, setVersions] = useState([]);
  const [selectedVersion, setSelectedVersion] = useState(null);
  
  // New document (right panel)
  const [newTitle, setNewTitle] = useState('');
  const [newContent, setNewContent] = useState('');
  const [loading, setLoading] = useState(true);
  const [creating, setCreating] = useState(false);
  const [error, setError] = useState('');
  
  const contentEditableRef = useRef(null);

  useEffect(() => {
    loadOriginalDocument();
    loadVersions();
  }, [id]);

  const loadOriginalDocument = async () => {
    try {
      const data = await documentService.getById(id);
      setOriginalDocument(data);
    } catch (err) {
      setError('Failed to load original document');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const loadVersions = async () => {
    try {
      const data = await documentService.getVersions(id);
      setVersions(data);
    } catch (err) {
      console.error('Failed to load versions', err);
    }
  };

  const handleViewVersion = (version) => {
    setSelectedVersion(selectedVersion?.id === version.id ? null : version);
  };

  const handleCreateNewDocument = async (e) => {
    e.preventDefault();
    setError('');
    setCreating(true);

    const currentContent = contentEditableRef.current?.innerHTML || newContent;

    try {
      const createdDoc = await documentService.create(newTitle, currentContent);
      // Navigate to the newly created document
      navigate(`/documents/${createdDoc.id}`);
    } catch (err) {
      setError('Failed to create document. Please try again.');
      console.error(err);
    } finally {
      setCreating(false);
    }
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  if (error && !originalDocument) {
    return (
      <div className="error-container">
        <h2>Error</h2>
        <p>{error}</p>
        <button onClick={() => navigate('/documents')} className="btn-back">
          Back to Documents
        </button>
      </div>
    );
  }

  return (
    <div className="split-view-container">
      {/* Left Panel - Original Document (Read-only) */}
      <div className="split-panel left-panel">
        <div className="panel-header">
          <h2>Original Document</h2>
          <button onClick={() => navigate(`/documents/${id}`)} className="btn-back-small">
            Back to Single View
          </button>
        </div>
        
        {originalDocument && (
          <div className="panel-content">
            <div className="document-title-display">
              <h1>{originalDocument.title}</h1>
            </div>
            
            <div className="document-info">
              <p><strong>Created:</strong> {new Date(originalDocument.createdAt).toLocaleString()}</p>
              <p><strong>Last Updated:</strong> {new Date(originalDocument.updatedAt).toLocaleString()}</p>
              <p><strong>Current Version:</strong> {originalDocument.currentVersion}</p>
            </div>

            <div className="document-section">
              <h3>Current Content</h3>
              <div 
                className="document-content-display"
                dangerouslySetInnerHTML={{ __html: originalDocument.content }}
              />
            </div>

            <div className="versions-section">
              <h3>Version History</h3>
              {versions.length === 0 ? (
                <p className="no-versions">No version history available.</p>
              ) : (
                <div className="versions-list">
                  {versions.map((version) => (
                    <div key={version.id} className="version-item">
                      <div className="version-header">
                        <span className="version-number">
                          Version {version.versionNumber}
                          {version.versionNumber === originalDocument.currentVersion && (
                            <span className="current-badge">CURRENT</span>
                          )}
                        </span>
                        <span className="version-date">
                          {new Date(version.createdAt).toLocaleString()}
                        </span>
                      </div>
                      <p className="version-description">
                        {version.changeDescription || 'No description'}
                      </p>
                      <div className="version-rating-section">
                        <label>Rating:</label>
                        <StarRating
                          rating={version.rating}
                          readOnly={true}
                          size="small"
                        />
                      </div>
                      <div className="version-actions">
                        <button
                          onClick={() => handleViewVersion(version)}
                          className="btn-view-version"
                        >
                          {selectedVersion?.id === version.id ? 'Hide' : 'View'}
                        </button>
                      </div>
                      {selectedVersion?.id === version.id && (
                        <div className="version-content">
                          <h4>Content at Version {version.versionNumber}:</h4>
                          <div 
                            className="version-text"
                            dangerouslySetInnerHTML={{ __html: version.content }}
                          />
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        )}
      </div>

      {/* Right Panel - New Document Creation */}
      <div className="split-panel right-panel">
        <div className="panel-header">
          <h2>Create New Document</h2>
        </div>
        
        <div className="panel-content">
          {error && <div className="error-message">{error}</div>}
          
          <form onSubmit={handleCreateNewDocument}>
            <div className="form-group">
              <label htmlFor="newTitle">Document Title</label>
              <input
                type="text"
                id="newTitle"
                value={newTitle}
                onChange={(e) => setNewTitle(e.target.value)}
                required
                placeholder="Enter document title"
                maxLength={200}
              />
            </div>

            <div className="form-group">
              <label htmlFor="newContent">Content</label>
              <TextFormatToolbar />
              <div
                ref={contentEditableRef}
                className="edit-contenteditable"
                contentEditable={true}
                suppressContentEditableWarning={true}
                onInput={(e) => setNewContent(e.currentTarget.innerHTML)}
                placeholder="Start typing your document content..."
              />
            </div>

            <div className="form-actions">
              <button type="submit" disabled={creating} className="btn-submit">
                {creating ? 'Creating...' : 'Create Document'}
              </button>
              <button
                type="button"
                onClick={() => navigate(`/documents/${id}`)}
                className="btn-cancel"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}

export default SplitViewDocument;
