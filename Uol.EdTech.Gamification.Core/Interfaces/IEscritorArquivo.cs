using System.Threading.Tasks;

namespace Uol.EdTech.Gamification.Core.Interfaces
{
    public interface IEscritorArquivo
    {
        Task ExecutarAsync(string texto);
    }
}
