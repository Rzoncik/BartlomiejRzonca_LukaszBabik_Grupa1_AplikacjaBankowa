/* ================================ BUTTON BEHAVIOUR ON HOVER AND ON CLICK ================================ */

document.addEventListener('DOMContentLoaded', () => {
    const services = [
        {button: 'sideBarButton1'},
        {button: 'sideBarButton2'},
        {button: 'sideBarButton3'},
        {button: 'sideBarButton4'},
        {button: 'sideBarButton5'},
        {button: 'sideBarButton6'},
        {button: 'sideBarButton7'},
        {button: 'sideBarButton8'},
    ];

    services.forEach(({button}) => {
        const Button = document.getElementById(button);

        // if (!Button) return;

        /* ================================ HOVER ================================ */
        Button.addEventListener('mouseenter', () => {
            Button.classList.add('sidebar-button-enter');
        });

        Button.addEventListener('mouseleave', () => {
            Button.classList.remove('sidebar-button-enter', 'sidebar-button-down');
        });

        /* ================================ CLICK ================================ */
        const pressStart = () => {
            Button.classList.remove('sidebar-button-enter');
            Button.classList.add('sidebar-button-down');
        };

        const pressEnd = () => {
            Button.classList.remove('sidebar-button-down');      // release press
        };

        Button.addEventListener('mousedown', pressStart);
        Button.addEventListener('mouseup', pressEnd);

        /* Also support touch */
        Button.addEventListener('touchstart', pressStart, {passive: true});
        Button.addEventListener('touchend', pressEnd);
    });
});