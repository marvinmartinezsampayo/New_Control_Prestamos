
function generarCodigo() {
    const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    let codigo = '';
    for (let i = 0; i < 8; i++) {
        codigo += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    document.getElementById("codigoGenerado").value = codigo;
}

document.getElementById('formGenerarCodigo').addEventListener('submit', async function (e) {
    e.preventDefault();

    const codigo = document.getElementById('codigoGenerado').value;
    const fechaInicio = document.getElementById('fechaInicio').value;
    const fechaFin = document.getElementById('fechaFin').value;
    const cantidadUsos = document.getElementById('cantidadUsos').value;


    const Obj = {
        Codigo: codigo,
        FechaInicio: fechaInicio,
        FechaFin: fechaFin,
        EmailAsociado: window.usuarioLogueado.email,
        CelularAsociado: window.usuarioLogueado.telefono,
        CantidadRegistros: parseInt(cantidadUsos),
        Imagen: "", // o puedes quitarlo si no estás usando imagen aún
        Habilitado: true,
        UsuarioCreacion: window.usuarioLogueado.usuarioEmpresarial,
        MaquinaCreacion: window.usuarioLogueado.nroIdentificacion,
        FechaCreacion: null,
        Id_Usuario: parseInt(window.usuarioLogueado.idUsuario),
        CodigoAcesso: codigo,

    };
    const ahora = new Date(); // fecha y hora actual

    if (new Date(fechaInicio) < ahora) {
        mostrarAlertaSweet({
            icon: "warning",
            titulo: "Fecha inválida",
            mensaje: "La fecha de inicio no puede ser anterior a la fecha actual."
        });
        return;
    }

    if (new Date(fechaFin) <= new Date(fechaInicio)) {
        mostrarAlertaSweet({
            icon: "warning",
            titulo: "Fechas inválidas",
            mensaje: "La fecha de fin debe ser mayor a la de inicio."
        });
        return;
    }


    if (!codigo || !fechaInicio || !fechaFin || !cantidadUsos) {
        mostrarAlertaSweet({
            icon: "warning",
            titulo: "Campos requeridos",
            mensaje: "Por favor completa todos los campos antes de guardar.",
        });
        return;
    }

    const response = await fetch('/Solicitud/CodigoAcceso/GuardarCodigoAcceso', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(Obj)
    });


    const result = await response.json();

    mostrarAlertaSweet({
        icon: result.exito ? "success" : "error",
        titulo: result.exito ? "¡Código Generado!" : "Error al Generar",
        mensaje: result.mensaje,
        limpiar: result.exito
    });
    if (result.exito) {
        document.getElementById("formGenerarCodigo").reset(); // limpia campos
        cargarCodigosDeHoy(); // recarga tabla
    }
});

