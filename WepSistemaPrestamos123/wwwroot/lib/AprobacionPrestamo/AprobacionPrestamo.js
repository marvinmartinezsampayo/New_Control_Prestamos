document.addEventListener("DOMContentLoaded", function () {
    cargarPrestamos();
});

async function cargarPrestamos() {
    try {
        
        const response = await fetch('/Gestion/AprobarPrestamo/ObtenerPrestamos'); 
        if (!response.ok) throw new Error("Error al obtener los préstamos");

        const data = await response.json();

        if (!data || !data.respuesta || data.respuesta.length === 0) {
            renderTabla([]);
            return;
        }

        renderTabla(data.respuesta);

    } catch (error) {
        console.error("Error en cargarPrestamos:", error);
        alert("Hubo un problema al cargar los préstamos");
    }
}

function renderTabla(prestamos) {
    const tbody = document.querySelector("#tablaPrestamos tbody");
    tbody.innerHTML = "";

    if (prestamos.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="7" class="text-center text-muted">
                    <i class="bi bi-inbox"></i> No se encontraron préstamos pendientes.
                </td>
            </tr>`;
        return;
    }

    prestamos.forEach(p => {
        const tr = document.createElement("tr");

        tr.innerHTML = `
            <td>${p.primerNombreSolicitante} ${p.segundoNombreSolicitante || ""} ${p.primerApellidoSolicitante} ${p.segundoApellidoSolicitante || ""}</td>
            <td>${p.numeroIdentificacion}</td>
            <td>${p.celular}</td>
            <td>${new Date(p.fechaCreacion).toLocaleDateString()}</td>
            <td>$${p.monto.toLocaleString()}</td>
            <td><span class="badge bg-warning text-dark">Pendiente</span></td>
            <td class="text-center">
                <button class="btn btn-sm btn-primary me-1" onclick="verPrestamo(${p.id}, ${p.estadoId})">
                    <i class="bi bi-eye"></i>
                </button>
                <button class="btn btn-sm btn-success me-1" onclick="aprobarPrestamo(${p.id}, ${p.estadoId})">
                    <i class="bi bi-check-circle"></i>
                </button>
                <button class="btn btn-sm btn-danger" onclick="cancelarPrestamo(${p.id}, ${p.estadoId})">
                    <i class="bi bi-x-circle"></i>
                </button>
            </td>
        `;

        tbody.appendChild(tr);
    });
}

// ===== Acciones con botones =====
function verPrestamo(id, estadoId) {
    console.log("ID:", id, "Estado:", estadoId);
    const url = `/Gestion/Solicitudes/Validar?id=${id}&estadoId=${estadoId}`;
    window.location.href = url;
}

// Función genérica para manejar cualquier acción de estado
async function cambiarEstadoSolicitud(id, estadoId, tipo) {
    const acciones = {
        aprobar: {
            url: '/Gestion/AprobarPrestamo/PrestamoAprobado',
            titulo: "¿Aprobar préstamo?",
            texto: "Si apruebas, pasará al estado 'Crédito aprobado'.",
            icono: "question",
            confirmButton: "Sí, aprobar",
            cancelButton: "Cancelar",
            successTitle: "Éxito"
        },
        cancelar: {
            url: '/Gestion/AprobarPrestamo/CancelarPrestamo',
            titulo: "¿Cancelar préstamo?",
            texto: "Si cancelas, pasará al estado 'Denegada'.",
            icono: "warning",
            confirmButton: "Sí, cancelar",
            cancelButton: "Volver",
            successTitle: "Cancelada"
        }
    };

    const cfg = acciones[tipo];

    const confirmacion = await Swal.fire({
        title: cfg.titulo,
        text: cfg.texto,
        icon: cfg.icono,
        showCancelButton: true,
        confirmButtonText: cfg.confirmButton,
        cancelButtonText: cfg.cancelButton
    });

    if (!confirmacion.isConfirmed) return;

    const obj = { IdSolicitud: id, NuevoEstadoId: estadoId };

    try {
        const response = await fetch(cfg.url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(obj)
        });

        const data = await response.json();

        if (data.success) {
            Swal.fire(cfg.successTitle, data.message, "success");
            setTimeout(() => window.location.reload(), 1200);
        } else {
            Swal.fire("Error", data.message, "error");
        }
    } catch (error) {
        Swal.fire("Excepción", error.message, "error");
    }
}

function aprobarPrestamo(id, estadoId) {
    cambiarEstadoSolicitud(id, estadoId, "aprobar");
}

function cancelarPrestamo(id, estadoId) {
    cambiarEstadoSolicitud(id, estadoId, "cancelar");
}
