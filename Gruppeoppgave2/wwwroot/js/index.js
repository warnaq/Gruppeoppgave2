$(function(){
    hentAlleobservasjoner();
});

function hentAlleobservasjoner() {
    $.get("observasjon/hentAlle", function (observasjoner) {
        formaterobservasjoner(observasjoner);
    }) 
    .fail(function (feil) {
        if (feil.status == 401) {
            window.location.href = 'loggInn.html'; 
        }
        else {
            $("#feil").html("Feil på server - prøv igjen senere");
        }
    });
}

function formaterobservasjoner(observasjoner) {
    let ut = "<table class='table table-striped'>" +
        "<tr>" +
        "<th>Fornavn</th><th>Etternavn</th><th>Adresse</th><th>Postnr</th><th>Poststed</th><th></th><th></th>" +
        "</tr>";
    for (let observasjon of observasjoner) {
        ut += "<tr>" + 
            "<td>" + observasjon.fornavn + "</td>" +
            "<td>" + observasjon.etternavn + "</td>" +
            "<td>" + observasjon.adresse + "</td>" +
            "<td>" + observasjon.postnr + "</td>" +
            "<td>" + observasjon.poststed + "</td>" +
            "<td> <a class='btn btn-primary' href='endre.html?id=" + observasjon.id+"'>Endre</a></td>"+
            "<td> <button class='btn btn-danger' onclick='slettobservasjon(" + observasjon.id+")'>Slett</button></td>"+
            "</tr>";
    }
    ut += "</table>";
    $("#observasjonene").html(ut);
}

function slettobservasjon(id) {
    const url = "observasjon/Slett?id="+id;
    
    $.get(url, function () {
        window.location.href = 'index.html';
    })
    .fail(function (feil) {
        if (feil.status == 401) {
            window.location.href = 'loggInn.html'; // ikke logget inn, redirect til loggInn.html
        }
        else {
            $("#feil").html("Feil på server - prøv igjen senere");
        }
    });
}