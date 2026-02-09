# Database Migration Guide

## Overview
This guide explains how to work with Entity Framework Core migrations in this project to avoid database schema issues.

## Problem That Was Fixed
The application was throwing this error:
```
SQLite Error 1: 'table DocumentVersions has no column named Rating'
```

This occurred because the migration file was created but never applied to the database.

## Solution Applied

### 1. Delete Old Database
```bash
cd DocRedactor.API
rm -f docredactor.db docredactor.db-shm docredactor.db-wal
```

### 2. Apply Migrations
```bash
dotnet ef database update
```

### 3. Verify Schema
```bash
sqlite3 docredactor.db "PRAGMA table_info(DocumentVersions);"
```

Expected output should include:
```
5|Rating|INTEGER|0||0
```

## For Future Development

### When Adding New Models or Properties

1. **Create Migration**
   ```bash
   cd DocRedactor.API
   dotnet ef migrations add YourMigrationName
   ```

2. **Review Migration File**
   - Check `Migrations/` folder
   - Verify the Up() method has correct changes
   - Verify the Down() method can reverse changes

3. **Apply Migration**
   ```bash
   dotnet ef database update
   ```

4. **Verify Changes**
   ```bash
   sqlite3 docredactor.db "PRAGMA table_info(TableName);"
   ```

### When Pulling Changes from Git

If someone else added a migration:

```bash
cd DocRedactor.API
dotnet ef database update
```

This ensures your local database schema matches the code.

### Common Commands

```bash
# List all migrations
dotnet ef migrations list

# Check if database exists and is up to date
dotnet ef database update --dry-run

# Remove last migration (if not yet applied)
dotnet ef migrations remove

# Drop database (WARNING: destroys all data)
dotnet ef database drop --force
```

## Testing After Migration

1. **Build Application**
   ```bash
   cd DocRedactor.API
   dotnet build
   ```

2. **Run Application**
   ```bash
   dotnet run
   ```

3. **Test API Endpoints**
   ```bash
   # Login
   curl -X POST http://localhost:5180/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"email":"alice@example.com","password":"Alice123"}'
   
   # Get Documents (use token from login response)
   curl http://localhost:5180/api/documents \
     -H "Authorization: Bearer YOUR_TOKEN_HERE"
   ```

## Database Files

These files are gitignored (don't commit them):
- `*.db` - SQLite database file
- `*.db-shm` - Shared memory file
- `*.db-wal` - Write-Ahead Log file

## Seeding Test Data

The application automatically seeds test data on startup:
- **Users**: alice, bob
- **Documents**: Meeting Notes, Project Proposal, Technical Specification

See `Program.cs` for seeding logic.

## Troubleshooting

### "Column not found" errors
- Run `dotnet ef database update`
- If persists, drop and recreate database

### "Migration already applied" error
- Check `dotnet ef migrations list`
- Database is probably up to date

### Build errors after migration
- Run `dotnet restore`
- Run `dotnet clean`
- Run `dotnet build`

## Notes

- Always test migrations in development first
- Keep migration files in source control
- Never edit applied migrations
- Use meaningful migration names (e.g., `AddRatingColumn` not `Migration1`)
