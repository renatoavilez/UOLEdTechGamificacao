using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UOLEdTechGamificacao.Enum;

namespace UOLEdTechGamificacao
{
    public class Worker : BackgroundService
    {
        private readonly ServiceConfigurations serviceConfigurations;

        public Worker(IConfiguration configuration)
        {
            serviceConfigurations = new ServiceConfigurations();

            new ConfigureFromConfigurationOptions<ServiceConfigurations>(configuration.GetSection("ServiceConfigurations"))
                    .Configure(serviceConfigurations);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var delay = new TimeSpan(serviceConfigurations.HorasEntreExecucao, 0, 0);

            while (!stoppingToken.IsCancellationRequested)
            {
                await ExecutarTarefa();

                Log.Information("Worker running at: {time}", DateTimeOffset.Now);

                await Task.Delay(delay, stoppingToken);
            }
        }

        private async Task ExecutarTarefa()
        {
            var jogadores = LerArquivo();

            SalvarJson(jogadores);

            AtualizarIdades(jogadores);

            AtualizarMedalhas(jogadores);

            AtualizarRanking(jogadores);

            var textoFormatado = new StringBuilder();

            var listaOrdenadaPorNome = OrdernarPorNome(jogadores);

            textoFormatado.Append(FormatarPorNome(listaOrdenadaPorNome));

            var listaOrdenadaPorRanking = OrdenarPorRanking(jogadores);

            textoFormatado.Append(FormatarPorRanking(listaOrdenadaPorRanking));

            var pontuacaoPorRegiao = ObterPontuacaoPorRegiao(jogadores);

            textoFormatado.Append(FormatarPorRegiao(pontuacaoPorRegiao));

            await EscreverResultado(textoFormatado.ToString());
        }

        private List<Jogador> LerArquivo()
        {
            var jogadores = new List<Jogador>();

            try
            {
                using (var sr = new StreamReader(serviceConfigurations.ArquivoLeitura))
                {
                    string linha;

                    while ((linha = sr.ReadLine()) != null)
                    {
                        var atributos = linha.Split(";");

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
                                UltimaPontuacao = DateTime.ParseExact(atributos[5], "dd/MM/yyyy", null),
                                Nivel = Nivelamento.A
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
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);

                return null;
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

        private void SalvarJson(List<Jogador> jogadores)
        {
            using (StreamWriter file = File.CreateText(serviceConfigurations.ArquivoJson))
            {
                var serializer = new JsonSerializer();

                serializer.Serialize(file, jogadores);
            }
        }

        private void AtualizarIdades(List<Jogador> jogadores)
        {
            var dataAtual = DateTime.UtcNow;

            foreach (var jogador in jogadores)
            {
                var idade = dataAtual.Year - jogador.Nascimento.Year;

                jogador.Idade = jogador.Nascimento.Date > jogador.Nascimento.AddYears(-idade) ? idade-- : idade;

                Log.Error("JOGADOR: {jogador} IDADE {idade}", jogador.Nome, jogador.Idade);
            }
        }

        private void AtualizarMedalhas(List<Jogador> jogadores)
        {
            foreach (var jogador in jogadores)
            {
                jogador.MedalhaOuro = jogador.Pontuacao / 10000;
                jogador.MedalhaPrata = (jogador.Pontuacao % 10000) / 1000;
                jogador.MedalhaBronze = ((jogador.Pontuacao % 10000) % 1000) / 100;
            }
        }

        private void AtualizarRanking(List<Jogador> jogadores)
        {
            var jogadoresEletivoOrdenados = jogadores.Where(jogador => jogador.Pontuacao > 10000).OrderBy(jogador => jogador.Pontuacao);

        }

        private List<Jogador> OrdernarPorNome(List<Jogador> jogadores)
        {
            return jogadores.OrderBy(jogador => jogador.Nome).ToList();
        }

        private string FormatarPorNome(List<Jogador> jogadores)
        {
            var textoFormatado = new StringBuilder();

            textoFormatado.AppendLine("Ordenação de acordo com o Nome\n");

            foreach (var jogador in jogadores)
            {
                textoFormatado.AppendLine(string.Format("{0} | {1} anos | Pontuação: {2} | Posição Ranking: {3}", jogador.Nome, jogador.Idade, jogador.Pontuacao, jogador.PosicaoRanking));
            }

            return textoFormatado.ToString();
        }

        private List<Jogador> OrdenarPorRanking(List<Jogador> jogadores)
        {
            return jogadores.OrderBy(jogador => jogador.PosicaoRanking).ToList();
        }

        private string FormatarPorRanking(List<Jogador> jogadores)
        {
            var textoFormatado = new StringBuilder();

            textoFormatado.AppendLine("\nOrdenação de acordo com o Ranking\n");

            foreach (var jogador in jogadores)
            {
                textoFormatado.AppendLine(string.Format("{0} | {1} anos | Ouro:{2} | Prata: {3} | Bronze: {4} | Pontuação: {5} | Posição Ranking: {6}", jogador.Nome, jogador.Idade, jogador.MedalhaOuro, jogador.MedalhaPrata, jogador.MedalhaBronze, jogador.Pontuacao, jogador.PosicaoRanking));
            }

            return textoFormatado.ToString();
        }

        private List<PontuacaoRegiao> ObterPontuacaoPorRegiao(List<Jogador> jogadores)
        {
            var pontuacaoPorRegiaoLista = new List<PontuacaoRegiao>();

            var grupos = jogadores.GroupBy(jogador => jogador.Regiao);

            foreach (var grupo in grupos)
            {
                var pontuacao = new PontuacaoRegiao
                {
                    Regiao = grupo.FirstOrDefault().Regiao,
                    MedalhaOuro = grupo.Sum(grupo => grupo.MedalhaOuro),
                    MedalhaPrata = grupo.Sum(grupo => grupo.MedalhaPrata),
                    MedalhaBronze = grupo.Sum(grupo => grupo.MedalhaBronze),
                };

                pontuacaoPorRegiaoLista.Add(pontuacao);
            }

            return pontuacaoPorRegiaoLista.OrderBy(regiao => regiao.Regiao).ToList();
        }

        private string FormatarPorRegiao(List<PontuacaoRegiao> regioes)
        {
            var textoFormatado = new StringBuilder();

            textoFormatado.AppendLine("\nOrdenação de acordo com o Região e agrupamento pela quantidade de medalhas\n");

            foreach (var regiao in regioes)
            {
                textoFormatado.AppendLine(string.Format("{0} | Ouro: {1} | Prata: {2} | Bronze: {3}", regiao.Regiao, regiao.MedalhaOuro, regiao.MedalhaPrata, regiao.MedalhaBronze));
            }

            return textoFormatado.ToString();
        }

        private async Task EscreverResultado(string texto)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(serviceConfigurations.ArquivoEscrita)))
            {
                await outputFile.WriteLineAsync(texto);
            }
        }

        private class PontuacaoRegiao
        {
            public string Regiao { get; set; }
            public int MedalhaOuro { get; set; }
            public int MedalhaPrata { get; set; }
            public int MedalhaBronze { get; set; }
        }
    }
}
