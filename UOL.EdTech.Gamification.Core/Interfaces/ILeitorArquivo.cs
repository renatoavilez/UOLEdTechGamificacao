using System.Collections.Generic;
using System.Threading.Tasks;
using UOL.EdTech.Gamification.Core.Entidades;

namespace UOL.EdTech.Gamification.Core.Interfaces
{
    public interface ILeitorArquivo
    {
        Task<List<Jogador>> ExecutarAsync();
    }
}
