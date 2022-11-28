using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gruppeoppgave2.Model;
using Microsoft.EntityFrameworkCore;

namespace observasjon.DB
{
    public class observasjonDB
    {
        private readonly observasjonContext _db;

        public observasjonDB(observasjonContext db)
        {
            _db = db;

        }

        public async Task<bool> Lagre(observasjon innobservasjon)
        {
            try
            {
                var nyobservasjonRad = new observasjoner();
                nyobservasjonRad.Fornavn = innobservasjon.Fornavn;
                nyobservasjonRad.Etternavn = innobservasjon.Etternavn;
                nyobservasjonRad.Adresse = innobservasjon.Adresse;

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
                _db.observasjonr.Add(nyobservasjonRad);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
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
                    Poststed = k.Poststed.Poststed
                }).ToListAsync();
                return alleobservasjoner;
            }
            catch
            {
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
            catch
            {
                return false;
            }
        }

        public async Task<observasjon> HentEn(int id)
        {
            observasjoner enobservasjon = await _db.observasjoner.FindAsync(id);
            var hentetobservasjon = new observasjon()
            {
                Id = enobservasjon.Id,
                Fornavn = enobservasjon.Fornavn,
                Etternavn = enobservasjon.Etternavn,
                Adresse = enobservasjon.Adresse,
                Postnr = enobservasjon.Poststed.Postnr,
                Poststed = enobservasjon.Poststed.Poststed
            };
            return hentetobservasjon;
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
                await _db.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
