/* ================================ USERS INFO TEXT SCALING ================================ */

/* ================================ USERS INFO TEXT SCALING ================================ */

function fitTextToContainer(containerSelector, textSelector) {
    const container = document.querySelector(containerSelector);
    const text = document.querySelector(textSelector);

    function scaleText() {
        if (!container || !text) return;

        // Reset scale to get natural size
        text.style.transform = 'scale(1)';
        text.style.transformOrigin = 'left top'; // Ensures it scales from the top-left

        const containerWidth = container.offsetWidth;
        const containerHeight = container.offsetHeight;
        const textWidth = text.offsetWidth;
        const textHeight = text.offsetHeight;

        // Calculate scale factors for width and height
        const scaleX = containerWidth / textWidth;
        const scaleY = containerHeight / textHeight;

        // Use the smaller of the two to ensure it fits both dimensions
        const scale = Math.min(scaleX, scaleY);

        text.style.transform = `scale(${scale})`;
    }

    // Scale on load
    window.addEventListener('load', scaleText);

    // Scale on resize
    window.addEventListener('resize', scaleText);

    // Optional: ensure font is loaded
    if (document.fonts) {
        document.fonts.ready.then(scaleText);
    }
}

// Example usage
fitTextToContainer('.sidebar-user-info', '.user-name');
fitTextToContainer('.sidebar-user-info', '.balance');