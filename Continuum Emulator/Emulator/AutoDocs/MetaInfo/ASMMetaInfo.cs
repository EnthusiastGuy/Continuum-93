using System.Collections.Generic;

namespace Continuum93.Emulator.AutoDocs.MetaInfo
{
    public static class ASMMetaInfo
    {
        public static List<ASMMeta> MetaData = new();

        public static string GetDefinition(string op)
        {
            foreach (ASMMeta meta in MetaData)
            {
                if (meta.Operators.Contains(op))
                {
                    return meta.Description;
                }
            }

            return "";
        }

        public static ASMMeta GetMeta(string op)
        {
            foreach (ASMMeta meta in MetaData)
            {
                if (meta.Operators.Contains(op))
                {
                    return meta;
                }
            }

            return null;
        }

        public static void Initialize()
        {
            MetaData.Add(ASMMeta.Create(
                new List<string>() { "NOP" },
                "Performs no operation and is effectively a placeholder that consumes a small amount of time (typically the time it takes to execute one instruction) without changing any flags, registers, or memory locations.",
                "Timing Adjustments: In time-sensitive applications, NOP can be used to introduce small delays for synchronization purposes. Placeholder during Development: NOP can be inserted in code under development, particularly where the final instruction sequence is not yet determined. Creating Instruction Space: NOP is used to reserve space for future code modifications, especially in the context of patching or updating software. NOP can also be used to overwrite redundant instructions in self-modifying code scenarios so other instructions that are no longer needed will be overwritten, ensuring they have no effect.",
                "NOP"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "LD" },
                "Loads data from the source operand2 into the destination operand1. The source is a register, memory location or value and the destination is a register or memory location. The contents of the source are read and then placed into the destination, leaving the source unchanged.",
                "Data Transfer: LD is primarily used to transfer data from memory, registers or immediate values to registers. Initialization of Registers: It's used for initializing registers with specific values at the beginning of a program or a subroutine. Memory Manipulation: LD is crucial in programs that need to read and process data stored in memory. Preparation for Arithmetic/Logical Operations: Before performing arithmetic or logical operations, operands are often loaded into registers using LD.",
                "LD[bits] operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "ADD" },
                "Adds the value of the source operand2 to the destination operand1 and stores the result in the destination. The source operand2 remains unchanged. Modifies the carry flag if the result carries over and the zero flag if the result is zero.",
                "Arithmetic Calculations: ADD is used for general arithmetic calculations, which form the basis of numerous algorithms and operations. Incrementing Values: It can be used to increment values, for example, increasing a counter or iterating through array indices. Memory Address Calculations: In pointer arithmetic, ADD is used for calculating new memory addresses, such as moving to the next element in an array. Aggregation: In algorithms that require summing up data, like calculating averages or total values. Combining Results: It's used to combine results from different computations or to accumulate values in a loop.",
                "ADD[bits] operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "SUB", "SUB16", "SUB24", "SUB32" },
                "Subtracts the value of the source operand2 from the destination operand1 and stores the result in the destination. The source operand2 remains unchanged. Modifies the carry flag if the result borrows and the zero flag if the result is zero.",
                "Arithmetic Calculations: SUB is used for general arithmetic calculations involving subtraction for numerous algorithms and operations. Decrementing Values: It can be used to decrement values, such as decreasing a counter or moving backwards through array indices. Memory Address Calculations: In pointer arithmetic, SUB is used for calculating new memory addresses, like moving to a previous element in an array. Difference Calculation: It's used to calculate the difference between two values, like in comparison operations or in algorithms requiring delta values. Data Processing: SUB is integral in data processing tasks that require the subtraction operation, such as in digital signal processing or in computing offsets.",
                "SUB[bits] operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "MUL" },
                "Multiplies the value of operand1 by the value of operand2 and overwrites operand1 with the result. The second operand is never modified. No flags are affected.",
                "Arithmetic Calculations: MUL is used for multiplication operations in various mathematical algorithms and calculations such as matrix multiplication, in cryptographic algorithms, or in physics simulations. Scaling Data: It can be used to scale or amplify data values, such as in graphics processing or in digital signal processing.",
                "MUL operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "DIV" },
                "Divides the value of operand1 by the value of operand2 and overwrites operand1 with the result. The second operand is never modified. If operand3 is present, it will hold the remainder of the division. No flags are affected.",
                "Arithmetic Calculations: DIV is used for division operations in various algorithms and mathematical calculations. Data Scaling: It can be used to scale down data, such as in normalization tasks or adjusting values. Modulo Operations: DIV also provides the remainder of the division, which is useful for modulo operations.",
                "DIV operand1, operand2, [operand3]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "SL" },
                "Bitwise shift the bits of operand1 to the left by the number of bits described by operand2. It only works on simple registers and memory locations. No flags are affected.",
                "Most commonly used to multiply by factors of two. When packing data that takes a small amount of bits, this can be used to position the target bits at a specified position within a byte then using OR instructions to add more data. It can be used in image manipulations to reposition pixels and to generate bit masks.",
                "SL operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "SR" },
                "Bitwise shift the bits of operand1 to the right by the number of bits described by operand2. It only works on simple registers and memory locations. No flags are affected.",
                "Most commonly used to divide by factors of two. When packing data that takes a small amount of bits, this can be used to position the target bits at a specified position within a byte then using OR instructions to add more data. It can be used in image manipulations to reposition pixels and to generate bit masks.",
                "SR operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "RL" },
                "Bitwise rotate the bits of operand1 to the left by the number of bits described by operand2. Bits exiting through the end bit come back at the first bit. It only works on simple registers and memory locations. No flags are affected.",
                "It can be used for data manipulation where the bit pattern needs to be rotated rather than shifted linearly, to scramble or encode data in a way that is easy to reverse but changes the bit pattern significantly, can be used as a part of an algorithm for efficiently performing certain multiplications or modulo operations, especially with numbers that are powers of two.",
                "RL operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "RR" },
                "Bitwise rotate the bits of operand1 to the right by the number of bits described by operand2. Bits exiting through first bit come back at end bit. It only works on simple registers and memory locations. No flags are affected.",
                "It can be used for data manipulation where the bit pattern needs to be rotated rather than shifted linearly, to scramble or encode data in a way that is easy to reverse but changes the bit pattern significantly, can be used as a part of an algorithm for efficiently performing certain multiplications or modulo operations, especially with numbers that are powers of two.",
                "RR operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "SET" },
                "Bitwise set operand1's bit to 1 at a position indicated by the bit index described by operand2. It only works on simple registers and memory locations. No flags are affected.",
                "This is normally used to set a bit in a register that acts as a status register within a context defined by the developer. Can be used to set specific bit colors. For a more complex implementation the AND instruction can be used",
                "SET operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "RES" },
                "Bitwise reset operand1's bit to 0 at a position indicated by the bit index described by operand2. It only works on simple registers and memory locations. No flags are affected.",
                "This is normally used to reset a bit in a register that acts as a status register within a context defined by the developer. Can be used to set specific bit colors. For a more complex implementation the AND instruction can be used.",
                "RES operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "BIT" },
                "Bitwise tests operand1's bit at a position indicated by the bit index described by operand2. It only works on simple registers and memory locations. If the tested bit's value is 1, the Z flag is set..",
                "This is normally used to check status flags in a conditional execution. It can also be used to get input specifics from an input bit map loaded in memory.",
                "BIT operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "AND", "AND16", "AND24", "AND32" },
                "Takes each bit in operand1 and operand2, performs the logical AND operation on each pair of bits, and stores the result back in operand1. The logical AND operation follows the basic rule: if both bits are 1, the result is 1; otherwise, the result is 0.",
                "Clearing Specific Bits: By using an appropriate mask (where you want to clear bits, set mask bits to 0), you can clear specific bits in a register or memory location. Bit Masking: To extract specific bits from a value, you can use the AND instruction with a mask that has 1s in the positions of interest. Setting Flags and Conditions: In conditions where certain flags are represented as bits, AND can be used to check the status of these flags. Optimizations: In some scenarios, AND can be used for certain optimizations, like ensuring a number is even (by ANDing with a specific value).",
                "AND[bits] operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "OR", "OR16", "OR24", "OR32" },
                "Takes each bit in operand1 and operand2, performs the logical OR operation on each pair of bits, and stores the result back in operand1. The logical OR operation follows the rule: if either bit in a pair is 1, the result is 1; if both bits are 0, the result is 0.",
                "Setting Specific Bits: By using an appropriate mask (where you want to set bits, set mask bits to 1), you can set specific bits in a register or memory location. Combining Bit Patterns: OR is used to combine multiple bit patterns, especially in scenarios where each pattern represents different sets of flags or conditions. Creating Bit Masks: It is useful for creating or modifying bit masks, which are used in various operations like bit manipulation, conditional logic, and more. Condition and Flag Manipulation: In situations where certain conditions are represented as bits, OR can be used to set or modify these conditions.",
                "OR[bits] operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "XOR", "XOR16", "XOR24", "XOR32" },
                "Takes each bit in operand1 and operand2, performs the logical XOR operation on each pair of bits, and stores the result back in operand1. The logical XOR operation follows the rule: the result is 1 if the bits are different (i.e., one is 1 and the other is 0), and 0 if the bits are the same.",
                "Toggling Bits: XOR is commonly used to toggle specific bits in a register or memory location. By XORing with a mask of 1s in the desired positions, the specified bits are toggled. Zeroing a Register: A register can be set to zero by XORing it with itself, a technique often used for register initialization due to its efficiency. Creating Checksums: In some simple checksum algorithms, XOR is used to combine data bytes in a way that can detect certain types of errors. Implementing Simple Encryption: XOR is a basic operation in some cryptographic algorithms, especially in stream ciphers, where data is encrypted by XORing with a key stream. Comparing Values: XORing two values and checking the result can be used to determine if the values are equal (result is zero) or different (result is non-zero).",
                "XOR[bits] operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "INV" },
                "Inverts each bit of the provided operand. If a bit in the operand is 0, it becomes 1, and if it is 1, it becomes 0. This operation is also known as bitwise complement.",
                "Creating Complement: It is often used to create the complement of binary values, which is useful in certain arithmetic operations and algorithms. Boolean Logic Implementations: In Boolean algebra, INV is used to implement the NOT operation, essential for constructing complex logical expressions. Data Masking: INV can be used in conjunction with other bitwise operations like AND, OR, and XOR for advanced data masking and manipulation. Condition Testing: Sometimes used to invert condition bits for specific testing or control flow scenarios in programming.",
                "INV operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "EX" },
                "Exchanges the contents of register1 and register2. After the execution of this instruction, register1 contains the value that was initially in register2, and vice versa. Works with simple and floating-point registers.",
                "Data Rearrangement: EX is used for rearranging data in registers, especially in sorting algorithms or when shuffling data is required. Register Saving: During function calls or interrupt handling, EX can be used to save and restore register values without needing additional memory. Optimizing Data Movement: It can optimize data movement by eliminating the need for a temporary storage or an extra register to perform swaps. Implementing Algorithms: Useful in the implementation of certain algorithms that require swapping elements, like in cryptographic algorithms or in certain sorting routines.",
                "EX operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "CP" },
                "Compares the values of operand1 and operand2 by effectively subtracting operand2 from operand1. However, it does not store the result of this subtraction; instead, it sets or clears flags based on the outcome.",
                "Conditional Branching: CP is extensively used for conditional branching in programs. Based on the comparison, subsequent instructions may branch to different parts of the code. Loop Control: In loop constructs, CP is used to compare counter values against limits to determine if the loop should continue or exit. Value Checking: It's used to check if a register or memory location holds a specific value, which is useful in various algorithms and routines. Sorting and Searching Algorithms: CP is integral in implementing sorting and searching algorithms, where elements are constantly compared. Input Validation: In input processing routines, CP is used to validate if the input falls within an expected range.",
                "CP operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "INC", "INC16", "INC24", "INC32" },
                "Increments the value of the specified operand by one. It adds 1 to the current value of operand, and stores the result back in operand. No flags are affected.",
                "Loop Control: INC is commonly used in loop constructs to increment loop counters. Memory Address Manipulation: In pointer arithmetic, it's used to increment memory addresses, useful in traversing arrays or data structures. Simple Arithmetic Operations: INC is part of basic arithmetic operations and can be used in various calculations where incremental steps are required. Counters and Timers: In systems programming, INC can be used to implement counters and timers. Indexing and Data Traversal: It's used for indexing in loops for traversing arrays, lists, or other data structures.",
                "INC[bits] operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "DEC", "DEC16", "DEC24", "DEC32" },
                "Decrements the value of the specified operand by one. It subtracts 1 from the current value of operand, and stores the result back in operand. No flags are affected.",
                "Loop Control: DEC is commonly used in loop constructs to decrement loop counters. Memory Address Manipulation: In pointer arithmetic, it's used to decrement memory addresses, useful in traversing arrays or data structures in reverse. Simple Arithmetic Operations: DEC is part of basic arithmetic operations and can be used in various calculations where decremental steps are required. Counters and Timers: In systems programming, DEC can be used to implement counters and timers. Indexing and Data Traversal: It's used for indexing in loops for traversing arrays, lists, or other data structures in a backward sequence.",
                "DEC[bits] operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "CALL" },
                "Transfers program execution to the subroutine located at the target absolute address specified by operand. It also saves the address of the next instruction (the return address) onto the stack. After the subroutine finishes execution, control returns to the instruction following the CALL, using a RET (Return) instruction within the subroutine. If a conditional flag is present as first operand, it evaluates the logical result of it and only performs the CALL if the condition is true. No flags are affected.",
                "Modular Programming: CALL is used to invoke subroutines, making programming more modular and code more reusable. Implementing Functions and Procedures: In higher-level languages, function calls are typically implemented using CALL in the compiled assembly code. Recursion: CALL can be used for recursive function calls, where a function calls itself, with each call being remembered in the stack. Structured Programming: It aids in structured programming by allowing the division of code into smaller, manageable blocks.",
                "CALL [flag condition,] operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "CALLR" },
                "Transfers program execution to the subroutine located at the target relative address specified by operand as a negative or positive offset from the current instruction position. As opposed to CALL, CALLR can only reach up to 8Mb forward or backward from current position. It also saves the address of the next instruction (the return address) onto the stack. After the subroutine finishes execution, control returns to the instruction following the CALLR, using a RET (Return) instruction within the subroutine.  If a conditional flag is present as first operand, it evaluates the logical result of it and only performs the CALLR if the condition is true. No flags are affected.",
                "Relocating code: CALLR allows the compiled code to be relocable within memory. Modular Programming: CALLR is used to invoke subroutines, making programming more modular and code more reusable. Implementing Functions and Procedures: In higher-level languages, function calls are typically implemented using CALLR in the compiled assembly code. Recursion: CALL can be used for recursive function calls, where a function calls itself, with each call being remembered in the stack. Structured Programming: It aids in structured programming by allowing the division of code into smaller, manageable blocks.",
                "CALLR [flag condition,] operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "JP" },
                "Performs a jump to the target absolute address described by operand. No flags are affected.",
                "Conditional Execution: JP is used for conditional branching in a program, allowing different code paths to be executed based on the positivity of a prior calculation. Error Handling: In some scenarios, JP can be used for error handling, jumping to error-handling routines based on the result of a previous operation. Loop Control: It can be part of loop constructs, especially in scenarios where the continuation or termination of a loop depends on whether the result of an operation is positive. State Machine Implementation: JP can be used in implementing state machines, where transitions between states are conditional on positive results. Algorithm Flow Control: In algorithms, JP can control the flow based on the results of comparisons, arithmetic operations, or specific state checks.",
                "JP [flag condition,] operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "JR" },
                "Performs a jump to the target relative address described by operand as a negative or positive offset from the current instruction position. As opposed to JP, JR can only reach up to 8Mb forward or backward from current position. No flags are affected.",
                "Relocating code: JR allows the compiled code to be relocable within memory. Conditional Execution: JR is used for conditional branching in a program, allowing different code paths to be executed based on the positivity of a prior calculation. Error Handling: In some scenarios, JR can be used for error handling, jumping to error-handling routines based on the result of a previous operation. Loop Control: It can be part of loop constructs, especially in scenarios where the continuation or termination of a loop depends on whether the result of an operation is positive. State Machine Implementation: JR can be used in implementing state machines, where transitions between states are conditional on positive results. Algorithm Flow Control: In algorithms, JR can control the flow based on the results of comparisons, arithmetic operations, or specific state checks.",
                "JR [flag condition,] operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "POP" },
                "Removes the topmost value from the stack and stores it in the destination operand1 which can only be a simple register. It first reads the value at the current stack pointer, then increments the stack pointer to remove this value from the stack. If a second operand2 register is specified, it retrieves the full range of registers between operand1 and operand2 in their proper places, the same way they have been pushed. Can work with simple and floating-point registers. When using on ranges, POP and PUSH are written the same (i.e. PUSH A, D then POP A, D and the registers are correctly assigned when popping). If this instruction operates on a range of registers, they can wrap around the margins, so POP Y, D would POP D, C, B, A, Z and Y respectively, in this order. No flags are affected.",
                "Function Return Values: POP is used to retrieve return saved registers at the end of a function call. Restoring Register States: It's often used to restore the previous states of registers saved at the beginning of a function. Handling Interrupts: In interrupt service routines, POP is used to restore registers and other state information that was saved when the interrupt occurred. Implementing Data Structures: POP can be part of implementing stack-based data structures in assembly language.",
                "POP operand1 [, operand2]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "PUSH" },
                "Increments the stack pointer and then stores the value of the source operand1 at the top of the stack. This operation effectively adds a value to the stack, with the stack pointer pointing to the new top of the stack. If a second operand2 register is specified, it pushes the full range of registers between operand1 and operand2 in their proper places, the same way they will be later POPed. Can work with simple and floating-point registers.  When using on ranges, POP and PUSH are written the same (i.e. PUSH A, D then POP A, D and the registers are correctly assigned when popping). If this instruction operates on a range of registers, they can wrap around the margins, so PUSH Y, D would PUSH Y, Z, A, B, C and D respectively, in this order. No flags are affected.",
                "Function Call Preparation: PUSH can be used to pass arguments to functions by pushing them onto the stack before a CALL instruction. Saving Register States: It's commonly used to save the state of registers at the beginning of a function so they can be restored later. Handling Interrupts: In interrupt service routines, PUSH is used to save registers and other state information that must be restored after the interrupt is serviced. Implementing Data Structures: PUSH can be part of implementing stack-based data structures in assembly language. Temporary Storage: Used for temporarily storing values that need to be retrieved later, facilitating various algorithmic and computational processes.",
                "PUSH operand1 [, operand2]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "INT" },
                "Triggers a synchronous software interrupt specified by operand1. The function to be executed within this interrupt is selected based on the value in the 8 bit register operand2. After the interrupt service routine is executed, if there are return values, the register operand2 and possibly consecutive registers are used to store return values. The return values can be a multitude of 8-bit, 16-bit, 24-bit and 32-bit simple registers. Each documented interrupt specifies the return structure. No flags are affected.",
                "System Service Invocation: Used to invoke various system services or operations that are handled by software interrupts. Device Drivers and I/O Operations: Facilitates communication with device drivers and hardware, especially for input/output operations. Modular Function Calls: Allows for a modular approach to calling system functions, where different functions can be accessed through the same interrupt number with different register values.",
                "INT operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "RAND" },
                "Generates a random number and stores it in the specified operand1 register. The size of the random number is determined by the bit-size of the register used. The instruction can optionally take an immediate numerical value as a second operand2, which specifies the maximum value of the generated random number starting from zero. No flags are affected.",
                "Generating Random Data: Used in algorithms requiring randomization, such as in simulations, gaming, or security applications. Testing and Debugging: Useful in testing scenarios where random input data is required. Cryptographic Operations: In cryptographic algorithms, random number generation is often a key component.",
                "RAND operand1 [, operand2]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "MIN" },
                "Compares the values in operand1 and operand2, and stores the minimum of the two values in operand1. This instruction is capable of handling comparisons between different types of operands, such as between registers (simple or floating point), and between a register and an immediate value. No flags are affected.",
                "Arithmetic Computations: Used in calculations where determining the minimum value is required, such as in statistical analysis, data processing, or in algorithms. Comparative Logic: Useful in decision-making processes within a program, particularly in sorting algorithms or conditional branching. Optimization Problems: Employed in solving optimization problems where the least value is sought after. Cross-Register Operations: The ability to compare across different types of registers (simple and floating point) broadens its application in diverse computational scenarios. Data Type Flexibility: The functionality to compare registers with immediate numerical values adds versatility to the instruction.",
                "MIN operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "MAX" },
                "Compares the values in operand1 and operand2, and stores the maximum of the two values in operand1. This instruction supports comparisons between different types of operands, such as between registers (simple or floating point) and between a register and an immediate value. No flags are affected.",
                "Arithmetic Computations: Utilized in calculations where determining the maximum value is essential, such as in statistical analysis, data processing, or in algorithms. Comparative Logic: Useful in decision-making processes within a program, especially in sorting algorithms or conditional branching. Optimization Problems: Employed in solving optimization problems where the greatest value is sought. Cross-Register Operations: The ability to compare across different types of registers (simple and floating point) enhances its application in diverse computational scenarios. Data Type Flexibility: The functionality to compare registers with immediate numerical values adds versatility to the instruction.",
                "MAX operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "SETF" },
                "Sets the flag specified by operand in the processor's status register to 1. This operation is used to manually control the state of various flags that typically represent the outcome of arithmetic or logical operations. Respective flag is affected.",
                "Conditional Logic Control: SETF can be used to manually set conditions for subsequent conditional branches or operations. Debugging and Testing: Useful in debugging scenarios where setting a flag to a known state can help test specific code paths or error handling. Simulating Outcomes: It can be used to simulate the outcomes of operations for testing or instructional purposes. Custom Control Flows: In cases where standard arithmetic or logical operations do not yield the desired flags, SETF allows for custom setting of these flags to influence program flow.",
                "SETF operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "RESF" },
                "Resets the flag specified by operand in the processor's status register to 0. This operation is used to manually control the state of various flags that typically represent the outcome of arithmetic or logical operations. Respective flag is affected.",
                "Conditional Logic Control: RESF can be used to manually reset conditions for subsequent conditional branches or operations. Debugging and Testing: Useful in debugging scenarios where resetting a flag to a known state can help in testing specific code paths or error handling. Simulating Outcomes: It can be used to simulate the outcomes of operations for testing or instructional purposes. Custom Control Flows: In cases where standard arithmetic or logical operations do not yield the desired flags, RESF allows for custom resetting of these flags to influence program flow.",
                "RESF operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "INVF" },
                "Inverts the flag specified by operand in the processor's status register. If the flag is set (1), it will be reset (0), and if it is reset (0), it will be set (1). This operation is used to toggle the state of various flags that typically represent the outcome of arithmetic or logical operations. Respective flag is affected.",
                "Conditional Logic Control: INVF can be used to toggle conditions for subsequent conditional branches or operations. Debugging and Testing: Useful in debugging scenarios where inverting a flag's state can help in testing specific code paths or error handling. Simulating Outcomes: It can be used to simulate the outcomes of operations for testing or instructional purposes by toggling flag states. Custom Control Flows: In cases where standard arithmetic or logical operations do not yield the desired flags, INVF allows for custom toggling of these flags to influence program flow.",
                "INVF operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "LDF" },
                "Load the processor's status flags into a register or memory location provided by operand1. It supports various operand types, including 8-bit registers, 24-bit registers for addressing memory locations, and immediate numerical values. The instruction can also apply an AND mask to the flags before loading them. No flags are affected.",
                "Flag State Examination: For inspecting the current state of the processor's status flags. Conditional Logic and Decision Making: Assists in complex decision-making processes based on flag states. Debugging and Status Monitoring: Useful in debugging and monitoring the status of the processor.",
                "LDF operand1 [, operand2]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "REGS" },
                "Changes the current active set of registers (register bank) to another, based on the provided operand index (0 - 255). This enables the use of multiple, isolated sets of registers in the same program, enhancing the capability for complex data manipulation and task switching. No flags are affected.",
                "Task Switching: Useful for saving and restoring register states when switching between different tasks or contexts in a program. Complex Computations: Facilitates complex computations that require multiple sets of registers for intermediate data storage. Memory Management: Assists in memory management scenarios where different register banks can be dedicated to specific memory manipulation tasks. Isolation of Operations: Allows for the isolation of operations, where each set of registers can be used independently without affecting others.",
                "REGS operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "WAIT" },
                "Suspends the program execution for a duration specified by the operand immediate value. The duration is given in milliseconds, and is a 24-bit immediate numerical value representing this time period. No flags are affected.",
                "Timing Control: Useful in scenarios where precise timing control is required, such as in embedded systems, hardware interfacing, or time-sensitive calculations. Delay Introductions: Assists in creating delays for synchronization purposes or to wait for external events or conditions to occur. Throttling: Helps in throttling the execution speed of a program, particularly in loops or repetitive tasks. Testing and Debugging: In debugging, WAIT can be used to introduce pauses at specific points to observe the behavior of a program over time. One example is introducing a WAIT instruction at the beginning of the OS until the network connection is established with Continuum Tools so debug can initialize.",
                "WAIT operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "VDL" },
                "Used in manual video buffer mode to specify which layer(s) of the video data should be refreshed. The layers to be refreshed are indicated by the bits in the operand, where each bit corresponds to a specific layer. The instruction copies the data for the specified layer(s) from video RAM to the video buffers, which will then be blended and drawn on screen. No flags are affected.",
                "Video Rendering: Facilitates selective updating of video layers in graphics and video rendering applications. Performance Optimization: Helps in optimizing rendering performance by refreshing only the necessary layers. Dynamic Content Management: Useful in scenarios where different layers contain dynamically changing content. Complex Graphics Manipulation: Assists in complex graphics operations where multiple layers are independently manipulated and combined.",
                "VDL operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "VCL" },
                "Used in manual video buffer mode to clear specified video layer(s). The layers to be cleared are indicated by the bits in the operand, where each bit corresponds to a specific layer. This instruction effectively clears or resets the data for the indicated layer(s) in the drawing buffer. No flags are affected.",
                "Video Layer Management: Facilitates the resetting or clearing of specific video layers in graphics and video rendering applications. Content Refreshing: Assists in preparing layers for new content by clearing old data.",
                "VCL operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "FIND" },
                "Searches for a specific byte value in memory, starting from an address provided by operand1. The value to search for can be specified either as an immediate value or as a value located at another memory address through operand2. When the specified byte is found, the instruction returns the address of this byte in the register used for the starting address. No flags are affected.",
                "Data Searching: Useful for locating specific data within a larger set, such as finding a character in a string or a particular value in an array. Memory Analysis: Assists in analyzing memory contents, finding occurrences of specific bytes. Buffer Processing: Can be used in buffer processing algorithms where searching for specific markers or delimiters is required. Pattern Matching: Facilitates simple pattern matching operations in memory.",
                "FIND operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "IMPLY", "IMPLY16", "IMPLY24", "IMPLY32" },
                "Performs a logical implication operation between operand1 and operand2 storing the result in operand1. The implication follows the truth table: A IMPLY B results in 1 except when A is 1 and B is 0. The operation is applied to various sizes of operands, including 8-bit, 16-bit, 24-bit, and 32-bit, and can involve immediate values, register contents, or memory addressed values. No flags are affected.",
                "Logical Computations: Useful in scenarios requiring complex logical operations beyond basic AND, OR, XOR functions. Conditional Processing: Assists in conditional logic implementations, especially in control flow and decision-making algorithms. Data Analysis: Can be employed in data analysis tasks where logical implication is a part of the processing logic. Algorithm Implementation: Useful in specialized algorithms that require logical implication as a fundamental operation.",
                "IMPLY[bits] operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "NAND", "NAND16", "NAND24", "NAND32" },
                "Performs a bitwise NAND operation between operand1 and operand2 storing the result in operand1. It first applies the AND operation between corresponding bits of the two operands and then inverts the result. In effect, the output of NAND is the logical negation of the AND operation. No flags are affected.",
                "Logical Computations: Useful for performing complex logical operations where NAND logic is required. Circuit Simulation and Design: Assists in simulating digital circuits and logic gates in software. Control Flow: Can be used in control flow and decision-making algorithms that depend on specific logical conditions. Data Manipulation: Employed in various data manipulation tasks where bitwise NAND operation is a part of the processing logic.",
                "NAND[bits] operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "NOR", "NOR16", "NOR24", "NOR32" },
                "Performs a bitwise NOR operation between operand1 and operand2 storing the result in operand1. It first conducts the OR operation between corresponding bits of the two operands and then inverts the result. Essentially, the output of NOR is the logical negation of the OR operation. No flags are affected.",
                "Logical Computations: Useful for performing complex logical operations where NOR logic is required. Circuit Simulation and Design: Assists in simulating digital circuits and logic gates, particularly where NOR gates are involved. Control Flow: Can be employed in control flow and decision-making algorithms that rely on specific logical conditions. Data Manipulation: Used in various data manipulation tasks where bitwise NOR operation forms part of the processing logic.",
                "NOR[bits] operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "XNOR", "XNOR16", "XNOR24", "XNOR32" },
                "Performs a bitwise XNOR operation between operand1 and operand2 storing the result in operand1. It first executes the XOR operation between corresponding bits of the two operands and then inverts the result. The XNOR operation essentially yields the logical negation of the XOR operation. No flags are affected.",
                "Logical Computations: Useful for performing complex logical operations where XNOR logic is required. Circuit Simulation and Design: Assists in simulating digital circuits and logic gates, especially where XNOR gates are involved. Control Flow: Can be employed in control flow and decision-making algorithms that depend on specific logical conditions. Data Manipulation: Used in various data manipulation tasks where bitwise XNOR operation is part of the processing logic.",
                "XNOR[bits] operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "GETBITS" },
                "Extracts a specified number of bits from memory and stores the resulting value into a destination register. The source bit address is specified by a 32-bit register, and the number of bits to extract is provided by the third operand.",
                "Data Manipulation: Essential for operations that require extraction of specific bitfields from memory. Commonly used in parsing, data compression, and protocol handling where bit-level operations are necessary. Efficient for extracting packed data structures or memory-mapped register fields.",
                "GETBITS destination, source_bit_address, num_bits"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "SETBITS" },
                "Sets a specified number of bits in memory using a value from a source register. The bit address in memory is specified by a 32-bit register, and the number of bits to set is provided by the third operand.",
                "Data Manipulation: Enables precise modification of bitfields in memory. Useful in tasks such as writing to memory-mapped device registers, modifying protocol headers, or updating packed data structures. Ensures efficient memory usage when operating on individual bits or bit ranges.",
                "SETBITS target_bit_address, source, num_bits"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "MEMF" },
                "Fills a block of memory with a specific value. The target memory address (operand1), the number of bytes to fill (operand2), and the fill value (operand3) can be specified through various combinations of 24-bit registers, 24-bit immediate values, and 8-bit registers or immediate values. No flags are affected.",
                "Memory Initialization: Useful for initializing or resetting a large area of memory to a specific value. Buffer Management: Assists in preparing buffers by filling them with a default or specified value. Data Overwriting: Can be used to overwrite sensitive data in memory for security purposes. Graphics and Rendering: In graphics programming, MEMF can be used to set pixel values in a frame buffer or texture.",
                "MEMF operand1, operand2, operand3"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "MEMC" },
                "Copies a specified number of bytes described by operand3 from one memory address described by operand1 to another described by operand2. The source and destination addresses, as well as the number of bytes to copy, can be specified through various combinations of 24-bit registers and 24-bit immediate values. No flags are affected.",
                "Data Movement: Useful for moving data between different areas of memory. Buffer Management: Assists in managing buffers, such as copying data to and from I/O buffers. Memory Initialization: Can be used for initializing memory regions with data from another location. Data Duplication: Facilitates duplicating blocks of data within memory for processing or manipulation.",
                "MEMC operand1, operand2, operand3"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "LDREGS" },
                "Loads registers range specified by operand1 and operand2 (can be in reverse) from a memory address specified by operand3. No flags are affected.",
                "Initializing Registers for Interrupts: Before executing certain interrupts, multiple registers need to be loaded sequencially with initial values. LDREGS can initialize these registers in one instruction, reducing setup time. Loop Unrolling Optimization: For loops that process arrays or data structures, LDREGS can be used to unroll loops by loading multiple iterations' worth of data into registers at once, reducing loop overhead and increasing execution speed..",
                "LDREGS operand1, operand2, operand3"
            ));
            MetaData.Add(ASMMeta.Create(
                new List<string>() { "STREGS" },
                "Stores registers range specified by operand2 and operand3 (can be in reverse) to a memory address specified by operand1. No flags are affected.",
                "Saving State: Before interrupt handling or when switching contexts in multitasking software, STREGS can quickly save the current state of multiple registers to a predefined memory area. Data Serialization: For operations that prepare data for transmission or storage, STREGS allows for efficient serialization of multiple data points held in registers into a contiguous block of memory.",
                "LDREGS operand1, operand2, operand3"
            ));
            MetaData.Add(ASMMeta.Create(
            new List<string>() { "SDIV" },
                "Divides a source operand1 (register or immediate value) by a destination operand2 (register or immediate value) as signed numbers. The result of the division is stored in the operand1 (destination). If the optional operand3 is specified it serves to hold the remainder of the division. No flags affected.",
                "Arithmetic Calculations: Useful for performing division operations in algorithms that require signed arithmetic. Data Processing: Employed in data processing tasks where signed division is a part of the logic. Algorithm Implementation: Assists in implementing algorithms that require signed division, such as in scientific computations or financial calculations.",
                "SDIV operand1, operand2 [, operand3]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "SMUL" },
                "Multiplies two signed numbers and stores the result in the first operand1. The operands can be specified as registers of various sizes (8-bit to 32-bit) or as immediate numerical values. The multiplication is signed, meaning it considers the sign (positive or negative) of the operands. No flags are affected.",
                "Arithmetic Calculations: Useful for performing signed multiplication operations in algorithms that require signed arithmetic. Data Processing: Employed in data processing tasks where signed multiplication is part of the logic. Algorithm Implementation: Assists in implementing algorithms that require signed multiplication, such as in scientific computations or financial calculations.",
                "SMUL operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "SCP" },
                "Compares two values as signed numbers and sets the processor's status flags based on the result. The comparison is effectively a subtraction of the second operand from the first, and the flags are set accordingly, although the result of the subtraction is not stored. No flags are affected.",
                "Conditional Branching: Used to set up conditions for subsequent conditional branches based on signed comparisons. Data Analysis: Useful in data analysis and processing where signed comparisons are required. Algorithm Implementation: Assists in implementing algorithms that rely on signed comparisons, such as sorting algorithms. Memory Comparisons: Facilitates signed comparisons between values stored in memory.",
                "SCP operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "POW" },
                "Performs an exponentiation operation where the operand1 (base) is raised to the power of the operand2 (exponent). The operation handles various operand types, including floating-point registers, integer registers of different sizes, and memory addresses. The result is stored back in operand1. No flags are affected.",
                "Scientific Computations: Useful for calculations in scientific applications where exponentiation is required. Financial Modeling: Employed in financial algorithms involving compound interest calculations or growth rate computations. Data Processing: Assists in data processing tasks where exponential functions are part of the logic. Graphics and Rendering: In graphics programming, POW can be used for shading effects, lighting calculations, and other graphical transformations.",
                "POW operand1, operand2"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "SQR" },
                "Computes the square root of the specified operand. The operand can be a floating-point or integer register, or a memory location. The result of the square root calculation is stored back in the first operand. If a second operand is present square root is performed on operand2 and the result placed in operand1. This way the source is preserved. No flags are affected.",
                "Mathematical Computations: Essential in scientific and engineering applications where square root calculations are required. Data Processing: Useful in algorithms and data processing tasks involving square root operations. Graphics and Physics Simulations: Employed in graphical computations and physics simulations where square root functions are part of the logic.",
                "SQR operand1 [, operand2]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "CBR" },
                "Computes the cube root of the specified operand. The operand can be a floating-point or integer register, or a memory location. The result of the cube root calculation is stored back in the first operand. If a second operand is present, cube root is performed on operand2 and the result placed in operand1. This way the source is preserved. No flags are affected.",
                "Scientific and Engineering Calculations: Essential in applications where cube root operations are required, such as in physics simulations. Data Processing: Useful in algorithms and data processing tasks involving cube root functions. Graphics Programming: Used in graphical computations, particularly in generating three-dimensional effects and transformations.",
                "CBR operand1 [, operand2]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "ISQR" },
                "Computes the inverse square root of the specified operand. The operand can be a floating-point or integer register, or a memory location. The result of the inverse square root calculation is stored back in the first operand. If a second operand is present, inverse square root is performed on operand2 and the result placed in operand1. This way the source is preserved. No flags are affected.",
                "Scientific and Engineering Calculations: Essential in applications where inverse square root operations are required, such as in physics simulations. Graphics Programming: Used in graphical computations, particularly in lighting and shading calculations. Data Processing: Useful in algorithms and data processing tasks involving inverse square root functions.",
                "ISQR operand1 [, operand2]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "ISGN" },
                "Inverts the sign of a floating-point value. If the operand is positive, it becomes negative, and if it is negative, it becomes positive. The instruction can operate on a single floating-point register (inverting its own value) or on two floating-point registers (inverting the value of the second register and storing the result in the first). No flags are affected.",
                "Mathematical Computations: Essential in calculations requiring the inversion of sign, such as solving equations or implementing certain mathematical functions. Data Processing: Useful in data transformation tasks where inverting the sign of a value is part of the processing logic. Graphics and Physics Simulations: In graphics and physics engines, sign inversion can be a crucial part of various algorithms.",
                "ISGN operand1 [, operand2]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "SIN" },
                "Calculates the sine of a floating-point value. The operation can be performed on a single floating-point register (calculating the sine of its own value) or on two floating-point registers (calculating the sine of the second register's value and storing the result in the first register). No flags are affected.",
                "Scientific Computations: Essential in scientific applications where trigonometric functions, such as sine, are required. Graphics and Animation: Used in graphics programming and animation for creating various effects and transformations. Signal Processing: In signal processing, sine functions are often used for analyzing and manipulating signals.",
                "SIN operand1 [, operand2]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "COS" },
                "Calculates the cosine of a floating-point value. It can be executed on a single floating-point register, where it calculates the cosine of the register's value and overwrites it with the result. Alternatively, it can operate on two floating-point registers, calculating the cosine of the value in the second register and storing the result in the first register. No flags are affected.",
                "Scientific Computations: Vital in scientific applications that require trigonometric functions, such as calculating the cosine. Graphics and Animation: Used in graphics programming for creating various visual effects and transformations. Signal Processing: In signal processing, cosine functions are crucial for analyzing and manipulating waveforms.",
                "COS operand1 [, operand2]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "TAN" },
                "Calculates the tangent of a floating-point value. The operation can be performed on a single floating-point register (calculating the tangent of its own value) or on two floating-point registers (calculating the tangent of the second register's value and storing the result in the first register). No flags are affected.",
                "Scientific Computations: Essential for scientific applications that require trigonometric calculations, particularly the tangent function. Graphics and Animation: Used in graphics programming and animation for calculating angles and creating effects. Engineering Applications: In engineering, tangent functions are often used in various calculations, including structural design and analysis.",
                "TAN operand1 [, operand2]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "ABS" },
                "Calculates the absolute value of a floating-point number. It can be executed on a single floating-point register, where it calculates the absolute value of the register's value and overwrites it with the result. Alternatively, it can operate on two floating-point registers, calculating the absolute value of the value in the second register and storing the result in the first register. No flags are affected.",
                "Mathematical Computations: Vital for calculations that require the non-negative magnitude of a value, such as in statistical analysis and numerical algorithms. Data Processing: Useful in data transformation tasks where obtaining the absolute value is part of the processing logic. Signal Processing: In signal processing, the absolute value is often used for analyzing the amplitude of signals.",
                "ABS operand1 [, operand2]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "ROUND" },
                "Rounds the value of a floating-point number to the nearest integer. It can operate on a single floating-point register, rounding its own value and overwriting it with the result. Alternatively, it can be used with two floating-point registers, rounding the value in the second register and storing the result in the first register. No flags are affected.",
                "Mathematical Computations: Essential in calculations that require rounding to the nearest integer, such as in statistical analysis, financial calculations, and numerical algorithms. Data Processing: Useful in data processing tasks where rounding of floating-point values is part of the logic. Graphics and Animation: Employed in graphics programming for pixel alignment and various graphical effects requiring rounded values.",
                "ROUND operand1 [, operand2]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "FLOOR" },
                "Rounds down the value of a floating-point number to the nearest integer that is less than or equal to the original value. It can be executed on a single floating-point register, where it rounds its own value and overwrites it with the result. Alternatively, it can be applied to two floating-point registers, rounding down the value in the second register and storing the result in the first register. No flags are affected.",
                "Mathematical Computations: Crucial for calculations that require rounding down to the nearest integer, such as in statistical analysis, financial calculations, and numerical algorithms. Data Processing: Useful in data processing tasks where flooring of floating-point values is necessary. Graphics and Animation: Used in graphics programming for pixel alignment and various graphical effects that require floor operations.",
                "FLOOR operand1 [, operand2]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "CEIL" },
                "rounds up the value of a floating-point number to the nearest integer that is greater than or equal to the original value. It can operate on a single floating-point register, where it rounds its own value and overwrites it with the result. Alternatively, it can be used with two floating-point registers, rounding up the value in the second register and storing the result in the first register. No flags are affected.",
                "Mathematical Computations: Essential for calculations that require rounding up to the nearest integer, such as in statistical analysis, financial calculations, and numerical algorithms. Data Processing: Useful in data processing tasks where ceiling of floating-point values is part of the logic. Graphics and Animation: Employed in graphics programming for pixel alignment and various graphical effects that require ceiling operations.",
                "CEIL operand1 [, operand2]"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "PLAY" },
                "Sends a complex set of parameters to the sound processor to produce a sound signal. The operand of PLAY holds an address to the parameters that will compose the sound.",
                "Sound effects, music.",
                "PLAY operand"
            ));

            // Color
            MetaData.Add(ASMMeta.Create(
                new List<string>() { "RGB2HSL" },
                "Converts a 24-bit RGB color value to the HSL color model. Expects the input RGB value as a 24-bit register, immediate value, or memory reference, formatted as 1 byte per color channel (Red, Green, Blue). The output is a 4-byte value where the first 2 bytes represent the Hue (16-bit, range 0-360), the third byte represents Saturation (0-100), and the fourth byte represents Lightness (0-100). This allows for precise color manipulation and transformation between color spaces.",
                "Color Conversion: RGB2HSL is used in graphics and color manipulation applications requiring a conversion from the RGB to the HSL color space. Color Adjustment: Facilitates color adjustments by converting to HSL, where changes in hue, saturation, and lightness are intuitive. Advanced Graphics Processing: Enables smooth transitions and transformations in color animations or visual effects.",
                "RGB2HSL operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "HSL2RGB" },
                "Converts an HSL color value to the RGB color model. Expects the input HSL values as 4 bytes: a 16-bit Hue (range 0-360), an 8-bit Saturation (0-100), and an 8-bit Lightness (0-100). Outputs a 24-bit RGB color, formatted as 1 byte per color channel (Red, Green, Blue), stored in a 24-bit register or memory location. This instruction allows for easy transition back to RGB after color manipulation in the HSL space.",
                "Color Conversion: HSL2RGB is used in graphics and color manipulation applications requiring conversion from HSL to RGB color space. Data Compatibility: Enables efficient storage and rendering of colors in RGB for systems that display colors in this format. Enhanced Color Effects: Supports advanced color effects by returning from HSL to RGB for rendering purposes.",
                "HSL2RGB operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "RGB2HSB" },
                "Converts a 24-bit RGB color value to the HSB color model. The input RGB value is accepted as a 24-bit register, immediate value, or memory reference, formatted as 1 byte per channel (Red, Green, Blue). Outputs a 4-byte value where the first 2 bytes represent Hue (16-bit, range 0-360), the third byte represents Saturation (0-100), and the fourth byte represents Brightness (0-100). This provides precise color manipulation for applications requiring HSB-based transformations.",
                "Color Conversion: RGB2HSB is used in graphics and color manipulation applications requiring conversion from RGB to the HSB color space. Brightness Adjustments: Facilitates intuitive brightness and saturation adjustments, which are particularly useful in digital artwork and visualization. Visual Effects Processing: Enables advanced visual effects, where adjustments in hue, saturation, and brightness are needed for dynamic scenes.",
                "RGB2HSB operand"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "HSB2RGB" },
                "Converts an HSB color value to the RGB color model. Expects HSB input as 4 bytes: a 16-bit Hue (range 0-360), an 8-bit Saturation (0-100), and an 8-bit Brightness (0-100). Outputs a 24-bit RGB color formatted as 1 byte per color channel (Red, Green, Blue), storing the result in a 24-bit register or memory location. This allows transitioning back to RGB from HSB color manipulations.",
                "Color Conversion: HSB2RGB is used in graphics applications that require conversion from HSB back to RGB, enabling compatibility with RGB-based displays. Dynamic Brightness Effects: Supports applications where dynamic color adjustments based on brightness are essential, such as in gaming and animation. Simplified Color Management: Facilitates the transition from HSB, commonly used for color effects, back to RGB for final output or rendering.",
                "HSB2RGB operand"
            ));
            // End color

            MetaData.Add(ASMMeta.Create(
                ["SETVAR"],
                "Writes 32 bit values to a separate hardware memory bank with the capacity of 64k addresses (that means 256k bytes, however 4 bytes are read at one read). The top part of this memory is used for some machine settings such as the jump address in case of a stack overflow, however, most of the addresses are free to use for other purposes.",
                "Fine tune a few system variables such as error jump address or read last error. Use as an extra storage, if needed.",
                "SETVAR operand"
            ));

            MetaData.Add(ASMMeta.Create(
                ["GETVAR"],
                "Reads 32 bit values from a separate hardware memory bank with the capacity of 64k addresses (that means 256k bytes, however 4 bytes are read at one read). The top part of this memory is used for some machine settings such as the jump address in case of a stack overflow, however, most of the addresses are free to use for other purposes.",
                "Fine tune a few system variables such as error jump address or read last error. Use as an extra storage, if needed.",
                "GETVAR operand"
            ));



            MetaData.Add(ASMMeta.Create(
                new List<string>() { "DEBUG" },
                "Signals the CPU to enter a debug state. This state is intended to interact with external debugging tools, such as the Continuum Tools, which can capture and analyze the signal. The primary purpose of this instruction is to facilitate debugging of assembly programs. It does not modify any registers, flags, memory, or stack contents, ensuring that the program's state is preserved for accurate debugging. No flags are affected.",
                "Program Debugging: Useful for signaling specific points in the program to an external debugger for detailed analysis and troubleshooting. Development and Testing: Assists developers in testing and refining their code by providing checkpoints where the program's state can be examined. Error Diagnosis: Facilitates the identification and diagnosis of issues in the code by allowing external tools to access the program's state at specific execution points.",
                "DEBUG"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "BREAK" },
                "Stops the execution of the machine entirely. Once executed, it brings all processing activities to a halt. This action is unrecoverable through software means, and the machine remains in a halted state until it is reset manually or through external intervention.",
                "Emergency Shutdown: Used in situations where an immediate halt of the machine is necessary, typically in emergency scenarios or critical system failures. System Testing: Employed in testing the robustness and fault tolerance of a system by simulating critical failure scenarios. Debugging: Can be useful in debugging, especially to halt the system at a specific point where a critical error is detected.",
                "BREAK"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "RET" },
                "Used at the end of a subroutine or function to return control to the point in the program where the subroutine or function was called. It typically works by popping the return address from the stack and jumping to that address, effectively resuming execution at the point immediately following the initial call to the subroutine. No flags are affected.",
                "Subroutine Management: Essential for returning from subroutines or functions after their execution is complete. Control Flow: Facilitates structured and modular programming by allowing the execution to return to different parts of the program depending on where the subroutine was called from. Recursion and Iteration: Used in implementing recursive functions and iterative procedures in assembly language.",
                "RET"
            ));

            MetaData.Add(ASMMeta.Create(
                new List<string>() { "RETIF" },
                "Used to conditionally return control to the point in the program where the subroutine or function was called, only if the provided flag is set. It typically works by popping the return address from the stack and jumping to that address, effectively resuming execution at the point immediately following the initial call to the subroutine. No flags are affected.",
                "Conditional Program Flow: Create conditional branches in program flow. For example, if a certain condition is met during the execution of a subroutine, the program may exit early based on the state of the Zero flag. Dynamic Function Exits: This instruction enables dynamic exits from functions or procedures based on runtime conditions. For example, a function may terminate early if a specific criteria, checked by the Zero flag, is satisfied. Error Handling: In error-handling routines, RETIF Z can be employed to check for specific conditions (such as a zero result) and return from the subroutine if the condition is met. This facilitates efficient error recovery.",
                "RET"
            ));

            /*
            MetaData.Add(ASMMeta.Create(
                new List<string>() { "XXXX" },
                "",
                "",
                "XXXX operand1, operand2"
            ));
            */
        }

        public static void DeInitialize()
        {
            MetaData.Clear();
        }

    }
}
