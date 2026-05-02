window.listaCodeudores = [];

// ─── GLOBAL: puede ser llamada desde carguedocumentoscodeudor.js ───────────
function validarFlujoCompleto() {
    const submitBtn = document.getElementById('submitBtn');
    const btnCodeudor = document.getElementById('btnCodeudor');

    if (!submitBtn || !btnCodeudor) return;

    // Solo actuar si estamos en el paso 4
    const stepActivo = document.querySelector('.step.active');
    const pasoActual = stepActivo ? parseInt(stepActivo.getAttribute('data-step')) : 0;
    if (pasoActual !== 4) return;

    const monto = Number(
        document.getElementById('montoSolicitado').value.replace(/\D/g, '')
    );

    // Si el monto está vacío no hacer nada
    if (!monto || monto === 0) {
        submitBtn.style.display = 'none';
        btnCodeudor.style.display = 'none';
        return;
    }

    const requiereCodeudor = monto > 2000000;
    const codeudorListo = window.listaCodeudores && window.listaCodeudores.length > 0;

    if (requiereCodeudor) {
        btnCodeudor.style.display = 'inline-block';

        if (codeudorListo) {
            submitBtn.style.display = 'inline-block';
            btnCodeudor.classList.remove('btn-warning');
            btnCodeudor.classList.add('btn-success');
            btnCodeudor.innerHTML = '<i class="fas fa-check me-2"></i> Codeudor Completado';
        } else {
            submitBtn.style.display = 'none';
            btnCodeudor.classList.remove('btn-success');
            btnCodeudor.classList.add('btn-warning');
            btnCodeudor.innerHTML = '<i class="fas fa-user-friends me-2"></i> Llenar Papeles del Codeudor';
        }
    } else {
        // Monto <= 2M: mostrar Enviar directamente, ocultar codeudor
        submitBtn.style.display = 'inline-block';
        btnCodeudor.style.display = 'none';
    }
}
// ──────────────────────────────────────────────────────────────────────────

