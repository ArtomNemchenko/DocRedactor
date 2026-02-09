import { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { documentService } from '../services/api';
import StarRating from '../components/StarRating';
import TextFormatToolbar from '../components/TextFormatToolbar';
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
  const [showCopyDropdown, setShowCopyDropdown] = useState(false);
  const [copySuccess, setCopySuccess] = useState(false);
  const contentEditableRef = useRef(null);

  useEffect(() => {
    loadDocument();
    loadVersions();
  }, [id]);

  const loadDocument = async () => {
    try {
      const data = await documentService.getById(id);
      setDocument(data);
      setEditedContent(data.content);
      if (contentEditableRef.current) {
        contentEditableRef.current.innerHTML = data.content;
      }
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
    const currentContent = contentEditableRef.current?.innerHTML || editedContent;
    
    if (currentContent === document.content) {
      setEditMode(false);
      return;
    }

    try {
      const updatedDoc = await documentService.update(
        parseInt(id),
        currentContent,
        changeDescription || 'Updated content'
      );
      setDocument(updatedDoc);
      setEditedContent(updatedDoc.content);
      if (contentEditableRef.current) {
        contentEditableRef.current.innerHTML = updatedDoc.content;
      }
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
    if (contentEditableRef.current) {
      contentEditableRef.current.innerHTML = document.content;
    }
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
      if (contentEditableRef.current) {
        contentEditableRef.current.innerHTML = updatedDoc.content;
      }
      setSelectedVersion(null);
      setShowRevertModal(false);
      setRevertDescription('');
      loadVersions();
    } catch (err) {
      alert('Failed to revert to version');
      console.error(err);
    }
  };

  const handleRateVersion = async (versionId, rating) => {
    try {
      await documentService.rateVersion(parseInt(id), versionId, rating);
      // Reload versions to show updated rating
      loadVersions();
    } catch (err) {
      alert('Failed to rate version');
      console.error(err);
    }
  };

  const copyWithStyling = async () => {
    try {
      await navigator.clipboard.writeText(document.content);
      setCopySuccess(true);
      setShowCopyDropdown(false);
      setTimeout(() => setCopySuccess(false), 2000);
    } catch (err) {
      alert('Failed to copy content');
      console.error(err);
    }
  };

  const copyWithoutStyling = async () => {
    try {
      // Create a temporary div to extract text content
      const tempDiv = window.document.createElement('div');
      tempDiv.innerHTML = document.content;
      const plainText = tempDiv.textContent || tempDiv.innerText || '';
      await navigator.clipboard.writeText(plainText);
      setCopySuccess(true);
      setShowCopyDropdown(false);
      setTimeout(() => setCopySuccess(false), 2000);
    } catch (err) {
      alert('Failed to copy content');
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
              <>
                <div className="copy-dropdown-container">
                  <button 
                    onClick={() => setShowCopyDropdown(!showCopyDropdown)} 
                    className="btn-copy"
                  >
                    Copy Content ▼
                  </button>
                  {showCopyDropdown && (
                    <div className="copy-dropdown">
                      <button onClick={copyWithStyling} className="dropdown-item">
                        Copy with Styling
                      </button>
                      <button onClick={copyWithoutStyling} className="dropdown-item">
                        Copy without Styling
                      </button>
                    </div>
                  )}
                </div>
                <button 
                  onClick={() => navigate(`/documents/${id}/split-view`)} 
                  className="btn-split-view"
                >
                  Open New Document
                </button>
                <button onClick={() => setEditMode(true)} className="btn-edit">
                  Edit Document
                </button>
              </>
            )}
          </div>
        </div>

        {copySuccess && (
          <div className="copy-success-notification">
            ✓ Content copied to clipboard!
          </div>
        )}

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
              <TextFormatToolbar />
              <div
                ref={contentEditableRef}
                className="edit-contenteditable"
                contentEditable={true}
                suppressContentEditableWarning={true}
                dangerouslySetInnerHTML={{ __html: editedContent }}
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
            <div 
              className="document-content"
              dangerouslySetInnerHTML={{ __html: document.content }}
            />
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
                  <div className="version-rating-section">
                    <label>Rate this version:</label>
                    <StarRating
                      rating={version.rating}
                      onRate={(rating) => handleRateVersion(version.id, rating)}
                      size="medium"
                    />
                  </div>
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
