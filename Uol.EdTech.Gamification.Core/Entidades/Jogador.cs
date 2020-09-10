using System;

namespace Uol.EdTech.Gamification.Core.Entidades
{
    public class Jogador
    {
        public int Id { get; set; }
        public int Idade { get; set; }
        public string Nome { get; set; }
        public string Regiao { get; set; }
        public int Pontuacao { get; set; }
        public int MedalhaBronze { get; set; }
        public int MedalhaPrata { get; set; }
        public int MedalhaOuro { get; set; }
        public int PosicaoRanking { get; set; }
        public DateTime Nascimento { get; set; }
        public DateTime UltimaPontuacao { get; set; }
    }
}
