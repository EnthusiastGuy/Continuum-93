using System.Collections.Generic;

namespace Continuum93.Emulator.AutoDocs.MetaInfo
{
    public class ASMMeta
    {
        public List<string> Operators;
        public string Description;
        public string Application;
        public string Format;

        public ASMMeta(List<string> operators, string description, string application, string format)
        {
            Description = description;
            Application = application;
            Format = format;
            Operators = operators;
        }

        public static ASMMeta Create(List<string> operators, string description, string application, string format)
        {
            return new ASMMeta(operators, description, application, format);
        }
    }
}
