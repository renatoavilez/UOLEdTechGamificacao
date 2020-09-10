using System.Collections.Generic;
using System.Threading.Tasks;
using Uol.EdTech.Gamification.Core.Entidades;

namespace Uol.EdTech.Gamification.Core.Interfaces
{
    public interface ILeitorArquivo
    {
        Task<List<Jogador>> ExecutarAsync();
    }
}
