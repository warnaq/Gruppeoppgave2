using System.Collections.Generic;
using System.Threading.Tasks;
using Gruppeoppgave2.Model;

namespace Gruppeoppgave2.DAL
{
    public interface IobservasjonRepository
    {
        Task<bool> Lagre(observasjon innobservasjon);
        Task<List<observasjon>> HentAlle();
        Task<bool> Slett(int id);
        Task<observasjon> HentEn(int id);
        Task<bool> Endre(observasjon endreobservasjon);
        Task<bool> LoggInn(Bruker bruker);
        //Task<bool> Loggut();
    }
}
