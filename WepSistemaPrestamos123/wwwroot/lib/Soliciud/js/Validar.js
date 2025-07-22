document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('click', function (e) {
        if (e.target.closest('.verDoc')) {
            e.preventDefault();

            const boton = e.target.closest('.verDoc');
            const contenidoDoc = boton.getAttribute('data');

            if (contenidoDoc) {
                const contenedorDoc = document.getElementById('contenedorDocumento');

                if (contenidoDoc.startsWith('data:image/')) {
                    contenedorDoc.innerHTML = `<img src="${contenidoDoc}" class="img-fluid mx-auto d-block" style="max-height: 600px;">`;
                } else {
                    contenedorDoc.innerHTML = `<iframe src="${contenidoDoc}" style="width: 100%; height: 600px; border: none;"></iframe>`;
                }

                const modal = new bootstrap.Modal(document.getElementById('modalVerDocumento'));
                modal.show();
            } else {
                alert('No hay contenido para mostrar');
            }
        }
    });
});

// funcion para vovler atras en el boton atras y recargar la vista de gestion 
function volverConRecarga() {
    const referrer = document.referrer;

    if (referrer) {
        window.location.href = referrer; // vuelve y recarga desde donde vino
    } else {
        window.history.back(); // fallback
    }
}
