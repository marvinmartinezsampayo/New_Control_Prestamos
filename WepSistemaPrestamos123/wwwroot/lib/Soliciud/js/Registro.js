document.addEventListener('DOMContentLoaded', function () {
    let currentStep = 1;
    const totalSteps = 4;
    const form = document.getElementById('loanForm');
    const nextBtn = document.getElementById('nextBtn');
    const prevBtn = document.getElementById('prevBtn');
    const submitBtn = document.getElementById('submitBtn');
    const tratamientoDatos = document.getElementById('tratamientoDatos');
    const loanForm = document.getElementById('loanForm');
    const monto = document.getElementById('montoSolicitado');

    function validateStep(stepNumber) {
        const currentSection = document.getElementById(`step${stepNumber}`);
        let isValid = true;

        // Obtener todos los campos requeridos del paso actual
        const requiredFields = currentSection.querySelectorAll('input[required], select[required]');

        // Validar cada campo requerido
        requiredFields.forEach(field => {
            if (!field.value.trim()) {
                isValid = false;
                field.classList.add('is-invalid');

                // Crear o actualizar mensaje de error
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

                // Validaciones específicas por tipo de campo
                if (field.type === 'email') {
                    const emailRegex = /^[^\s]+[^\s]+\.[^\s]+$/;
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

        // Si el paso no es válido, mostrar mensaje general
        if (!isValid) {
            const alertDiv = document.createElement('div');
            alertDiv.className = 'alert alert-danger mt-3';
            alertDiv.role = 'alert';
            alertDiv.textContent = 'Por favor, complete todos los campos obligatorios correctamente.';

            // Remover alerta anterior si existe
            const existingAlert = currentSection.querySelector('.alert');
            if (existingAlert) {
                existingAlert.remove();
            }

            currentSection.appendChild(alertDiv);

            // Remover la alerta después de 3 segundos
            setTimeout(() => {
                alertDiv.remove();
            }, 3000);
        }

        return isValid;
    }

    function updateButtons() {
        prevBtn.style.display = currentStep === 1 ? 'none' : 'block';
        nextBtn.style.display = currentStep === totalSteps ? 'none' : 'block';
        submitBtn.style.display = currentStep === totalSteps ? 'block' : 'none';
    }

    function updateSteps() {
        document.querySelectorAll('.step').forEach((step, index) => {
            if (index + 1 < currentStep) {
                step.classList.add('completed');
                step.classList.remove('active');
            } else if (index + 1 === currentStep) {
                step.classList.add('active');
                step.classList.remove('completed');
            } else {
                step.classList.remove('active', 'completed');
            }
        });

        document.querySelectorAll('.form-section').forEach((section, index) => {
            if (index + 1 === currentStep) {
                section.classList.add('active');
            } else {
                section.classList.remove('active');
            }
        });
    }

    nextBtn.addEventListener('click', () => {
        const pasoActual = obtenerPasoActual();
        const sw = tratamientoDatos.value == 1 ? true : false;
        if (pasoActual === 3 ) {
            

            if (validateStep(currentStep) && sw) {
                tratamientoDatos.classList.remove("is-invalid");
                tratamientoDatos.classList.add("is-valid");

                if (currentStep < totalSteps) {
                    currentStep++;
                    updateSteps();
                    updateButtons();
                }
            }
            else {
                tratamientoDatos.classList.remove("is-valid");
                tratamientoDatos.classList.add("is-invalid");
            }
        }
        else
        {
            if (validateStep(currentStep)) {
                if (currentStep < totalSteps) {
                    currentStep++;
                    updateSteps();
                    updateButtons();
                }
            }

            tratamientoDatos.classList.remove("is-valid");
            tratamientoDatos.classList.add("is-invalid");
        }
    });

    prevBtn.addEventListener('click', () => {
        if (currentStep > 1) {
            currentStep--;
            updateSteps();
            updateButtons();
        }
    });
    loanForm.addEventListener('submit',async function (event) {
        event.preventDefault();
        event.stopPropagation();

        if (!loanForm.checkValidity()) {
            loanForm.classList.add('was-validated');
            console.log('Formulario tiene errores de validación');
            return;
        }

        ////Armamos el documento
        var doc1 = await extraerDocumentos(archivosCargas);
        var doc = JSON.stringify(await extraerDocumentos(archivosCargas));
                       
        const formData = {
            id: 0, // Nuevo registro
            p_nombre_solicitante: document.getElementById('pNombre').value,
            s_nombre_solicitante: document.getElementById('sNombre').value || null,
            p_apellido_solicitante: document.getElementById('pApellido').value,
            s_apellido_solicitante: document.getElementById('sApellido').value || null,
            tipo_identificacion: parseInt(document.getElementById('listTipoDocumentos').value),
            numero_identificacion: parseInt(document.getElementById('identificacion').value),
            id_depto_residencia: parseInt(document.getElementById('ListaDepartamentos').value),
            id_mpio_residencia: parseInt(document.getElementById('ListaMunicipios').value),
            id_barrio_residencia: parseInt(document.getElementById('ListaBarrios').value),
            direccion_residencia: document.getElementById('direccion').value,
            id_estado: 12, // No veo este campo en el formulario, asignar valor por defecto
            email: document.getElementById('email').value,
            celular: document.getElementById('celular').value,
            monto: document.getElementById('montoSolicitado').value,
            codigo_acceso: document.getElementById('codigoAcceso').value || null,
            habilitado: 1, // Habilitado por defecto
            usuario_creacion: "WEB_USER", // O el usuario actual si lo tienes
            maquina_creacion: navigator.userAgent || "WEB_CLIENT",
            fecha_creacion: new Date().toISOString(),
            lista_documentos: doc1
        };

        // Deshabilitar el botón de envío
        const submitBtn = document.getElementById('submitBtn');
        const originalText = submitBtn.innerHTML;
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Enviando...';

        try {
            // Realizar la petición POST
            const response = await fetch('/Solicitud/Prestamo/Add_Loan_Request', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    // Si usas token CSRF en ASP.NET MVC
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                },
                body: JSON.stringify(formData)
            });

            if (response.ok) {
                const result = await response.json();


                Swal.fire({
                    title: "Señor(a) Usuario",
                    text: "Su solicitud fue enviada exitosamente",
                    icon: "success",
                    showCancelButton: false,
                    confirmButtonColor: "#3085d6",
                    cancelButtonColor: "#d33",
                    confirmButtonText: "Aceptar"
                }).then((result) => {
                    //if (result.isConfirmed) {
                    //    Swal.fire({
                    //        title: "Deleted!",
                    //        text: "Your file has been deleted.",
                    //        icon: "success"
                    //    });
                    //}
                    window.location.href = '/Solicitud/prestamo/solicitar';
                    loanForm.reset();
                });
                // Mostrar mensaje de éxito
                //showSuccessMessage('Solicitud enviada correctamente');
                //redirigir o limpiar formulario               

            }
            else
            {
                // Error del servidor
                const errorData = await response.json();
                /*console.error('Error del servidor:', errorData);*/
                showErrorMessage('Error al enviar la solicitud: ' + (errorData.message || 'Error desconocido'));
            }

        } catch (error) {
            // Error de red o JavaScript
           /* console.error('Error:', error);*/
            showErrorMessage('Error de conexión. Por favor, intenta nuevamente.');
        } finally {
            // Rehabilitar el botón
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalText;
        }

    });

    // Funciones auxiliares para mostrar mensajes
    function showSuccessMessage(message) {
        Swal.fire({
            title: "Señor usuario:",
            text: message,
            icon: "success"
        });
        /*Puedes usar SweetAlert, Bootstrap alerts, o crear tu propio sistema*/
        //alert(message); // Temporal - reemplaza con tu sistema de notificaciones
    }

    function showErrorMessage(message) {
        Swal.fire({
            title: "Señor usuario:",
            text: message,
            icon: "error"
        });
        // Puedes usar SweetAlert, Bootstrap alerts, o crear tu propio sistema
        //alert(message); // Temporal - reemplaza con tu sistema de notificaciones
    }

    // Validación en tiempo real

    document.querySelectorAll('input, select').forEach(field => {
        field.addEventListener('input', function () {
            if (this.hasAttribute('required')) {
                if (this.value.trim()) {
                    this.classList.remove('is-invalid');
                    this.classList.add('is-valid');
                } else {
                    this.classList.remove('is-valid');
                    this.classList.add('is-invalid');
                }
            }
        });
    });

    form.addEventListener('submit', function (event) {
        event.preventDefault();
        if (validateStep(currentStep) && form.checkValidity()) {
            // Aquí iría la lógica para enviar el formulario
            alert('Formulario enviado correctamente');
        }
        form.classList.add('was-validated');
    });

    document.querySelectorAll('.floating-label select').forEach(select => {
        select.addEventListener('change', function () {
            if (this.value !== '') {
                this.classList.add('has-value');
            } else {
                this.classList.remove('has-value');
            }
        });

        // Verificar estado inicial
        if (select.value !== '') {
            select.classList.add('has-value');
        }
    });

    async function extraerDocumentos(documentosMap) {
        const documentos = [];
        documentosMap.forEach((documento, key) => {
            const dto = {
                idSolicitud: 123,
                idDocumento: documento.id,
                contenidoDoc: documento.data,
                formato: documento.tipo,
                tamanio: documento.tamaño,
                usuarioCreacion: "usuarioCreacion",
                maquinaCreacion: "maquinaCreacion",
                habilitado: true
            };
            documentos.push(dto);
        });

        return documentos;
    }

    //*************************/
    //*********Eventos**********
    //*************************/

    document.getElementById("ListaDepartamentos").addEventListener("change", async () => {
        const listaMunicipios = document.getElementById("ListaMunicipios");

        // Limpiar el select de municipios
        listaMunicipios.innerHTML = "";

        try {
            const listaDepartamentos = document.getElementById("ListaDepartamentos");
            const sel = listaDepartamentos.value;

            if (sel === "" || sel === null) {
                listaMunicipios.innerHTML = '<option value="" selected disabled>Seleccione...</option>';
            }
            else {               
                const data = {
                    Tipo_Lugar: "MU",
                    Codigo_Dane_Padre: parseInt(listaDepartamentos.value) 
                };

                const resPost = await fetch('/Solicitud/Prestamo/ConsultarMunicipios', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(data)
                });

                const post = await resPost.json();

                if (post.respuesta.length > 0) {
                    listaMunicipios.innerHTML = '<option value="" selected>Seleccione...</option>';

                    // Iterar sobre los resultados y agregar opciones
                    post.respuesta.forEach(function (item) {
                        const option = document.createElement('option');
                        option.value = item.codigoDane;
                        option.textContent = item.descripcion;
                        listaMunicipios.appendChild(option);
                    });
                }
            }
        } catch (e) {
            AlertaGenericaBase("error", "Señor(a) Funcionario(a:)", "Se presento un error al consultar los municipios del departamento seleccionado.");
        }
    });

    document.getElementById("ListaMunicipios").addEventListener("change", async () => {
        const listaBarrios = document.getElementById("ListaBarrios");

        // Limpiar el select de barrios
        listaBarrios.innerHTML = "";

        try {
            const listaMunicipios = document.getElementById("ListaMunicipios");
            const sel = listaMunicipios.value;

            if (sel === "" || sel === null) {
                listaBarrios.innerHTML = '<option value="" selected disabled>Seleccione...</option>';
            }
            else {
                const data = {                   
                    Codigo_Dane_Municipio: parseInt(listaMunicipios.value)
                };

                const resPost = await fetch('/Solicitud/Prestamo/ConsultarBarrios', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(data)
                });

                const post = await resPost.json();

                if (post.respuesta.length > 0) {
                    listaBarrios.innerHTML = '<option value="" selected>Seleccione...</option>';

                    // Iterar sobre los resultados y agregar opciones
                    post.respuesta.forEach(function (item) {
                        const option = document.createElement('option');
                        option.value = item.id;
                        option.textContent = item.nombre;
                        listaBarrios.appendChild(option);
                    });
                }
            }
        } catch (e) {
            AlertaGenericaBase("error", "Señor(a) Funcionario(a:)", "Se presento un error al consultar los municipios del departamento seleccionado.");
        }
    });

    tratamientoDatos.addEventListener('change', function () {
        this.classList.toggle('is-valid', this.checked);
        this.classList.toggle('is-invalid', !this.checked);
        this.value = this.checked ? '1' : '0';
    });

    monto.addEventListener('input', (e) => {
        // Guardamos la posición del cursor para mantenerla después del formateo
        const cursorPosition = monto.selectionStart;

        // Eliminamos todo lo que no sea número para evitar caracteres inválidos
        let valorSinFormato = monto.value.replace(/[^0-9]/g, '');

        if (valorSinFormato === '') {
            monto.value = '';
            return;
        }

        // Convertimos el valor a número para formatearlo
        const numero = parseInt(valorSinFormato, 10);

        // Formateamos el número a pesos colombianos sin decimales
        const valorFormateado = numero.toLocaleString('es-CO', {
            style: 'currency',
            currency: 'COP',
            minimumFractionDigits: 0,
            maximumFractionDigits: 0
        });
        monto.value = valorFormateado;
    });

    //***********************************/
    //*********Floating Select **********/
    //**********************************/

    // Función para manejar el estado flotante de los selects
    function updateFloatingSelect(selectElement) {
        const hasValue = selectElement.value !== "" && selectElement.value !== "0";

        if (hasValue) {
            selectElement.classList.add('has-value');
        } else {
            selectElement.classList.remove('has-value');
        }
    }

    // Inicializar todos los selects flotantes al cargar la página
    document.addEventListener('DOMContentLoaded', function () {
        // Seleccionar todos los selects dentro de .floating-select
        const floatingSelects = document.querySelectorAll('.floating-select .form-select');

        floatingSelects.forEach(function (select) {
            // Configurar el estado inicial
            updateFloatingSelect(select);

            // Agregar event listener para cambios
            select.addEventListener('change', function () {
                updateFloatingSelect(this);
            });

            // Agregar event listener para focus
            select.addEventListener('focus', function () {
                this.classList.add('focused');
            });

            // Agregar event listener para blur
            select.addEventListener('blur', function () {
                this.classList.remove('focused');
            });
        });
    });

    // También puedes llamar esta función manualmente cuando cambies opciones dinámicamente
    function initializeFloatingSelects() {
        const floatingSelects = document.querySelectorAll('.floating-select .form-select');
        floatingSelects.forEach(function (select) {
            updateFloatingSelect(select);
        });
    }

    function obtenerPasoActual() {
        const stepActivo = document.querySelector('.step.active');
        return stepActivo ? parseInt(stepActivo.getAttribute('data-step')) : null;
    }

});