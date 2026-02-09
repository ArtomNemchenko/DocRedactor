# Text Formatting Guide

## Overview

DocRedactor now supports rich text formatting, allowing users to format portions of their document text with bold, italic, and color styling.

## Features

### 1. Bold Text
- **Keyboard Shortcut**: Ctrl+B (Cmd+B on Mac)
- **Button**: Click the **B** button in the formatting toolbar
- **Usage**: Select the text you want to make bold, then click the B button or use the keyboard shortcut

### 2. Italic Text
- **Keyboard Shortcut**: Ctrl+I (Cmd+I on Mac)
- **Button**: Click the *I* button in the formatting toolbar
- **Usage**: Select the text you want to italicize, then click the I button or use the keyboard shortcut

### 3. Text Color
- **Button**: Click the **A** button (with red underline) in the formatting toolbar
- **Color Options**: 12 popular colors available:
  - Black (#000000)
  - Red (#FF0000)
  - Orange (#FFA500)
  - Yellow (#FFFF00)
  - Green (#008000)
  - Blue (#0000FF)
  - Purple (#800080)
  - Pink (#FFC0CB)
  - Brown (#8B4513)
  - Gray (#808080)
  - White (#FFFFFF)
  - Teal (#008080)
- **Usage**: Select the text you want to color, click the A button, then choose a color from the picker

## How to Use

### Creating a New Document with Formatting

1. Navigate to **Create New Document**
2. Enter your document title
3. Click in the content area
4. Type your text
5. Select the text you want to format
6. Click the appropriate formatting button:
   - **B** for bold
   - *I* for italic
   - **A** for color (then select from the color picker)
7. Click **Create Document** to save

### Editing an Existing Document with Formatting

1. Open a document from your documents list
2. Click **Edit Document**
3. The formatting toolbar will appear above the content area
4. Select the text you want to format
5. Apply formatting using the toolbar buttons
6. Add a change description (optional)
7. Click **Save Changes**

## Examples

### Bold Text
```
This is **bold text**.
```
Result: This is **bold text**.

### Italic Text
```
This is *italic text*.
```
Result: This is *italic text*.

### Colored Text
Select text and choose red from the color picker:
```
This is red text.
```
Result: This is <span style="color: #FF0000;">red text</span>.

### Combined Formatting
You can combine multiple formatting options:
```
This is **bold and red** text.
This is *italic and green* text.
```

## Tips

1. **Select First, Format Second**: Always select the text you want to format before clicking the formatting button
2. **Mix and Match**: You can apply multiple formats to the same text (e.g., bold + color)
3. **Clear Formatting**: To remove formatting, you'll need to select the formatted text and rewrite it
4. **Partial Formatting**: You can format individual words or even single characters within a sentence
5. **Version History**: All formatting is preserved in version history

## Technical Details

- Formatting is stored as HTML in the database
- The content editor uses `contentEditable` div for rich text support
- Formatting commands use `document.execCommand()` API
- All formatting is preserved across document versions
- Formatted content is displayed correctly in:
  - Document view mode
  - Edit mode
  - Version history

## Accessibility

- All toolbar buttons have proper labels and titles
- Keyboard shortcuts are available for common formatting (bold, italic)
- Color picker swatches are labeled with color names
- Focus states are clearly visible for all interactive elements

## Browser Compatibility

The text formatting features work in all modern browsers:
- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Opera (latest)

## Limitations

1. The color picker provides 12 preset colors (custom colors not available)
2. No undo/redo functionality within the editor (refresh to reset)
3. Pasting formatted text from external sources may include unwanted formatting

## Future Enhancements

Potential future improvements:
- Underline text
- Text highlighting/background color
- Font size controls
- Font family selection
- Lists (ordered/unordered)
- Alignment (left/center/right)
- Undo/redo within editor
- Clear formatting button
