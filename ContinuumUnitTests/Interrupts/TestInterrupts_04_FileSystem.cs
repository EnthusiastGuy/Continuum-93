using Continuum93.Emulator;
using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;
using System.Text;


namespace Interrupts
{

    public class TestInterrupts_FileSystem
    {
        [Fact]
        public void TestInt_FileExists()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 2; // 0x02 - CheckIfFileExists

            computer.LoadMemAt(10000, new byte[] { 0x72, 0x65, 0x61, 0x64, 0x6d, 0x65, 0x2e, 0x74, 0x78, 0x74, 0 });    // readme.txt

            cp.Build(@"
                LD BCD, 10000
                INT 4, A
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFF, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_DirectoryExists_1()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 3; // 0x03 - CheckIfDirectoryExists

            computer.LoadMemAt(10000, new byte[] { 0x72, 0x65, 0x61, 0x64, 0x6d, 0x65, 0x2e, 0x74, 0x78, 0x74, 0 });    // readme.txt

            cp.Build(@"
                LD BCD, 10000
                INT 4, A
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0x0, computer.CPU.REGS.A);  // This is a file, so it should be zero
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_DirectoryExists_2()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 3; // 0x03 - CheckIfDirectoryExists

            string path = Path.Combine("Data", "filesystem", "testDir");
            Directory.CreateDirectory(path);

            computer.LoadMemAt(10000, new byte[] { 0x74, 0x65, 0x73, 0x74, 0x44, 0x69, 0x72, 0 });    // testDir

            cp.Build(@"
                LD BCD, 10000
                INT 4, A
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Directory.Delete(path);

            Assert.Equal(0xFF, computer.CPU.REGS.A);
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_ListFilesInDirectory()
        {
            Assembler cp = new();
            using Computer computer = new();

            computer.CPU.REGS.A = 4; // 0x04 - ListFilesInDirectory
            string[] files = new string[] { "aFile2.txt", "autoexec.bat", "container.bin", "NO_EXT" };
            string[] directories = new string[] { "bin", "logs", "trash" };

            string testDir = Path.Combine("Data", "filesystem", "testDir");
            Directory.CreateDirectory(testDir);

            foreach (string directory in directories)
                Directory.CreateDirectory(Path.Combine("Data", "filesystem", "testDir", directory));

            foreach (string file in files)
                File.Create(Path.Combine("Data", "filesystem", "testDir", file)).Close();

            computer.LoadMemAt(10000, new byte[] { 0x74, 0x65, 0x73, 0x74, 0x44, 0x69, 0x72, 0 });    // testDir

            cp.Build(@"
                LD BCD, 10000
                LD EFG, 20
                INT 4, A
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Set the verification array
            string allFiles = "";

            foreach (string file in files)
                allFiles += Path.GetFileName(file) + Constants.TERMINATOR;

            allFiles += Constants.TERMINATOR;

            byte[] byteFiles = Encoding.ASCII.GetBytes(allFiles);
            byte[] actual = computer.GetMemFrom(20, (uint)allFiles.Length);

            Assert.Equal(byteFiles, actual);
            Assert.Equal(0x04, (long)computer.CPU.REGS.BCD);

            // Cleanup
            foreach (string file in files)
                File.Delete(Path.Combine("Data", "filesystem", "testDir", file));

            foreach (string directory in directories)
                Directory.Delete(Path.Combine("Data", "filesystem", "testDir", directory));

            Directory.Delete(testDir);
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_ListFilesInDirectoryNotFound()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD A, 4     ; 0x04 - ListFilesInDirectory
                LD BCD, .Inexistent
                LD EFG, 20
                INT 4, A
                BREAK
            .Inexistent
                #DB ""ThisDoesNotExist"", 0
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFFFFFF, (long)computer.CPU.REGS.BCD);
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_ListDirectoriesInDirectory()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 5; // 0x05 - ListDirectoriesInDirectory
            string[] files = new string[] { "aFile2.txt", "autoexec.bat", "container.bin", "NO_EXT" };
            string[] directories = new string[] { "bin", "logs", "trash" };

            string testDir = Path.Combine("Data", "filesystem", "testDir");
            Directory.CreateDirectory(testDir);

            foreach (string directory in directories)
                Directory.CreateDirectory(Path.Combine("Data", "filesystem", "testDir", directory));

            foreach (string file in files)
                File.Create(Path.Combine("Data", "filesystem", "testDir", file)).Close();

            computer.LoadMemAt(10000, new byte[] { 0x74, 0x65, 0x73, 0x74, 0x44, 0x69, 0x72, 0 });    // testDir

            cp.Build(@"
                LD BCD, 10000
                LD EFG, 20
                INT 4, A
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Set the verification array
            string allDirs = "";

            foreach (string directory in directories)
                allDirs += Path.GetFileName(directory) + Constants.TERMINATOR;

            allDirs += Constants.TERMINATOR;

            byte[] byteFiles = Encoding.ASCII.GetBytes(allDirs);
            byte[] actual = computer.GetMemFrom(20, (uint)allDirs.Length);

            // Cleanup
            foreach (string file in files)
                File.Delete(Path.Combine("Data", "filesystem", "testDir", file));

            foreach (string directory in directories)
                Directory.Delete(Path.Combine("Data", "filesystem", "testDir", directory));

            Directory.Delete(testDir);

            Assert.Equal(byteFiles, actual);
            Assert.Equal(0x03, (long)computer.CPU.REGS.BCD);

            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_ListDirectoriesAndFilesInDirectory()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0x15; // 0x15 - ListDirectoriesAndFilesInDirectory
            string[] files = new string[] { "aFile2.txt", "autoexec.bat", "container.bin", "NO_EXT" };
            string[] directories = new string[] { "bin", "logs", "trash" };

            string testDir = Path.Combine("Data", "filesystem", "testDir");
            Directory.CreateDirectory(testDir);

            foreach (string directory in directories)
                Directory.CreateDirectory(Path.Combine("Data", "filesystem", "testDir", directory));

            foreach (string file in files)
                File.Create(Path.Combine("Data", "filesystem", "testDir", file)).Close();

            computer.LoadMemAt(10000, new byte[] { 0x74, 0x65, 0x73, 0x74, 0x44, 0x69, 0x72, 0 });    // testDir

            cp.Build(@"
                LD BCD, 10000
                LD EFG, 20
                INT 0x04, A
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Set the verification array
            string allEntries = "";

            foreach (string directory in directories)
                allEntries += Path.GetFileName(directory) + Constants.TERMINATOR;

            allEntries += Constants.TERMINATOR;

            foreach (string file in files)
                allEntries += Path.GetFileName(file) + Constants.TERMINATOR;

            allEntries += Constants.TERMINATOR;

            byte[] byteFiles = Encoding.ASCII.GetBytes(allEntries);
            byte[] actual = computer.GetMemFrom(20, (uint)allEntries.Length);

            // Cleanup
            foreach (string file in files)
                File.Delete(Path.Combine("Data", "filesystem", "testDir", file));

            foreach (string directory in directories)
                Directory.Delete(Path.Combine("Data", "filesystem", "testDir", directory));

            Directory.Delete(testDir);

            Assert.Equal(byteFiles, actual);
            Assert.Equal(7, (long)computer.CPU.REGS.BCD);
            Assert.Equal(computer.CPU.REGS.EFG + computer.CPU.REGS.HIJ, (long)computer.CPU.REGS.BCD);
            Assert.Equal(3, (long)computer.CPU.REGS.EFG);
            Assert.Equal(4, (long)computer.CPU.REGS.HIJ);
            Assert.Equal(16, (long)computer.CPU.REGS.KLM);

            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_ListDirectoriesAndFilesInEmptyDirectory()
        {
            Assembler cp = new();
            using Computer computer = new();


            computer.CPU.REGS.A = 0x15; // 0x15 - ListDirectoriesAndFilesInDirectory
            string[] files = new string[] { };
            string[] directories = new string[] { };

            string testDir = Path.Combine("Data", "filesystem", "testDir");
            Directory.CreateDirectory(testDir);

            foreach (string directory in directories)
                Directory.CreateDirectory(Path.Combine("Data", "filesystem", "testDir", directory));

            foreach (string file in files)
                File.Create(Path.Combine("Data", "filesystem", "testDir", file)).Close();

            computer.LoadMemAt(10000, new byte[] { 0x74, 0x65, 0x73, 0x74, 0x44, 0x69, 0x72, 0 });    // testDir

            cp.Build(@"
                LD BCD, 10000
                LD EFG, 20
                INT 0x04, A
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            // Set the verification array
            string allEntries = "";

            foreach (string directory in directories)
                allEntries += Path.GetFileName(directory) + Constants.TERMINATOR;

            allEntries += Constants.TERMINATOR;

            foreach (string file in files)
                allEntries += Path.GetFileName(file) + Constants.TERMINATOR;

            allEntries += Constants.TERMINATOR;

            byte[] byteFiles = Encoding.ASCII.GetBytes(allEntries);
            byte[] actual = computer.GetMemFrom(20, (uint)allEntries.Length);

            // Cleanup
            foreach (string file in files)
                File.Delete(Path.Combine("Data", "filesystem", "testDir", file));

            foreach (string directory in directories)
                Directory.Delete(Path.Combine("Data", "filesystem", "testDir", directory));

            Directory.Delete(testDir);

            Assert.Equal(byteFiles, actual);
            Assert.Equal(0, (long)computer.CPU.REGS.BCD);
            Assert.Equal(computer.CPU.REGS.EFG + computer.CPU.REGS.HIJ, (long)computer.CPU.REGS.BCD);
            Assert.Equal(0, (long)computer.CPU.REGS.EFG);
            Assert.Equal(0, (long)computer.CPU.REGS.HIJ);
            Assert.Equal(1, (long)computer.CPU.REGS.KLM);

            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_ListDirectoriesInDirectoryNotFound()
        {
            Assembler cp = new();
            using Computer computer = new();


            cp.Build(@"
                LD A, 5     ; 0x05 - ListDirectoriesInDirectory
                LD BCD, .Inexistent
                LD EFG, 20
                INT 4, A
                BREAK
            .Inexistent
                #DB ""ThisDoesNotExist"", 0
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            Assert.Equal(0xFFFFFF, (long)computer.CPU.REGS.BCD);
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_SaveFile()
        {
            Assembler cp = new();
            using Computer computer = new();


            string fileName = "MyTextFile.txt";
            byte[] fileNamebytes = Encoding.ASCII.GetBytes(fileName);
            byte[] fileContents = new byte[] {
                0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x77, 0x6f,
                0x72, 0x6c, 0x64, 0x2e, 0x20, 0x54, 0x68, 0x69,
                0x73, 0x20, 0x69, 0x73, 0x20, 0x61, 0x20, 0x66,
                0x69, 0x6c, 0x65, 0x21
            };

            computer.CPU.REGS.HIJ = (uint)fileContents.Length;

            computer.LoadMemAt(10000, fileNamebytes);
            computer.LoadMemAt(20000, fileContents);

            cp.Build(@"
                LD A, 6         ; 0x06 - SaveFile
                LD BCD, 10000   ; file name pointer
                LD EFG, 20000   ; memory start
                INT 4, A
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();

            string filePath = Path.Combine(Constants.FS_ROOT, fileName);

            if (File.Exists(filePath) && new FileInfo(filePath).Length == fileContents.Length)
            {
            }
            else
            {
                Assert.True(false, "Correct file creation failed");
            }

            File.Delete(filePath);
            TUtils.IncrementCountedTests("interrupts");
        }

        [Fact]
        public void TestInt_LoadFile()
        {
            Assembler cp = new();
            using Computer computer = new();


            string fileName = "MyReadTextFile.txt";
            byte[] fileNamebytes = Encoding.ASCII.GetBytes(fileName);
            byte[] fileContents = new byte[] {
                0x54, 0x68, 0x69, 0x73, 0x20, 0x69, 0x73, 0x20,
                0x61, 0x20, 0x66, 0x69, 0x6c, 0x65, 0x20, 0x74,
                0x68, 0x61, 0x74, 0x20, 0x6e, 0x65, 0x65, 0x64,
                0x73, 0x20, 0x74, 0x6f, 0x20, 0x62, 0x65, 0x20,
                0x72, 0x65, 0x61, 0x64, 0x20, 0x69, 0x6e, 0x20,
                0x6d, 0x65, 0x6d, 0x6f, 0x72, 0x79, 0x2e
            };
            string filePath = Path.Combine(Constants.FS_ROOT, fileName);

            File.WriteAllBytes(filePath, fileContents);

            computer.LoadMemAt(10000, fileNamebytes);

            cp.Build(@"
                LD A, 7         ; 0x07 - LoadFile
                LD BCD, 10000   ; file name pointer
                LD EFG, 50      ; memory address to load the file to
                INT 4, A
                BREAK
            ");

            byte[] compiled = cp.GetCompiledCode();

            computer.LoadMem(compiled);
            computer.Run();
            File.Delete(filePath);

            byte[] actual = computer.GetMemFrom(50, (uint)fileContents.Length);

            Assert.Equal(fileContents, actual);
            TUtils.IncrementCountedTests("interrupts");
        }
    }
}
