using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using Uol.EdTech.Gamification.Util;
using Uol.EdTech.Gamification.Util.Excecoes;
using Uol.EdTech.Gamification.WorkerExecutor;
using Uol.EdTech.Gamification.Core.Entidades;
using Uol.EdTech.Gamification.Core.Interfaces;

namespace Uol.EdTech.Gamification.Testes
{
    [TestClass]
    public class WorkerTeste
    {
        private Mock<ILeitorArquivo> leitorArquivo;
        private Mock<IEscritorArquivo> escritorArquivo;
        private Mock<IEscritorJsonArquivo> escritorJson;
        private Mock<ApplicationConfig> applicationConfig;

        [TestInitialize]
        public void CriarMocks()
        {
            leitorArquivo = new Mock<ILeitorArquivo>();
            escritorArquivo = new Mock<IEscritorArquivo>();
            escritorJson = new Mock<IEscritorJsonArquivo>();
            applicationConfig = new Mock<ApplicationConfig>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidLevelSettingException))]
        public void ErroValidacaoAppsettings()
        {
            var appSettings = new ApplicationConfig
            {
                Niveis = new List<int> { 1, 3 }
            };

            WorkerExecutor.ValidarServiceConfiguration(appSettings);
        }

        private Worker WorkerExecutor
        {
            get
            {
                return new Worker(
                    leitorArquivo.Object,
                    escritorArquivo.Object,
                    escritorJson.Object,
                    applicationConfig.Object
                    );
            }
        }

        private ApplicationConfig ObterAppConfig()
        {
            return new ApplicationConfig
            {
                ArquivoJson = "ArquivoJson",
                ArquivoLeitura = "ArquivoLeitura",
                ArquivoEscrita = "ArquivoResultado",
                HorasEntreExecucao = 2,
                Niveis = new List<int> { 4, 3, 2, 1 }
            };
        }

        private Jogador ObterJogador(string regiao, int pontucao)
        {
            return new Jogador
            {
                Id = It.IsAny<int>(),
                Nascimento = DateTime.Now.AddYears(-18),
                Nome = "Julio Pescador",
                Regiao = regiao,
                UltimaPontuacao = DateTime.Now.AddDays(-1),
                Pontuacao = pontucao
            };
        }
    }
}
