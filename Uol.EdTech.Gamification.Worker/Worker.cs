using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uol.EdTech.Gamification.Util;
using Uol.EdTech.Gamification.Util.Excecoes;
using Uol.EdTech.Gamification.Core.Dtos;
using Uol.EdTech.Gamification.Core.Entidades;
using Uol.EdTech.Gamification.Core.Interfaces;

[assembly: InternalsVisibleTo("Uol.EdTech.Gamification.Testes")]

namespace Uol.EdTech.Gamification.WorkerExecutor
{
    public class Worker : BackgroundService
    {
        private readonly ILeitorArquivo leitorArquivo;
        private readonly IEscritorArquivo escritorArquivo;
        private readonly IEscritorJsonArquivo escritorJson;
        private readonly ApplicationConfig applicationConfig;

        public Worker(ILeitorArquivo leitorArquivo, IEscritorArquivo escritorArquivo, IEscritorJsonArquivo escritorJson, ApplicationConfig applicationConfig)
        {
            this.leitorArquivo = leitorArquivo;
            this.escritorArquivo = escritorArquivo;
            this.escritorJson = escritorJson;
            this.applicationConfig = applicationConfig;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ValidarServiceConfiguration(applicationConfig);

            var delay = new TimeSpan(applicationConfig.HorasEntreExecucao, 0, 0);

            while (!stoppingToken.IsCancellationRequested)
            {
                await ExecutarAsync();

                Log.Information("Execucao: {time}", DateTimeOffset.UtcNow);

                await Task.Delay(delay, stoppingToken);
            }
        }

        internal void ValidarServiceConfiguration(ApplicationConfig applicationConfig)
        {
            if (applicationConfig.Niveis.Sum() != 100)
            {
                throw new InvalidLevelSettingException();
            }
        }

        internal async Task ExecutarAsync()
        {
            var jogadores = await leitorArquivo.ExecutarAsync();

            escritorJson.Executar(jogadores);

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

            Log.Information(textoFormatado.ToString());

            await escritorArquivo.ExecutarAsync(textoFormatado.ToString());
        }

        private void AtualizarIdades(List<Jogador> jogadores)
        {
            var dataAtual = DateTime.UtcNow;

            foreach (var jogador in jogadores)
            {
                var idade = dataAtual.Year - jogador.Nascimento.Year;

                jogador.Idade = jogador.Nascimento.Date > jogador.Nascimento.AddYears(-idade) ? idade-- : idade;
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
            var jogadoresEletivoOrdenados = jogadores.Where(jogador => jogador.Pontuacao > 10000).OrderByDescending(jogador => jogador.Pontuacao).ThenByDescending(jogador => jogador.UltimaPontuacao).ToList();

            var posicao = 1;

            foreach (var jogador in jogadoresEletivoOrdenados)
            {
                jogador.PosicaoRanking = posicao;
                posicao++;
            }
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
                if (jogador.Pontuacao >= 10000)
                {
                    textoFormatado.AppendLine(string.Format("{0} | {1} anos | Pontuação: {2} | Posição Ranking: {3}", jogador.Nome, jogador.Idade, jogador.Pontuacao, jogador.PosicaoRanking));
                }
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
                if (jogador.Pontuacao >= 10000)
                {
                    textoFormatado.AppendLine(string.Format("{0} | {1} anos | Ouro:{2} | Prata: {3} | Bronze: {4} | Pontuação: {5} | Posição Ranking: {6}", jogador.Nome, jogador.Idade, jogador.MedalhaOuro, jogador.MedalhaPrata, jogador.MedalhaBronze, jogador.Pontuacao, jogador.PosicaoRanking));
                }
            }

            return textoFormatado.ToString();
        }

        private List<PontuacaoDto> ObterPontuacaoPorRegiao(List<Jogador> jogadores)
        {
            var pontuacaoPorRegiaoLista = new List<PontuacaoDto>();

            var grupos = jogadores.GroupBy(jogador => jogador.Regiao);

            foreach (var grupo in grupos)
            {
                var pontuacao = new PontuacaoDto
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

        private string FormatarPorRegiao(List<PontuacaoDto> regioes)
        {
            var textoFormatado = new StringBuilder();

            textoFormatado.AppendLine("\nOrdenação de acordo com o Região e agrupamento pela quantidade de medalhas\n");

            foreach (var regiao in regioes)
            {
                textoFormatado.AppendLine(string.Format("{0} | Ouro: {1} | Prata: {2} | Bronze: {3}", regiao.Regiao, regiao.MedalhaOuro, regiao.MedalhaPrata, regiao.MedalhaBronze));
            }

            return textoFormatado.ToString();
        }
    }
}

