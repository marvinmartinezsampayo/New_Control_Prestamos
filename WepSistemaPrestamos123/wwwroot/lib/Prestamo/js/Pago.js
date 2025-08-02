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
        

    // Establecer fecha actual
    const ahora = new Date();
    const fechaLocal = new Date(ahora.getTime() - ahora.getTimezoneOffset() * 60000);
    document.getElementById('fechaPago').value = fechaLocal.toISOString().slice(0, 16);


    modalPago.show(); 
}


function formatearNumero(numero) {
    // Convertir a string si no lo es
    const numeroStr = numero.toString();

    // Separar parte entera y decimal (si existe)
    const partes = numeroStr.split('.');
    const parteEntera = partes[0];
    const parteDecimal = partes[1];

    // Agregar puntos cada 3 dígitos desde la derecha
    const numeroFormateado = parteEntera.replace(/\B(?=(\d{3})+(?!\d))/g, '.');

    // Retornar con decimales si existen
    return parteDecimal ? numeroFormateado + ',' + parteDecimal : numeroFormateado;
}

// IMPORTANTE: Cambiar el input HTML a:
// <input type="text" id="monto" inputmode="numeric">

document.getElementById('monto').addEventListener('input', function (e) {
    const input = e.target;
    let valor = input.value;

    // Guardar posición del cursor
    const cursorPos = input.selectionStart;

    // Remover todos los caracteres no numéricos
    const numeroLimpio = valor.replace(/[^\d]/g, '');

    let resultado;
    let nuevaCursorPos = cursorPos;

    if (numeroLimpio.length > 3 && numeroLimpio !== '') {
        resultado = formatearNumero1(Number(numeroLimpio));

        // Calcular nueva posición del cursor
        const caracteresOriginales = valor.length;
        const caracteresNuevos = resultado.length;
        const diferencia = caracteresNuevos - caracteresOriginales;
        nuevaCursorPos = Math.max(0, cursorPos + diferencia);

    } else if (numeroLimpio !== '') {
        resultado = numeroLimpio;
        nuevaCursorPos = Math.min(cursorPos, resultado.length);
    } else {
        resultado = '';
        nuevaCursorPos = 0;
    }

    // Actualizar valor
    input.value = resultado;

    // Restaurar posición del cursor
    requestAnimationFrame(() => {
        const posicionFinal = Math.max(0, Math.min(nuevaCursorPos, resultado.length));
        input.setSelectionRange(posicionFinal, posicionFinal);
    });
});

function formatearNumero1(numero) {
    return numero.toLocaleString('es-CO');
}

// Función auxiliar para obtener el valor numérico real
function obtenerValorNumerico(inputId) {
    const valor = document.getElementById(inputId).value;
    return Number(valor.replace(/[^\d]/g, ''));
}

// Función para establecer valor inicial
function establecerValor(inputId, valor) {
    const input = document.getElementById(inputId);
    const numeroLimpio = String(valor).replace(/[^\d]/g, '');

    if (numeroLimpio.length > 3) {
        input.value = formatearNumero1(Number(numeroLimpio));
    } else {
        input.value = numeroLimpio;
    }
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
               
        fetch('/Prestamo/Gestion/RegistrarPago', {
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
            //console.error(error);
        });
    });

});




