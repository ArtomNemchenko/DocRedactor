# Split-View Document Editing Documentation

## Overview

DocRedactor now supports split-view document editing, allowing users to create new documents while referencing existing ones side-by-side. Additionally, users can copy document content with or without styling.

## Features

### 1. Split-View Document Editor

Open a new document creation form while viewing an existing document. Perfect for:
- Creating new documents based on existing content
- Comparing documents side-by-side
- Referencing version history while writing
- Maintaining context from previous documents

### 2. Copy Content

Copy document content in two ways:
- **With Styling**: Preserves all formatting (HTML)
- **Without Styling**: Plain text only

## How to Use

### Opening Split View

1. Navigate to any document
2. Click the **"Open New Document"** button in the header
3. The screen splits into two panels:
   - **Left Panel**: Original document (read-only) with full version history
   - **Right Panel**: New document creation form

### Creating a Document in Split View

1. In the right panel, enter a title for your new document
2. Use the formatting toolbar to style your content
3. Write your document while referencing the left panel
4. Click **"Create Document"** to save
5. You'll be redirected to the newly created document

### Copying Content

1. Open any document
2. Click the **"Copy Content"** dropdown button
3. Select one of two options:
   - **Copy with Styling**: Copies formatted HTML content
   - **Copy without Styling**: Copies plain text only
4. A success notification appears: "✓ Content copied to clipboard!"
5. Paste the content wherever needed

### Navigation

- **From single view to split view**: Click "Open New Document"
- **From split view to single view**: Click "Back to Single View"
- **Cancel in split view**: Click "Cancel" to return without creating

## Technical Details

### Split View Layout

```
┌────────────────────────┬────────────────────────┐
│   Original Document    │    New Document        │
│   (Read-only)          │    (Editable)          │
│                        │                        │
│   - Document Title     │    - Title Input       │
│   - Content Display    │    - Content Editor    │
│   - Document Info      │    - Format Toolbar    │
│   - Version History    │    - Create Button     │
│   - Version Ratings    │    - Cancel Button     │
└────────────────────────┴────────────────────────┘
```

### Copy Functionality

**Copy with Styling:**
- Copies the raw HTML content
- Preserves bold, italic, underline, colors, lists
- Suitable for pasting into rich text editors
- Example: `<b>Bold</b> <i>Italic</i>`

**Copy without Styling:**
- Extracts plain text from HTML
- Removes all formatting tags
- Suitable for plain text fields
- Example: `Bold Italic`

### Routes

- Single view: `/documents/:id`
- Split view: `/documents/:id/split-view`

## Browser Support

### Clipboard API
- Chrome 63+
- Firefox 53+
- Safari 13.1+
- Edge 79+

### Split View Layout
- All modern browsers with flexbox support
- Responsive design for mobile/tablet

## Responsive Design

### Desktop (>968px)
- 50/50 split side-by-side
- Independent scrolling in each panel

### Mobile/Tablet (≤968px)
- Panels stack vertically
- 50vh height for each panel
- Full width for better readability

## Use Cases

### Use Case 1: Document Templating
1. Open a template document
2. Enter split view
3. Reference the template while creating new content
4. Save the new document

### Use Case 2: Version Improvement
1. Open an old document
2. Review version history in split view
3. Create improved version based on feedback
4. Save as new document

### Use Case 3: Content Extraction
1. Open formatted document
2. Click "Copy without Styling"
3. Paste plain text into email, chat, etc.

### Use Case 4: Content Reuse
1. Open source document
2. Click "Copy with Styling"
3. Paste into new document editor
4. Modify and save

## Keyboard Shortcuts

### In Split View Right Panel
- **Ctrl+B / Cmd+B**: Bold
- **Ctrl+I / Cmd+I**: Italic
- **Ctrl+U / Cmd+U**: Underline

### Copy Operations
- Manual selection only (use dropdown buttons)

## Security Considerations

### Clipboard Access
- Uses modern Clipboard API
- Requires user interaction (button click)
- No automatic clipboard access

### Content Safety
- HTML content is user-generated
- Per-user authorization enforced
- No XSS risk (users only access their own content)

## Best Practices

1. **Use Split View For**:
   - Creating documents based on templates
   - Referencing multiple document versions
   - Side-by-side comparison
   - Learning from past documents

2. **Use Copy With Styling When**:
   - Moving content between documents
   - Pasting into rich text editors
   - Preserving formatting is important

3. **Use Copy Without Styling When**:
   - Pasting into plain text fields
   - Removing all formatting
   - Extracting raw content

4. **Performance Tips**:
   - Large documents may take longer to load in split view
   - Consider breaking very long documents into sections
   - Use version history for incremental changes

## Troubleshooting

### Copy Not Working
- Ensure browser supports Clipboard API
- Check browser permissions for clipboard access
- Try using keyboard shortcuts (Ctrl+C) as fallback

### Split View Not Loading
- Check internet connection
- Refresh the page
- Verify document exists and you have access

### Layout Issues on Mobile
- Rotate device for better view
- Use landscape mode for side-by-side
- Consider using single view on small screens

## Future Enhancements

Potential improvements being considered:
- Adjustable split panel sizes (drag divider)
- Multiple document comparison (3+ panels)
- Diff view between documents
- Export split view as PDF
- Sync scrolling option

## Feedback

Found a bug or have suggestions? Please report issues through the repository's issue tracker.

## Version History

- **v1.0** (2026-02-08): Initial release of split-view and copy features
