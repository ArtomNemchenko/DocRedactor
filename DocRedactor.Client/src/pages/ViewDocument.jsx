import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { documentService, redactionService } from '../services/api';
import './ViewDocument.css';

function ViewDocument() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [document, setDocument] = useState(null);
  const [redactions, setRedactions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [selectedText, setSelectedText] = useState({ start: -1, end: -1 });
  const [reason, setReason] = useState('');
  const [showRedactionForm, setShowRedactionForm] = useState(false);

  useEffect(() => {
    loadDocument();
    loadRedactions();
  }, [id]);

  const loadDocument = async () => {
    try {
      const data = await documentService.getById(id);
      setDocument(data);
    } catch (err) {
      setError('Failed to load document');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const loadRedactions = async () => {
    try {
      const data = await redactionService.getByDocument(id);
      setRedactions(data);
    } catch (err) {
      console.error('Failed to load redactions', err);
    }
  };

  const handleTextSelection = () => {
    const selection = window.getSelection();
    const text = selection.toString();
    
    if (text.length > 0 && document) {
      const range = selection.getRangeAt(0);
      const preSelectionRange = range.cloneRange();
      preSelectionRange.selectNodeContents(document.getElementById('document-content'));
      preSelectionRange.setEnd(range.startContainer, range.startOffset);
      const start = preSelectionRange.toString().length;
      const end = start + text.length;
      
      setSelectedText({ start, end });
      setShowRedactionForm(true);
    }
  };

  const handleCreateRedaction = async (e) => {
    e.preventDefault();
    
    if (selectedText.start === -1 || selectedText.end === -1) {
      alert('Please select text to redact');
      return;
    }

    try {
      const newRedaction = await redactionService.create(
        parseInt(id),
        selectedText.start,
        selectedText.end,
        reason
      );
      setRedactions([...redactions, newRedaction]);
      setReason('');
      setSelectedText({ start: -1, end: -1 });
      setShowRedactionForm(false);
      window.getSelection().removeAllRanges();
    } catch (err) {
      alert('Failed to create redaction');
      console.error(err);
    }
  };

  const handleDeleteRedaction = async (redactionId) => {
    if (!window.confirm('Are you sure you want to remove this redaction?')) {
      return;
    }

    try {
      await redactionService.delete(redactionId);
      setRedactions(redactions.filter((r) => r.id !== redactionId));
    } catch (err) {
      alert('Failed to delete redaction');
      console.error(err);
    }
  };

  const getRedactedContent = () => {
    if (!document) return '';
    
    let content = document.content;
    const sortedRedactions = [...redactions].sort((a, b) => b.startPosition - a.startPosition);
    
    sortedRedactions.forEach((redaction) => {
      const before = content.substring(0, redaction.startPosition);
      const redacted = '█'.repeat(redaction.endPosition - redaction.startPosition);
      const after = content.substring(redaction.endPosition);
      content = before + redacted + after;
    });
    
    return content;
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
          <button onClick={() => navigate('/documents')} className="btn-back">
            Back to Documents
          </button>
        </div>

        <div className="document-info">
          <p>Created: {new Date(document.createdAt).toLocaleString()}</p>
          <p>Redactions: {redactions.length}</p>
        </div>

        <div className="document-section">
          <h3>Original Content</h3>
          <div
            id="document-content"
            className="document-content"
            onMouseUp={handleTextSelection}
          >
            {document.content}
          </div>
        </div>

        <div className="document-section">
          <h3>Redacted Content</h3>
          <div className="document-content redacted">
            {getRedactedContent()}
          </div>
        </div>

        {showRedactionForm && (
          <div className="redaction-form">
            <h3>Create Redaction</h3>
            <p>
              Selected text: positions {selectedText.start} to {selectedText.end}
            </p>
            <form onSubmit={handleCreateRedaction}>
              <div className="form-group">
                <label htmlFor="reason">Reason (optional)</label>
                <input
                  type="text"
                  id="reason"
                  value={reason}
                  onChange={(e) => setReason(e.target.value)}
                  placeholder="Enter reason for redaction"
                  maxLength={500}
                />
              </div>
              <div className="form-actions">
                <button type="submit" className="btn-submit">
                  Create Redaction
                </button>
                <button
                  type="button"
                  onClick={() => {
                    setShowRedactionForm(false);
                    setReason('');
                    setSelectedText({ start: -1, end: -1 });
                  }}
                  className="btn-cancel"
                >
                  Cancel
                </button>
              </div>
            </form>
          </div>
        )}

        <div className="redactions-section">
          <h3>Redactions ({redactions.length})</h3>
          {redactions.length === 0 ? (
            <p className="no-redactions">
              No redactions yet. Select text in the original content to create one.
            </p>
          ) : (
            <div className="redactions-list">
              {redactions.map((redaction) => (
                <div key={redaction.id} className="redaction-item">
                  <div className="redaction-info">
                    <p>
                      <strong>Position:</strong> {redaction.startPosition} - {redaction.endPosition}
                    </p>
                    {redaction.reason && (
                      <p>
                        <strong>Reason:</strong> {redaction.reason}
                      </p>
                    )}
                    <p className="redaction-date">
                      Created: {new Date(redaction.createdAt).toLocaleString()}
                    </p>
                  </div>
                  <button
                    onClick={() => handleDeleteRedaction(redaction.id)}
                    className="btn-delete-small"
                  >
                    Remove
                  </button>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

export default ViewDocument;
