
// Almacenar archivos cargados (simulando localStorage con variables en memoria)
const archivosCargas = new Map();

// Funciones para manejar el almacenamiento local simulado
function guardarEnStorage(docId, archivo) {
    // Convertir archivo a base64 para almacenamiento
    const reader = new FileReader();
    reader.onload = function (e) {
        const documentoData = {
            id: docId,
            nombre: archivo.name,
            tamaño: archivo.size,
            tipo: archivo.type,
            data: e.target.result,
            fechaCarga: new Date().toISOString()
        };

        // Guardar en el Map (simulando storage local)
        archivosCargas.set(docId, documentoData);

        // En un entorno real, aquí usarías localStorage:
        // localStorage.setItem(`documento_${docId}`, JSON.stringify(documentoData));

        console.log(`Documento ${docId} guardado en storage local:`, documentoData.nombre);
        actualizarContadorDocumentos();
    };
    reader.readAsDataURL(archivo);
}

function removerDeStorage(docId) {
    archivosCargas.delete(docId);

    // En un entorno real, aquí usarías localStorage:
    // localStorage.removeItem(`documento_${docId}`);

    console.log(`Documento ${docId} removido del storage local`);
    actualizarContadorDocumentos();
}

function cargarDesdeStorage() {
    // En un entorno real, aquí cargarías desde localStorage:
    /*
    documentosRequeridos.forEach(doc => {
        const stored = localStorage.getItem(`documento_${doc.Id}`);
        if (stored) {
            const documentoData = JSON.parse(stored);
            archivosCargas.set(doc.Id, documentoData);
            restaurarArchivoEnUI(doc.Id, documentoData);
        }
    });
    */
}

function actualizarContadorDocumentos() {
    const totalDocumentos = documentosRequeridos.length;
    const documentosCargados = archivosCargas.size;
    const botonProcesar = document.getElementById('procesarDocumentos');

    botonProcesar.innerHTML = `
                <i class="fas fa-cloud-upload-alt me-2"></i>
                Procesar Documentos (${documentosCargados}/${totalDocumentos})
            `;

    if (documentosCargados === totalDocumentos) {
        botonProcesar.classList.remove('btn-primary');
        botonProcesar.classList.add('btn-success');
    } else {
        botonProcesar.classList.remove('btn-success');
        botonProcesar.classList.add('btn-primary');
    }
}

