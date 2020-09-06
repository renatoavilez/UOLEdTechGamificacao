using System;
using System.Runtime.Serialization;

namespace UOLEdTechGamificacao
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
