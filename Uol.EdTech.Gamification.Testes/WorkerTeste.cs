using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Uol.EdTech.Gamification.Util;
using Uol.EdTech.Gamification.WorkerExecutor;
using UOL.EdTech.Gamification.Core.Interfaces;

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
        public void TestMethod1()
        {
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
    }
}
