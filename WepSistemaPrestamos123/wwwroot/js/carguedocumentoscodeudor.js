
const archivosCargasCodeudor = new Map();
const docsValidosCodeudor = documentosRequeridosCodeudor
    .filter(doc => doc.Id === 1); 

function guardarEnStorageCodeudor(docId, archivo) {
    const reader = new FileReader();
    reader.onload = function (e) {
        archivosCargasCodeudor.set(docId, {
            id: docId,
            nombre: archivo.name,
            tamaño: archivo.size,
            tipo: archivo.type,
            data: e.target.result,
            fechaCarga: new Date().toISOString()
        });
        actualizarContadorDocumentosCodeudor();
    };
    reader.readAsDataURL(archivo);
}

function removerDeStorageCodeudor(docId) {
    archivosCargasCodeudor.delete(docId);
    actualizarContadorDocumentosCodeudor();
}

function actualizarContadorDocumentosCodeudor() {
    const total = docsValidosCodeudor.length; // 🔥 YA NO RESTAS
    const cargados = archivosCargasCodeudor.size;

    const btn = document.getElementById('btnGuardarCodeudor');
    if (!btn) return;

    btn.innerHTML = `<i class="fas fa-save me-2"></i> Guardar Codeudor (${cargados}/${total} documentos)`;

    if (cargados === total && total > 0) {
        btn.classList.remove('btn-secondary');
        btn.classList.add('btn-success');
    } else {
        btn.classList.remove('btn-success');
        btn.classList.add('btn-secondary');
    }
}

