namespace Continuum93.Emulator.Mnemonics
{
    public static class MainOperatorsInit
    {
        public static void InitializeOperators()
        {
            Mnem.AddPrimeOp(
                "NOP", Mnem.NOOP, "No operation", ["No operation is executed."]);
            Mnem.AddPrimeOp(
                "LD", Mnem.LD, "Load data", ["Loads into the first operand({0}) the value of the second operand ({1})."]);
            //Mnem.AddPrimeOp("LD16", Mnem.LD16, "Load data on 16-bit", ["Loads into the first operand({0}) the value of the second operand ({1})."]);
            //Mnem.AddPrimeOp("LD24", Mnem.LD24, "Load data on 24 bit", ["Loads into the first operand({0}) the value of the second operand ({1})."]);
            //Mnem.AddPrimeOp("LD32", Mnem.LD32, "Load data on 32 bit", ["Loads into the first operand({0}) the value of the second operand ({1})."]);

            Mnem.AddPrimeOp("ADD", Mnem.ADD, "Add data", ["Adds the second operand ({1}) to the value of the first operand ({0})."]);
            //Mnem.AddPrimeOp("ADD16", Mnem.ADD16, "Add data on 16-bit", ["Adds the second operand ({1}) to the value of the first operand ({0})."]);
            //Mnem.AddPrimeOp("ADD24", Mnem.ADD24, "Add data on 24 bit", ["Adds the second operand ({1}) to the value of the first operand ({0})."]);
            //Mnem.AddPrimeOp("ADD32", Mnem.ADD32, "Add data on 32 bit",
            //    ["Adds the second operand ({1}) to the value of the first operand ({0})."]);

            Mnem.AddPrimeOp("SUB", Mnem.SUB, "Subtract data",
                ["Subtracts the second operand ({1}) from the first operand ({0})."]);
            //Mnem.AddPrimeOp("SUB16", Mnem.SUB16, "Subtract data on 16-bit",
            //    ["Subtracts the second operand ({1}) from the first operand ({0})."]); ;
            //Mnem.AddPrimeOp("SUB24", Mnem.SUB24, "Subtract data on 24 bit",
            //    ["Subtracts the second operand ({1}) from the first operand ({0})."]);
            //Mnem.AddPrimeOp("SUB32", Mnem.SUB32, "Subtract data on 32 bit",
            //    ["Subtracts the second operand ({1}) from the first operand ({0})."]);

            Mnem.AddPrimeOp("DIV", Mnem.DIV, "Divide data",
                [
                    "Divides first operand ({0}) by the second operand ({1}). Result is stored back in the first operand.",
                    "Legacy remainder variants have been removed; use MOD to compute the remainder." ]);

            Mnem.AddPrimeOp("MOD", Mnem.MOD, "Modulo (remainder)",
                [
                    "Divides first operand ({0}) by the second operand ({1}) and stores the remainder back in the first operand."
                ]);
            Mnem.AddPrimeOp("MUL", Mnem.MUL, "Multiply data",
                ["Multiplies first operand ({0}) by the value of the second operand ({1}). Result is stored back in the first operand."]);

            Mnem.AddPrimeOp("SL", Mnem.SL, "Shift bits left",
                ["Shift to the left all bits of first operand ({0}) by a number of bits described by the second operand ({1}). Rightmost bits are being filled with zero."]);
            Mnem.AddPrimeOp("SR", Mnem.SR, "Shift bits right",
                ["Shift to the right all bits of first operand ({0}) by a number of bits described by the second operand ({1}). Leftmost bits are being filled with zero."]);
            Mnem.AddPrimeOp("RL", Mnem.RL, "Roll bits left",
                ["Roll to the left all bits of first operand ({0}) by a number of bits described by the second operand ({1}). Rightmost bits are being filled with the bits exiting from the left side."]);
            Mnem.AddPrimeOp("RR", Mnem.RR, "Roll bits right",
                ["Roll to the right all bits of first operand ({0}) by a number of bits described by the second operand ({1}). Leftmost bits are being filled with the bits exiting from the right side."]);

            Mnem.AddPrimeOp("SET", Mnem.SET, "Set bit",
                ["Set one bit of first operand ({0}) at a position indicated by the second operand ({1})."]);
            Mnem.AddPrimeOp("RES", Mnem.RES, "Reset bit",
                ["Reset one bit of first operand ({0}) at a position indicated by the second operand ({1})."]);
            Mnem.AddPrimeOp("BIT", Mnem.BIT, "Test bit",
                ["Tests one bit of first operand ({0}) at a position indicated by the second operand ({1}). Flag Z will be set depending on the result."]);

            Mnem.AddPrimeOp("AND", Mnem.AND, "Logical AND",
                ["Performs a logical AND operation on the first operand ({0}) against the second operand ({1})"]);
            Mnem.AddPrimeOp("AND16", Mnem.AND16, "Logical AND 16-bit",
                ["Performs a logical AND operation on the first operand ({0}) against the second operand ({1})"]);
            Mnem.AddPrimeOp("AND24", Mnem.AND24, "Logical AND 24 bit",
                ["Performs a logical AND operation on the first operand ({0}) against the second operand ({1})"]);
            Mnem.AddPrimeOp("AND32", Mnem.AND32, "Logical AND 32 bit",
                ["Performs a logical AND operation on the first operand ({0}) against the second operand ({1})"]);
            Mnem.AddPrimeOp("OR", Mnem.OR, "Logical OR",
                ["Performs a logical OR operation on the first operand ({0}) against the second operand ({1})"]);
            Mnem.AddPrimeOp("OR16", Mnem.OR16, "Logical OR 16-bit",
                ["Performs a logical OR operation on the first operand ({0}) against the second operand ({1})"]);
            Mnem.AddPrimeOp("OR24", Mnem.OR24, "Logical OR 24 bit",
                ["Performs a logical OR operation on the first operand ({0}) against the second operand ({1})"]);
            Mnem.AddPrimeOp("OR32", Mnem.OR32, "Logical OR 32 bit",
                ["Performs a logical OR operation on the first operand ({0}) against the second operand ({1})"]);
            Mnem.AddPrimeOp("XOR", Mnem.XOR, "Logical exclusive OR",
                ["Performs a logical exclusive OR operation on the first operand ({0}) against the second operand ({1})"]);
            Mnem.AddPrimeOp("XOR16", Mnem.XOR16, "Logical exclusive OR 16-bit",
                ["Performs a logical exclusive OR operation on the first operand ({0}) against the second operand ({1})"]);
            Mnem.AddPrimeOp("XOR24", Mnem.XOR24, "Logical exclusive OR 24 bit",
                ["Performs a logical exclusive OR operation on the first operand ({0}) against the second operand ({1})"]);
            Mnem.AddPrimeOp("XOR32", Mnem.XOR32, "Logical exclusive OR 32 bit",
                ["Performs a logical exclusive OR operation on the first operand ({0}) against the second operand ({1})"]);

            Mnem.AddPrimeOp("INV", Mnem.INV, "Invert bits",
                ["Inverts all bits of {0}"]);

            Mnem.AddPrimeOp("EX", Mnem.EX, "Exchange values",
                ["Exchanges between {0} and {1}"]);

            Mnem.AddPrimeOp("CP", Mnem.CP, "Compare",
                ["Compares {0} with {1}. Modifies flags accordingly."]);

            // INC, DEC
            Mnem.AddPrimeOp("INC", Mnem.INC, "Increment",
                ["Increments {0}"]);
            Mnem.AddPrimeOp("INC16", Mnem.INC16, "Increment 16-bit",
                ["Increments {0}"]);
            Mnem.AddPrimeOp("INC24", Mnem.INC24, "Increment 24 bit",
                ["Increments {0}"]);
            Mnem.AddPrimeOp("INC32", Mnem.INC32, "Increment 32 bit",
                ["Increments {0}"]);
            Mnem.AddPrimeOp("DEC", Mnem.DEC, "Decrement",
                ["Decrements {0}"]);
            Mnem.AddPrimeOp("DEC16", Mnem.DEC16, "Decrement 16-bit",
                ["Decrements {0}"]);
            Mnem.AddPrimeOp("DEC24", Mnem.DEC24, "Decrement 24 bit",
                ["Decrements {0}"]);
            Mnem.AddPrimeOp("DEC32", Mnem.DEC32, "Decrement 32 bit",
                ["Decrements {0}"]);

            // CALL, JUMP
            Mnem.AddPrimeOp("CALL", Mnem.CALL, "Call",
                [
                    "Calls to a global address pointed by {0}. It leaves the address of the next instruction in the call stack so a RET instruction will be able to resume from there.",
                    "Conditionally calls to a global address pointed by {1} if {0} is true. It leaves the address of the next instruction in the call stack so a RET instruction will be able to resume from there."
                ]);
            Mnem.AddPrimeOp("CALLR", Mnem.CALLR, "Call relative",
                [
                    "Calls to an address relative from the current position pointed by {0}. It leaves the address of the next instruction in the call stack so a RET instruction will be able to resume from there.",
                    "Conditionally calls to an address relative from the current position pointed by {1} if {0} is true. It leaves the address of the next instruction in the call stack so a RET instruction will be able to resume from there."
                ]);

            Mnem.AddPrimeOp("JP", Mnem.JP, "Jump",
                [
                    "Jumps to a global address pointed by {0}. No return address is kept in the stack.",
                    "Conditionally jumps to a global address pointed by {1} if {0} is true. No return address is kept in the stack."
                ]);
            Mnem.AddPrimeOp("JR", Mnem.JR, "Jump relative",
                [
                    "Jumps to an address relative from the current position pointed by {0}. No return address is kept in the stack.",
                    "Jumps to an address relative from the current position pointed by {1} if {0} is true. No return address is kept in the stack."
                ]);

            // POP. PUSH
            Mnem.AddPrimeOp("POP", Mnem.POP, "Pop from stack",
                [
                    "Retrieves from the register stack a corresponding value to load in {0}.",
                    "Retrieves from the register stack all corresponding values to load in registers starting with {0} up to {1}. PUSH r1, rn will be retrieved correctly by POP r1, rn."
                ]);
            Mnem.AddPrimeOp("POP16", Mnem.POP, "Pop from stack a 16 bit value",
                [
                    "Retrieves from the register stack a corresponding value to load in {0}.",
                ]);
            Mnem.AddPrimeOp("POP24", Mnem.POP, "Pop from stack a 24 bit value",
                [
                    "Retrieves from the register stack a corresponding value to load in {0}.",
                ]);
            Mnem.AddPrimeOp("POP32", Mnem.POP, "Pop from stack a 32 bit value",
                [
                    "Retrieves from the register stack a corresponding value to load in {0}.",
                ]);
            Mnem.AddPrimeOp("PUSH", Mnem.PUSH, "Push to stack",
                [
                    "Stores to the register stack the value of {0}.",
                    "Stores to the register stack the values of {0} up to {1}. PUSH r1, rn will be retrieved correctly by POP r1, rn."
                ]);
            Mnem.AddPrimeOp("PUSH16", Mnem.PUSH16, "Push 16 bit to stack",
                [
                    "Stores to the register stack the value of {0}.",
                ]);
            Mnem.AddPrimeOp("PUSH24", Mnem.PUSH24, "Push 24 bit to stack",
                [
                    "Stores to the register stack the value of {0}.",
                ]);
            Mnem.AddPrimeOp("PUSH32", Mnem.PUSH32, "Push 32 bit to stack",
                [
                    "Stores to the register stack the value of {0}.",
                ]);

            Mnem.AddPrimeOp("INT", Mnem.INT, "Interrupt",
                ["Calls an interrupt specified by {0} with {1} as a parameter. Combinations of interrupt number and parameter value point to a specific interrupt function to be called."]);

            Mnem.AddPrimeOp("RAND", Mnem.RAND, "Random",
                [
                    "Generates a random number stored in the first operand ({0}) of corresponding max bit size value.",
                    "Generates a random number stored in the first operand ({0}) of a maximum value described by the second operand ({1})."
                ]);

            Mnem.AddPrimeOp("MIN", Mnem.MIN, "Minimum",
                [
                    "Calculates the minimum value between {0} and {1} and loads the result in the first register operand."
                ]);
            Mnem.AddPrimeOp("MAX", Mnem.MAX, "Maximum",
                [
                    "Calculates the maximum value between {0} and {1} and loads the result in the first register operand."
                ]);

            Mnem.AddPrimeOp("DJNZ", Mnem.DJNZ, "Decrement and jump if not zero", ["Decrements {0} and if it reached zero, jumps to {1}."]);

            Mnem.AddPrimeOp("DJNZ16", Mnem.DJNZ, "Decrement and jump if not zero", ["Decrements 16 bit {0} and if it reached zero, jumps to {1}."]);

            Mnem.AddPrimeOp("DJNZ24", Mnem.DJNZ, "Decrement and jump if not zero", ["Decrements 24 bit {0} and if it reached zero, jumps to {1}."]);

            Mnem.AddPrimeOp("DJNZ32", Mnem.DJNZ, "Decrement and jump if not zero", ["Decrements 32 bit {0} and if it reached zero, jumps to {1}."]);

            Mnem.AddPrimeOp("SETF", Mnem.SETF, "Set flag", ["Sets {0} to 1."]);

            Mnem.AddPrimeOp("RESF", Mnem.RESF, "Reset flag", ["Resets {0} to 0."]);

            Mnem.AddPrimeOp("INVF", Mnem.INVF, "Invert flag", ["Inverts {0}."]);

            Mnem.AddPrimeOp("LDF", Mnem.LDF, "Load flags", [
                "Load flags as a byte of state bits to {0}.",
                "Load flags as a byte of state bits, uses {1} as an AND mask and deposits the result in {0}."
            ]);

            Mnem.AddPrimeOp("REGS", Mnem.REGS, "Active register bank", ["Sets the current register bank to the value described by {0}."]);

            Mnem.AddPrimeOp("WAIT", Mnem.WAIT, "Wait (milliseconds)", ["Waits for a specified number of milliseconds given by the value described by {0}."]);

            Mnem.AddPrimeOp("VDL", Mnem.VDL, "Video draw layers",
                [ "When Buffer Auto Mode is set to manual mode, allows copying the respective layer(s) data, indicated by {0} from video RAM to the video buffers to be drawn on screen."
                ]);

            Mnem.AddPrimeOp("VCL", Mnem.VCL, "Video clear layers",
                [ "When Buffer Auto Mode is set to manual mode, allows clearing the respective backbuffer layer(s) data, indicated by {0}."
                ]);

            Mnem.AddPrimeOp("FIND", Mnem.FIND, "Find a byte", ["Find a byte given an address to start searching from. Starts searching at {0} for {1}. When it finds the byte, it returns the address in the first register operand."]);

            Mnem.AddPrimeOp("IMPLY", Mnem.IMPLY, "Logical IMPLY",
                ["Performs a logical IMPLY operation on {0} against {1}"]);
            Mnem.AddPrimeOp("IMPLY16", Mnem.IMPLY, "Logical IMPLY 16-bit",
                ["Performs a logical IMPLY operation on {0} against {1}"]);
            Mnem.AddPrimeOp("IMPLY24", Mnem.IMPLY, "Logical IMPLY 24 bit",
                ["Performs a logical IMPLY operation on {0} against {1}"]);
            Mnem.AddPrimeOp("IMPLY32", Mnem.IMPLY, "Logical IMPLY 32 bit",
                ["Performs a logical IMPLY operation on {0} against {1}"]);

            //NAND
            Mnem.AddPrimeOp("NAND", Mnem.NAND, "Logical NAND",
                ["Performs a logical NAND operation on {0} against {1}"]);
            Mnem.AddPrimeOp("NAND16", Mnem.NAND, "Logical NAND 16-bit",
                ["Performs a logical NAND operation on {0} against {1}"]);
            Mnem.AddPrimeOp("NAND24", Mnem.NAND, "Logical NAND 24 bit",
                ["Performs a logical NAND operation on {0} against {1}"]);
            Mnem.AddPrimeOp("NAND32", Mnem.NAND, "Logical NAND 32 bit",
                ["Performs a logical NAND operation on {0} against {1}"]);

            //NOR
            Mnem.AddPrimeOp("NOR", Mnem.NOR, "Logical NOR",
                ["Performs a logical NOR operation on {0} against {1}"]);
            Mnem.AddPrimeOp("NOR16", Mnem.NOR, "Logical NOR 16-bit",
                ["Performs a logical NOR operation on {0} against {1}"]);
            Mnem.AddPrimeOp("NOR24", Mnem.NOR, "Logical NOR 24 bit",
                ["Performs a logical NOR operation on {0} against {1}"]);
            Mnem.AddPrimeOp("NOR32", Mnem.NOR, "Logical NOR 32 bit",
                ["Performs a logical NOR operation on {0} against {1}"]);

            //XNOR
            Mnem.AddPrimeOp("XNOR", Mnem.XNOR, "Logical XNOR",
                ["Performs a logical XNOR operation on {0} against {1}"]);
            Mnem.AddPrimeOp("XNOR16", Mnem.XNOR, "Logical XNOR 16-bit",
                ["Performs a logical XNOR operation on {0} against {1}"]);
            Mnem.AddPrimeOp("XNOR24", Mnem.XNOR, "Logical XNOR 24 bit",
                ["Performs a logical XNOR operation on {0} against {1}"]);
            Mnem.AddPrimeOp("XNOR32", Mnem.XNOR, "Logical XNOR 32 bit",
                ["Performs a logical XNOR operation on {0} against {1}"]);

            Mnem.AddPrimeOp("GETBITS", Mnem.GETBITS, "Extracts bits from memory", [
                "Extracts a specified number of bits specified by operand 3 ({2}) starting from a bit address described by operand 2 ({1}) and stores the resulting value into a register described by operand 1 ({0})."
            ]);

            Mnem.AddPrimeOp("SETBITS", Mnem.SETBITS, "Sets bits in memory", [
                "Sets a specified number of bits specified by operand 3 ({2}) in memory starting at a bit address described by operand 2 ({1}) using the value contained in the register described by operand 1 ({0})."
            ]);

            Mnem.AddPrimeOp("MEMF", Mnem.MEMF, "Memory fill", ["Fills target address described by operand 1 ({0}) with a number of bytes described by operand 2 ({1}) of value described by operand 3 ({2})."]);

            Mnem.AddPrimeOp("MEMC", Mnem.MEMC, "Memory copy", ["Copies from address described by operand 1 ({0}) to address described by operand 2 ({1}) number of bytes described by operand 3 ({2})."]);

            Mnem.AddPrimeOp("LDREGS", Mnem.LDREGS, "Load registers from memory",
                ["Loads the provided range of registers indicated by the first two parameters with the data pulled from {2}."]);

            Mnem.AddPrimeOp("STREGS", Mnem.STREGS, "Stores registers to memory",
                ["Stores the provided range of registers indicated by the last two parameters into memory at an address indicated by {0}."]);

            Mnem.AddPrimeOp("SDIV", Mnem.SDIV, "Signed divide",
                [
                    "Divides first operand ({0}) by the second operand ({1}) as signed numbers. Result is stored in the first register.",
                    "Divides first operand ({0}) by the second operand ({1}) as signed numbers. Result is stored in the first register while the remainder is stored in the third operand ({2})."
                ]);

            Mnem.AddPrimeOp("SMUL", Mnem.SMUL, "Signed multiply",
                ["Multiplies the first operand ({0}) by the second operand ({1})."]);

            Mnem.AddPrimeOp("SCP", Mnem.SCP, "Signed compare",
                ["Compares as signed the first operand ({0}) with the second operand ({1}). Modifies flags accordingly."]);

            Mnem.AddPrimeOp("FREGS", Mnem.FREGS, "Active float register bank", ["Sets the current floating register bank to the value described by {0}."]);

            Mnem.AddPrimeOp("POW", Mnem.POW, "Raise to power", ["Raises first operand {0} to the power indicated by second operand {1} and deposits the result back in the first operand."]);

            Mnem.AddPrimeOp("SQR", Mnem.SQR, "Square root", [
                "Calculates the square root of {0} and overwrites it with the result.",
                "Calculates the square root of the second operand {1} and deposits the result back in the first operand: {0}."
            ]);

            Mnem.AddPrimeOp("CBR", Mnem.CBR, "Raise to power", [
                "Calculates the cube root of {0} and overwrites it with the result.",
                "Calculates the cube root of second operand {1} and deposits the result back in the first operand: {0}."
            ]);

            Mnem.AddPrimeOp("ISQR", Mnem.ISQR, "Inverse square root", [
                "Calculates the inverse square root of {0} and overwrites it with the result.",
                "Calculates the inverse square root of the second operand {1} and deposits the result back in the first operand: {0}."
            ]);

            Mnem.AddPrimeOp("ISGN", Mnem.ISGN, "Inverse sign of float", [
                "Inverts the sign of {0} and overwrites it with the result.",
                "Inverts the sign of the second operand {1} and deposits the result back in the first operand: {0}."
            ]);

            Mnem.AddPrimeOp("SIN", Mnem.SIN, "Sine",
                [
                    "Calculates sine of {0} and overwrites with the result.",
                    "Calculates the sine value of {1} as the second operand and deposits the result in {0} which is the first operand."
                ]);

            Mnem.AddPrimeOp("COS", Mnem.COS, "Cosine",
                [
                    "Calculates cosine of {0} and overwrites with the result.",
                    "Calculates the cosine value of {1} as the second operand and deposits the result in {0} which is the first operand."
                ]);

            Mnem.AddPrimeOp("TAN", Mnem.TAN, "Tangent",
                [
                    "Calculates tangent of {0} and overwrites with the result.",
                    "Calculates the tangent value {1} as the second operand and deposits the result in {0} which is the first operand."
                ]);

            Mnem.AddPrimeOp("ABS", Mnem.ABS, "Absolute value",
                [
                    "Calculates the absolute value of {0} and overwrites with the result.",
                    "Calculates the absolute value of {1} as the second operand and deposits the result in {0} which is the first operand."
                ]);

            Mnem.AddPrimeOp("ROUND", Mnem.ROUND, "Rounds value",
                [
                    "Rounds the value of {0} and overwrites with the result.",
                    "Rounds the value of {1} as the second operand and deposits the result in {0} which is the first operand."
                ]);

            Mnem.AddPrimeOp("FLOOR", Mnem.FLOOR, "Rounds down value",
                [
                    "Rounds down the value of {0} and overwrites with the result.",
                    "Rounds down the value of {1} as the second operand and deposits the result in {0} which is the first operand."
                ]);

            Mnem.AddPrimeOp("CEIL", Mnem.CEIL, "Rounds up value",
                [
                    "Rounds up the value of {0} and overwrites with the result.",
                    "Rounds up the value of {1} as the second operand and deposits the result in {0} which is the first operand."
                ]);

            Mnem.AddPrimeOp("PLAY", Mnem.PLAY, "Plays a sound",
                ["Plays a sound with parameters located at {0}"]);

            Mnem.AddPrimeOp("SETVAR", Mnem.SETVAR, "Sets a system variable", ["Sets the value of the system variable specified by index by {0} to the value specified by {1}"]);
            Mnem.AddPrimeOp("GETVAR", Mnem.GETVAR, "Gets a system variable", ["Gets the value of the system variable specified by index by the first operand: {0} into {1}"]);

            Mnem.AddPrimeOp("RGB2HSL", Mnem.RGB2HSL, "Converts a RGB value to the HSL equivalent",
                ["Converts the RGB value of {0} to HSL and stores it into the result {1}."]);

            Mnem.AddPrimeOp("HSL2RGB", Mnem.HSL2RGB, "Converts a HSL value to the RGB equivalent",
                ["Converts the HSL value of {0} to RGB and stores it into the result {1}."]);

            Mnem.AddPrimeOp("RGB2HSB", Mnem.RGB2HSB, "Converts a RGB value to the HSB equivalent",
                ["Converts the RGB value of {0} to HSB and stores it into the result {1}."]);

            Mnem.AddPrimeOp("HSB2RGB", Mnem.HSB2RGB, "Converts a HSB value to the RGB equivalent",
                ["Converts the HSB value of {0} to RGB and stores it into the result {1}."]);


            Mnem.AddPrimeOp("DEBUG", Mnem.DEBUG, "Debug state initiator",
                ["Signals a debug state to the CPU which is then relayed to an outside source, in this case the Continuum Tools which catches it. This is used for debugging only and does not affect any registers, flags, memory or stacks."]);

            Mnem.AddPrimeOp("BREAK", Mnem.BREAK, "Break CPU",
                ["Stops execution of the whole machine. This is unrecoverable until a full machine reset."]);
            Mnem.AddPrimeOp("RETIF", Mnem.RETIF, "Return from Call if flag is true",
                ["Returns from a subroutine if the corresponding flag is true by resuming execution to the next address saved to stack while removing that address from stack."]);
            Mnem.AddPrimeOp("RET", Mnem.RET, "Return from Call",
                ["Returns from a subroutine by resuming execution to the next address saved to stack while removing that address from stack."]);

            /*string output = "";

            foreach (KeyValuePair<string, Oper> primeOp in Mnem.OPS)
            {
                Oper oper = primeOp.Value;

                if (oper.IsPrimary)
                {
                    output += oper.Mnemonic + ": " + oper.Title + Constants.CR;
                }
            }
            */

        }
    }
}
