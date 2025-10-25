using Continuum93.Emulator.Audio.XSound;
using Continuum93.Emulator.Audio.XSound.ExecutionUtilities;
using Continuum93.Emulator.Mnemonics;

namespace Continuum93.Emulator.Execution
{
    public class ExPLAY
    {
        public static void Process(Computer computer)
        {
            byte ldOp = computer.MEMC.Fetch();

            switch (ldOp)
            {
                case Mnem.PLAY_nnn:     // PLAY nnn
                    {
                        uint address = computer.MEMC.Fetch24();

                        XSoundParams soundParams = XSoundParamsProcessing.ReadSoundParams(address, computer);
                        computer.APU.PlaySound(soundParams);

                        return;
                    }
                case Mnem.PLAY_rrr:     // PLAY rrr
                    {
                        uint address = computer.CPU.REGS.Get24BitRegister((byte)(computer.MEMC.Fetch() & 0b00011111));

                        XSoundParams soundParams = XSoundParamsProcessing.ReadSoundParams(address, computer);
                        computer.APU.PlaySound(soundParams);

                        return;
                    }
            }
        }
    }
}
