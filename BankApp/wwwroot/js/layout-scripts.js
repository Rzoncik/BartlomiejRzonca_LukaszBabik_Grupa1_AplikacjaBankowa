/* ================================ BUTTON BEHAVIOUR ON HOVER AND ON CLICK ================================ */

document.addEventListener('DOMContentLoaded', () => {
    const services = [
        {button: 'navButton1'},
        {button: 'navButton2'},
        {button: 'navButton3'},
    ];

    services.forEach(({button}) => {
        const Button = document.getElementById(button);

        /* ================================ HOVER ================================ */
        Button.addEventListener('mouseenter', () => {
            Button.classList.add('nav-button-enter');
        });

        Button.addEventListener('mouseleave', () => {
            Button.classList.remove('nav-button-enter', 'nav-button-down');
        });
        
        /* ================================ CLICK ================================ */
        const pressStart = () => {
            Button.classList.remove('nav-button-enter');
            Button.classList.add('nav-button-down');
        };

        const pressEnd = () => {
            Button.classList.remove('nav-button-down');      // release press
        };

        Button.addEventListener('mousedown', pressStart);
        Button.addEventListener('mouseup', pressEnd);

        /* Also support touch */
        Button.addEventListener('touchstart', pressStart, {passive: true});
        Button.addEventListener('touchend', pressEnd);
    });
});