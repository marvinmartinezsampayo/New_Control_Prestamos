document.addEventListener("DOMContentLoaded", function () {
    // Manejo de SweetAlerts para mensajes del servidor
    const alertDiv = document.getElementById('sweetAlertData');
    if (alertDiv) {
        const icon = alertDiv.getAttribute('data-icon');
        const titulo = alertDiv.getAttribute('data-titulo');
        const mensaje = alertDiv.getAttribute('data-mensaje');
        const limpiar = alertDiv.getAttribute('data-limpiar') === 'true';

        if (icon && mensaje) {
            Swal.fire({
                title: titulo || (icon === 'success' ? "Operación Exitosa" : "Error"),
                html: mensaje,
                icon: icon,
                confirmButtonColor: icon === 'success' ? "#28a745" : "#dc3545",
                confirmButtonText: "Aceptar",
                allowOutsideClick: false,
                allowEscapeKey: false,
                customClass: {
                    popup: 'animated bounceIn'
                }
            }).then((result) => {
                if (limpiar && result.isConfirmed) {
                    resetForm();
                }
            });
        }
    }

    // Función para resetear completamente el formulario
    function resetForm() {
        const form = document.getElementById('miFormulario');
        if (form) {
            form.reset();

            // Resetear selects a su primera opción
            const selects = form.querySelectorAll('select');
            selects.forEach(select => {
                select.selectedIndex = 0;
            });

            // Resetear checkbox de habilitado
            const habilitado = document.getElementById('habilitado');
            if (habilitado) {
                habilitado.checked = true;
            }

            // Enfocar el primer campo
            const primerCampo = document.getElementById('usuarioEmpresarial');
            if (primerCampo) {
                primerCampo.focus();
            }
        }
    }

    // Validación del formulario antes de enviar
    const form = document.getElementById('miFormulario');
    if (form) {
        form.addEventListener('submit', function (event) {
            // Validación básica de campos requeridos
            const requiredFields = form.querySelectorAll('[required]');
            let isValid = true;
            let firstInvalidField = null;

            requiredFields.forEach(field => {
                if (!field.value.trim()) {
                    isValid = false;
                    field.classList.add('is-invalid');

                    if (!firstInvalidField) {
                        firstInvalidField = field;
                    }
                } else {
                    field.classList.remove('is-invalid');
                }
            });

            // Validación específica para contraseña
            const password = document.getElementById('newcontrasena');
            if (password && password.value.length > 0 && (password.value.length < 8 || password.value.length > 20)) {
                isValid = false;
                password.classList.add('is-invalid');
                if (!firstInvalidField) {
                    firstInvalidField = password;
                }

                Swal.fire({
                    title: "Error en Contraseña",
                    text: "La contraseña debe tener entre 8 y 20 caracteres.",
                    icon: "error",
                    confirmButtonColor: "#dc3545"
                });
            }

            // Validación de email
            const email = document.getElementById('email');
            if (email && email.value && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)) {
                isValid = false;
                email.classList.add('is-invalid');
                if (!firstInvalidField) {
                    firstInvalidField = email;
                }

                Swal.fire({
                    title: "Error en Email",
                    text: "Por favor ingrese un correo electrónico válido.",
                    icon: "error",
                    confirmButtonColor: "#dc3545"
                });
            }

            if (!isValid) {
                event.preventDefault();
                event.stopPropagation();

                if (firstInvalidField) {
                    firstInvalidField.scrollIntoView({
                        behavior: 'smooth',
                        block: 'center'
                    });
                }
            } else {
                // Mostrar loader mientras se procesa
                Swal.fire({
                    title: "Procesando",
                    html: "Por favor espere mientras registramos al usuario...",
                    allowOutsideClick: false,
                    allowEscapeKey: false,
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });
            }
        });
    }

    // Manejo de eventos para mejorar la experiencia de usuario
    const inputs = document.querySelectorAll('input, select');
    inputs.forEach(input => {
        // Remover clases de error al empezar a escribir
        input.addEventListener('input', function () {
            if (this.classList.contains('is-invalid')) {
                this.classList.remove('is-invalid');
            }
        });

        // Manejar enter para navegar entre campos
        input.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                const form = document.getElementById('miFormulario');
                const inputs = form.querySelectorAll('input, select, textarea');
                const currentIndex = Array.from(inputs).indexOf(this);

                if (currentIndex < inputs.length - 1) {
                    inputs[currentIndex + 1].focus();
                }
            }
        });
    });

    // Validación en tiempo real para el número de identificación
    const nroIdentificacion = document.getElementById('nroIdentificacion');
    if (nroIdentificacion) {
        nroIdentificacion.addEventListener('input', function () {
            if (this.value.length > 15) {
                this.value = this.value.slice(0, 15);
            }
        });
    }

    // Validación en tiempo real para el teléfono
    const telefono = document.getElementById('telefono');
    if (telefono) {
        telefono.addEventListener('input', function () {
            this.value = this.value.replace(/[^0-9]/g, '');
            if (this.value.length > 10) {
                this.value = this.value.slice(0, 10);
            }
        });
    }
});