function formatBytesCodeudor(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

function generarTarjetasDocumentosCodeudor() {
    const zona = document.getElementById('zonaDocumentosCodeudor');
    if (!zona) return;

    // Filtrar solo los que necesita el codeudor
    const soloParaCodeudor = documentosRequeridosCodeudor
        .filter(doc => doc.Id === 1);
       // .filter(doc => doc.Nombre.toUpperCase().includes('CEDULA'));
    // o por Id: .filter(doc => doc.Id === 1)

    if (!soloParaCodeudor || documentosRequeridosCodeudor.length === 0) {
        zona.innerHTML = '<div class="alert alert-info">No hay documentos requeridos para el codeudor</div>';
        return;
    }

    let html = '';
    soloParaCodeudor.forEach(doc => {
        html += `
            <div class="col-12 mb-4">
                <div class="document-card" id="card-codeudor-${doc.Id}" data-doc-id="${doc.Id}">
                    <button type="button" class="remove-file" onclick="removerArchivoCodeudor(${doc.Id})">
                        <i class="fas fa-times"></i>
                    </button>
                    <div class="required-label" id="required-codeudor-${doc.Id}">
                        <i class="fas fa-exclamation-triangle me-1"></i>
                        Este documento es requerido
                    </div>
                    <div class="upload-zone p-0" onclick="abrirSelectorCodeudor(${doc.Id})">
                        <i class="fas fa-cloud-upload-alt upload-icon"></i>
                        <h5 class="document-title">
                            ${doc.Nombre} <span class="text-danger">*</span>
                        </h5>
                        <p class="document-description">${doc.Descripcion || ''}</p>
                        <p class="weight-limit">
                            <i class="fas fa-weight-hanging me-1"></i>Máximo: ${formatBytesCodeudor(doc.PesoMaximo)}
                        </p>
                        <p class="text-muted mb-0"><small>Haz clic o arrastra el archivo aquí</small></p>
                    </div>
                    <div class="file-info" id="info-codeudor-${doc.Id}">
                        <div class="d-flex align-items-center">
                            <i class="fas fa-file-alt text-success me-3" style="font-size:2rem;"></i>
                            <div class="flex-grow-1">
                                <h6 class="mb-1" id="filename-codeudor-${doc.Id}"></h6>
                                <p class="file-size mb-0" id="filesize-codeudor-${doc.Id}"></p>
                            </div>
                        </div>
                        <div class="progress-container">
                            <div class="progress" style="height:8px;">
                                <div class="progress-bar bg-success" id="progress-codeudor-${doc.Id}" style="width:100%"></div>
                            </div>
                        </div>
                    </div>
                    <div class="error-message" id="error-codeudor-${doc.Id}"></div>
                    <input type="file" class="file-input" id="file-codeudor-${doc.Id}"
                           accept=".pdf,.jpg,.jpeg,.png,.doc,.docx"
                           onchange="manejarArchivoCodeudor(${doc.Id}, this.files[0])">
                </div>
            </div>`;
    });

    zona.innerHTML = html;

    documentosRequeridosCodeudor.forEach(doc => {
        const card = document.getElementById(`card-codeudor-${doc.Id}`);
        if (!card) return;
        card.addEventListener('dragover', e => { e.preventDefault(); card.classList.add('dragover'); });
        card.addEventListener('dragleave', () => card.classList.remove('dragover'));
        card.addEventListener('drop', e => {
            e.preventDefault();
            card.classList.remove('dragover');
            if (e.dataTransfer.files[0]) manejarArchivoCodeudor(doc.Id, e.dataTransfer.files[0]);
        });
    });

    actualizarContadorDocumentosCodeudor();
}

function abrirSelectorCodeudor(docId) {
    document.getElementById(`file-codeudor-${docId}`).click();
}

function manejarArchivoCodeudor(docId, archivo) {
    const card          = document.getElementById(`card-codeudor-${docId}`);
    if (!card) return;
    const uploadZone    = card.querySelector('.upload-zone');
    const fileInfo      = document.getElementById(`info-codeudor-${docId}`);
    const requiredLabel = document.getElementById(`required-codeudor-${docId}`);
    const documento     = documentosRequeridosCodeudor.find(d => d.Id === docId);
    if (!documento) return;

    limpiarErrorCodeudor(docId);

    if (archivo.size > documento.PesoMaximo) {
        mostrarErrorCodeudor(docId, `El archivo excede el tamaño máximo (${formatBytesCodeudor(documento.PesoMaximo)})`);
        return;
    }
    const tipos = ['application/pdf','image/jpeg','image/jpg','image/png',
                   'application/msword','application/vnd.openxmlformats-officedocument.wordprocessingml.document'];
    if (!tipos.includes(archivo.type)) {
        mostrarErrorCodeudor(docId, 'Tipo no permitido. Use PDF, JPG, PNG, DOC o DOCX');
        return;
    }

    mostrarCargandoCodeudor(docId, true);
    setTimeout(() => {
        document.getElementById(`filename-codeudor-${docId}`).textContent = archivo.name;
        document.getElementById(`filesize-codeudor-${docId}`).textContent = formatBytesCodeudor(archivo.size);

        uploadZone.style.display = 'none';
        fileInfo.classList.add('show');
        card.classList.add('uploaded', 'success-animation');
        card.classList.remove('required', 'error');
        requiredLabel.classList.remove('show');

        guardarEnStorageCodeudor(docId, archivo);
        mostrarCargandoCodeudor(docId, false);
        setTimeout(() => card.classList.remove('success-animation'), 600);
        mostrarNotificacionCodeudor(`Documento "${archivo.name}" cargado exitosamente`, 'success');
    }, 800);
}

function mostrarErrorCodeudor(docId, mensaje) {
    const card  = document.getElementById(`card-codeudor-${docId}`);
    const err   = document.getElementById(`error-codeudor-${docId}`);
    const label = document.getElementById(`required-codeudor-${docId}`);
    if (card)  card.classList.add('error', 'required');
    if (err)   { err.textContent = mensaje; err.classList.add('show'); }
    if (label) label.classList.add('show');
    setTimeout(() => limpiarErrorCodeudor(docId), 5000);
}

function limpiarErrorCodeudor(docId) {
    const card  = document.getElementById(`card-codeudor-${docId}`);
    const err   = document.getElementById(`error-codeudor-${docId}`);
    const label = document.getElementById(`required-codeudor-${docId}`);
    if (card) card.classList.remove('error');
    if (err)  err.classList.remove('show');
    if (archivosCargasCodeudor.has(docId)) {
        if (card)  card.classList.remove('required');
        if (label) label.classList.remove('show');
    }
}

function marcarCamposRequeridosCodeudor() {
    let vacios = [];

    docsValidosCodeudor.forEach(doc => { 
        if (!archivosCargasCodeudor.has(doc.Id)) {
            const card = document.getElementById(`card-codeudor-${doc.Id}`);
            const label = document.getElementById(`required-codeudor-${doc.Id}`);

            if (card) card.classList.add('required');
            if (label) label.classList.add('show');

            vacios.push(doc.Nombre);
        }
    });

    return vacios;
}

function validarCamposRequeridosCodeudor() {
    const vacios = marcarCamposRequeridosCodeudor();
    if (vacios.length > 0) {
        mostrarNotificacionCodeudor(`Documentos faltantes: ${vacios.join(', ')}`, 'error');
        const primero = docsValidosCodeudor.find(d => !archivosCargasCodeudor.has(d.Id));
        if (primero) document.getElementById(`card-codeudor-${primero.Id}`)
                             ?.scrollIntoView({ behavior: 'smooth', block: 'center' });
        return false;
    }
    return true;
}

function removerArchivoCodeudor(docId) {
    const card      = document.getElementById(`card-codeudor-${docId}`);
    if (!card) return;
    const nombre    = archivosCargasCodeudor.get(docId)?.nombre || 'archivo';

    card.classList.remove('uploaded', 'error');
    card.classList.add('required');
    card.querySelector('.upload-zone').style.display    = 'flex';
    document.getElementById(`info-codeudor-${docId}`)  ?.classList.remove('show');
    document.getElementById(`file-codeudor-${docId}`)  .value = '';
    document.getElementById(`error-codeudor-${docId}`) ?.classList.remove('show');
    document.getElementById(`required-codeudor-${docId}`)?.classList.add('show');

    removerDeStorageCodeudor(docId);
    mostrarNotificacionCodeudor(`Documento "${nombre}" eliminado`, 'warning');
}

function procesarDocumentosCodeudor() {
    const faltantes = docsValidosCodeudor.filter(d => !archivosCargasCodeudor.has(d.Id));
    if (faltantes.length > 0) {
        mostrarNotificacionCodeudor(`Faltan: ${faltantes.map(d => d.Nombre).join(', ')}`, 'error');
        return;
    }
    mostrarNotificacionCodeudor('Todos los documentos del codeudor están listos', 'success');
}

function limpiarDocumentosCodeudor() {
    archivosCargasCodeudor.clear();
    docsValidosCodeudor.forEach(doc => removerArchivoCodeudor(doc.Id));
    mostrarNotificacionCodeudor('Documentos del codeudor eliminados', 'info');
}

function mostrarCargandoCodeudor(docId, mostrar) {
    const bar = document.getElementById(`progress-codeudor-${docId}`);
    if (!bar) return;
    bar.classList.toggle('progress-bar-animated', mostrar);
    bar.classList.toggle('progress-bar-striped', mostrar);
}

function mostrarNotificacionCodeudor(mensaje, tipo) {
    const notif = document.createElement('div');
    notif.className = `alert alert-${tipo === 'error' ? 'danger' : tipo} alert-dismissible fade show position-fixed`;
    notif.style.cssText = 'top:20px;right:20px;z-index:10000;max-width:380px;';
    notif.innerHTML = `${mensaje}<button type="button" class="btn-close" data-bs-dismiss="alert"></button>`;
    document.body.appendChild(notif);
    setTimeout(() => notif.parentNode && notif.remove(), 5000);
}

// ─── GUARDAR CODEUDOR ─────────────────────────────────────────────────────
async function guardarCodeudor() {

    const formCodeudor = document.getElementById('formCodeudor');

    if (!formCodeudor.checkValidity()) {
        formCodeudor.reportValidity();
        return;
    }

    if (!validarCamposRequeridosCodeudor()) return;

    // 🔹 DOCUMENTOS
    const listaDocumentos = [];
    archivosCargasCodeudor.forEach(doc => {
        listaDocumentos.push({
            idDocumento: doc.id,
            contenidoDoc: doc.data,
            formato: doc.tipo,
            tamanio: doc.tamaño,
            usuarioCreacion: 'WEB_USER',
            maquinaCreacion: "::1",
            habilitado: true
        });
    });

    // 🔹 OBJETO CODEUDOR (ESTRUCTURA EXACTA DEL DTO)
    const codeudor = {
        primerNombre: document.getElementById('pNombreCodeudor').value.trim().toUpperCase(),
        segundoNombre: document.getElementById('sNombreCodeudor').value.trim().toUpperCase() || null,
        primerApellido: document.getElementById('pApellidoCodeudor').value.trim().toUpperCase(),
        segundoApellido: document.getElementById('sApellidoCodeudor').value.trim().toUpperCase() || null,
        tipoIdentificacion: parseInt(document.getElementById('tipoIdentificacionCodeudor').value),
        numeroIdentificacion: parseInt(document.getElementById('numeroIdentificacionCodeudor').value),
        direccion: document.getElementById('direccionCodeudor').value.trim(),
        email: document.getElementById('emailCodeudor').value.trim(),
        celular: parseInt(document.getElementById('celularCodeudor').value),
        documentos: listaDocumentos
    };


 
    // 🔹 GUARDAR EN MEMORIA
    window.listaCodeudores.push(codeudor);

    console.log("CODEUDOR AGREGADO:", listaCodeudores);

    mostrarNotificacionCodeudor('Codeudor agregado correctamente', 'success');

    bootstrap.Modal.getInstance(document.getElementById('modalCodeudorFormulario'))?.hide();

    if (typeof validarFlujoCompleto === 'function') {
        validarFlujoCompleto();
    }

}



// ─── INICIALIZACIÓN ──────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', function () {
    const modalEl = document.getElementById('modalCodeudorFormulario');
    if (!modalEl) return;

    modalEl.addEventListener('shown.bs.modal', function () {
        const zona = document.getElementById('zonaDocumentosCodeudor');
        if (zona && !zona.querySelector('.document-card')) {
            generarTarjetasDocumentosCodeudor();
            setTimeout(() => marcarCamposRequeridosCodeudor(), 400);
        }
    });
});


