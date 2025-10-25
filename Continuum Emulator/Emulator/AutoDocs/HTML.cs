namespace Continuum93.Emulator.AutoDocs
{
    public static class HTML
    {
        const string HTML_ROOT = "<html lang=\"en\"><head><meta charset=\"utf-8\"><title>{0}</title><link rel=\"stylesheet\" href=\"{2}\"></head><body>\n{1}\n</body></html>";
        const string META_ROW = "<tr><td style=\"min-width:200px\"><strong>{0}</strong></td><td>{1}</td></tr>";
        const string META_ROW_STYLED = "<tr><td class=\"first-column\" style=\"min-width:200px\"><strong>{0}</strong></td><td class=\"second-column\">{1}</td></tr>";
        const string INTERRUPT_ROW = "<tr><td style=\"min-width:100px\">{0}</td><td>{1}</td><td style='text-align: left'>{2}</td><td>{3}</td><td>{4}</td></tr>";

        public static string HTMLRoot(string title, string input, string styleSheet) => string.Format(HTML_ROOT, title, input, styleSheet) + Constants.CR;

        public static string Div(string input, string className = "") => string.Format("<div class=\"{1}\">{0}</div>", input, className) + Constants.CR;

        public static string P(string input) => string.Format("<p>{0}</p>", input) + Constants.CR;

        public static string PSubtitle(string input) => string.Format("<p style=\"margin-top: 0;\">{0}</p>", input) + Constants.CR;

        public static string B(string input) => string.Format("<b>{0}</b>", input) + Constants.CR;

        public static string H1(string input) => string.Format("<h1>{0}</h1>", input) + Constants.CR;

        public static string H2(string input) => string.Format("<h2>{0}</h2>", input) + Constants.CR;

        public static string H1Title(string input) => string.Format("<h1 style=\"margin-bottom: 0;\">{0}</h1><hr>", input) + Constants.CR;

        public static string H2Title(string input) => string.Format("<h2 style=\"margin-bottom: 0;\">{0}</h2><hr>", input) + Constants.CR;

        public static string H3Title(string input) => string.Format("<h3 style=\"margin-bottom: 0;\">{0}</h3><hr>", input) + Constants.CR;

        public static string H3(string input) => string.Format("<h3>{0}</h3>", input) + Constants.CR;

        public static string Table(string input) => string.Format("<table>{0}</table>", input) + Constants.CR;

        public static string IntTable(string input) => string.Format("<table class=\"intTable\">{0}</table>", input) + Constants.CR;

        public static string Strong(string input) => string.Format("<strong>{0}</strong>", input) + Constants.CR;

        public static string TR(string input, string className = "") => string.Format("<tr class=\"{1}\">{0}</tr>", input, className) + Constants.CR;

        public static string TD(string input, string className = "") => string.Format("<td class=\"{1}\">{0}</td>", input, className) + Constants.CR;

        public static string MetaRow(string name, string input) => string.Format(META_ROW, name, input) + Constants.CR;

        public static string MetaStyledRow(string name, string input) => string.Format(META_ROW_STYLED, name, input) + Constants.CR;

        public static string InterruptRow(string reg, string exReg, string name, string format, string input) => string.Format(INTERRUPT_ROW, reg, exReg, name, format, input) + Constants.CR;

        public static string HR => "<hr style=\"margin: 2px 0 2px 0;\" />" + Constants.CR;

        public static string BR => "<br />" + Constants.CR;
    }
}
