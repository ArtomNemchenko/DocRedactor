# DocRedactor

A full-stack document versioning and management application built with ASP.NET Core Web API and React.

## Test Credentials

The application comes pre-seeded with test users and documents:

| Email | Username | Password |
|-------|----------|----------|
| alice@example.com | alice | Alice123 |
| bob@example.com | bob | Bob123 |

- **Alice** has 2 test documents: "Meeting Notes" and "Project Proposal"
- **Bob** has 1 test document: "Technical Specification"

## Features

- **User Authentication**: Secure registration and login using JWT tokens
- **Document Management**: Create, view, edit, and delete documents
- **Rich Text Formatting**: Format text with bold, italic, and 12 color options
- **Document Versioning**: Track all changes with full version history
- **Version Control**: Edit documents to create new versions, view any previous version, and revert to any version (creates a new version at HEAD)
- **Version Rating**: Rate document versions with a 5-star system
- **Per-User Authorization**: Users can only access their own documents and versions
- **Audit Logging**: All operations are logged for security and compliance
- **Change Descriptions**: Add descriptions when updating or reverting documents
- **Modern UI**: Responsive React frontend with beautiful gradients
- **Clean Architecture**: Service layer, repository pattern, and dependency injection

## Architecture

### Backend (ASP.NET Core Web API)

- **Framework**: .NET 9.0
- **Database**: SQLite with Entity Framework Core
- **Authentication**: ASP.NET Identity with JWT Bearer tokens
- **Architecture Pattern**: 
  - Controllers → Services → Repositories
  - Dependency Injection for all services
  - Separated JWT token generation service
- **API Structure**:
  - `AuthController`: User registration and login (uses IJwtTokenService)
  - `DocumentsController`: CRUD operations and version management (uses IDocumentService)
  - `IDocumentService`: Business logic for document operations
  - `IDocumentRepository`: Data access for documents
  - `IDocumentVersionRepository`: Data access for document versions

### Frontend (React)

- **Framework**: React 18 with Vite
- **Routing**: React Router DOM for navigation
- **HTTP Client**: Axios for API communication
- **State Management**: React Context API for authentication
- **Styling**: Custom CSS with modern gradients

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/)

### Backend Setup

1. Navigate to the API directory:
```bash
cd DocRedactor.API
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the API:
```bash
dotnet run
```

The API will start at `http://localhost:5180`

### Frontend Setup

1. Navigate to the client directory:
```bash
cd DocRedactor.Client
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm run dev
```

The frontend will start at `http://localhost:5173`

## Usage

### 1. Login with Test Account
- Use one of the test credentials (alice@example.com / Alice123 or bob@example.com / Bob123)
- Or register a new account with email, username, and password (minimum 6 characters)
- Password must contain uppercase, lowercase, and digit

### 2. Create a Document
- After logging in, click "Create New Document"
- Enter a title and content
- **Use the formatting toolbar** to apply bold, italic, or color to selected text:
  - Select text and click **B** for bold
  - Select text and click *I* for italic  
  - Select text and click **A** to choose from 12 colors
- Click "Create Document" - this creates version 1

### 3. Edit a Document (Create New Version)
- Open a document
- Click "Edit Document"
- The formatting toolbar appears above the content
- Modify the content and apply formatting as needed
- Add an optional change description
- Click "Save Changes" - this creates a new version

### 4. View Version History
- Scroll down to the "Version History" section
- See all versions with version numbers, timestamps, and descriptions
- Click "View" on any version to see its content
- The current version is marked with a "CURRENT" badge

### 5. Revert to a Previous Version
- In the version history, click "Revert to This Version" on any past version
- Add an optional description for why you're reverting
- Click "Confirm Revert" - this creates a new version at HEAD with the old content

### 6. Manage Documents
- View all your documents on the main page
- Each document shows its current version number
- Delete documents as needed
- All changes are tracked and logged

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and receive JWT token

### Documents
- `GET /api/documents` - Get all documents for current user
- `GET /api/documents/{id}` - Get specific document
- `POST /api/documents` - Create new document (creates v1)
- `PUT /api/documents/{id}` - Update document (creates new version)
- `DELETE /api/documents/{id}` - Delete document
- `GET /api/documents/{id}/versions` - Get all versions of a document
- `POST /api/documents/{id}/revert` - Revert to a specific version (creates new version at HEAD)

## Security Features

1. **JWT Authentication**: Stateless token-based authentication with separate service
2. **Password Requirements**: Strong password policy enforced
3. **Per-User Authorization**: Users can only access their own data
4. **Audit Logging**: All operations logged with user ID and timestamp
5. **Input Validation**: Request validation on both client and server
6. **CORS Configuration**: Restricted to known frontend origins
7. **Version Tracking**: Complete audit trail of all document changes
8. **Clean Architecture**: Separation of concerns with repository and service patterns

**Important Security Note**: For production deployments, the JWT secret key in `appsettings.json` should be moved to environment variables or a secure secret management system like Azure Key Vault, AWS Secrets Manager, or user secrets for development.

## Database Schema

### Users (AspNetUsers)
- Standard ASP.NET Identity user table

### Documents
- `Id`: Primary key
- `Title`: Document title (max 200 chars)
- `Content`: Current document content
- `CurrentVersion`: Current version number
- `UserId`: Foreign key to user
- `CreatedAt`: Original creation timestamp
- `UpdatedAt`: Last update timestamp

### DocumentVersions
- `Id`: Primary key
- `DocumentId`: Foreign key to document
- `Content`: Content at this version
- `VersionNumber`: Sequential version number
- `ChangeDescription`: Description of what changed
- `UserId`: Foreign key to user who made the change
- `CreatedAt`: When this version was created
- `Reason`: Optional explanation (max 500 chars)
- `CreatedAt`: Timestamp

## Development

### Backend Development
```bash
cd DocRedactor.API
dotnet watch run
```

### Frontend Development
```bash
cd DocRedactor.Client
npm run dev
```

### Building for Production

Backend:
```bash
cd DocRedactor.API
dotnet publish -c Release
```

Frontend:
```bash
cd DocRedactor.Client
npm run build
```

## Technologies Used

### Backend
- ASP.NET Core 9.0
- Entity Framework Core 9.0
- ASP.NET Identity
- JWT Bearer Authentication
- SQLite

### Frontend
- React 18
- Vite
- React Router DOM
- Axios
- Modern CSS3

## License

This project is open source and available for educational purposes.
