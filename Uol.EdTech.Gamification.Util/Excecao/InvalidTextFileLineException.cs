using System;
using System.Runtime.Serialization;

namespace Uol.EdTech.Gamification.Util.Excecoes
{
    [Serializable]
    public sealed class InvalidTextFileLineException : Exception
    {
        public InvalidTextFileLineException()
               : base("Linha lida invalida. ")
        { }

        private InvalidTextFileLineException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}