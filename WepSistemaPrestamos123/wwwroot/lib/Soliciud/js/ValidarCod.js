
function verSolicitud(id, estadoId) {
    const url = `/Gestion/Solicitudes/Validar?id=${id}&estadoId=${estadoId}`;
    window.location.href = url;
}

