# DocRedactor

A full-stack document redaction application built with ASP.NET Core Web API and React.

## Features

- **User Authentication**: Secure registration and login using JWT tokens
- **Document Management**: Create, view, and delete documents
- **Document Redaction**: Apply redactions to sensitive content with position tracking
- **Per-User Authorization**: Users can only access their own documents and redactions
- **Audit Logging**: All operations are logged for security and compliance
- **Immutable Documents**: Document content is preserved for auditability
- **Modern UI**: Responsive React frontend with beautiful gradients

## Architecture

### Backend (ASP.NET Core Web API)

- **Framework**: .NET 9.0
- **Database**: SQL Server LocalDB with Entity Framework Core
- **Authentication**: ASP.NET Identity with JWT Bearer tokens
- **Security**: Password hashing, token-based auth, per-user data isolation
- **API Structure**:
  - `AuthController`: User registration and login
  - `DocumentsController`: CRUD operations for documents
  - `RedactionsController`: CRUD operations for redactions

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
- SQL Server LocalDB (included with Visual Studio)

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

The API will start at `http://localhost:5000`

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

### 1. Register a New Account
- Navigate to the registration page
- Enter email, username, and password (minimum 6 characters)
- Password must contain uppercase, lowercase, and digit

### 2. Create a Document
- After logging in, click "Create New Document"
- Enter a title and content
- Click "Create Document"

### 3. Apply Redactions
- Open a document
- Select text in the "Original Content" section
- Enter an optional reason for the redaction
- Click "Create Redaction"
- View the redacted content in the "Redacted Content" section

### 4. Manage Documents and Redactions
- View all your documents on the main page
- Delete documents or individual redactions as needed
- All changes are tracked and logged

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and receive JWT token

### Documents
- `GET /api/documents` - Get all documents for current user
- `GET /api/documents/{id}` - Get specific document
- `POST /api/documents` - Create new document
- `DELETE /api/documents/{id}` - Delete document

### Redactions
- `GET /api/redactions/document/{documentId}` - Get redactions for a document
- `POST /api/redactions` - Create new redaction
- `DELETE /api/redactions/{id}` - Delete redaction

## Security Features

1. **JWT Authentication**: Stateless token-based authentication
2. **Password Requirements**: Strong password policy enforced
3. **Per-User Authorization**: Users can only access their own data
4. **Audit Logging**: All operations logged with user ID and timestamp
5. **Input Validation**: Request validation on both client and server
6. **CORS Configuration**: Restricted to known frontend origins
7. **Immutable Content**: Documents cannot be modified after creation

## Database Schema

### Users (AspNetUsers)
- Standard ASP.NET Identity user table

### Documents
- `Id`: Primary key
- `Title`: Document title (max 200 chars)
- `Content`: Document text content (immutable)
- `UserId`: Foreign key to user
- `CreatedAt`: Timestamp

### Redactions
- `Id`: Primary key
- `DocumentId`: Foreign key to document
- `UserId`: Foreign key to user
- `StartPosition`: Start index of redacted text
- `EndPosition`: End index of redacted text
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
- SQL Server

### Frontend
- React 18
- Vite
- React Router DOM
- Axios
- Modern CSS3

## License

This project is open source and available for educational purposes.
