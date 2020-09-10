using Newtonsoft.Json;
using System.IO;
using Uol.EdTech.Gamification.Util;
using Uol.EdTech.Gamification.Core.Interfaces;

namespace Uol.EdTech.Gamification.Core.Servicos
{
    public class EscritorJsonArquivo : IEscritorJsonArquivo
    {
        private readonly ApplicationConfig applicationConfig;
        public EscritorJsonArquivo(ApplicationConfig applicationConfig)
        {
            this.applicationConfig = applicationConfig;
        }

        public void Executar(object model)
        {
            using (StreamWriter file = File.CreateText(applicationConfig.ArquivoJson))
            {
                var serializer = new JsonSerializer();

                serializer.Serialize(file, model);
            }
        }
    }
}