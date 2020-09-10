using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uol.EdTech.Gamification.Util;
using Uol.EdTech.Gamification.Util.Excecoes;
using Uol.EdTech.Gamification.Core.Entidades;
using Uol.EdTech.Gamification.Core.Interfaces;

namespace Uol.EdTech.Gamification.Core.Servicos
{
    public class LeitorArquivo : ILeitorArquivo
    {
        private readonly ApplicationConfig applicationConfig;

        public LeitorArquivo(ApplicationConfig applicationConfig)
        {
            this.applicationConfig = applicationConfig;
        }

        public async Task<List<Jogador>> ExecutarAsync()
        {
            var jogadores = new List<Jogador>();

            try
            {
                using (var streamReader = new StreamReader(applicationConfig.ArquivoLeitura))
                {
                    string linha;

                    while ((linha = await streamReader.ReadLineAsync()) != null)
                    {
                        var atributos = linha.Split(';');

                        try
                        {
                            ValidarAtributos(atributos);

                            var jogador = new Jogador
                            {
                                Id = int.Parse(atributos[0]),
                                Nome = atributos[1],
                                Nascimento = DateTime.ParseExact(atributos[2], "dd/MM/yyyy", null),
                                Regiao = atributos[3],
                                Pontuacao = int.Parse(atributos[4]),
                                UltimaPontuacao = DateTime.ParseExact(atributos[5], "dd/MM/yyyy", null)
                            };

                            jogadores.Add(jogador);
                        }
                        catch (InvalidTextFileLineException excecao)
                        {
                            Log.Error(excecao.Message + linha);
                        }
                    }
                }

                return jogadores;
            }
            catch (Exception excecao)
            {
                Log.Error("Erro na leitura do arquivo");
                throw excecao;
            }
        }

        private void ValidarAtributos(string[] atributos)
        {
            if (atributos.Count() != 6)
            {
                throw new InvalidTextFileLineException();
            }

            //Verificar se precisa de mais validacao
        }
    }
}
