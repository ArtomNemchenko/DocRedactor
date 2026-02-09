import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import userEvent from '@testing-library/user-event';
import Documents from '../pages/Documents';
import { documentService } from '../services/api';

// Mock the AuthContext
jest.mock('../contexts/AuthContext', () => ({
  AuthContext: {
    Consumer: ({ children }) => children({ user: { userName: 'testuser' }, logout: jest.fn() }),
  },
  useAuth: () => ({ user: { userName: 'testuser' }, logout: jest.fn() }),
}));

// Mock the API service
jest.mock('../services/api', () => ({
  documentService: {
    getAll: jest.fn(),
    delete: jest.fn(),
  },
}));

// Mock react-router-dom hooks
jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: () => jest.fn(),
}));

describe('Documents Component', () => {
  const mockDocuments = [
    {
      id: 1,
      title: 'Document 1',
      content: 'This is the content of document 1',
      currentVersion: 1,
      userId: 'user1',
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-01T00:00:00Z',
    },
    {
      id: 2,
      title: 'Document 2',
      content: 'This is the content of document 2',
      currentVersion: 2,
      userId: 'user1',
      createdAt: '2024-01-02T00:00:00Z',
      updatedAt: '2024-01-02T00:00:00Z',
    },
  ];

  beforeEach(() => {
    jest.clearAllMocks();
    documentService.getAll.mockResolvedValue(mockDocuments);
  });

  test('renders documents list correctly', async () => {
    render(
      <BrowserRouter>
        <Documents />
      </BrowserRouter>
    );

    await waitFor(() => {
      expect(screen.getByText('Document 1')).toBeInTheDocument();
    });

    expect(screen.getByText('Document 2')).toBeInTheDocument();
    expect(screen.getByText(/This is the content of document 1/)).toBeInTheDocument();
    expect(screen.getByText(/This is the content of document 2/)).toBeInTheDocument();
  });

  test('displays version numbers for documents', async () => {
    render(
      <BrowserRouter>
        <Documents />
      </BrowserRouter>
    );

    await waitFor(() => {
      expect(screen.getByText('Document 1')).toBeInTheDocument();
    });

    expect(screen.getByText(/Version: 1/)).toBeInTheDocument();
    expect(screen.getByText(/Version: 2/)).toBeInTheDocument();
  });

  test('shows empty state when no documents', async () => {
    documentService.getAll.mockResolvedValue([]);

    render(
      <BrowserRouter>
        <Documents />
      </BrowserRouter>
    );

    await waitFor(() => {
      expect(screen.getByText('No documents yet')).toBeInTheDocument();
    });

    expect(screen.getByText('Create your first document to get started!')).toBeInTheDocument();
  });

  test('loads documents on mount', async () => {
    render(
      <BrowserRouter>
        <Documents />
      </BrowserRouter>
    );

    await waitFor(() => {
      expect(documentService.getAll).toHaveBeenCalled();
    });
  });
});
