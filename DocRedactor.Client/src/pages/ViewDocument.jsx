import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { documentService } from '../services/api';
import './ViewDocument.css';

function ViewDocument() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [document, setDocument] = useState(null);
  const [versions, setVersions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [editMode, setEditMode] = useState(false);
  const [editedContent, setEditedContent] = useState('');
  const [changeDescription, setChangeDescription] = useState('');
  const [selectedVersion, setSelectedVersion] = useState(null);
  const [showRevertModal, setShowRevertModal] = useState(false);
  const [revertDescription, setRevertDescription] = useState('');

  useEffect(() => {
    loadDocument();
    loadVersions();
  }, [id]);

  const loadDocument = async () => {
    try {
      const data = await documentService.getById(id);
      setDocument(data);
      setEditedContent(data.content);
    } catch (err) {
      setError('Failed to load document');
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

  const handleUpdate = async () => {
    if (editedContent === document.content) {
      setEditMode(false);
      return;
    }

    try {
      const updatedDoc = await documentService.update(
        parseInt(id),
        editedContent,
        changeDescription || 'Updated content'
      );
      setDocument(updatedDoc);
      setEditMode(false);
      setChangeDescription('');
      loadVersions();
    } catch (err) {
      alert('Failed to update document');
      console.error(err);
    }
  };

  const handleCancelEdit = () => {
    setEditedContent(document.content);
    setChangeDescription('');
    setEditMode(false);
  };

  const handleViewVersion = (version) => {
    // Toggle: if the version is already selected, hide it; otherwise show it
    setSelectedVersion(selectedVersion?.id === version.id ? null : version);
  };

  const handleRevert = async () => {
    if (!selectedVersion) return;

    try {
      const updatedDoc = await documentService.revertToVersion(
        parseInt(id),
        selectedVersion.versionNumber,
        revertDescription || `Reverted to version ${selectedVersion.versionNumber}`
      );
      setDocument(updatedDoc);
      setEditedContent(updatedDoc.content);
      setSelectedVersion(null);
      setShowRevertModal(false);
      setRevertDescription('');
      loadVersions();
    } catch (err) {
      alert('Failed to revert to version');
      console.error(err);
    }
  };

  if (loading) {
    return <div className="loading">Loading document...</div>;
  }

  if (error || !document) {
    return (
      <div className="error-container">
        <h2>Error</h2>
        <p>{error || 'Document not found'}</p>
        <button onClick={() => navigate('/documents')} className="btn-back">
          Back to Documents
        </button>
      </div>
    );
  }

  return (
    <div className="view-document-page">
      <div className="view-document-container">
        <div className="document-header">
          <h1>{document.title}</h1>
          <div className="header-actions">
            <button onClick={() => navigate('/documents')} className="btn-back">
              Back to Documents
            </button>
            {!editMode && (
              <button onClick={() => setEditMode(true)} className="btn-edit">
                Edit Document
              </button>
            )}
          </div>
        </div>

        <div className="document-info">
          <p>Created: {new Date(document.createdAt).toLocaleString()}</p>
          <p>Last Updated: {new Date(document.updatedAt).toLocaleString()}</p>
          <p>Current Version: {document.currentVersion}</p>
          <p>Total Versions: {versions.length}</p>
        </div>

        <div className="document-section">
          <h3>Current Content</h3>
          {editMode ? (
            <div className="edit-form">
              <textarea
                className="edit-textarea"
                value={editedContent}
                onChange={(e) => setEditedContent(e.target.value)}
                rows={15}
              />
              <div className="form-group">
                <label htmlFor="changeDescription">Change Description</label>
                <input
                  type="text"
                  id="changeDescription"
                  value={changeDescription}
                  onChange={(e) => setChangeDescription(e.target.value)}
                  placeholder="Describe your changes..."
                  maxLength={500}
                />
              </div>
              <div className="form-actions">
                <button onClick={handleUpdate} className="btn-submit">
                  Save Changes
                </button>
                <button onClick={handleCancelEdit} className="btn-cancel">
                  Cancel
                </button>
              </div>
            </div>
          ) : (
            <div className="document-content">
              {document.content}
            </div>
          )}
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
                      {version.versionNumber === document.currentVersion && (
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
                  <div className="version-actions">
                    <button
                      onClick={() => handleViewVersion(version)}
                      className="btn-view-version"
                    >
                      {selectedVersion?.id === version.id ? 'Hide' : 'View'}
                    </button>
                    {version.versionNumber !== document.currentVersion && (
                      <button
                        onClick={() => {
                          setSelectedVersion(version);
                          setShowRevertModal(true);
                        }}
                        className="btn-revert"
                      >
                        Revert to This Version
                      </button>
                    )}
                  </div>
                  {selectedVersion?.id === version.id && (
                    <div className="version-content">
                      <h4>Content at Version {version.versionNumber}:</h4>
                      <pre className="version-text">{version.content}</pre>
                    </div>
                  )}
                </div>
              ))}
            </div>
          )}
        </div>

        {showRevertModal && (
          <div className="modal-overlay" onClick={() => setShowRevertModal(false)}>
            <div className="modal-content" onClick={(e) => e.stopPropagation()}>
              <h3>Revert to Version {selectedVersion?.versionNumber}</h3>
              <p>
                This will create a new version (v{document.currentVersion + 1}) with the content
                from version {selectedVersion?.versionNumber}.
              </p>
              <div className="form-group">
                <label htmlFor="revertDescription">Change Description (Optional)</label>
                <input
                  type="text"
                  id="revertDescription"
                  value={revertDescription}
                  onChange={(e) => setRevertDescription(e.target.value)}
                  placeholder="Reason for reverting..."
                  maxLength={500}
                />
              </div>
              <div className="modal-actions">
                <button onClick={handleRevert} className="btn-confirm">
                  Confirm Revert
                </button>
                <button onClick={() => setShowRevertModal(false)} className="btn-cancel">
                  Cancel
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default ViewDocument;
