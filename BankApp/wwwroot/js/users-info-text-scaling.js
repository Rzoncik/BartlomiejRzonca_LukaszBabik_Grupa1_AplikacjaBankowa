/* ================================ USERS INFO TEXT SCALING ================================ */

function fitTextToContainer(containerSelector, textSelector) {
    const container = document.querySelector(containerSelector);
    const text = document.querySelector(textSelector);

    function scaleText() {
        if (!container || !text) return;
        
        text.style.transform = 'scale(1)';
        text.style.transformOrigin = 'left top';

        const containerWidth = container.offsetWidth;
        const containerHeight = container.offsetHeight;
        const textWidth = text.offsetWidth;
        const textHeight = text.offsetHeight;
        
        const scaleX = containerWidth / textWidth;
        const scaleY = containerHeight / textHeight;
        
        const scale = Math.min(scaleX, scaleY);

        text.style.transform = `scale(${scale})`;
    }
    
    window.addEventListener('load', scaleText);
    
    window.addEventListener('resize', scaleText);
    
    if (document.fonts) {
        document.fonts.ready.then(scaleText);
    }
}

fitTextToContainer('.sidebar-user-info', '.user-name');
fitTextToContainer('.sidebar-user-info', '.balance');