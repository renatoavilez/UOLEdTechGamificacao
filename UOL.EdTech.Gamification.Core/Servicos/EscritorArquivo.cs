using System.IO;
using System.Threading.Tasks;
using Uol.EdTech.Gamification.Util;
using Uol.EdTech.Gamification.Core.Interfaces;

namespace Uol.EdTech.Gamification.Core.Servicos
{
    public class EscritorArquivo : IEscritorArquivo
    {
        private readonly ApplicationConfig applicationConfig;

        public EscritorArquivo(ApplicationConfig applicationConfig)
        {
            this.applicationConfig = applicationConfig;
        }

        public async Task ExecutarAsync(string texto)
        {
            using (StreamWriter streamWriter = new StreamWriter(Path.Combine(applicationConfig.ArquivoEscrita)))
            {
                await streamWriter.WriteLineAsync(texto);
            }
        }
    }
}
