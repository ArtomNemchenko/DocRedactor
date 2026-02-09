import { useState, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { documentService } from '../services/api';
import TextFormatToolbar from '../components/TextFormatToolbar';
import './CreateDocument.css';

function CreateDocument() {
  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const contentEditableRef = useRef(null);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    const currentContent = contentEditableRef.current?.innerHTML || content;

    try {
      await documentService.create(title, currentContent);
      navigate('/documents');
    } catch (err) {
      setError('Failed to create document. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="create-document-page">
      <div className="create-document-container">
        <div className="create-document-header">
          <h2>Create New Document</h2>
          <button onClick={() => navigate('/documents')} className="btn-back">
            Back to Documents
          </button>
        </div>

        {error && <div className="error-message">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="title">Document Title</label>
            <input
              type="text"
              id="title"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              required
              placeholder="Enter document title"
              maxLength={200}
            />
          </div>

          <div className="form-group">
            <label htmlFor="content">Content</label>
            <TextFormatToolbar />
            <div
              ref={contentEditableRef}
              className="edit-contenteditable"
              contentEditable={true}
              suppressContentEditableWarning={true}
              onInput={(e) => setContent(e.currentTarget.innerHTML)}
            />
          </div>

          <div className="form-actions">
            <button type="submit" disabled={loading} className="btn-submit">
              {loading ? 'Creating...' : 'Create Document'}
            </button>
            <button
              type="button"
              onClick={() => navigate('/documents')}
              className="btn-cancel"
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default CreateDocument;
