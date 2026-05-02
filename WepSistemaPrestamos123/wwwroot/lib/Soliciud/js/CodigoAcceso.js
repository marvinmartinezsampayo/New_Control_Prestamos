const txtCod = document.getElementById('txt_codigo');
const btnValida = document.getElementById('btnValidarCod');
const txtError = document.getElementById('txtError');

btnValida.addEventListener('click', function (e) {
    const regex = /^[A-Za-z0-9]+$/;
    const inputValue = txtCod.value;

    if (!regex.test(inputValue) || (inputValue.length < 5 && inputValue.length > 10))
    {
        txtCod.classList.add('is-invalid');
        txtError.innerText = "El código es requerido y debe tener entre 5 y 10 caracteres, sin caracteres especiales.";
    }
    else
    {
        txtCod.classList.remove('is-invalid');

        var respValidar = ValidarCodigo(inputValue);

    }
}, false);

//console.log("CodigoAcceso cargado");


//document.addEventListener("DOMContentLoaded", function () {

//    const txtCod = document.getElementById('txt_codigo');
//    const btnValida = document.getElementById('btnValidarCod');
//    const txtError = document.getElementById('txtError');

//    btnValida.addEventListener('click', function (e) {

//        const regex = /^[A-Za-z0-9]+$/;
//        const inputValue = txtCod.value;

//        console.log("el cdigo es" + inputValue)

//        if (!regex.test(inputValue) || inputValue.length < 5 || inputValue.length > 10) {
//            txtCod.classList.add('is-invalid');
//            txtError.innerText = "El código es requerido y debe tener entre 5 y 10 caracteres, sin caracteres especiales.";
//        }
//        else {
//            txtCod.classList.remove('is-invalid');
//            ValidarCodigo(inputValue);
//        }

//    });

//});


async function ValidarCodigo(codigo)
{
    try {
        if (codigo === null || codigo === undefined || codigo === "") {

        }
        else {
            const data = new FormData();
            data.append('_codigoUser', codigo);

            console.log("BASE URL:", window.appConfig);

            const resPost = await fetch( `${window.appConfig.baseUrl}Solicitud/Prestamo/ValidarCodigo`, {
            /*const resPost = await fetch("/Solicitud/Prestamo/ValidarCodigo", {*/
                method: 'POST',
                body: data
            });


            const post = await resPost.json();

            if (post.estado) {

                txtCod.classList.remove('is-invalid');

                console.log("POST:", post);
                console.log("txtCod:", txtCod);

                var url = `${window.appConfig.baseUrl}Solicitud/Prestamo/Registro?json=${encodeURIComponent(post.respuesta)}`;

               window.location.href = url;

            }
            else
            {
                txtCod.classList.add('is-invalid');   
                txtError.innerText = "Codigo no valido, verifique e intente nuevamente.";
            }

        }

    }
    catch (e) {
        console.error("ERROR REAL:", e);
    }
}

