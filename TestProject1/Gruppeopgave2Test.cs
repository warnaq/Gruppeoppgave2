using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Gruppeoppgave2.Controllers;
using Gruppeoppgave2.DAL;
using Gruppeoppgave2.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Gruppeoppgave2Test
{
    public class GruppeoppgaveTest
    {
        private const string _loggetInn = "loggetInn";
        private const string _ikkeLoggetInn = "";

        private readonly Mock<IobservasjonRepository> mockRep = new Mock<IobservasjonRepository>();
        private readonly Mock<ILogger<observasjonController>> mockLog = new Mock<ILogger<observasjonController>>();

        private readonly Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
        private readonly MockHttpSession mockSession = new MockHttpSession();

        [Fact]
        public async Task HentAlleLoggetInnOK()
        {
            // Arrange
            var observasjon1 = new observasjon {Id = 1,Fornavn = "Per",Etternavn = "Hansen",Adresse = "Askerveien 82",
                                    Postnr = "1370",Poststed = "Asker"};
            var observasjon2 = new observasjon {Id = 2,Fornavn = "Ole",Etternavn = "Olsen",Adresse = "Osloveien 82",
                                    Postnr = "0270",Poststed = "Oslo"};
            var observasjon3 = new observasjon {Id = 1,Fornavn = "Finn",Etternavn = "Finnsen",Adresse = "Bergensveien 82",
                                    Postnr = "5000",Poststed = "Bergen"};

            var observasjonListe = new List<observasjon>();
            observasjonListe.Add(observasjon1);
            observasjonListe.Add(observasjon2);
            observasjonListe.Add(observasjon3);

            mockRep.Setup(k => k.HentAlle()).ReturnsAsync(observasjonListe);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.HentAlle() as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK,resultat.StatusCode);
            Assert.Equal<List<observasjon>>((List<observasjon>)resultat.Value, observasjonListe);
        }

        [Fact]
        public async Task HentAlleIkkeLoggetInn()
        {
            // Arrange

            //var tomListe = new List<observasjon>();

            mockRep.Setup(k => k.HentAlle()).ReturnsAsync(It.IsAny<List<observasjon>>());

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.HentAlle() as UnauthorizedObjectResult;
           
            // Assert 
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }
   
        [Fact]
        public async Task LagreLoggetInnOK()
        {

            /*  Kan unngå denne med It.IsAny<observasjon>
            var observasjon1 = new observasjon {Id = 1,Fornavn = "Per",Etternavn = "Hansen",Adresse = "Askerveien 82",
                                    Postnr = "1370",Poststed = "Asker"};
            */

            // Arrange

            mockRep.Setup(k => k.Lagre(It.IsAny<observasjon>())).ReturnsAsync(true);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.Lagre(It.IsAny<observasjon>()) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal("observasjonen lagret", resultat.Value);
        }

        [Fact]
        public async Task LagreLoggetInnIkkeOK()
        {
            // Arrange

            mockRep.Setup(k => k.Lagre(It.IsAny<observasjon>())).ReturnsAsync(false);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.Lagre(It.IsAny<observasjon>()) as BadRequestObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("observasjonen kunne ikke lagres", resultat.Value);
        }

        [Fact]
        public async Task LagreLoggetInnFeilModel()
        {
            // Arrange
            // observasjonen er indikert feil med tomt fornavn her.
            // det har ikke noe å si, det er introduksjonen med ModelError under som tvinger frem feilen
            // kunnde også her brukt It.IsAny<observasjon>
            var observasjon1 = new observasjon {Id = 1,Fornavn = "",Etternavn = "Hansen",Adresse = "Askerveien 82",
                                    Postnr = "1370",Poststed = "Asker", Når="11", Beskrivelse="Jeg så en UFO ved stranda"};

            mockRep.Setup(k => k.Lagre(observasjon1)).ReturnsAsync(true);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            observasjonController.ModelState.AddModelError("Fornavn", "Feil i inputvalidering på server");

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.Lagre(observasjon1) as BadRequestObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalidering på server", resultat.Value);
        }

        [Fact]
        public async Task LagreIkkeLoggetInn()
        {
            mockRep.Setup(k => k.Lagre(It.IsAny<observasjon>())).ReturnsAsync(true);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.Lagre(It.IsAny<observasjon>()) as UnauthorizedObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task SlettLoggetInnOK()
        {
            // Arrange

            mockRep.Setup(k => k.Slett(It.IsAny<int>())).ReturnsAsync(true);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.Slett(It.IsAny<int>()) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal("observasjonen slettet", resultat.Value);
        }

        [Fact]
        public async Task SlettLoggetInnIkkeOK()
        {
            // Arrange

            mockRep.Setup(k => k.Slett(It.IsAny<int>())).ReturnsAsync(false);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.Slett(It.IsAny<int>()) as NotFoundObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Sletting av observasjonen ble ikke utført", resultat.Value);
        }

        [Fact]
        public async Task SletteIkkeLoggetInn()
        {
            mockRep.Setup(k => k.Slett(It.IsAny<int>())).ReturnsAsync(true);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.Slett(It.IsAny<int>()) as UnauthorizedObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentEnLoggetInnOK()
        {
            // Arrange
            var observasjon1 = new observasjon
            {
                Id = 1,
                Fornavn = "Per",
                Etternavn = "Hansen",
                Adresse = "Askerveien 82",
                Postnr = "1370",
                Poststed = "Asker",
                Når = "11",
                Beskrivelse = "Jeg så en UFO ved stranda"
            };

            mockRep.Setup(k => k.HentEn(It.IsAny<int>())).ReturnsAsync(observasjon1);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.HentEn(It.IsAny<int>()) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<observasjon>(observasjon1, (observasjon)resultat.Value);
        }

        [Fact]
        public async Task HentEnLoggetInnIkkeOK()
        {
            // Arrange

            mockRep.Setup(k => k.HentEn(It.IsAny<int>())).ReturnsAsync(()=>null); // merk denne null setting!

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.HentEn(It.IsAny<int>()) as NotFoundObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Fant ikke observasjon", resultat.Value);
        }

        [Fact]
        public async Task HentEnIkkeLoggetInn()
        {
            mockRep.Setup(k => k.HentEn(It.IsAny<int>())).ReturnsAsync(()=>null);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.HentEn(It.IsAny<int>()) as UnauthorizedObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task EndreLoggetInnOK()
        {
            // Arrange

            mockRep.Setup(k => k.Endre(It.IsAny<observasjon>())).ReturnsAsync(true);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.Endre(It.IsAny<observasjon>()) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal("observasjon endret", resultat.Value);
        }

        [Fact]
        public async Task EndreLoggetInnIkkeOK()
        {
            // Arrange

            mockRep.Setup(k => k.Lagre(It.IsAny<observasjon>())).ReturnsAsync(false);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.Endre(It.IsAny<observasjon>()) as NotFoundObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Endringen av observasjon kunne ikke utføres", resultat.Value);
        }

        [Fact]
        public async Task EndreLoggetInnFeilModel()
        {
            // Arrange
            // observasjon er indikert feil med tomt fornavn her.
            // det har ikke noe å si, det er introduksjonen med ModelError under som tvinger frem feilen
            // kunnde også her brukt It.IsAny<observasjon>
            var observasjon1 = new observasjon
            {
                Id = 1,
                Fornavn = "",
                Etternavn = "Hansen",
                Adresse = "Askerveien 82",
                Postnr = "1370",
                Poststed = "Asker"
            };

            mockRep.Setup(k => k.Endre(observasjon1)).ReturnsAsync(true);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            observasjonController.ModelState.AddModelError("Fornavn", "Feil i inputvalidering på server");

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.Endre(observasjon1) as BadRequestObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalidering på server", resultat.Value);
        }

        [Fact]
        public async Task EndreIkkeLoggetInn()
        {
            mockRep.Setup(k => k.Endre(It.IsAny<observasjon>())).ReturnsAsync(true);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.Endre(It.IsAny<observasjon>()) as UnauthorizedObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task LoggInnOK()
        {
            mockRep.Setup(k => k.LoggInn(It.IsAny<Bruker>())).ReturnsAsync(true);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.LoggInn(It.IsAny<Bruker>()) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        [Fact]
        public async Task LoggInnFeilPassordEllerBruker()
        {
            mockRep.Setup(k => k.LoggInn(It.IsAny<Bruker>())).ReturnsAsync(false);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.LoggInn(It.IsAny<Bruker>()) as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.False((bool)resultat.Value);
        }

        [Fact]
        public async Task LoggInnInputFeil()
        {
            mockRep.Setup(k => k.LoggInn(It.IsAny<Bruker>())).ReturnsAsync(true);

            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);

            observasjonController.ModelState.AddModelError("Brukernavn", "Feil i inputvalidering på server");

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            var resultat = await observasjonController.LoggInn(It.IsAny<Bruker>()) as BadRequestObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalidering på server", resultat.Value);
        }

        [Fact]
        public void LoggUt()
        {
            var observasjonController = new observasjonController(mockRep.Object, mockLog.Object);
            
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            mockSession[_loggetInn] = _loggetInn;
            observasjonController.ControllerContext.HttpContext = mockHttpContext.Object;

            // Act
            observasjonController.LoggUt();

            // Assert
           Assert.Equal(_ikkeLoggetInn,mockSession[_loggetInn]);
        }
    }
}
