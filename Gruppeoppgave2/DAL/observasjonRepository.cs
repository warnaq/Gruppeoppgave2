using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Gruppeoppgave2.Model;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gruppeoppgave2.DAL
{
    [ExcludeFromCodeCoverage]
    public class observasjonRepository : IobservasjonRepository
    {
        private observasjonContext _db;

        private ILogger<observasjonRepository> _log;

        public observasjonRepository(observasjonContext db, ILogger<observasjonRepository> log)
        {
            _db = db;
            _log = log;
        }

        public async Task<bool> Lagre(observasjon innobservasjon)
        {
            try
            {
                var nyobservasjonRad = new observasjoner();
                nyobservasjonRad.Fornavn = innobservasjon.Fornavn;
                nyobservasjonRad.Etternavn = innobservasjon.Etternavn;
                nyobservasjonRad.Adresse = innobservasjon.Adresse;
                nyobservasjonRad.Når = innobservasjon.Når;
                nyobservasjonRad.Beskrivelse = innobservasjon.Beskrivelse;

                var sjekkPostnr = await _db.Poststeder.FindAsync(innobservasjon.Postnr);
                if (sjekkPostnr == null)
                {
                    var poststedsRad = new Poststeder();
                    poststedsRad.Postnr = innobservasjon.Postnr;
                    poststedsRad.Poststed = innobservasjon.Poststed;
                    nyobservasjonRad.Poststed = poststedsRad;
                }
                else
                {
                    nyobservasjonRad.Poststed = sjekkPostnr;
                }
                _db.observasjoner.Add(nyobservasjonRad);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _log.LogInformation(e.Message);
                return false;
            }
        }

        public async Task<List<observasjon>> HentAlle()
        {
            try
            {
                List<observasjon> alleobservasjoner = await _db.observasjoner.Select(k => new observasjon
                {
                    Id = k.Id,
                    Fornavn = k.Fornavn,
                    Etternavn = k.Etternavn,
                    Adresse = k.Adresse,
                    Postnr = k.Poststed.Postnr,
                    Poststed = k.Poststed.Poststed,
                    Når = k.Når,
                    Beskrivelse = k.Beskrivelse
                }).ToListAsync();
                return alleobservasjoner;
            }
            catch (Exception e)
            {
                _log.LogInformation(e.Message);
                return null;
            }
        }

        public async Task<bool> Slett(int id)
        {
            try
            {
                observasjoner enDBobservasjon = await _db.observasjoner.FindAsync(id);
                _db.observasjoner.Remove(enDBobservasjon);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _log.LogInformation(e.Message);
                return false;
            }
        }

        public async Task<observasjon> HentEn(int id)
        {
            try
            {
                observasjoner enobservasjon = await _db.observasjoner.FindAsync(id);
                var hentetobservasjon = new observasjon()
                {
                    Id = enobservasjon.Id,
                    Fornavn = enobservasjon.Fornavn,
                    Etternavn = enobservasjon.Etternavn,
                    Adresse = enobservasjon.Adresse,
                    Postnr = enobservasjon.Poststed.Postnr,
                    Poststed = enobservasjon.Poststed.Poststed,
                    Når = enobservasjon.Når,
                    Beskrivelse = enobservasjon.Beskrivelse
                };
                return hentetobservasjon;
            }
            catch (Exception e)
            {
                _log.LogInformation(e.Message);
                return null;
            }
        }

        public async Task<bool> Endre(observasjon endreobservasjon)
        {
            try
            {
                var endreObjekt = await _db.observasjoner.FindAsync(endreobservasjon.Id);
                if (endreObjekt.Poststed.Postnr != endreobservasjon.Postnr)
                {
                    var sjekkPostnr = _db.Poststeder.Find(endreobservasjon.Postnr);
                    if (sjekkPostnr == null)
                    {
                        var poststedsRad = new Poststeder();
                        poststedsRad.Postnr = endreobservasjon.Postnr;
                        poststedsRad.Poststed = endreobservasjon.Poststed;
                        endreObjekt.Poststed = poststedsRad;
                    }
                    else
                    {
                        endreObjekt.Poststed.Postnr = endreobservasjon.Postnr;
                    }
                }
                endreObjekt.Fornavn = endreobservasjon.Fornavn;
                endreObjekt.Etternavn = endreobservasjon.Etternavn;
                endreObjekt.Adresse = endreobservasjon.Adresse;
                endreObjekt.Når = endreobservasjon.Når;
                endreObjekt.Beskrivelse = endreobservasjon.Beskrivelse;

                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _log.LogInformation(e.Message);
                return false;
            }
            return true;
        }

        public static byte[] LagHash(string passord, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(
                                password: passord,
                                salt: salt,
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: 1000,
                                numBytesRequested: 32);
        }

        public static byte[] LagSalt()
        {
            var csp = new RNGCryptoServiceProvider();
            var salt = new byte[24];
            csp.GetBytes(salt);
            return salt;
        }

        public async Task<bool> LoggInn(Bruker bruker)
        {
            try
            {
                Brukere funnetBruker = await _db.Brukere.FirstOrDefaultAsync(b => b.Brukernavn == bruker.Brukernavn);
                // sjekk passordet
                byte[] hash = LagHash(bruker.Passord, funnetBruker.Salt);
                bool ok = hash.SequenceEqual(funnetBruker.Passord);
                if (ok)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                _log.LogInformation(e.Message);
                return false;
            }
        }
    }
}
