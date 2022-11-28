$(function () {
    const id = window.location.search.substring(1);
    const url = "observasjon/HentEn?" + id;
    $.get(url, function (observasjon) {
        $("#id").val(observasjon.id); 
        $("#fornavn").val(observasjon.fornavn);
        $("#etternavn").val(observasjon.etternavn);
        $("#adresse").val(observasjon.adresse);
        $("#postnr").val(observasjon.postnr);
        $("#poststed").val(observasjon.poststed);
        $("#når").val(observasjon.Når);
        $("#beskrivelse").val(observasjon.beskrivelse);

    }); 
});

function validerOgEndreobservasjon() {
    const fornavnOK = validerFornavn($("#fornavn").val());
    const etternavnOK = validerEtternavn($("#etternavn").val());
    const adresseOK = validerAdresse($("#adresse").val());
    const postnrOK = validerPostnr($("#postnr").val());
    const poststedOK = validerPoststed($("#poststed").val());
    const nårOK = validerNår($("#når").val());
    const beskrivelseOK= validerFornavn($("#beskrivelse").val());
    if (fornavnOK && etternavnOK && adresseOK && postnrOK && poststedOK && nårOK && beksirvelseOK) {
        endreobservasjon();
    }
}

function endreobservasjon() {
    const observasjon = {
        id: $("#id").val(), 
        fornavn: $("#fornavn").val(),
        etternavn: $("#etternavn").val(),
        adresse: $("#adresse").val(),
        postnr: $("#postnr").val(),
        poststed: $("#poststed").val(),
        når: $("#når").val(),
        beskrivelse: $("#beksrivelse").val()
    };
    $.post("observasjon/Endre", observasjon, function () {
        window.location.href = 'index.html';
    })
    .fail(function (feil) {
        if (feil.status == 401) { 
            window.location.href = 'loggInn.html';  // ikke logget inn, redirect til loggInn.html
        }
        else {
            $("#feil").html("Feil på server - prøv igjen senere");
        }
    });
}