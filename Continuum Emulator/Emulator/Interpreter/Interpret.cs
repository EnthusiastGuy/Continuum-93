using System;
using System.Collections.Generic;
using System.IO;

namespace Continuum93.Emulator.Interpreter
{
    public static class Interpret
    {
        private static readonly string[] separators = { "\r\n", "\n" };
        private static readonly List<string> includeFiles = new();
        public static string[] GetFullSourceLines(string source, string basePath = "")
        {
            includeFiles.Clear();
            List<string> processedLines = new();
            string[] lines = source.Split(separators, StringSplitOptions.None);
            List<string> includedContent = new();

            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("#include"))
                {
                    string includeFilePath = line.Trim().Substring("#include".Length).Trim();
                    includeFilePath = includeFilePath.Trim('"', '<', '>', ' ');

                    if (!Path.IsPathRooted(includeFilePath) && !string.IsNullOrEmpty(basePath))
                    {
                        includeFilePath = Path.Combine(basePath, DataConverter.GetCrossPlatformPath(includeFilePath));
                    }

                    bool fileExists = File.Exists(includeFilePath);
                    bool notAlreadyIncluded = !includeFiles.Contains(includeFilePath.ToLower());

                    if (fileExists && notAlreadyIncluded)
                    {
                        string includedSource = File.ReadAllText(includeFilePath);
                        string[] includedLines = GetFullSourceLines(includedSource, Path.GetDirectoryName(includeFilePath));
                        includedContent.AddRange(includedLines); // Store included content
                        includeFiles.Add(includeFilePath.ToLower());
                        CompileLog.Log($"#include {includeFilePath} succesfully added.", CompileIssue.Info);
                    }
                    else if (!fileExists)
                    {
                        CompileLog.Log($"#include {includeFilePath} was not found.", CompileIssue.Error);
                    }
                    else if (!notAlreadyIncluded)
                    {
                        CompileLog.Log($"#include {includeFilePath} was already added, skipping.", CompileIssue.Warning);
                    }
                }
                else
                {
                    processedLines.Add(line);
                }
            }

            processedLines.AddRange(includedContent);
            return processedLines.ToArray();
        }

        public static string CleanLine(string line)
        {
            string cleanLine = line.Trim().Replace("\t", " ");
            while (cleanLine.IndexOf("  ") >= 0)
                cleanLine = cleanLine.Replace("  ", " ");

            return cleanLine;
        }

        public static string CleanLineIncludingCommas(string line)
        {
            string cleanLine = line.Trim().Replace("\t", " ");
            while (cleanLine.IndexOf("  ") >= 0)
                cleanLine = cleanLine.Replace("  ", " ");

            while (cleanLine.IndexOf(", ") >= 0)
                cleanLine = cleanLine.Replace(", ", ",");

            return cleanLine;
        }

        // A label definition is always in front of code or alone on a line
        // Only one label is permitted per line
        public static List<string> GetLabels(string line)
        {
            List<string> labels = new();

            // Clean the string
            string cleanLine = CleanLineIncludingCommas(line);
            string[] tokens = cleanLine.Split(' ');

            foreach (string token in tokens)
            {
                if (token.Length >= 2 && (token[0] == '.' || token[0] == '~'))
                {
                    labels.Add(token);
                }
                else
                {
                    break;
                }
            }

            return labels;
        }

        public static string GetCommentFromLine(string line)
        {
            bool inString = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    inString = !inString; // Toggle state on encountering a double quote.
                }
                else if (!inString && c == ';')
                {
                    // Found a semicolon outside a string, treat it as the comment start.
                    return line.Substring(i).Trim();
                }
            }
            return "";
        }

        public static string GetLabelFromLine(string line)
        {
            List<string> labels = GetLabels(line);

            if (labels.Count == 1 && DataConverter.IsLabelValid(labels[0]))
                return labels[0];

            return null;
        }

        public static string GetMnemonicFromLine(string line)
        {
            string cleanLine = line;
            int commentStart = -1;
            bool inString = false;

            // Find the first semicolon that is not inside a string.
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    inString = !inString; // Toggle the in-string status.
                }
                else if (!inString && c == ';')
                {
                    commentStart = i;
                    break;
                }
            }

            // If a comment delimiter was found outside of a string, remove the comment.
            if (commentStart != -1)
            {
                cleanLine = line.Substring(0, commentStart).Trim();
            }

            // Remove any labels from the cleaned line.
            List<string> labels = GetLabels(cleanLine);
            if (labels.Count == 1)
            {
                int index = cleanLine.IndexOf(labels[0]);
                cleanLine = (index < 0)
                    ? cleanLine
                    : cleanLine.Remove(index, labels[0].Length);
            }

            return cleanLine.Trim();
        }

        public static bool IsCompilerDirective(string mnemonic)
        {
            return mnemonic.Length > 0 && mnemonic.Trim()[0] == '#';
        }

        public static string GetOp(string mnemonic)
        {

            if (mnemonic.Contains(' '))
            {
                return mnemonic.Trim()[..mnemonic.Trim().IndexOf(" ")].Trim().ToUpper();
            }
            else
            {
                return mnemonic;
            }
        }

        public static string GetArguments(string mnemonic)
        {
            if (mnemonic.Contains(' '))
            {
                return mnemonic.Trim()[mnemonic.Trim().IndexOf(' ')..].Trim();
            }
            else
            {
                return "";
            }
        }

        // Gets all defined labels in the code
        public static Dictionary<string, uint> GetAllDefinedLabels(string[] lines)
        {
            Dictionary<string, uint> result = [];

            for (uint i = 0; i < lines.Length; i++)
            {
                List<string> labels = GetLabels(lines[i]);

                if (labels.Count > 1)   // Multiple labels found
                {
                    CompileLog.Log(string.Format("Multiple labels found at line {0}: {1}", i, lines[i]), CompileIssue.Error);
                    break;
                }
                else if (labels.Count == 1 && !DataConverter.IsLabelValid(labels[0]))
                {
                    CompileLog.Log(string.Format("Invalid label defined at line {0}: {1}", i, lines[i]), CompileIssue.Error);
                    break;
                }
                else if (
                        labels.Count == 1 &&
                        DataConverter.IsLabelValid(labels[0]) &&
                        result.ContainsKey(labels[0])
                    )
                {
                    CompileLog.Log(string.Format("Label defined at {0} already defined before: {1}", i, lines[i]), CompileIssue.Error);
                    break;
                }
                else if (labels.Count == 1 && DataConverter.IsLabelValid(labels[0]))
                {
                    result.Add(labels[0], 0);
                    CompileLog.Log(string.Format("Found new label {0} at line {1}: {2}", labels[0], i, lines[i]), CompileIssue.Info);
                }
            }

            return result;
        }
    }
}
