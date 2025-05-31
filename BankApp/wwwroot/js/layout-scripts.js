/* ================================ BUTTON BEHAVIOUR ON HOVER AND ON CLICK ================================ */

document.addEventListener('DOMContentLoaded', () => {
    const services = [
        {button: 'navButton1'},
        {button: 'navButton2'},
        {button: 'navButton3'},
    ];

    services.forEach(({button}) => {
        const cardEl = document.getElementById(button);

        // if (!cardEl) return;

        /* ================================ HOVER ================================ */
        cardEl.addEventListener('mouseenter', () => {
            cardEl.classList.add('nav-button-enter');
        });

        cardEl.addEventListener('mouseleave', () => {
            cardEl.classList.remove('nav-button-enter', 'nav-button-down');
        });
        
        /* ================================ CLICK ================================ */
        const pressStart = () => {
            cardEl.classList.remove('nav-button-enter');
            cardEl.classList.add('nav-button-down');
        };

        const pressEnd = () => {
            cardEl.classList.remove('nav-button-down');      // release press
        };

        cardEl.addEventListener('mousedown', pressStart);
        cardEl.addEventListener('mouseup', pressEnd);

        /* Also support touch */
        cardEl.addEventListener('touchstart', pressStart, {passive: true});
        cardEl.addEventListener('touchend', pressEnd);
    });
});