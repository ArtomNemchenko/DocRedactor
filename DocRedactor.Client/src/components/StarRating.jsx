import { useState } from 'react';
import './StarRating.css';

function StarRating({ rating, onRate, readOnly = false, size = 'medium' }) {
  const [hoverRating, setHoverRating] = useState(0);

  const handleClick = (value) => {
    if (!readOnly && onRate) {
      onRate(value);
    }
  };

  const handleMouseEnter = (value) => {
    if (!readOnly) {
      setHoverRating(value);
    }
  };

  const handleMouseLeave = () => {
    if (!readOnly) {
      setHoverRating(0);
    }
  };

  const displayRating = hoverRating || rating || 0;

  return (
    <div className={`star-rating ${size} ${readOnly ? 'read-only' : 'interactive'}`}>
      {[1, 2, 3, 4, 5].map((value) => (
        <span
          key={value}
          className={`star ${value <= displayRating ? 'filled' : 'empty'}`}
          onClick={() => handleClick(value)}
          onMouseEnter={() => handleMouseEnter(value)}
          onMouseLeave={handleMouseLeave}
          role={readOnly ? 'img' : 'button'}
          aria-label={`${value} star${value !== 1 ? 's' : ''}`}
        >
          ★
        </span>
      ))}
      {rating > 0 && (
        <span className="rating-value">({rating}/5)</span>
      )}
    </div>
  );
}

export default StarRating;
