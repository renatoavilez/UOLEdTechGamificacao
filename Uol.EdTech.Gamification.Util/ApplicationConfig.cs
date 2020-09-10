using System.Collections.Generic;

namespace Uol.EdTech.Gamification.Util
{
    public class ApplicationConfig
    {
        public List<int> Niveis { get; set; }
        public string ArquivoLeitura { get; set; }
        public string ArquivoEscrita { get; set; }
        public string ArquivoJson { get; set; }
        public int HorasEntreExecucao { get; set; }
    }
}
