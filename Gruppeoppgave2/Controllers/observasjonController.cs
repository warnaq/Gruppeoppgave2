using System.Collections.Generic;
using System.Threading.Tasks;
using Gruppeoppgave2.DAL;
using Gruppeoppgave2.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gruppeoppgave2.Controllers
{
    [Route("[controller]/[action]")]
    public class observasjonController : ControllerBase
    {
        private IobservasjonRepository _db;

        private ILogger<observasjonController> _log;

        private const string _loggetInn = "loggetInn";
        private const string _ikkeLoggetInn = "";


        public observasjonController(IobservasjonRepository db, ILogger<observasjonController> log)
        {
            _db = db;
            _log = log;
        }

        public async Task<ActionResult> Lagre(observasjon innobservasjon)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            if (ModelState.IsValid)
            {
                bool returOK = await _db.Lagre(innobservasjon);
                if (!returOK)
                {
                    _log.LogInformation("observasjonen kunne ikke lagres!");
                    return BadRequest("observasjonen kunne ikke lagres");
                }
                return Ok("observasjon lagret");
            }
            _log.LogInformation("Feil i inputvalidering");
            return BadRequest("Feil i inputvalidering på server");
        }

        public async Task<ActionResult> HentAlle()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<observasjon> alleobservasjon = await _db.HentAlle();
            return Ok(alleobservasjon); 
        }

        public async Task<ActionResult> Slett(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            bool returOK = await _db.Slett(id);
            if (!returOK)
            {
                _log.LogInformation("Sletting av observasjonen ble ikke utført");
                return NotFound("Sletting av observasjonen ble ikke utført");
            }
            return Ok("observasjon slettet");
        }

        public async Task<ActionResult> HentEn(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            observasjon observasjonen = await _db.HentEn(id);
            if (observasjonen == null)
            {
                _log.LogInformation("Fant ikke observasjonen");
                return NotFound("Fant ikke observasjonen");
            }
            return Ok(observasjonen);
        }

        public async Task<ActionResult> Endre(observasjon endreobservasjon)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            if (ModelState.IsValid)
            {
                bool returOK = await _db.Endre(endreobservasjon);
                if (!returOK)
                {
                    _log.LogInformation("Endringen kunne ikke utføres");
                    return NotFound("Endringen av observasjonen kunne ikke utføres");
                }
                return Ok("observasjon endret");
            }
            _log.LogInformation("Feil i inputvalidering");
            return BadRequest("Feil i inputvalidering på server");
        }
        public async Task<ActionResult> LoggInn(Bruker bruker) 
        {
            if (ModelState.IsValid)
            {
                bool returnOK = await _db.LoggInn(bruker);
                if (!returnOK)
                {
                    _log.LogInformation("Innloggingen feilet for bruker");
                    HttpContext.Session.SetString(_loggetInn,_ikkeLoggetInn);
                    return Ok(false);
                }
                HttpContext.Session.SetString(_loggetInn, _loggetInn);
                return Ok(true);
            }
            _log.LogInformation("Feil i inputvalidering");
            return BadRequest("Feil i inputvalidering på server");
        }

        public void LoggUt()
        {
            HttpContext.Session.SetString(_loggetInn,_ikkeLoggetInn);
        }
    }
}

    
