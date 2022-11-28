using Gruppeoppgave2.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Gruppeoppgave2.Model
{
    public static class DBInit
    {
        [ExcludeFromCodeCoverage]
        public static void Initialize(IApplicationBuilder app)
        {
            var serviceScope = app.ApplicationServices.CreateScope();
           
            var db = serviceScope.ServiceProvider.GetService<observasjonContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var poststed1 = new Poststeder {Postnr = "0270", Poststed = "Oslo"}; 
            var poststed2 = new Poststeder {Postnr = "1370", Poststed = "Asker"};

            var observasjon1 = new observasjoner { Fornavn = "Iron", Etternavn = "Man", Adresse = "Trondheimsveien 92", Poststed = poststed1};
            var observasjon2 = new observasjoner { Fornavn = "Cap", Etternavn = "America", Adresse = "Askerveien 72", Poststed = poststed2 };

            db.observasjoner.Add(observasjon1);
            db.observasjoner.Add(observasjon2);

            // lag en påoggingsbruker
            var bruker = new Brukere();
            bruker.Brukernavn = "Admin";
            string Passord = "Ufo12345";
            byte[] salt = observasjonRepository.LagSalt();
            byte[] hash = observasjonRepository.LagHash(Passord, salt);
            bruker.Passord = hash;
            bruker.Salt = salt;
            db.Brukere.Add(bruker);

            db.SaveChanges();
        }
    }
       
}