document.addEventListener('DOMContentLoaded', function () {
    let currentStep = 1;
    const totalSteps = 4;

    const prevBtn = document.getElementById('prevBtn');
    const tratamientoDatos = document.getElementById('tratamientoDatos');
    const loanForm = document.getElementById('loanForm');
    const nextBtn = document.getElementById('nextBtn');
    const submitBtn = document.getElementById('submitBtn');
    const montoInput = document.getElementById('montoSolicitado');
    const btnCodeudor = document.getElementById('btnCodeudor');
    const modalCodeudor = new bootstrap.Modal(document.getElementById('modalCodeudor'));
    const modalCodeudorFormulario = new bootstrap.Modal(document.getElementById('modalCodeudorFormulario'));

    // ----------------------------------------------------------
    // Validación de cada paso
    // ----------------------------------------------------------
    function validateStep(stepNumber) {
        const currentSection = document.getElementById(`step${stepNumber}`);
        let isValid = true;

        currentSection.querySelectorAll('input[required], select[required]').forEach(field => {
            if (!field.value.trim()) {
                isValid = false;
                field.classList.add('is-invalid');

                let errorDiv = field.nextElementSibling;
                if (!errorDiv || !errorDiv.classList.contains('invalid-feedback')) {
                    errorDiv = document.createElement('div');
                    errorDiv.className = 'invalid-feedback';
                    field.parentNode.appendChild(errorDiv);
                }
                errorDiv.textContent = 'Este campo es obligatorio';
            } else {
                field.classList.remove('is-invalid');
                field.classList.add('is-valid');

                if (field.type === 'email') {
                    const emailRegex = /^[^\s]+@[^\s]+\.[^\s]+$/;
                    if (!emailRegex.test(field.value)) {
                        isValid = false;
                        field.classList.add('is-invalid');
                        let errorDiv = field.nextElementSibling;
                        if (!errorDiv || !errorDiv.classList.contains('invalid-feedback')) {
                            errorDiv = document.createElement('div');
                            errorDiv.className = 'invalid-feedback';
                            field.parentNode.appendChild(errorDiv);
                        }
                        errorDiv.textContent = 'Ingrese un correo electrónico válido';
                    }
                }

                if (field.id === 'celular') {
                    const phoneRegex = /^\d{10}$/;
                    if (!phoneRegex.test(field.value)) {
                        isValid = false;
                        field.classList.add('is-invalid');
                        let errorDiv = field.nextElementSibling;
                        if (!errorDiv || !errorDiv.classList.contains('invalid-feedback')) {
                            errorDiv = document.createElement('div');
                            errorDiv.className = 'invalid-feedback';
                            field.parentNode.appendChild(errorDiv);
                        }
                        errorDiv.textContent = 'Ingrese un número de celular válido (10 dígitos)';
                    }
                }
            }
        });

        if (!isValid) {
            const existing = currentSection.querySelector('.alert');
            if (existing) existing.remove();

            const alertDiv = document.createElement('div');
            alertDiv.className = 'alert alert-danger mt-3';
            alertDiv.role = 'alert';
            alertDiv.textContent = 'Por favor, complete todos los campos obligatorios correctamente.';
            currentSection.appendChild(alertDiv);
            setTimeout(() => alertDiv.remove(), 3000);
        }

        return isValid;
    }

    // ----------------------------------------------------------
    // Actualizar wizard visual
    // ----------------------------------------------------------
    function updateButtons() {
        prevBtn.style.display = currentStep === 1 ? 'none' : 'block';
        nextBtn.style.display = currentStep === totalSteps ? 'none' : 'block';
        // submitBtn lo maneja SOLO validarFlujoCompleto
        submitBtn.style.display = 'none';
    }

    function updateSteps() {
        document.querySelectorAll('.step').forEach((step, index) => {
            const n = index + 1;
            step.classList.toggle('completed', n < currentStep);
            step.classList.toggle('active', n === currentStep);
        });
        document.querySelectorAll('.form-section').forEach((section, index) => {
            section.classList.toggle('active', index + 1 === currentStep);
        });
    }

    function obtenerPasoActual() {
        const stepActivo = document.querySelector('.step.active');
        return stepActivo ? parseInt(stepActivo.getAttribute('data-step')) : null;
    }

    // ----------------------------------------------------------
    // Navegación: Siguiente
    // ----------------------------------------------------------
    nextBtn.addEventListener('click', () => {
        const pasoActual = obtenerPasoActual();
        const tratamientoOk = tratamientoDatos.value == 1;

        if (pasoActual === 3) {
            if (!validateStep(currentStep) || !tratamientoOk) {
                tratamientoDatos.classList.remove('is-valid');
                tratamientoDatos.classList.add('is-invalid');
                return;
            }

            tratamientoDatos.classList.remove('is-invalid');
            tratamientoDatos.classList.add('is-valid');

            const monto = Number(document.getElementById('montoSolicitado').value.replace(/\D/g, ''));

            // Mostrar modal informativa si monto > 2M
            if (monto > 2000000) {
                modalCodeudor.show();
            }

            currentStep++;
            updateSteps();
            updateButtons();

            // Al llegar al paso 4, validarFlujoCompleto decide qué botón mostrar
            validarFlujoCompleto();

        } else {
            if (!validateStep(currentStep)) return;

            currentStep++;
            updateSteps();
            updateButtons();
            // En pasos 1, 2: validarFlujoCompleto no hace nada (pasoActual !== 4)
            validarFlujoCompleto();
        }
    });

    // ----------------------------------------------------------
    // Navegación: Anterior
    // ----------------------------------------------------------
    prevBtn.addEventListener('click', () => {
        if (currentStep > 1) {
            currentStep--;
            updateSteps();
            updateButtons();
            // Si volvemos del paso 4, ocultar botones de acción
            submitBtn.style.display = 'none';
            btnCodeudor.style.display = 'none';
        }
    });

    // ----------------------------------------------------------
    // Submit
    // ----------------------------------------------------------
    loanForm.addEventListener('submit', async function (event) {
        event.preventDefault();
        event.stopPropagation();

        const montoValor = Number(document.getElementById('montoSolicitado').value.replace(/\D/g, ''));

        if (montoValor > 2000000 && (!window.listaCodeudores || window.listaCodeudores.length === 0)) {
            Swal.fire({
                icon: 'warning',
                title: 'Codeudor requerido',
                text: 'Debe agregar al menos un codeudor para montos superiores a $2.000.000'
            });
            return;
        }

        const doc1 = await extraerDocumentos(archivosCargas);

        const formData = {
            id: 0,
            p_nombre_solicitante: document.getElementById('pNombre').value.trim().toUpperCase(),
            s_nombre_solicitante: document.getElementById('sNombre').value.trim().toUpperCase() || null,
            p_apellido_solicitante: document.getElementById('pApellido').value.trim().toUpperCase(),
            s_apellido_solicitante: document.getElementById('sApellido').value.trim().toUpperCase() || null,
            tipo_identificacion: parseInt(document.getElementById('listTipoDocumentos').value),
            numero_identificacion: parseInt(document.getElementById('identificacion').value),
            id_depto_residencia: parseInt(document.getElementById('ListaDepartamentos').value),
            id_mpio_residencia: parseInt(document.getElementById('ListaMunicipios').value),
            id_barrio_residencia: parseInt(document.getElementById('ListaBarrios').value),
            direccion_residencia: document.getElementById('direccion').value,
            id_estado: 12,
            email: document.getElementById('email').value,
            celular: document.getElementById('celular').value,
            monto: montoValor,
            codigo_acceso: document.getElementById('codigoAcceso').value || null,
            habilitado: 1,
            usuario_creacion: 'WEB_USER',
            maquina_creacion: "123",
            fecha_creacion: new Date().toISOString(),
            lista_documentos: doc1,
            codeudores: [...window.listaCodeudores]
        };

        const originalText = submitBtn.innerHTML;
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Enviando...';

        console.log('PAYLOAD FINAL:', formData);

        try {
            const response = await fetch('/Solicitud/Prestamo/Add_Loan_Request', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                },
                body: JSON.stringify(formData)
            });

            const result = await response.json();

            if (response.ok && result.estado === true) {
                Swal.fire({
                    title: 'Señor(a) Usuario',
                    text: 'Su solicitud fue enviada exitosamente',
                    icon: 'success',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'Aceptar'
                }).then(() => {
                    window.location.href = '/Solicitud/prestamo/solicitar';
                    loanForm.reset();
                    window.listaCodeudores = [];
                });
            } else {
                showErrorMessage(result.mensaje || 'No se pudo enviar la solicitud. Intente nuevamente.');
            }
        } catch (error) {
            showErrorMessage('Error de conexión. Por favor, intenta nuevamente.');
        } finally {
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalText;
        }
    });

    // ----------------------------------------------------------
    // Botón codeudor del wizard → abre modal
    // ----------------------------------------------------------
    btnCodeudor.addEventListener('click', function () {
        modalCodeudorFormulario.show();
    });

    // ----------------------------------------------------------
    // Mensajes
    // ----------------------------------------------------------
    function showErrorMessage(message) {
        Swal.fire({ title: 'Señor usuario:', text: message, icon: 'error' });
    }

    // ----------------------------------------------------------
    // Validación en tiempo real - solo clases, sin tocar botones
    // Si estamos en paso 4, re-evaluar flujo
    // ----------------------------------------------------------
    document.querySelectorAll('input, select').forEach(field => {
        field.addEventListener('input', function () {
            if (this.hasAttribute('required')) {
                this.classList.toggle('is-valid', !!this.value.trim());
                this.classList.toggle('is-invalid', !this.value.trim());
            }
            // Solo recalcular botones si ya estamos en paso 4
            if (obtenerPasoActual() === 4) {
                validarFlujoCompleto();
            }
        });
    });

    // ----------------------------------------------------------
    // Tratamiento de datos
    // ----------------------------------------------------------
    tratamientoDatos.addEventListener('change', function () {
        this.value = this.checked ? '1' : '0';
        this.classList.toggle('is-valid', this.checked);
        this.classList.toggle('is-invalid', !this.checked);
    });

    // ----------------------------------------------------------
    // Formato monto COP
    // ----------------------------------------------------------
    montoInput.addEventListener('input', () => {
        const raw = montoInput.value.replace(/[^0-9]/g, '');
        if (!raw) { montoInput.value = ''; return; }
        montoInput.value = parseInt(raw, 10).toLocaleString('es-CO', {
            style: 'currency', currency: 'COP',
            minimumFractionDigits: 0, maximumFractionDigits: 0
        });
    });

    // ----------------------------------------------------------
    // Departamentos → Municipios → Barrios
    // ----------------------------------------------------------
    document.getElementById('ListaDepartamentos').addEventListener('change', async () => {
        const listaMunicipios = document.getElementById('ListaMunicipios');
        listaMunicipios.innerHTML = '';
        const sel = document.getElementById('ListaDepartamentos').value;
        if (!sel) {
            listaMunicipios.innerHTML = '<option value="" selected disabled>Seleccione...</option>';
            return;
        }
        try {
            const res = await fetch('/Solicitud/Prestamo/ConsultarMunicipios', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Tipo_Lugar: 'MU', Codigo_Dane_Padre: parseInt(sel) })
            });
            const post = await res.json();
            if (post.respuesta?.length > 0) {
                listaMunicipios.innerHTML = '<option value="" selected>Seleccione...</option>';
                post.respuesta.forEach(item => {
                    const opt = document.createElement('option');
                    opt.value = item.codigoDane;
                    opt.textContent = item.descripcion;
                    listaMunicipios.appendChild(opt);
                });
            }
        } catch {
            AlertaGenericaBase('error', 'Señor(a) Funcionario(a):', 'Error al consultar municipios.');
        }
    });

    document.getElementById('ListaMunicipios').addEventListener('change', async () => {
        const listaBarrios = document.getElementById('ListaBarrios');
        listaBarrios.innerHTML = '';
        const sel = document.getElementById('ListaMunicipios').value;
        if (!sel) {
            listaBarrios.innerHTML = '<option value="" selected disabled>Seleccione...</option>';
            return;
        }
        try {
            const res = await fetch('/Solicitud/Prestamo/ConsultarBarrios', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Codigo_Dane_Municipio: parseInt(sel) })
            });
            const post = await res.json();
            if (post.respuesta?.length > 0) {
                listaBarrios.innerHTML = '<option value="" selected>Seleccione...</option>';
                post.respuesta.forEach(item => {
                    const opt = document.createElement('option');
                    opt.value = item.id;
                    opt.textContent = item.nombre;
                    listaBarrios.appendChild(opt);
                });
            }
        } catch {
            AlertaGenericaBase('error', 'Señor(a) Funcionario(a):', 'Error al consultar barrios.');
        }
    });

    // ----------------------------------------------------------
    // Floating selects
    // ----------------------------------------------------------
    function updateFloatingSelect(el) {
        el.classList.toggle('has-value', el.value !== '' && el.value !== '0');
    }

    document.querySelectorAll('.floating-select .form-select').forEach(select => {
        updateFloatingSelect(select);
        select.addEventListener('change', () => updateFloatingSelect(select));
        select.addEventListener('focus', () => select.classList.add('focused'));
        select.addEventListener('blur', () => select.classList.remove('focused'));
    });

    // ----------------------------------------------------------
    // Extractor de documentos para el payload
    // ----------------------------------------------------------
    async function extraerDocumentos(documentosMap) {
        const documentos = [];
        documentosMap.forEach((documento) => {
            documentos.push({
                idSolicitud: 0,
                idDocumento: documento.id,
                contenidoDoc: documento.data,
                formato: documento.tipo,
                tamanio: documento.tamaño,
                usuarioCreacion: 'WEB_USER',
                maquinaCreacion: navigator.userAgent || 'WEB_CLIENT',
                habilitado: true
            });
        });
        return documentos;
    }

}); // fin DOMContentLoaded
