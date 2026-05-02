// -------------------------------------------------------
// DROPDOWN FIX
// -------------------------------------------------------

// 👇 SOLO este cambio: 'shown' en vez de 'show'
document.addEventListener('shown.bs.dropdown', function (e) {
    const btn = e.target;
    const menu = btn.nextElementSibling;
    if (!menu) return;

    menu._triggerBtn = btn;
    document.body.appendChild(menu);
    menu.style.display = 'block';

    // 👇 Sin setTimeout — shown ya garantiza que el menú está listo
    const rect = btn.getBoundingClientRect();
    const menuW = menu.offsetWidth || 160;
    const menuH = menu.offsetHeight || 120;
    const scrollTop = window.scrollY || document.documentElement.scrollTop;

    let left = rect.right - menuW;
    if (left < 8) left = 8;
    if (left + menuW > window.innerWidth - 8) left = window.innerWidth - menuW - 8;

    let top = rect.bottom + scrollTop + 4;
    if (rect.bottom + menuH > window.innerHeight) {
        top = rect.top + scrollTop - menuH - 4;
    }

    menu.style.position = 'absolute';
    menu.style.top = top + 'px';
    menu.style.left = left + 'px';
    menu.style.zIndex = '9999';
    menu.style.margin = '0';
});

// El hide y el click listener quedan exactamente igual que los tienes
document.addEventListener('hide.bs.dropdown', function (e) {
    const btn = e.target;
    const dropdown = btn.closest('.dropdown');
    const menu = document.body.querySelector('ul.dropdown-menu[style]');

    if (menu && dropdown) {
        dropdown.appendChild(menu);
        menu.style.cssText = '';
    }
});

document.addEventListener('click', function (e) {
    const menu = document.body.querySelector('ul.dropdown-menu[style]');
    if (!menu) return;

    const btn = menu._triggerBtn;
    if (menu.contains(e.target) || (btn && btn.contains(e.target))) return;

    if (btn) {
        const bsDropdown = bootstrap.Dropdown.getInstance(btn);
        if (bsDropdown) bsDropdown.hide();
    }
}, true);

// -------------------------------------------------------
// ********************  FUNCIONES ***********************
// -------------------------------------------------------
function getFechaColombia() {
    const now = new Date();
    const colombia = new Date(now.toLocaleString('en-US', { timeZone: 'America/Bogota' }));
    const yyyy = colombia.getFullYear();
    const MM = String(colombia.getMonth() + 1).padStart(2, '0');
    const dd = String(colombia.getDate()).padStart(2, '0');
    const hh = String(colombia.getHours()).padStart(2, '0');
    const mm = String(colombia.getMinutes()).padStart(2, '0');
    return `${yyyy}-${MM}-${dd}T${hh}:${mm}`;
}

function formatCOP(value) {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        maximumFractionDigits: 0
    }).format(value);
}

function AgregarPago(idPrestamo, interesMes, retornoMes) {
    document.getElementById('pago_ID_PRESTAMO').value = idPrestamo;
    document.getElementById('pago_INTERES_MES').value = interesMes;
    document.getElementById('pago_FECHA_PAGO').value = getFechaColombia();
    document.getElementById('pago_MONTO').value = '';
    document.getElementById('pago_MONTO').min = interesMes;
    document.getElementById('pago_PAGO_INTERESES').checked = false;
    document.getElementById('pago_alerta').className = 'alert d-none';
    document.getElementById('pago_monto_feedback').classList.add('d-none');
    document.getElementById('btnGuardarPago').disabled = true;

    const infoBox = document.getElementById('pago_info_interes');
    infoBox.classList.remove('d-none');
    document.getElementById('pago_label_interes').textContent = formatCOP(interesMes);
    document.getElementById('pago_label_minimo').textContent = formatCOP(interesMes + retornoMes);

    new bootstrap.Modal(document.getElementById('modalAgregarPago')).show();
}

function validarMonto() {
    const interesMes = parseFloat(document.getElementById('pago_INTERES_MES').value) || 0;
    const monto = parseFloat(document.getElementById('pago_MONTO').value) || 0;
    const feedback = document.getElementById('pago_monto_feedback');
    const feedbackTxt = document.getElementById('pago_monto_feedback_txt');
    const btnGuardar = document.getElementById('btnGuardarPago');

    if (monto <= 0) {
        feedbackTxt.textContent = 'El monto debe ser mayor a cero.';
        feedback.classList.remove('d-none');
        btnGuardar.disabled = true;
        return;
    }

    if (monto < interesMes) {
        feedbackTxt.textContent = `El monto mínimo requerido es ${formatCOP(interesMes)}. No se permite un valor inferior a los intereses del mes.`;
        feedback.classList.remove('d-none');
        btnGuardar.disabled = true;
        return;
    }

    feedback.classList.add('d-none');
    btnGuardar.disabled = false;
}

