using Continuum93.Emulator;
using Continuum93.Emulator.AutoDocs;
using Continuum93.Emulator.AutoDocs.MetaInfo;
using Continuum93.Emulator.AutoDocs.styling;
using System;
using System.IO;

namespace Continuum93.Utils
{
    public static class UtilsManager
    {
        public static string[] Arguments;

        public static bool HasArguments()
        {
            return Arguments != null && (Arguments.Length > 0);
        }

        /// <summary>
        /// Processes the command-line arguments to determine the desired application action.
        /// New functionality is based on flags like '-docs' or '-png2font'.
        /// </summary>
        public static void ProcessArguments()
        {
            // If no arguments exist, we assume the application should start normally (e.g., run the emulator).
            if (!HasArguments())
            {
                // In a real application, you'd likely return here to proceed with normal emulator startup.
                // Log.WriteLine("No command-line arguments found. Starting emulator...");
                return;
            }

            Log.WriteLine("Processing command-line arguments...");

            // Loop through the arguments, looking for specific flags.
            for (int i = 0; i < Arguments.Length; i++)
            {
                string arg = Arguments[i].ToLower(); // Case-insensitive checking for flags.

                if (arg == "-docs" || arg == "/docs" || arg == "--docs")
                {
                    Log.WriteLine("Building documentation");
                    BuildDocumentation();
                    Log.WriteLine("Documentation built");

                    return;
                }

                if (arg == "-help" || arg == "/help" || arg == "--help")
                {
                    Log.WriteLine("Running help display mode.");
                    ShowHelp();
                    return;
                }

                // --- New Functionality: PNG to Font Conversion Flag ---
                // Expected format: Continuum93 -png2font [path]
                if (arg == "-png2font" || arg == "/png2font")
                {
                    // Check if the next argument (the path) exists.
                    if (i + 1 < Arguments.Length)
                    {
                        string fontPath = Arguments[i + 1];
                        Log.WriteLine($"Found '-png2font' flag. Attempting to convert file: {fontPath}");

                        if (File.Exists(fontPath) && Path.GetExtension(fontPath).ToLower() == ".png")
                        {
                            ConvertFont(fontPath);
                            // Skip the next argument since it was consumed as the path.
                            i++;
                            // Since this is a tool-like action, we can exit after execution.
                            return;
                        }
                        else
                        {
                            Log.WriteLine($"ERROR: File not found or is not a PNG at the specified path: {fontPath}");
                            // An error occurred, so we should likely stop execution.
                            return;
                        }
                    }
                    else
                    {
                        Log.WriteLine("ERROR: '-png2font' flag requires a path to a PNG file.");
                        return; // Stop due to incomplete command.
                    }
                }

                // If we reach here, the argument was not recognized as a flag or a direct path.
                Log.WriteLine($"WARNING: Unrecognized argument: {Arguments[i]}");
            }
        }

        public static void ConvertFont(string fontPath)
        {
            //Log.WriteLine("Converting Font");
            string filenameNoExt = Path.GetFileNameWithoutExtension(fontPath);
            string directory = Path.GetDirectoryName(fontPath);

            //Log.WriteLine("filenameNoExt: " + filenameNoExt);
            //Log.WriteLine("directory: " + directory);

            byte[] font = FontConvertor.ConvertFont(fontPath);
            //Log.WriteLine("Font created. Size: " + font.Length);
            File.WriteAllBytes(Path.Combine(directory, filenameNoExt + ".font"), font);
        }

        /// <summary>
        /// Placeholder method for displaying application documentation or help info.
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine("\n");
            Console.WriteLine("--- Continuum93 Emulator Command-Line Usage ---");
            Console.WriteLine("\n");
            Console.WriteLine("No arguments: Starts the emulator normally.");
            Console.WriteLine("\n");
            Console.WriteLine("-help: Shows this help page.");
            Console.WriteLine("-docs: Prepares the documentation suite.");
            Console.WriteLine("-png2font [path]: Converts the PNG file at [path] to a .font file.");
            Console.WriteLine("\n");
            Console.WriteLine("Emulator briefly shows up and then closes automatically when the operation is finished.");
            Console.WriteLine("\n");
        }

        private static void BuildDocumentation()
        {
            IntLib.Initialize();    // Move to initialize if not working
            ASMMetaInfo.Initialize();

            Directory.CreateDirectory("docs");
            File.WriteAllText(Path.Combine("docs", "styles.css"), DocStyle.GetDefaultStyle());
            File.WriteAllText(Path.Combine("docs", "stylesInfo.css"), DocStyle.GetInfoStyle());

            File.WriteAllText(Path.Combine("docs", "assembly.html"), ASMReferenceGenerator.GenerateInstructionPages());
            File.WriteAllText(Path.Combine("docs", "assembly_compact.html"), ASMMetaGenerator.GenerateMetaInfo());
            File.WriteAllText(Path.Combine("docs", "assembly_compact_brief.html"), ASMMetaGenerator.GenerateMetaInfo(true));

            File.WriteAllText(Path.Combine("docs", "interrupts.html"), IntLib.GenerateInterruptPages());
            File.WriteAllText(Path.Combine("docs", "sampleInstructions.txt"), DocsInstructionSamples.Generate());

            File.WriteAllText(Path.Combine("docs", "operatorsTable.txt"), ASMReferenceGenerator.GenerateOPSTable());
        }
    }
}
