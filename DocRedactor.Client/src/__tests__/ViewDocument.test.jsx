import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import userEvent from '@testing-library/user-event';
import ViewDocument from '../pages/ViewDocument';
import { documentService } from '../services/api';

// Mock the API service
jest.mock('../services/api', () => ({
  documentService: {
    getById: jest.fn(),
    getVersions: jest.fn(),
    update: jest.fn(),
    revertToVersion: jest.fn(),
  },
}));

// Mock react-router-dom hooks
jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useParams: () => ({ id: '1' }),
  useNavigate: () => jest.fn(),
}));

describe('ViewDocument Component', () => {
  const mockDocument = {
    id: 1,
    title: 'Test Document',
    content: 'This is test content',
    currentVersion: 2,
    userId: 'user1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-02T00:00:00Z',
  };

  const mockVersions = [
    {
      id: 2,
      documentId: 1,
      content: 'This is test content',
      versionNumber: 2,
      changeDescription: 'Updated content',
      userId: 'user1',
      createdAt: '2024-01-02T00:00:00Z',
    },
    {
      id: 1,
      documentId: 1,
      content: 'Original content',
      versionNumber: 1,
      changeDescription: 'Initial version',
      userId: 'user1',
      createdAt: '2024-01-01T00:00:00Z',
    },
  ];

  beforeEach(() => {
    jest.clearAllMocks();
    documentService.getById.mockResolvedValue(mockDocument);
    documentService.getVersions.mockResolvedValue(mockVersions);
  });

  test('renders document details correctly', async () => {
    render(
      <BrowserRouter>
        <ViewDocument />
      </BrowserRouter>
    );

    await waitFor(() => {
      expect(screen.getByText('Test Document')).toBeInTheDocument();
    });

    expect(screen.getByText(/This is test content/)).toBeInTheDocument();
    expect(screen.getByText(/Current Version: 2/)).toBeInTheDocument();
    expect(screen.getByText(/Total Versions: 2/)).toBeInTheDocument();
  });

  test('displays version history', async () => {
    render(
      <BrowserRouter>
        <ViewDocument />
      </BrowserRouter>
    );

    await waitFor(() => {
      expect(screen.getByText('Test Document')).toBeInTheDocument();
    });

    expect(screen.getByText('Version 2')).toBeInTheDocument();
    expect(screen.getByText('Version 1')).toBeInTheDocument();
    expect(screen.getByText('Updated content')).toBeInTheDocument();
    expect(screen.getByText('Initial version')).toBeInTheDocument();
  });

  test('toggles version content visibility when View/Hide button is clicked', async () => {
    const user = userEvent.setup();
    
    render(
      <BrowserRouter>
        <ViewDocument />
      </BrowserRouter>
    );

    await waitFor(() => {
      expect(screen.getByText('Test Document')).toBeInTheDocument();
    });

    // Find the View buttons (there will be multiple)
    const viewButtons = screen.getAllByText('View');
    
    // Click on the first View button (for version 2)
    await user.click(viewButtons[0]);

    // Wait for the content to appear
    await waitFor(() => {
      expect(screen.getByText('Content at Version 2:')).toBeInTheDocument();
    });

    // Now the button should say "Hide"
    expect(screen.getByText('Hide')).toBeInTheDocument();

    // Click Hide button
    await user.click(screen.getByText('Hide'));

    // Content should be hidden
    await waitFor(() => {
      expect(screen.queryByText('Content at Version 2:')).not.toBeInTheDocument();
    });
  });

  test('shows edit mode when Edit Document button is clicked', async () => {
    const user = userEvent.setup();
    
    render(
      <BrowserRouter>
        <ViewDocument />
      </BrowserRouter>
    );

    await waitFor(() => {
      expect(screen.getByText('Test Document')).toBeInTheDocument();
    });

    const editButton = screen.getByText('Edit Document');
    await user.click(editButton);

    // Edit mode should show
    expect(screen.getByText('Save Changes')).toBeInTheDocument();
    expect(screen.getByText('Cancel')).toBeInTheDocument();
    expect(screen.getByLabelText('Change Description')).toBeInTheDocument();
  });

  test('updates document when Save Changes is clicked', async () => {
    const user = userEvent.setup();
    documentService.update.mockResolvedValue({
      ...mockDocument,
      content: 'Updated content',
      currentVersion: 3,
    });

    render(
      <BrowserRouter>
        <ViewDocument />
      </BrowserRouter>
    );

    await waitFor(() => {
      expect(screen.getByText('Test Document')).toBeInTheDocument();
    });

    // Enter edit mode
    const editButton = screen.getByText('Edit Document');
    await user.click(editButton);

    // Change content
    const textarea = screen.getByRole('textbox', { name: '' });
    await user.clear(textarea);
    await user.type(textarea, 'Updated content');

    // Save changes
    const saveButton = screen.getByText('Save Changes');
    await user.click(saveButton);

    await waitFor(() => {
      expect(documentService.update).toHaveBeenCalledWith(
        1,
        'Updated content',
        expect.any(String)
      );
    });
  });

  test('loads document and versions on mount', async () => {
    render(
      <BrowserRouter>
        <ViewDocument />
      </BrowserRouter>
    );

    await waitFor(() => {
      expect(documentService.getById).toHaveBeenCalledWith('1');
      expect(documentService.getVersions).toHaveBeenCalledWith('1');
    });
  });
});
