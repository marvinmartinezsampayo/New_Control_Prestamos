
function verSolicitud(id, estadoId) {
    const url = `/Gestion/Solicitudes/Validar?id=${id}&estadoId=${estadoId}`;
    window.location.href = url;
}




async function aprobarSolicitud(id, estadoId) {
    debugger;
    

    Obj = {
        IdSolicitud : id,
        NuevoEstadoId: estadoId
    }

    try {
        const response = await fetch('/Gestion/Solicitudes/Aprobar', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(Obj)
        });

        const data = await response.json();

        if (data.success) {
            Swal.fire("Éxito", data.message, "success");
            window.location.reload(); // ojo esto solo si no hay uchos datos esto requiere mucho mas codigo para solo hacer la peticion por (AJAX + PartialView) mucho mas codigo
            // refrescar tabla o actualizar UI
        } else {
            Swal.fire(" Error", data.message, "error");
        }
    } catch (error) {
        Swal.fire(" Excepción", error.message, "error");
    }
}

async function cancelarSolicitud(id, estadoId) {
    debugger;
    console.log("Cancelar:", id, estadoId);

    const obj = {
        IdSolicitud: id,
        NuevoEstadoId: estadoId 
    };

    try {
        const response = await fetch('/Gestion/Solicitudes/cancelarSolicitud', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(obj)
        });

        const data = await response.json();

        if (data.success) {
            Swal.fire("Cancelada", data.message, "success");
            window.location.reload();
        } else {
            Swal.fire("Error", data.message, "error");
        }
    } catch (error) {
        Swal.fire("Excepción", error.message, "error");
    }
}



