using System.Threading.Tasks;

namespace UOL.EdTech.Gamification.Core.Interfaces
{
    public interface IEscritorArquivo
    {
        Task ExecutarAsync(string texto);
    }
}