function GenerarMulta(idPrestamo) {
    document.getElementById('multa_IdPrestamo').value = idPrestamo;
    document.getElementById('multa_ValorMulta').value = '';
    document.getElementById('multa_DescripcionMotivo').value = '';
    document.getElementById('multa_IdMotivo').value = '';

    new bootstrap.Modal(document.getElementById('modalGenerarMulta')).show();
}

function ConfirmarGenerarMulta() {
    const idPrestamo = document.getElementById('multa_IdPrestamo').value;
    const valorMulta = parseInt(document.getElementById('multa_ValorMulta').value);
    const idMotivo = document.getElementById('multa_IdMotivo').value;
    const descripcion = document.getElementById('multa_DescripcionMotivo').value.trim();

    if (!valorMulta || valorMulta <= 0) {
        Swal.fire('Validación', 'El valor de la multa debe ser mayor a cero.', 'warning');
        return;
    }
    if (!idMotivo) {
        Swal.fire('Validación', 'Debe seleccionar un motivo.', 'warning');
        return;
    }
    if (!descripcion) {
        Swal.fire('Validación', 'Debe ingresar una descripción del motivo.', 'warning');
        return;
    }

    const modelo = {
        idPrestamo: parseInt(idPrestamo),
        valorMulta: valorMulta,
        saldoMulta: valorMulta,
        fechaImposicion: new Date().toISOString(),
        idMotivo: parseInt(idMotivo),
        descripcionMotivo: descripcion,
        idEstado: 64,
        usuarioCreacion: window.location.hostname,
        maquinaCreacion: window.location.hostname
    };

    Swal.fire({
        title: '¿Confirmar multa?',
        text: `Se generará una multa por $${valorMulta} al préstamo #${idPrestamo}.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, generar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#ffc107'
    }).then((result) => {
        if (!result.isConfirmed) return;

        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        fetch('/Prestamo/Gestion/InsertarMulta', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(modelo)
        })
            .then(r => r.json())
            .then(resp => {
                if (resp.codigo === 1) {
                    bootstrap.Modal.getInstance(document.getElementById('modalGenerarMulta')).hide();
                    Swal.fire('Éxito', resp.mensaje, 'success').then(() => location.reload());
                } else {
                    Swal.fire('Error', resp.mensaje, 'error');
                }
            })
            .catch(() => Swal.fire('Error', 'Ocurrió un error al generar la multa.', 'error'));
    });
}

function CambiarCategoria(idSolicitud) {
    console.log('Cambiar categoría - ID:', idSolicitud);
}


// -------------------------------------------------------
// ********************  EVENTOS  ************************
// -------------------------------------------------------
document.getElementById('pago_PAGO_INTERESES').addEventListener('change', function () {
    const interesMes = parseFloat(document.getElementById('pago_INTERES_MES').value) || 0;
    const montoInput = document.getElementById('pago_MONTO');

    if (this.checked) {
        montoInput.value = interesMes;
        montoInput.readOnly = true;
        validarMonto();
    } else {
        montoInput.value = '';
        montoInput.readOnly = false;
        document.getElementById('btnGuardarPago').disabled = true;
        document.getElementById('pago_monto_feedback').classList.add('d-none');
    }
});

document.getElementById('pago_MONTO').addEventListener('input', validarMonto);

document.getElementById('btnGuardarPago').addEventListener('click', async () => {
    const idPrestamo = document.getElementById('pago_ID_PRESTAMO').value;
    const fechaPago = document.getElementById('pago_FECHA_PAGO').value;
    const monto = document.getElementById('pago_MONTO').value;
    const token = document.querySelector('#formAgregarPago input[name="__RequestVerificationToken"]').value;
    const alerta = document.getElementById('pago_alerta');
    const spinner = document.getElementById('btnGuardarPago_spinner');
    const icon = document.getElementById('btnGuardarPago_icon');

    spinner.classList.remove('d-none');
    icon.classList.add('d-none');
    document.getElementById('btnGuardarPago').disabled = true;

    try {
        const response = await fetch(urlRegistrarPagos, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify({
                ID_PRESTAMO: parseInt(idPrestamo),
                FECHA_PAGO: fechaPago,
                MONTO: parseFloat(monto),
                PAGO_INTERESES: false
            })
        });

        const data = await response.json();

        if (data.codigo === 1) {
            bootstrap.Modal.getInstance(document.getElementById('modalAgregarPago')).hide();
            Swal.fire({
                icon: 'success',
                title: '¡Éxito!',
                text: data.mensaje,
                confirmButtonText: 'Aceptar'
            }).then(() => location.reload());
        } else {
            alerta.className = 'alert alert-danger mt-3';
            alerta.textContent = data.mensaje;
            alerta.classList.remove('d-none');
            document.getElementById('btnGuardarPago').disabled = false;
        }

    } catch (e) {
        alerta.className = 'alert alert-danger mt-3';
        alerta.textContent = 'Error de conexión. Intente nuevamente.';
        alerta.classList.remove('d-none');
        document.getElementById('btnGuardarPago').disabled = false;
    } finally {
        spinner.classList.add('d-none');
        icon.classList.remove('d-none');
    }
});
