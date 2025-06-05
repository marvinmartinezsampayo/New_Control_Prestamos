document.addEventListener('DOMContentLoaded', function () {
    let currentStep = 1;
    const totalSteps = 4;
    const form = document.getElementById('loanForm');
    const nextBtn = document.getElementById('nextBtn');
    const prevBtn = document.getElementById('prevBtn');
    const submitBtn = document.getElementById('submitBtn');

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
        if (validateStep(currentStep)) {
            if (currentStep < totalSteps) {
                currentStep++;
                updateSteps();
                updateButtons();
            }
        }
    });

    prevBtn.addEventListener('click', () => {
        if (currentStep > 1) {
            currentStep--;
            updateSteps();
            updateButtons();
        }
    });

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
});