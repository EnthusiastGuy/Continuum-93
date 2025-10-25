namespace ContinuumUnitTests._Tools
{
    public static class ReportsManager
    {
        public static void SaveReport(string reportName, string reportContents)
        {
            // start from the test‐bin folder, go up 4 levels to the Continuum project root
            var baseDir = AppContext.BaseDirectory;
            var projectDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));
            // ensure our “Automated test reports” folder exists
            var reportsDir = Path.Combine(projectDir, "Automated reports");
            Directory.CreateDirectory(reportsDir);
            // write (or overwrite) the file
            var reportPath = Path.Combine(reportsDir, $"{reportName}.txt");
            File.WriteAllText(reportPath, reportContents);
        }
    }
}
