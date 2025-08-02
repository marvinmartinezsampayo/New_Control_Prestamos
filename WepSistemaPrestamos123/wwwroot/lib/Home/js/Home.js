document.addEventListener("DOMContentLoaded", function () {
    // Espera un segundo y oculta el loader con una transición suave
    const loader = document.getElementById("loader");
    loader.style.opacity = 1;

    setTimeout(() => {
        loader.style.transition = "opacity 0.5s ease-out";
        loader.style.opacity = 0;
        setTimeout(() => loader.style.display = "none", 500);
    }, 1000);
});
