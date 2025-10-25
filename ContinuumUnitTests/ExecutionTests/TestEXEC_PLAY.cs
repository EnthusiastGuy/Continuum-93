using Continuum93.Emulator.Interpreter;
using Continuum93.Emulator;
using Continuum93.Emulator.Audio.XSound;

namespace ExecutionTests
{
    public class TestEXEC_PLAY
    {

        [Fact]
        public void TestEXEC_PLAY_nnn()
        {
            Assembler cp = new();
            using Computer computer = new(true);

            cp.Build(@"
                PLAY .SoundDefinition
                WAIT 1000
                BREAK

            .SoundDefinition
                #DB 0b00000000, 0b00000000, 440.0, 200.0, 1.0
            ");

            Assert.Null(computer.APU.GetLastPlayedSound());

            computer.LoadMemAt(0, cp.GetCompiledCode());
            computer.Run();

            XSoundParams actualParams = computer.APU.GetLastPlayedSound();

            Assert.NotNull(actualParams);

            Assert.Equal(440, actualParams.Frequency);
            Assert.Equal(200, actualParams.EnvelopeSustain);
            Assert.Equal(1.0f, actualParams.SoundVolume);

            computer.Dispose();
        }

        [Fact]
        public void TestEXEC_PLAY_rrr()
        {
            Assembler cp = new();
            using Computer computer = new(true);

            cp.Build(@"
                LD ABC, .SoundDefinition
                PLAY ABC
                WAIT 1000
                BREAK

            .SoundDefinition
                #DB 0b00000000, 0b00000000, 420.0, 150.0, 0.8
            ");

            Assert.Null(computer.APU.GetLastPlayedSound());

            computer.LoadMemAt(0, cp.GetCompiledCode());
            computer.Run();

            XSoundParams actualParams = computer.APU.GetLastPlayedSound();

            Assert.NotNull(actualParams);

            Assert.Equal(420, actualParams.Frequency);
            Assert.Equal(150, actualParams.EnvelopeSustain);
            Assert.Equal(0.8f, actualParams.SoundVolume);

            computer.Dispose();
        }
    }
}
