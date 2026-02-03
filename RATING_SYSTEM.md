# 5-Star Rating System Implementation

## Overview
This document describes the implementation of a 5-star rating system for document versions in the DocRedactor application.

## Features

### 1. Main Documents Page
- Displays all documents with their current version's rating
- Shows star rating visually (★★★★★)
- Only displays rating if one has been set
- Small, compact display optimized for card view

Example:
```
┌─────────────────────────────────┐
│ Meeting Notes                   │
│                                 │
│ Discussion about Q1 goals...   │
│                                 │
│ Created: 2/3/2026 | Version: 1 │
│ ★★★★★ (5/5)                    │
│                                 │
│ [View] [Delete]                 │
└─────────────────────────────────┘
```

### 2. Document Version History Page
- Each version shows its individual rating
- Interactive stars - click to rate
- Visual feedback on hover
- Rating persists immediately

Example:
```
Version 1                     2/3/2026, 2:55:56 PM
─────────────────────────────────────────────────
Initial version

Rate this version: ★★★★★ (5/5)
                   [Click stars to rate]

[View] [Revert to This Version]
```

## API Endpoints

### Rate a Version
```http
PUT /api/documents/{documentId}/versions/{versionId}/rate
Authorization: Bearer {token}
Content-Type: application/json

{
  "rating": 5
}
```

Response:
```json
{
  "id": 1,
  "documentId": 1,
  "content": "Document content...",
  "versionNumber": 1,
  "changeDescription": "Initial version",
  "rating": 5,
  "userId": "...",
  "createdAt": "2026-02-03T14:55:56.2003512"
}
```

### Get Documents with Ratings
```http
GET /api/documents
Authorization: Bearer {token}
```

Response:
```json
[
  {
    "id": 1,
    "title": "Meeting Notes",
    "content": "...",
    "currentVersion": 1,
    "currentVersionRating": 5,
    "userId": "...",
    "createdAt": "...",
    "updatedAt": "..."
  }
]
```

## Database Schema

### DocumentVersion Table
```sql
ALTER TABLE DocumentVersions 
ADD Rating INTEGER NULL CHECK (Rating >= 1 AND Rating <= 5);
```

## Component Structure

### StarRating Component
```jsx
<StarRating 
  rating={5}           // Current rating (1-5 or null)
  onRate={(r) => ...}  // Callback when rating changes
  readOnly={false}     // Display only (no interaction)
  size="medium"        // small, medium, or large
/>
```

### Visual States
- Empty star: ☆ (grey)
- Filled star: ★ (gold with glow)
- Hover: slightly larger with smooth transition
- Interactive vs read-only modes

## Testing

### API Tests
✅ Rate version with 5 stars
✅ Rate version with 4 stars  
✅ Retrieve document with rating in list
✅ Retrieve versions with ratings
✅ Update existing rating

### Frontend Integration
✅ Display ratings on main page
✅ Display ratings in version history
✅ Interactive rating (click to rate)
✅ Visual feedback (hover, fill states)

## Security

- User authorization: Only document owners can rate versions
- Input validation: Rating must be 1-5
- Audit logging: All rating changes logged
- Per-user isolation: Users only see/rate their documents

## Usage Example

1. User views their documents on main page
2. Document shows current version rating (e.g., ★★★★☆ 4/5)
3. User clicks on document to view details
4. Version history shows individual ratings
5. User clicks stars to rate a version
6. Rating saves immediately and updates display
7. Updated rating appears on main page

## Files Modified

### Backend
- Models/DocumentVersion.cs - Added Rating property
- DTOs/DocumentDtos.cs - Added rating fields
- Services/DocumentService.cs - RateVersionAsync method
- Controllers/DocumentsController.cs - Rate endpoint
- Repositories/DocumentVersionRepository.cs - UpdateAsync
- Migration: AddRatingToDocumentVersion

### Frontend
- components/StarRating.jsx - Reusable rating component
- components/StarRating.css - Star styling
- pages/Documents.jsx - Display ratings on cards
- pages/ViewDocument.jsx - Interactive rating
- services/api.js - rateVersion API call

## Future Enhancements

Potential future improvements:
- Average rating across all versions
- Rating statistics and analytics
- Comment/review with rating
- Rating history/audit trail
- Export ratings to reports
