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
        {button: 'sideBarToggler'},
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

/* ================================ SHOW AND HIDE SIDEBAR ON CLICK ================================ */

function toggleSidebar() {
    const sidebar = document.getElementById('sideBar');
    const mainWithSideBar = document.getElementById('mainWithSideBar');
    const sideBarTogglerIcon = document.getElementById('sideBarTogglerIcon');

    if (!sidebar.classList.contains('sidebar-expanded')) return;

    const isHidden = sidebar.classList.toggle('sidebar-hidden');
    sideBarTogglerIcon.classList.toggle('show-sidebar-toggler-icon');
    mainWithSideBar.classList.toggle('main-with-sidebar-hidden');
    
    localStorage.setItem('sidebarHidden', isHidden ? 'true' : 'false');
}

// Restore state on page load
window.addEventListener('DOMContentLoaded', () => {
    const sidebar = document.getElementById('sideBar');
    const mainWithSideBar = document.getElementById('mainWithSideBar');
    const sideBarTogglerIcon = document.getElementById('sideBarTogglerIcon');

    const sidebarHidden = localStorage.getItem('sidebarHidden') === 'true';

    if (sidebarHidden && sidebar.classList.contains('sidebar-expanded')) {
        sidebar.classList.add('sidebar-hidden');
        sideBarTogglerIcon.classList.add('show-sidebar-toggler-icon');
        mainWithSideBar.classList.add('main-with-sidebar-hidden');
    }
});


/* ================================ DATE AND TIME ================================ */

function updateDateTime() {
    const now = new Date();
    
    const date = now.toLocaleDateString("pl-PL", {
        weekday: "long",
        year: "numeric",
        month: "long",
        day: "numeric"
    });

    const time = now.toLocaleTimeString("pl-PL", {
        hour: "2-digit",
        minute: "2-digit"
    });

    document.getElementById("dateTime").innerHTML = `${time}<br/>${date}`;
}

updateDateTime();
setInterval(updateDateTime, 30000);