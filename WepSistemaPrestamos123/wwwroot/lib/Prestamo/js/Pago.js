document.addEventListener('DOMContentLoaded', function () {
    const inputIdentificacion = document.getElementById('nroIdentificacion');
    const form = inputIdentificacion?.closest('form');

    if (inputIdentificacion) {
        // Solo permitir números
        inputIdentificacion.addEventListener('input', function (e) {
            this.value = this.value.replace(/[^0-9]/g, '');
        });

        // Enviar formulario con Enter
        inputIdentificacion.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault(); // Evita doble envío
                if (form) form.submit();
            }
        });
    }

    // Mostrar mensaje SweetAlert si viene TempData
    const alertData = document.getElementById("sweetAlertData");
    if (alertData && alertData.dataset.icon && alertData.dataset.icon !== "") {
        const icon = alertData.dataset.icon;
        const titulo = alertData.dataset.titulo || (icon === 'success' ? "Operación Exitosa" : "Error");
        const mensaje = alertData.dataset.mensaje;
        const limpiar = alertData.dataset.limpiar === "true";

        Swal.fire({
            icon: icon,
            title: titulo,
            html: mensaje,
            confirmButtonText: 'Aceptar',
            confirmButtonColor: icon === 'success' ? '#28a745' : '#dc3545',
            allowOutsideClick: false,
            allowEscapeKey: false,
            customClass: {
                popup: 'animated bounceIn',
                confirmButton: 'btn btn-success' // Puedes cambiar a otro si quieres más coherencia visual
            },
            buttonsStyling: false
        }).then((result) => {
            if (limpiar && result.isConfirmed) {
                inputIdentificacion.value = '';
            }
        });
    }
    window.addEventListener('beforeunload', function () {
        if (inputIdentificacion) inputIdentificacion.value = '';
    });
});
