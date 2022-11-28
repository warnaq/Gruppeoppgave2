function loggUt() {
    $.get("observasjon/LoggUt", function () {
        window.location.href = 'loggInn.html';
    });
}