// Función para formatear bytes a texto legible
function formatBytes(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

// Generar las tarjetas de documentos dinámicamente
function generarTarjetasDocumentos() {
    const zonaDocumentos = document.getElementById('ZonaDocumentos');
    let html = '';

    documentosRequeridos.forEach(doc => {
        html += `
                    <div class="col-12 mb-4">
                        <div class="document-card" id="card-${doc.Id}" data-doc-id="${doc.Id}">
                            <button type="button" class="remove-file" onclick="removerArchivo(${doc.Id})">
                                <i class="fas fa-times"></i>
                            </button>
                            
                            <div class="required-label" id="required-${doc.Id}">
                                <i class="fas fa-exclamation-triangle me-1"></i>
                                Este documento es requerido
                            </div>
                            
                            <div class="upload-zone p-0" onclick="abrirSelector(${doc.Id})">
                                <i class="fas fa-cloud-upload-alt upload-icon"></i>
                                <h5 class="document-title">
                                    ${doc.Nombre}
                                    <span class="text-danger">*</span>
                                </h5>
                                <p class="document-description">${doc.Descripcion}</p>
                                <p class="weight-limit">
                                    <i class="fas fa-weight-hanging me-1"></i>Máximo: ${formatBytes(doc.PesoMaximo)}
                                </p>
                                <p class="text-muted mb-0">
                                    <small>Haz clic o arrastra el archivo aquí</small>
                                </p>
                            </div>
                            
                            <div class="file-info" id="info-${doc.Id}">
                                <div class="d-flex align-items-center">
                                    <i class="fas fa-file-alt text-success me-3" style="font-size: 2rem;"></i>
                                    <div class="flex-grow-1">
                                        <h6 class="mb-1" id="filename-${doc.Id}"></h6>
                                        <p class="file-size mb-0" id="filesize-${doc.Id}"></p>
                                    </div>
                                </div>
                                <div class="progress-container">
                                    <div class="progress" style="height: 8px;">
                                        <div class="progress-bar bg-success" id="progress-${doc.Id}" style="width: 100%"></div>
                                    </div>
                                </div>
                            </div>
                            
                            <div class="error-message" id="error-${doc.Id}"></div>
                            
                            <input type="file" class="file-input" id="file-${doc.Id}" 
                                   accept=".pdf,.jpg,.jpeg,.png,.doc,.docx" 
                                   onchange="manejarArchivo(${doc.Id}, this.files[0])"
                                   required>
                        </div>
                    </div>
                `;
    });

    zonaDocumentos.innerHTML = html;

    // Agregar event listeners para drag and drop
    documentosRequeridos.forEach(doc => {
        const card = document.getElementById(`card-${doc.Id}`);

        card.addEventListener('dragover', (e) => {
            e.preventDefault();
            card.classList.add('dragover');
        });

        card.addEventListener('dragleave', () => {
            card.classList.remove('dragover');
        });

        card.addEventListener('drop', (e) => {
            e.preventDefault();
            card.classList.remove('dragover');
            const file = e.dataTransfer.files[0];
            if (file) {
                manejarArchivo(doc.Id, file);
            }
        });
    });
}

// Abrir selector de archivos
function abrirSelector(docId) {
    document.getElementById(`file-${docId}`).click();
}

// Manejar archivo seleccionado
function manejarArchivo(docId, archivo) {
    const card = document.getElementById(`card-${docId}`);
    const uploadZone = card.querySelector('.upload-zone');
    const fileInfo = document.getElementById(`info-${docId}`);
    const errorDiv = document.getElementById(`error-${docId}`);
    const requiredLabel = document.getElementById(`required-${docId}`);
    const documento = documentosRequeridos.find(d => d.Id === docId);

    // Limpiar errores previos
    limpiarError(docId);

    // Validar tamaño
    if (archivo.size > documento.PesoMaximo) {
        mostrarError(docId, `El archivo excede el tamaño máximo permitido (${formatBytes(documento.PesoMaximo)})`);
        return;
    }

    // Validar tipo de archivo
    const tiposPermitidos = ['application/pdf', 'image/jpeg', 'image/jpg', 'image/png', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'];
    if (!tiposPermitidos.includes(archivo.type)) {
        mostrarError(docId, 'Tipo de archivo no permitido. Use PDF, JPG, PNG, DOC o DOCX');
        return;
    }

    // Mostrar loading
    mostrarCargando(docId, true);

    // Simular carga y guardar en storage
    setTimeout(() => {
        // Actualizar información del archivo
        document.getElementById(`filename-${docId}`).textContent = archivo.name;
        document.getElementById(`filesize-${docId}`).textContent = formatBytes(archivo.size);

        // Mostrar información y ocultar zona de carga
        uploadZone.style.display = 'none';
        fileInfo.classList.add('show');

        // Marcar como cargado y limpiar estado de requerido
        card.classList.add('uploaded', 'success-animation');
        card.classList.remove('required');
        requiredLabel.classList.remove('show');

        // Guardar archivo en storage local
        guardarEnStorage(docId, archivo);

        // Ocultar loading
        mostrarCargando(docId, false);

        // Remover animación después de completarse
        setTimeout(() => {
            card.classList.remove('success-animation');
        }, 600);

        // Mostrar notificación de éxito
        mostrarNotificacion(`Documento "${archivo.name}" cargado y guardado exitosamente`, 'success');

    }, 800);
}

// Mostrar error
function mostrarError(docId, mensaje) {
    const card = document.getElementById(`card-${docId}`);
    const errorDiv = document.getElementById(`error-${docId}`);
    const requiredLabel = document.getElementById(`required-${docId}`);

    card.classList.add('error', 'required');
    errorDiv.textContent = mensaje;
    errorDiv.classList.add('show');
    requiredLabel.classList.add('show');

    // Remover el error después de 5 segundos
    setTimeout(() => {
        limpiarError(docId);
    }, 5000);
}

// Limpiar error
function limpiarError(docId) {
    const card = document.getElementById(`card-${docId}`);
    const errorDiv = document.getElementById(`error-${docId}`);
    const requiredLabel = document.getElementById(`required-${docId}`);

    card.classList.remove('error');
    errorDiv.classList.remove('show');

    // Solo ocultar required label si el documento está cargado
    if (archivosCargas.has(docId)) {
        card.classList.remove('required');
        requiredLabel.classList.remove('show');
    }
}

// Marcar campos requeridos vacíos
function marcarCamposRequeridos() {
    let camposVacios = [];

    documentosRequeridos.forEach(doc => {
        if (!archivosCargas.has(doc.Id)) {
            const card = document.getElementById(`card-${doc.Id}`);
            const requiredLabel = document.getElementById(`required-${doc.Id}`);

            card.classList.add('required');
            requiredLabel.classList.add('show');
            camposVacios.push(doc.Nombre);

            // Efecto de shake
            card.style.animation = 'none';
            setTimeout(() => {
                card.style.animation = 'shakeError 0.5s ease-in-out';
            }, 10);
        }
    });

    return camposVacios;
}

// Validar todos los campos requeridos
function validarCamposRequeridos() {
    const camposVacios = marcarCamposRequeridos();

    if (camposVacios.length > 0) {
        const mensaje = `Los siguientes documentos son requeridos:\n• ${camposVacios.join('\n• ')}`;
        mostrarNotificacion(mensaje, 'error');

        // Scroll al primer campo vacío
        const primerCampoVacio = documentosRequeridos.find(doc => !archivosCargas.has(doc.Id));
        if (primerCampoVacio) {
            const card = document.getElementById(`card-${primerCampoVacio.Id}`);
            card.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }

        return false;
    }

    return true;
}

// Remover archivo
function removerArchivo(docId) {
    const card = document.getElementById(`card-${docId}`);
    const uploadZone = card.querySelector('.upload-zone');
    const fileInfo = document.getElementById(`info-${docId}`);
    const fileInput = document.getElementById(`file-${docId}`);
    const errorDiv = document.getElementById(`error-${docId}`);
    const requiredLabel = document.getElementById(`required-${docId}`);

    // Obtener nombre del archivo antes de eliminarlo
    const documentoData = archivosCargas.get(docId);
    const nombreArchivo = documentoData ? documentoData.nombre : 'archivo';

    // Limpiar estado visual
    card.classList.remove('uploaded', 'error');
    card.classList.add('required'); // Marcar como requerido nuevamente
    uploadZone.style.display = 'flex';
    fileInfo.classList.remove('show');
    fileInput.value = '';
    errorDiv.classList.remove('show');
    requiredLabel.classList.add('show'); // Mostrar etiqueta de requerido

    // Remover archivo del almacenamiento local
    removerDeStorage(docId);

    // Mostrar notificación de eliminación
    mostrarNotificacion(`Documento "${nombreArchivo}" eliminado del almacenamiento`, 'warning');
}

// Procesar todos los documentos
function procesarTodosLosDocumentos() {
    const documentosFaltantes = documentosRequeridos.filter(doc => !archivosCargas.has(doc.Id));

    if (documentosFaltantes.length > 0) {
        const mensaje = `Faltan por cargar los siguientes documentos:\n${documentosFaltantes.map(d => d.Nombre).join('\n')}`;
        mostrarNotificacion(mensaje, 'error');
        return;
    }

    // Mostrar información de los documentos almacenados
    console.log('Documentos almacenados localmente:', archivosCargas);

    // Crear FormData con los documentos del storage
    const formData = new FormData();
    const documentosParaEnviar = [];

    archivosCargas.forEach((documentoData, docId) => {
        // Convertir base64 de vuelta a File para envío
        const byteCharacters = atob(documentoData.data.split(',')[1]);
        const byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        const archivo = new File([byteArray], documentoData.nombre, { type: documentoData.tipo });

        formData.append(`documento_${docId}`, archivo);
        documentosParaEnviar.push({
            id: docId,
            nombre: documentoData.nombre,
            tamaño: formatBytes(documentoData.tamaño),
            fechaCarga: documentoData.fechaCarga
        });
    });

    // Mostrar resumen antes de procesar
    let resumen = "Documentos a procesar:\n";
    documentosParaEnviar.forEach(doc => {
        resumen += `• ${doc.nombre} (${doc.tamaño})\n`;
    });

    if (confirm(resumen + "\n¿Desea procesar todos los documentos?")) {
        mostrarNotificacion('Procesando documentos...', 'info');

        // Aquí harías la petición AJAX al servidor
        // fetch('/ProcesarDocumentos', {
        //     method: 'POST',
        //     body: formData
        // }).then(response => response.json())
        //   .then(data => {
        //       mostrarNotificacion('Documentos procesados exitosamente', 'success');
        //       // Opcional: limpiar storage después del procesamiento exitoso
        //       // limpiarStorage();
        //   })
        //   .catch(error => {
        //       mostrarNotificacion('Error al procesar documentos', 'error');
        //   });

        // Simulación de procesamiento exitoso
        setTimeout(() => {
            mostrarNotificacion('¡Todos los documentos han sido procesados correctamente!', 'success');
        }, 2000);
    }
}

// Función para limpiar todo el storage
function limpiarStorage() {
    archivosCargas.clear();
    // En un entorno real:
    // documentosRequeridos.forEach(doc => {
    //     localStorage.removeItem(`documento_${doc.Id}`);
    // });

    // Restaurar UI
    documentosRequeridos.forEach(doc => {
        removerArchivo(doc.Id);
    });

    mostrarNotificacion('Almacenamiento local limpiado', 'info');
}

// Funciones auxiliares
function mostrarCargando(docId, mostrar) {
    const progressBar = document.getElementById(`progress-${docId}`);
    if (mostrar) {
        progressBar.classList.add('progress-bar-animated', 'progress-bar-striped');
    } else {
        progressBar.classList.remove('progress-bar-animated', 'progress-bar-striped');
    }
}

function mostrarNotificacion(mensaje, tipo) {
    // Crear elemento de notificación
    const notificacion = document.createElement('div');
    notificacion.className = `alert alert-${tipo === 'error' ? 'danger' : tipo} alert-dismissible fade show position-fixed`;
    notificacion.style.cssText = 'top: 20px; right: 20px; z-index: 9999; max-width: 350px;';
    notificacion.innerHTML = `
                ${mensaje}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            `;

    document.body.appendChild(notificacion);

    // Auto-eliminar después de 5 segundos
    setTimeout(() => {
        if (notificacion.parentNode) {
            notificacion.remove();
        }
    }, 5000);
}

function restaurarArchivoEnUI(docId, documentoData) {
    const card = document.getElementById(`card-${docId}`);
    const uploadZone = card.querySelector('.upload-zone');
    const fileInfo = document.getElementById(`info-${docId}`);
    const requiredLabel = document.getElementById(`required-${docId}`);

    // Actualizar información del archivo
    document.getElementById(`filename-${docId}`).textContent = documentoData.nombre;
    document.getElementById(`filesize-${docId}`).textContent = formatBytes(documentoData.tamaño);

    // Mostrar información y ocultar zona de carga
    uploadZone.style.display = 'none';
    fileInfo.classList.add('show');
    card.classList.add('uploaded');
    card.classList.remove('required');
    requiredLabel.classList.remove('show');
}

// Inicializar al cargar la página
document.addEventListener('DOMContentLoaded', function () {
    generarTarjetasDocumentos();
    cargarDesdeStorage(); // Cargar documentos previamente guardados
    actualizarContadorDocumentos();

    // Marcar campos requeridos al cargar
    setTimeout(() => {
        marcarCamposRequeridos();
    }, 500);

    // Mostrar información de storage al cargar
    if (archivosCargas.size > 0) {
        mostrarNotificacion(`Se encontraron ${archivosCargas.size} documentos guardados previamente`, 'info');
    }
});