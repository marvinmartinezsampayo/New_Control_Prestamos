/*!
    * Start Bootstrap - SB Admin v7.0.7 (https://startbootstrap.com/template/sb-admin)
    * Copyright 2013-2023 Start Bootstrap
    * Licensed under MIT (https://github.com/StartBootstrap/startbootstrap-sb-admin/blob/master/LICENSE)
    */
// 
// Scripts
// 

window.addEventListener('DOMContentLoaded', () => {

    initSidebarToggle();
    initPasswordToggle('togglePassword', 'Clave');

});

function initSidebarToggle() {
    const sidebarToggle = document.body.querySelector('#sidebarToggle');
    if (!sidebarToggle) return;

    sidebarToggle.addEventListener('click', event => {
        event.preventDefault();
        document.body.classList.toggle('sb-sidenav-toggled');
        localStorage.setItem(
            'sb|sidebar-toggle',
            document.body.classList.contains('sb-sidenav-toggled')
        );
    });
}

function initPasswordToggle(toggleId, inputId) {
    const toggle = document.getElementById(toggleId);
    const input = document.getElementById(inputId);
    if (!toggle || !input) return;

    const showPassword = () => {
        input.type = 'text';
        toggle.classList.remove('bi-eye-slash-fill');
        toggle.classList.add('bi-eye-fill');
    };

    const hidePassword = () => {
        input.type = 'password';
        toggle.classList.remove('bi-eye-fill');
        toggle.classList.add('bi-eye-slash-fill');
    };

    // Mouse (PC)
    toggle.addEventListener('mousedown', showPassword);
    toggle.addEventListener('mouseup', hidePassword);
    toggle.addEventListener('mouseleave', hidePassword);

    // Touch (móvil)
    toggle.addEventListener('touchstart', showPassword);
    toggle.addEventListener('touchend', hidePassword);
    toggle.addEventListener('touchcancel', hidePassword);
}