// renderizar 
function renderizarCodigos(codigos) {
    const tbody = document.querySelector("#tablaCodigos tbody");
    tbody.innerHTML = "";

    if (!codigos || codigos.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="7" class="text-center text-muted">
                    <i class="bi bi-inbox"></i> No se encontraron códigos.
                </td>
            </tr>`;
        return;
    }

    codigos.forEach(codigo => {
        const row = `
            <tr>
                <td>${codigo.codigo}</td>
                <td>${new Date(codigo.fechaInicio).toLocaleString()}</td>
                <td>${new Date(codigo.fechaFin).toLocaleString()}</td>
                <td>${codigo.cantidadRegistros}</td>
                <td>${codigo.usuarioCreacion}</td>
                <td>${codigo.habilitado ? 'Habilitado' : 'Inhabilitado'}</td>
                <td class="text-center">
                    <button class="btn btn-success btn-sm" title="Editar"
                            onclick="editarCodigoAcceso('${codigo.codigo}', '${codigo.fechaInicio}', '${codigo.fechaFin}', ${codigo.habilitado}, ${codigo.cantidadRegistros})"
                            data-bs-toggle="modal" data-bs-target="#modalEditarCodigo">
                            <i class="bi bi-pencil-square"></i>
                    </button>
                </td>
            </tr>`;
        tbody.innerHTML += row;
    });
}

//vamos a llamar los codigos de acceso del dia de hoy una vez se halla agregado un codigo 
function cargarCodigosDeHoy() {
    fetch('/Solicitud/CodigoAcceso/ObtenerCodigosHoy')
        .then(response => response.json())
        .then(data => {
            renderizarCodigos(data);
        })
        .catch(error => {
            Swal.fire('Error', '❌ Error al cargar códigos: cargarCodigosDeHoy()', 'error');
        });
}

// busqueda por fechas filtradas 
document.getElementById('formBuscarCodigos').addEventListener('submit', async function (e) {
    e.preventDefault();

    const fechaDesde = document.getElementById('fechaDesde').value;
    const fechaHasta = document.getElementById('fechaHasta').value;

    if (!fechaDesde || !fechaHasta) {
        Swal.fire('Faltan datos', 'Debe ingresar ambas fechas', 'warning');
        return;
    }

    try {
        const response = await fetch(`/Solicitud/CodigoAcceso/BuscarCodigosPorFechas?fechaInicio=${fechaDesde}&fechaFin=${fechaHasta}`);
        const codigos = await response.json();
        renderizarCodigos(codigos);
    } catch (error) {
        console.error('Error al buscar códigos:', error);
        Swal.fire('Error', 'Ocurrió un error al buscar los códigos.', 'error');
    }
});
//funcion para las tabla 
function inicializarTablaCodigos(idTabla = "tablaCodigos") {
    const tabla = document.querySelector(`#${idTabla}`);
    if (tabla) {
        new simpleDatatables.DataTable(tabla, {
            perPage: 10,
            perPageSelect: [5, 10, 20, 50],
            labels: {
                placeholder: "Buscar...",
                perPage: "{select} registros por página",
                noRows: "No hay resultados",
                info: "Mostrando {start} a {end} de {rows} registros"
            }
        });
    }
}

// Función del botón editar para mostrar la modal
function editarCodigoAcceso(codigo, fechaInicio, fechaFin, habilitado, cantidadRegistros) {

    document.getElementById('editCodigo').value = codigo;
    document.getElementById('editFechaInicio').value = fechaInicio;
    document.getElementById('editFechaFin').value = fechaFin;
    document.getElementById('editcantidadUsos').value = cantidadRegistros;
    document.getElementById('editHabilitado').checked = habilitado === true;
    document.getElementById('editId').value = codigo;
}


//funcion para actualizar codigo 
async function guardarEdicionCodigo() {
    // Capturo valores de vista 
    const codigo = document.getElementById('editCodigo').value;
    const id = document.getElementById('editId').value;
    const fechaInicio = document.getElementById('editFechaInicio').value;
    const fechaFin = document.getElementById('editFechaFin').value;
    const cantidadRegistros = document.getElementById('editcantidadUsos').value;
    const habilitado = document.getElementById('editHabilitado').checked;

    // Creo el objeto 
    const datos = {
        codigo,
        fechaInicio,
        fechaFin,
        cantidadRegistros,
        habilitado,
        Id_Usuario: parseInt(window.usuarioLogueado.idUsuario),
    };

    try {
        const response = await fetch('/Solicitud/CodigoAcceso/ActualizarCodigoAcceso', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(datos)
        });

        const resultado = await response.json();

        if (resultado.estado) {
            Swal.fire('¡Actualizado!', 'El código fue actualizado correctamente.', 'success');
            cargarCodigosDeHoy();
            const modal = bootstrap.Modal.getInstance(document.getElementById('modalEditarCodigo'));
            modal.hide();
            limpiarCamposGenerarCodigo();
        } else {
            Swal.fire('Error', 'No se pudo actualizar el código.', 'error');
            limpiarCamposGenerarCodigo();
        }

    } catch (error) {
        console.error('❌ Error al actualizar código:', error);
        Swal.fire('Error', 'Ocurrió un error inesperado.', 'error');
    }
}

function mostrarAlertaSweet({ icon = "success", titulo = "", mensaje = "", limpiar = false, onClose = null }) {
    Swal.fire({
        icon: icon,
        title: titulo || (icon === 'success' ? "¡Operación Exitosa!" : "Ocurrió un error"),
        html: mensaje,
        confirmButtonText: 'Aceptar',
        confirmButtonColor: icon === 'success' ? '#28a745' : '#dc3545',
        allowOutsideClick: false,
        allowEscapeKey: false,
        customClass: {
            popup: 'animated bounceIn',
            confirmButton: 'btn btn-success'
        },
        buttonsStyling: false
    }).then((result) => {
        if (result.isConfirmed && limpiar) {
            limpiarCamposGenerarCodigo();
        }

        if (onClose && typeof onClose === 'function') {
            onClose();
        }
    });
}

function verificarSweetAlertDesdeVista() {
    const alertData = document.getElementById("sweetAlertData");
    if (alertData && alertData.dataset.icon && alertData.dataset.icon !== "") {
        mostrarAlertaSweet({
            icon: alertData.dataset.icon,
            titulo: alertData.dataset.titulo,
            mensaje: alertData.dataset.mensaje,
            limpiar: alertData.dataset.limpiar === "true"
        });
    }
}
function limpiarCamposGenerarCodigo() {
    document.getElementById("codigoGenerado").value = "";
    document.getElementById("fechaInicio").value = "";
    document.getElementById("fechaFin").value = "";
    document.getElementById("cantidadUsos").value = "";
    document.getElementById("fechaDesde").value = "";
    document.getElementById("fechaHasta").value = "";

}

document.addEventListener("DOMContentLoaded", function () {
    verificarSweetAlertDesdeVista();
    cargarCodigosDeHoy(); 
});


