document.addEventListener('DOMContentLoaded', function () {

    const inputIdentificacion = document.getElementById('nroIdentificacion');
    const btnBuscar = document.getElementById('btnBuscar');
    const resultadoUsuario = document.getElementById('resultadoUsuario');
    const defaultFoto = '/IMG/Imagenes/default-user.png';

    // Solo permitir números
    inputIdentificacion.addEventListener('input', function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    // Buscar con Enter o con el botón
    inputIdentificacion.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') buscarUsuario();
    });
    btnBuscar.addEventListener('click', buscarUsuario);

    // ✅ Fetch al controlador — buscar usuario
    async function buscarUsuario() {
        const id = inputIdentificacion.value.trim();

        if (!id) {
            mostrarAlerta('warning', 'Campo vacío', 'Por favor ingresa un número de identificación.');
            return;
        }

        try {
            const response = await fetch(`/Administracion/AdministraUsuarios/BuscarPorIdentificacion?nroIdentificacion=${id}`);
            const data = await response.json();

            if (!data.exito) {
                resultadoUsuario.classList.add('d-none');
                mostrarAlerta('error', 'No encontrado', data.mensaje);
                return;
            }

            // ✅ El JS llena la UI con los datos que llegaron
            document.getElementById('fotoUsuario').src = data.usuario.foto || defaultFoto;
            document.getElementById('nombreUsuario').innerText = data.usuario.nombreCompleto;
            document.getElementById('usuarioEmpresarial').innerText = data.usuario.usuarioEmpresarial;
            document.getElementById('identificacionUsuario').innerText = data.usuario.nroIdentificacion;
            document.getElementById('telefonoUsuario').innerText = data.usuario.telefono;
            document.getElementById('idUsuarioActual').value = data.usuario.id;

            // Llenar roles asignados
            const listaRoles = document.getElementById('listaRoles');
            const sinRoles = document.getElementById('sinRoles');
            listaRoles.innerHTML = '';

            if (data.roles && data.roles.length > 0) {
                sinRoles.classList.add('d-none');
                data.roles.forEach(rol => {
                    const li = document.createElement('li');
                    li.innerHTML = `<strong>${rol.nombre}</strong> - ${rol.descripcion}`;
                    listaRoles.appendChild(li);
                });
            } else {
                sinRoles.classList.remove('d-none');
            }

            resultadoUsuario.classList.remove('d-none');
            mostrarAlerta('success', 'Usuario encontrado', `El usuario fue encontrado exitosamente.`);

        } catch (error) {
            mostrarAlerta('error', 'Error', 'Ocurrió un error al buscar el usuario.');
        }
    }

    // ✅ Fetch al controlador — gestionar rol
    async function gestionarRol(accion) {
        const idUsuario = document.getElementById('idUsuarioActual').value;
        const idRol = document.getElementById('rolSeleccionado').value;

        if (!idRol) {
            mostrarAlerta('warning', 'Sin rol', 'Debes seleccionar un rol.');
            return;
        }

        try {
            const response = await fetch('/Administracion/AdministraUsuarios/GestionarRol', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    idUsuario: parseInt(idUsuario),
                    idRol: parseInt(idRol),
                    accion: accion
                })
            });

            const data = await response.json();

            if (data.exito) {
                mostrarAlerta('success', 'Rol actualizado', data.mensaje, () => {
                    // Refresca los roles del usuario automáticamente
                    buscarUsuario();
                });
            } else {
                mostrarAlerta('error', 'Error', data.mensaje);
            }

        } catch (error) {
            mostrarAlerta('error', 'Error', 'Ocurrió un error al gestionar el rol.');
        }
    }

    document.getElementById('btnAsignar').addEventListener('click', () => gestionarRol('asignar'));
    document.getElementById('btnQuitar').addEventListener('click', () => gestionarRol('quitar'));

    // Función reutilizable para SweetAlert
    function mostrarAlerta(icon, titulo, mensaje, callback = null) {
        Swal.fire({
            icon,
            title: titulo,
            html: mensaje,
            confirmButtonText: 'Aceptar',
            confirmButtonColor: icon === 'success' ? '#28a745' : '#dc3545',
            allowOutsideClick: false,
            allowEscapeKey: false
        }).then((result) => {
            if (result.isConfirmed && callback) callback();
        });
    }

    // Limpiar input al salir
    window.addEventListener('beforeunload', function () {
        inputIdentificacion.value = '';
    });
});