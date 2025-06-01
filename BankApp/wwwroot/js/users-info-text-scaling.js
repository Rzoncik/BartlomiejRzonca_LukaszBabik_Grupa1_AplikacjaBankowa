/* ================================ USERS INFO TEXT SCALING ================================ */

function fitTextToWidth(containerSelector, textSelector) {
    const container = document.querySelector(containerSelector);
    const text = document.querySelector(textSelector);

    function scaleText() {
        if (!container || !text) return;

        text.style.transform = 'scale(1)'; // Reset scaling
        const scale = container.offsetWidth / text.offsetWidth;
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

fitTextToWidth('.sidebar-user-info', '.user-name');
fitTextToWidth('.sidebar-user-info', '.balance');