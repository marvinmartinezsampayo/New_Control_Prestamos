//Variables 
let modalPago = new bootstrap.Modal(document.getElementById('modalPagarPrestamo'));
let valInteresPago = document.getElementById("valInteresPago");
let valCapitalPago = document.getElementById("valCapitalPago");

//Funciones
function pagarPrestamo(idPrestamo, monto, interes, saldo, num_cuotas) {

    // Limpiar formulario
    document.getElementById('formPagarPrestamo').reset();

    // Establecer valores
    document.getElementById('idPrestamo').value = idPrestamo;

    let inter = Math.round(saldo * (interes / 100));
    let cap = Math.round(monto / num_cuotas);

    //Calculado el valor de interes y el capital
    valInteresPago.innerHTML = inter.toLocaleString('es-CO');
    valCapitalPago.innerHTML = cap.toLocaleString('es-CO');


    //// Generar opciones de cuotas
    //const selectCuotas = document.getElementById('numeroCuota');
    //selectCuotas.innerHTML = '<option value="">Seleccione la cuota</option>';

    //for (let i = 1; i <= nroCuota; i++) {
    //    const option = document.createElement('option');
    //    option.value = i;
    //    option.textContent = `Cuota ${i} de ${nroCuota}`;

    //    // Bloquear  id = valInteresPagocuotas ya pagadas
    //    if (i <= nroCuotaPagas) {
    //        option.disabled = true;
    //        option.textContent = `Cuota ${i} de ${nroCuota} (PAGADA)`;
    //        option.style.color = '#6c757d'; // Color gris para indicar que está pagada
    //        option.style.fontStyle = 'italic';
    //    }

    //    selectCuotas.appendChild(option);
    //}


    // Establecer fecha actual
    const ahora = new Date();
    const fechaLocal = new Date(ahora.getTime() - ahora.getTimezoneOffset() * 60000);
    document.getElementById('fechaPago').value = fechaLocal.toISOString().slice(0, 16);


    modalPago.show(); 
}


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

    // Manejar envío del formulario
    document.getElementById('formPagarPrestamo').addEventListener('submit', function (e) {
        e.preventDefault();

        const formData = new FormData(this);

        fetch('@Url.Action("RegistrarPago", "Prestamos")', {
            method: 'POST',
            body: formData
        })
        .then(response => response.json())
        .then(result => {
            if (result.success) {
                modalPagarPrestamo.hide();
                alert('Pago registrado exitosamente');
                // Recargar la página o actualizar la tabla
                location.reload();
            } else {
                alert('Error: ' + result.message);
            }
        })
        .catch(error => {
            alert('Error al procesar el pago');
            console.error(error);
        });
    });

});




