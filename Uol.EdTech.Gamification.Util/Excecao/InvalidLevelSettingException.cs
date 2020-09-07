using System;
using System.Runtime.Serialization;

namespace Uol.EdTech.Gamification.Util.Excecoes
{
    [Serializable]
    public sealed class InvalidLevelSettingException : Exception
    {
        public InvalidLevelSettingException()
            : base("Arquivo de configuração inválido.") { }

        private InvalidLevelSettingException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
