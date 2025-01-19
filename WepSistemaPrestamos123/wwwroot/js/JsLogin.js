var inactivityTime = function () {
    var time;

    // Redirigir al login si se detecta inactividad
    function logout() {
        window.location.href = '/Login/Login';  // Cambia la URL si es necesario
    }

    // Resetea el temporizador de inactividad
    function resetTimer() {
        clearTimeout(time);
        time = setTimeout(logout, 60000);  // ms = 1 minutos
    }

    // Eventos que detectan actividad del usuario
    window.onload = resetTimer;
    window.onmousemove = resetTimer;
    window.onmousedown = resetTimer;
    window.ontouchstart = resetTimer;
    window.onclick = resetTimer;
    window.onkeypress = resetTimer;
    window.addEventListener('scroll', resetTimer, true);
};

inactivityTime();  // Inicia el temporizador




//// scripts.js
//document.addEventListener('DOMContentLoaded', function () {
//    const videos = [
//        document.getElementById('backgroundVideo1'),
//        document.getElementById('backgroundVideo2'),
//        document.getElementById('backgroundVideo3')
//    ];

//    let currentIndex = 0;

//    // Configura los videos
//    videos.forEach((video, index) => {        
//        video.src = `img/videos/${index + 1}.mp4`;
//        video.addEventListener('ended', function () {
//            // Reproduce el siguiente video cuando el actual termina
//            currentIndex = (currentIndex + 1) % videos.length;
//            videos[currentIndex].play();
//        });
//    });

//    // Inicia la reproducción del primer video
//    if (videos.length > 0) {
//        videos[0].play();
//    }
//});
