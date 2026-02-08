import { useState } from 'react';
import './TextFormatToolbar.css';

function TextFormatToolbar({ onFormatApply }) {
  const [showColorPicker, setShowColorPicker] = useState(false);

  const colors = [
    { name: 'Black', value: '#000000' },
    { name: 'Red', value: '#FF0000' },
    { name: 'Orange', value: '#FFA500' },
    { name: 'Yellow', value: '#FFFF00' },
    { name: 'Green', value: '#008000' },
    { name: 'Blue', value: '#0000FF' },
    { name: 'Purple', value: '#800080' },
    { name: 'Pink', value: '#FFC0CB' },
    { name: 'Brown', value: '#8B4513' },
    { name: 'Gray', value: '#808080' },
    { name: 'White', value: '#FFFFFF' },
    { name: 'Teal', value: '#008080' }
  ];

  const applyFormat = (command, value = null) => {
    document.execCommand(command, false, value);
    if (onFormatApply) {
      onFormatApply();
    }
  };

  const handleBold = () => {
    applyFormat('bold');
  };

  const handleItalic = () => {
    applyFormat('italic');
  };

  const handleColor = (color) => {
    applyFormat('foreColor', color);
    setShowColorPicker(false);
  };

  return (
    <div className="text-format-toolbar">
      <button
        type="button"
        className="format-btn"
        onClick={handleBold}
        title="Bold (Ctrl+B)"
      >
        <strong>B</strong>
      </button>
      <button
        type="button"
        className="format-btn"
        onClick={handleItalic}
        title="Italic (Ctrl+I)"
      >
        <em>I</em>
      </button>
      <div className="color-picker-container">
        <button
          type="button"
          className="format-btn color-btn"
          onClick={() => setShowColorPicker(!showColorPicker)}
          title="Text Color"
        >
          <span className="color-icon">A</span>
        </button>
        {showColorPicker && (
          <div className="color-picker-dropdown">
            <div className="color-grid">
              {colors.map((color) => (
                <button
                  key={color.value}
                  type="button"
                  className="color-option"
                  style={{ backgroundColor: color.value }}
                  onClick={() => handleColor(color.value)}
                  title={color.name}
                  aria-label={color.name}
                />
              ))}
            </div>
          </div>
        )}
      </div>
      <div className="toolbar-divider"></div>
      <span className="toolbar-hint">Select text to format</span>
    </div>
  );
}

export default TextFormatToolbar;
