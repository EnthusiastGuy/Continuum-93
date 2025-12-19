using System;
using System.Collections.Generic;
using System.Text;

namespace Continuum93.Emulator.Compilers.C93Basic
{
    /// <summary>
    /// Emits assembly code from AST.
    /// </summary>
    public class Emitter
    {
        private SymbolTable _symbolTable;
        private VariableManager _variableManager;
        private uint _codeStartAddress;
        private uint _currentAddress;
        private StringBuilder _assembly;
        private int _labelCounter;
        private List<CompileError> _errors;
        private Dictionary<string, uint> _labelAddresses;
        private List<(string label, uint address)> _forwardReferences;
        private Stack<string> _availableIntRegisters; // Pool of available integer registers for reuse
        private Stack<string> _availableFloatRegisters; // Pool of available float registers for reuse
        private Dictionary<string, string> _stringLiterals; // Map string literals to their labels
        private int _stringCounter; // Counter for string labels
        private StringBuilder _dataSection; // Data section for strings
        private bool _printIntEmitted; // Track if PrintInt subroutine has been emitted
        private string _printIntLabel = ".PrintInt"; // Label for PrintInt subroutine
        private string _stringFmtLabel; // Label for format string "{0}"
        private string _printIntBufferLabel = ".string_print_int_buffer"; // Label for PrintInt string buffer
        private uint _printIntBufferAddr; // Address for PrintInt string buffer
        private StringBuilder _variableSection; // Variable declarations section
        private Dictionary<string, bool> _stringHelpersEmitted; // Track which string helper subroutines have been emitted
        private int _stringHelperCounter; // Counter for string helper labels
        private Dictionary<string, string> _stringTempBuffers; // Map function calls to temporary buffer labels

        public List<CompileError> Errors => _errors;

        public Emitter(SymbolTable symbolTable, VariableManager variableManager)
        {
            _symbolTable = symbolTable;
            _variableManager = variableManager;
            _errors = new List<CompileError>();
            _labelAddresses = new Dictionary<string, uint>();
            _forwardReferences = new List<(string, uint)>();
            _tempRegisterCounter = 0;
            _availableIntRegisters = new Stack<string>();
            _availableFloatRegisters = new Stack<string>();
            _stringLiterals = new Dictionary<string, string>();
            _stringCounter = 0;
            _dataSection = new StringBuilder();
        }

        public void SetCodeStartAddress(uint address)
        {
            _codeStartAddress = address;
            _currentAddress = address;
        }

        public string Emit(ProgramNode program)
        {
            _assembly = new StringBuilder();
            _dataSection = new StringBuilder();
            _variableSection = new StringBuilder();
            _labelCounter = 0;
            _stringCounter = 0;
            _tempRegisterCounter = 0; // Reset register counter for each compilation
            _availableIntRegisters.Clear();
            _availableFloatRegisters.Clear();
            _stringLiterals.Clear();
            _errors.Clear();
            _labelAddresses.Clear();
            _forwardReferences.Clear();
            _stringHelpersEmitted = new Dictionary<string, bool>();
            _stringHelperCounter = 0;
            _stringTempBuffers = new Dictionary<string, string>();

            if (program == null || program.Statements == null || program.Statements.Count == 0)
            {
                _errors.Add(new CompileError(0, 0, "Program has no statements"));
                return $"#ORG {_codeStartAddress:X6}\n    RET  ; Empty program\n";
            }

            // First pass: collect labels and allocate variables
            CollectLabelsAndVariables(program);

            // Second pass: emit code
            _currentAddress = _codeStartAddress;
            _assembly.AppendLine($"#ORG {_codeStartAddress:X6}");

            foreach (var stmt in program.Statements)
            {
                if (stmt != null)
                {
                    EmitStatement(stmt);
                }
            }

            // Resolve forward references
            ResolveForwardReferences();

            // Set variable section
            _variableManager.SetCodeEndAddress(_currentAddress);

            // If no code was emitted, add at least a RET
            string result = _assembly.ToString();
            if (result.Length <= 30) // Just the #ORG line
            {
                _assembly.AppendLine("    RET  ; No code generated");
                result = _assembly.ToString();
            }

            // Append PrintInt subroutine if it was used
            if (_printIntEmitted)
            {
                result += "\n; ---------------------------------------------------------\n";
                result += "; SHARED SUBROUTINES\n";
                result += "; ---------------------------------------------------------\n";
                result += EmitPrintIntSubroutine();
            }
            
            // Append string helper subroutines if they were used
            if (_stringHelpersEmitted != null && _stringHelpersEmitted.Count > 0)
            {
                if (!_printIntEmitted)
                {
                    result += "\n; ---------------------------------------------------------\n";
                    result += "; SHARED SUBROUTINES\n";
                    result += "; ---------------------------------------------------------\n";
                }
                foreach (var helper in _stringHelpersEmitted.Keys)
                {
                    if (_stringHelpersEmitted[helper])
                    {
                        result += EmitStringHelper(helper);
                    }
                }
            }

            // Append data section with strings
            if (_dataSection.Length > 0 || _printIntEmitted)
            {
                result += "\n; ---------------------------------------------------------\n";
                result += "; DATA SECTION\n";
                result += "; ---------------------------------------------------------\n";
                result += _dataSection.ToString();
                
                // Add PrintInt buffer if needed
                if (_printIntEmitted)
                {
                    result += $"{_printIntBufferLabel}\n";
                    result += "    #DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0  ; Reserve 12 characters, enough to print the largest int number with minus in front.\n";
                }
            }
            
            // Append variable declarations section
            if (_variableSection.Length > 0)
            {
                result += "\n; ---------------------------------------------------------\n";
                result += "; VARIABLES\n";
                result += "; ---------------------------------------------------------\n";
                result += _variableSection.ToString();
            }

            return result;
        }

        private void CollectLabelsAndVariables(ProgramNode program)
        {
            uint address = _codeStartAddress;

            foreach (var stmt in program.Statements)
            {
                if (stmt is LabelNode label)
                {
                    _labelAddresses[label.Name] = address;
                    _symbolTable.AddLabel(label.Name, address);
                }
                else if (stmt is DimNode dim)
                {
                    // Allocate variable
                    List<int> dims = new List<int>();
                    foreach (var dimExpr in dim.Dimensions)
                    {
                        if (dimExpr is LiteralNode lit && lit.Value is int)
                        {
                            dims.Add((int)lit.Value);
                        }
                    }
                    string varLabel = _variableManager.GenerateVariableLabel(dim.VariableName, dim.Type);
                    var varInfo = new SymbolInfo
                    {
                        Name = dim.VariableName,
                        Type = dim.Type,
                        Label = varLabel,
                        ArrayDimensions = dims
                    };
                    _symbolTable.AddVariable(dim.VariableName, varInfo);
                    
                    // Declare array variable in variable section
                    DeclareVariable(varInfo, null);
                }
                else
                {
                    // Estimate statement size (rough estimate)
                    address += EstimateStatementSize(stmt);
                }
            }
        }

        private uint EstimateStatementSize(StatementNode stmt)
        {
            // Rough estimates - will be refined
            return 10; // Default estimate
        }

        private void EmitStatement(StatementNode stmt)
        {
            if (stmt == null) return;

            switch (stmt)
            {
                case LabelNode label:
                    _assembly.AppendLine($".{label.Name}");  // Labels don't have colons
                    _labelAddresses[label.Name] = _currentAddress;
                    break;
                case AssignmentNode assign:
                    EmitAssignment(assign);
                    break;
                case PrintNode print:
                    EmitPrint(print);
                    break;
                case IfNode ifNode:
                    EmitIf(ifNode);
                    break;
                case ForNode forNode:
                    EmitFor(forNode);
                    break;
                case WhileNode whileNode:
                    EmitWhile(whileNode);
                    break;
                case GotoNode gotoNode:
                    EmitGoto(gotoNode);
                    break;
                case GosubNode gosub:
                    EmitGosub(gosub);
                    break;
                case ReturnNode ret:
                    EmitReturn();
                    break;
                case EndNode end:
                    EmitEnd();
                    break;
                case ClsNode cls:
                    EmitCls(cls);
                    break;
                case WaitNode wait:
                    EmitWait(wait);
                    break;
                case SleepNode sleep:
                    EmitSleep(sleep);
                    break;
                case PlotNode plot:
                    EmitPlot(plot);
                    break;
                case LineNode line:
                    EmitLine(line);
                    break;
                case RectangleNode rect:
                    EmitRectangle(rect);
                    break;
                case CircleNode circle:
                    EmitCircle(circle);
                    break;
                case EllipseNode ellipse:
                    EmitEllipse(ellipse);
                    break;
                case ScreenNode screen:
                    EmitScreen(screen);
                    break;
                case VideoNode video:
                    EmitVideo(video);
                    break;
                case InkNode ink:
                    EmitInk(ink);
                    break;
                case PaperNode paper:
                    EmitPaper(paper);
                    break;
                default:
                    _errors.Add(new CompileError(stmt.Line, stmt.Column, $"Unsupported statement: {stmt.GetType().Name}"));
                    break;
            }
        }

        private void EmitAssignment(AssignmentNode assign)
        {
            // Get variable info
            SymbolInfo varInfo = _symbolTable.GetVariable(assign.Variable.Name);
            string initialValue = null;
            
            // Check if this is a string literal assignment for initialization
            if (assign.Variable.Type == VariableType.String && assign.Expression is LiteralNode lit && lit.Value is string strVal)
            {
                initialValue = strVal;
            }
            
            if (varInfo == null)
            {
                // Allocate new variable - generate label
                string label = _variableManager.GenerateVariableLabel(assign.Variable.Name, assign.Variable.Type);
                varInfo = new SymbolInfo
                {
                    Name = assign.Variable.Name,
                    Type = assign.Variable.Type,
                    Label = label
                };
                _symbolTable.AddVariable(assign.Variable.Name, varInfo);
                
                // Declare variable in variable section with initial value if string literal
                DeclareVariable(varInfo, initialValue);
                
                // If it's a string literal, we're done (value is already in the declaration)
                if (initialValue != null)
                {
                    return;
                }
            }

            // Evaluate expression and store result
            string exprReg = EmitExpression(assign.Expression, assign.Variable.Type);

            // Use LD to store variable (LD (label), reg or fr)
            if (assign.Variable.Type == VariableType.Integer)
            {
                _assembly.AppendLine($"    LD ({varInfo.Label}), {exprReg}");
                _currentAddress += 7; // LD (addr), rrrr = 7 bytes (32-bit)
            }
            else if (assign.Variable.Type == VariableType.Float)
            {
                // For floats, exprReg should be a float register (F0-F15)
                _assembly.AppendLine($"    LD ({varInfo.Label}), {exprReg}");
                _currentAddress += 7; // LD (addr), fr = 7 bytes
            }
            else // String
            {
                // For strings, we need to copy the string data
                // exprReg contains the string address, copy to variable address
                _assembly.AppendLine($"    MEMC {exprReg}, {varInfo.Label}, 256");
                _currentAddress += 10; // Approximate
            }

            // Release the register back to the pool for reuse after storing
            ReleaseRegister(exprReg, assign.Variable.Type);
        }

        private string EmitExpression(ExpressionNode expr, VariableType targetType = VariableType.Integer)
        {
            switch (expr)
            {
                case LiteralNode lit:
                    return EmitLiteral(lit, targetType);
                case VariableNode var:
                    return EmitVariableLoad(var, targetType);
                case BinaryExpressionNode bin:
                    return EmitBinaryExpression(bin, targetType);
                case UnaryExpressionNode unary:
                    return EmitUnaryExpression(unary, targetType);
                case FunctionCallNode func:
                    return EmitFunctionCall(func, targetType);
                default:
                    _errors.Add(new CompileError(expr.Line, expr.Column, $"Unsupported expression: {expr.GetType().Name}"));
                    return "A"; // Default register
            }
        }

        private string EmitLiteral(LiteralNode lit, VariableType targetType)
        {
            string reg = GetTempRegister(targetType);
            
            if (lit.Type == VariableType.Integer && lit.Value is int intVal)
            {
                _assembly.AppendLine($"    LD {reg}, {intVal}");
                _currentAddress += 9; // LD rrrr, nnnn = 9 bytes (1 opcode + 1 subop + 1 reg + 4 value)
            }
            else if (lit.Type == VariableType.Float && lit.Value is float floatVal)
            {
                // Convert float to hex representation for assembly
                byte[] bytes = BitConverter.GetBytes(floatVal);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(bytes);
                uint floatBits = BitConverter.ToUInt32(bytes, 0);
                // For float literals, use LD fr, nnnn (32-bit float bits)
                _assembly.AppendLine($"    LD {reg}, {floatBits:X8}");
                _currentAddress += 9; // LD fr, nnnn = 9 bytes
            }
            else if (lit.Type == VariableType.String && lit.Value is string strVal)
            {
                // Store string in data section and get label
                string strLabel = AllocateString(strVal);
                // Load 24-bit address of label into register
                _assembly.AppendLine($"    LD {reg}, {strLabel}");
                _currentAddress += 7;
            }
            
            return reg;
        }

        private string EmitVariableLoad(VariableNode var, VariableType targetType)
        {
            SymbolInfo varInfo = _symbolTable.GetVariable(var.Name);
            if (varInfo == null)
            {
                // Allocate new variable - generate label
                string label = _variableManager.GenerateVariableLabel(var.Name, var.Type);
                varInfo = new SymbolInfo
                {
                    Name = var.Name,
                    Type = var.Type,
                    Label = label
                };
                _symbolTable.AddVariable(var.Name, varInfo);
                
                // Declare variable in variable section
                DeclareVariable(varInfo, null);
            }

            string reg = GetTempRegister(targetType);

            if (var.Indices.Count == 0)
            {
                // Simple variable
            if (varInfo.Type == VariableType.Integer)
            {
                _assembly.AppendLine($"    LD {reg}, ({varInfo.Label})");
                _currentAddress += 7; // LD rrrr, (nnn) = 7 bytes
            }
            else if (varInfo.Type == VariableType.Float)
            {
                _assembly.AppendLine($"    LD {reg}, ({varInfo.Label})");
                _currentAddress += 7; // LD fr, (nnn) = 7 bytes
            }
            else // String
            {
                // String address is stored directly (24-bit address)
                _assembly.AppendLine($"    LD {reg}, {varInfo.Label}");
                _currentAddress += 7; // LD rrr, nnn = 7 bytes
            }
            }
            else
            {
                // Array access - calculate offset
                if (varInfo.IsArray && var.Indices.Count > 0)
                {
                    // Calculate array element address
                    // For 1D: address = base + index * elementSize
                    // For 2D: address = base + (row * colSize + col) * elementSize
                    int elementSize = varInfo.Type == VariableType.Integer ? 4 : 
                                     varInfo.Type == VariableType.Float ? 4 : 256;
                    
                    string indexReg = EmitExpression(var.Indices[0], VariableType.Integer);
                    string tempReg = GetTempRegister(VariableType.Integer);
                    
                    // Multiply index by element size
                    _assembly.AppendLine($"    LD {tempReg}, {indexReg}");
                    _assembly.AppendLine($"    MUL {tempReg}, {elementSize}");
                    
                    // Add to base address
                    _assembly.AppendLine($"    LD {reg}, {varInfo.Label}");
                    _assembly.AppendLine($"    ADD {reg}, {tempReg}");
                    
                    // Load from calculated address
                    if (varInfo.Type == VariableType.Integer)
                    {
                        _assembly.AppendLine($"    LD {reg}, ({reg})");
                    }
                    else if (varInfo.Type == VariableType.Float)
                    {
                        _assembly.AppendLine($"    LD {reg}, ({reg})");
                    }
                    else
                    {
                        _assembly.AppendLine($"    LD {reg}, ({reg})");
                    }
                    _currentAddress += 25;
                }
                else
                {
                    // Fallback for non-array or missing indices
                    if (varInfo.Type == VariableType.Integer)
                    {
                        _assembly.AppendLine($"    LD {reg}, ({varInfo.Label})");
                    }
                    else if (varInfo.Type == VariableType.Float)
                    {
                        _assembly.AppendLine($"    LD {reg}, ({varInfo.Label})");
                    }
                    else
                    {
                        _assembly.AppendLine($"    LD {reg}, {varInfo.Label}");
                    }
                    _currentAddress += 7;
                }
            }

            return reg;
        }

        private string EmitBinaryExpression(BinaryExpressionNode bin, VariableType targetType)
        {
            string leftReg = EmitExpression(bin.Left, targetType);
            string rightReg = EmitExpression(bin.Right, targetType);
            
            // Optimize: reuse leftReg as result register instead of allocating a new one
            // This avoids unnecessary LD operations and register allocation
            string resultReg = leftReg;

            switch (bin.Operator)
            {
                case TokenType.PLUS:
                    // Check if this is string concatenation
                    VariableType leftType = GetExpressionType(bin.Left);
                    VariableType rightType = GetExpressionType(bin.Right);
                    if (leftType == VariableType.String || rightType == VariableType.String)
                    {
                        // String concatenation
                        return EmitStringConcatenation(bin.Left, bin.Right, leftReg, rightReg);
                    }
                    // Numeric addition
                    _assembly.AppendLine($"    ADD {resultReg}, {rightReg}");
                    _currentAddress += 5; // ADD rrrr, rrrr = 5 bytes
                    // Release rightReg as it's no longer needed after the operation
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.MINUS:
                    _assembly.AppendLine($"    SUB {resultReg}, {rightReg}");
                    _currentAddress += 5; // SUB rrrr, rrrr = 5 bytes
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.MULTIPLY:
                    _assembly.AppendLine($"    MUL {resultReg}, {rightReg}");
                    _currentAddress += 5; // MUL rrrr, rrrr = 5 bytes
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.DIVIDE:
                    _assembly.AppendLine($"    DIV {resultReg}, {rightReg}");
                    _currentAddress += 5; // DIV rrrr, rrrr = 5 bytes
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.INT_DIVIDE:
                    // Integer division - use DIV then truncate
                    _assembly.AppendLine($"    DIV {resultReg}, {rightReg}");
                    _assembly.AppendLine($"    INT {resultReg}, {resultReg}");
                    _currentAddress += 10;
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.MODULO:
                    // MOD - use DIV and get remainder
                    // Note: DIV modifies resultReg, remainder stays in resultReg
                    _assembly.AppendLine($"    DIV {resultReg}, {rightReg}");
                    // Remainder is in resultReg after DIV
                    _currentAddress += 5;
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.POWER:
                    // Use POW instruction for floats, or implement for integers
                    _assembly.AppendLine($"    POW {resultReg}, {rightReg}");
                    _currentAddress += 5;
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.EQUAL:
                    _assembly.AppendLine($"    CP {resultReg}, {rightReg}");
                    _assembly.AppendLine($"    LD A, 1");
                    _assembly.AppendLine($"    JR EQ, .eq_{_labelCounter}");
                    _assembly.AppendLine($"    LD A, 0");
                    _assembly.AppendLine($".eq_{_labelCounter}");
                    _labelCounter++;
                    _currentAddress += 20;
                    ReleaseRegister(rightReg, targetType);
                    ReleaseRegister(resultReg, targetType);
                    return "A"; // Return 8-bit register for boolean
                case TokenType.NOT_EQUAL:
                    _assembly.AppendLine($"    CP {resultReg}, {rightReg}");
                    _assembly.AppendLine($"    LD A, 1");
                    _assembly.AppendLine($"    JR NE, .ne_{_labelCounter}");
                    _assembly.AppendLine($"    LD A, 0");
                    _assembly.AppendLine($".ne_{_labelCounter}");
                    _labelCounter++;
                    _currentAddress += 20;
                    ReleaseRegister(rightReg, targetType);
                    ReleaseRegister(resultReg, targetType);
                    return "A"; // Return 8-bit register for boolean
                case TokenType.LESS_THAN:
                    _assembly.AppendLine($"    CP {resultReg}, {rightReg}");
                    _assembly.AppendLine($"    LD A, 1");
                    _assembly.AppendLine($"    JR LT, .lt_{_labelCounter}");
                    _assembly.AppendLine($"    LD A, 0");
                    _assembly.AppendLine($".lt_{_labelCounter}");
                    _labelCounter++;
                    _currentAddress += 20;
                    ReleaseRegister(rightReg, targetType);
                    ReleaseRegister(resultReg, targetType);
                    return "A"; // Return 8-bit register for boolean
                case TokenType.GREATER_THAN:
                    _assembly.AppendLine($"    CP {resultReg}, {rightReg}");
                    _assembly.AppendLine($"    LD A, 1");
                    _assembly.AppendLine($"    JR GT, .gt_{_labelCounter}");
                    _assembly.AppendLine($"    LD A, 0");
                    _assembly.AppendLine($".gt_{_labelCounter}");
                    _labelCounter++;
                    _currentAddress += 20;
                    ReleaseRegister(rightReg, targetType);
                    ReleaseRegister(resultReg, targetType);
                    return "A"; // Return 8-bit register for boolean
                case TokenType.LESS_EQUAL:
                    _assembly.AppendLine($"    CP {resultReg}, {rightReg}");
                    _assembly.AppendLine($"    LD A, 1");
                    _assembly.AppendLine($"    JR LTE, .le_{_labelCounter}");
                    _assembly.AppendLine($"    LD A, 0");
                    _assembly.AppendLine($".le_{_labelCounter}:");
                    _labelCounter++;
                    _currentAddress += 20;
                    ReleaseRegister(rightReg, targetType);
                    ReleaseRegister(resultReg, targetType);
                    return "A"; // Return 8-bit register for boolean
                case TokenType.GREATER_EQUAL:
                    _assembly.AppendLine($"    CP {resultReg}, {rightReg}");
                    _assembly.AppendLine($"    LD A, 1");
                    _assembly.AppendLine($"    JR GTE, .ge_{_labelCounter}");
                    _assembly.AppendLine($"    LD A, 0");
                    _assembly.AppendLine($".ge_{_labelCounter}:");
                    _labelCounter++;
                    _currentAddress += 20;
                    ReleaseRegister(rightReg, targetType);
                    ReleaseRegister(resultReg, targetType);
                    return "A"; // Return 8-bit register for boolean
                case TokenType.AND:
                    _assembly.AppendLine($"    AND {resultReg}, {rightReg}");
                    _currentAddress += 5;
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.OR:
                    _assembly.AppendLine($"    OR {resultReg}, {rightReg}");
                    _currentAddress += 5;
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.XOR:
                    _assembly.AppendLine($"    XOR {resultReg}, {rightReg}");
                    _currentAddress += 5;
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.NAND:
                    _assembly.AppendLine($"    NAND {resultReg}, {rightReg}");
                    _currentAddress += 5;
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.NOR:
                    _assembly.AppendLine($"    NOR {resultReg}, {rightReg}");
                    _currentAddress += 5;
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.XNOR:
                    _assembly.AppendLine($"    XNOR {resultReg}, {rightReg}");
                    _currentAddress += 5;
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.IMPLY:
                    _assembly.AppendLine($"    IMPLY {resultReg}, {rightReg}");
                    _currentAddress += 5;
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.SHL:
                    _assembly.AppendLine($"    SL {resultReg}, {rightReg}");
                    _currentAddress += 5;
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.SHR:
                    _assembly.AppendLine($"    SR {resultReg}, {rightReg}");
                    _currentAddress += 5;
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.ROL:
                    _assembly.AppendLine($"    RL {resultReg}, {rightReg}");
                    _currentAddress += 5;
                    ReleaseRegister(rightReg, targetType);
                    break;
                case TokenType.ROR:
                    _assembly.AppendLine($"    RR {resultReg}, {rightReg}");
                    _currentAddress += 5;
                    ReleaseRegister(rightReg, targetType);
                    break;
                default:
                    _errors.Add(new CompileError(bin.Line, bin.Column, $"Unsupported operator: {bin.Operator}"));
                    return leftReg;
            }

            return resultReg;
        }

        private string EmitUnaryExpression(UnaryExpressionNode unary, VariableType targetType)
        {
            string operandReg = EmitExpression(unary.Operand, targetType);
            string resultReg = GetTempRegister(targetType);

            switch (unary.Operator)
            {
                case TokenType.MINUS:
                    _assembly.AppendLine($"    LD {resultReg}, 0");
                    _assembly.AppendLine($"    SUB {resultReg}, {operandReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.NOT:
                    _assembly.AppendLine($"    LD {resultReg}, {operandReg}");
                    _assembly.AppendLine($"    INV {resultReg}");
                    _currentAddress += 10;
                    break;
                default:
                    _errors.Add(new CompileError(unary.Line, unary.Column, $"Unsupported unary operator: {unary.Operator}"));
                    return operandReg;
            }

            return resultReg;
        }

        private string EmitFunctionCall(FunctionCallNode func, VariableType targetType)
        {
            string resultReg = GetTempRegister(targetType);
            string funcName = func.FunctionName.ToUpper();

            switch (funcName)
            {
                // Math functions
                case "ABS":
                    if (func.Arguments.Count != 1) throw new Exception("ABS requires 1 argument");
                    string absReg = EmitExpression(func.Arguments[0], targetType);
                    _assembly.AppendLine($"    LD {resultReg}, {absReg}");
                    _assembly.AppendLine($"    ABS {resultReg}");
                    _currentAddress += 8;
                    break;
                case "SGN":
                    if (func.Arguments.Count != 1) throw new Exception("SGN requires 1 argument");
                    string sgnReg = EmitExpression(func.Arguments[0], targetType);
                    _assembly.AppendLine($"    LD {resultReg}, {sgnReg}");
                    _assembly.AppendLine($"    ISGN {resultReg}");
                    _currentAddress += 8;
                    break;
                case "INT":
                case "FIX":
                    if (func.Arguments.Count != 1) throw new Exception("INT requires 1 argument");
                    string intReg = EmitExpression(func.Arguments[0], VariableType.Float);
                    _assembly.AppendLine($"    LD {resultReg}, {intReg}");
                    _assembly.AppendLine($"    INT {resultReg}");
                    _currentAddress += 8;
                    break;
                case "FLOOR":
                    if (func.Arguments.Count != 1) throw new Exception("FLOOR requires 1 argument");
                    string floorReg = EmitExpression(func.Arguments[0], VariableType.Float);
                    _assembly.AppendLine($"    LD {resultReg}, {floorReg}");
                    _assembly.AppendLine($"    FLOOR {resultReg}");
                    _currentAddress += 8;
                    break;
                case "CEIL":
                    if (func.Arguments.Count != 1) throw new Exception("CEIL requires 1 argument");
                    string ceilReg = EmitExpression(func.Arguments[0], VariableType.Float);
                    _assembly.AppendLine($"    LD {resultReg}, {ceilReg}");
                    _assembly.AppendLine($"    CEIL {resultReg}");
                    _currentAddress += 8;
                    break;
                case "ROUND":
                    if (func.Arguments.Count < 1 || func.Arguments.Count > 2) throw new Exception("ROUND requires 1-2 arguments");
                    string roundReg = EmitExpression(func.Arguments[0], VariableType.Float);
                    _assembly.AppendLine($"    LD {resultReg}, {roundReg}");
                    _assembly.AppendLine($"    ROUND {resultReg}");
                    _currentAddress += 8;
                    break;
                case "SIN":
                    if (func.Arguments.Count != 1) throw new Exception("SIN requires 1 argument");
                    string sinReg = EmitExpression(func.Arguments[0], VariableType.Float);
                    _assembly.AppendLine($"    LD {resultReg}, {sinReg}");
                    _assembly.AppendLine($"    SIN {resultReg}");
                    _currentAddress += 8;
                    break;
                case "COS":
                    if (func.Arguments.Count != 1) throw new Exception("COS requires 1 argument");
                    string cosReg = EmitExpression(func.Arguments[0], VariableType.Float);
                    _assembly.AppendLine($"    LD {resultReg}, {cosReg}");
                    _assembly.AppendLine($"    COS {resultReg}");
                    _currentAddress += 8;
                    break;
                case "TAN":
                    if (func.Arguments.Count != 1) throw new Exception("TAN requires 1 argument");
                    string tanReg = EmitExpression(func.Arguments[0], VariableType.Float);
                    _assembly.AppendLine($"    LD {resultReg}, {tanReg}");
                    _assembly.AppendLine($"    TAN {resultReg}");
                    _currentAddress += 8;
                    break;
                case "SQR":
                    if (func.Arguments.Count != 1) throw new Exception("SQR requires 1 argument");
                    string sqrReg = EmitExpression(func.Arguments[0], VariableType.Float);
                    _assembly.AppendLine($"    LD {resultReg}, {sqrReg}");
                    _assembly.AppendLine($"    SQR {resultReg}");
                    _currentAddress += 8;
                    break;
                case "POW":
                    if (func.Arguments.Count != 2) throw new Exception("POW requires 2 arguments");
                    string powBase = EmitExpression(func.Arguments[0], VariableType.Float);
                    string powExp = EmitExpression(func.Arguments[1], VariableType.Float);
                    _assembly.AppendLine($"    LD {resultReg}, {powBase}");
                    _assembly.AppendLine($"    POW {resultReg}, {powExp}");
                    _currentAddress += 8;
                    break;
                case "MIN":
                    if (func.Arguments.Count != 2) throw new Exception("MIN requires 2 arguments");
                    string min1 = EmitExpression(func.Arguments[0], targetType);
                    string min2 = EmitExpression(func.Arguments[1], targetType);
                    _assembly.AppendLine($"    LD {resultReg}, {min1}");
                    _assembly.AppendLine($"    MIN {resultReg}, {min2}");
                    _currentAddress += 8;
                    break;
                case "MAX":
                    if (func.Arguments.Count != 2) throw new Exception("MAX requires 2 arguments");
                    string max1 = EmitExpression(func.Arguments[0], targetType);
                    string max2 = EmitExpression(func.Arguments[1], targetType);
                    _assembly.AppendLine($"    LD {resultReg}, {max1}");
                    _assembly.AppendLine($"    MAX {resultReg}, {max2}");
                    _currentAddress += 8;
                    break;
                case "RND":
                    _assembly.AppendLine($"    RAND {resultReg}");
                    if (func.Arguments.Count > 0)
                    {
                        // RND(n) - random integer 1 to n
                        string nReg = EmitExpression(func.Arguments[0], VariableType.Integer);
                        _assembly.AppendLine($"    MOD {resultReg}, {nReg}");
                        _assembly.AppendLine($"    INC {resultReg}");
                    }
                    _currentAddress += 5;
                    break;
                case "PI":
                    // PI constant
                    float pi = (float)Math.PI;
                    byte[] piBytes = BitConverter.GetBytes(pi);
                    if (BitConverter.IsLittleEndian) Array.Reverse(piBytes);
                    uint piBits = BitConverter.ToUInt32(piBytes, 0);
                    _assembly.AppendLine($"    LD {resultReg}, {piBits:X8}");
                    _currentAddress += 9; // LD fr, nnnn = 9 bytes
                    break;
                case "E":
                    // E constant
                    float e = (float)Math.E;
                    byte[] eBytes = BitConverter.GetBytes(e);
                    if (BitConverter.IsLittleEndian) Array.Reverse(eBytes);
                    uint eBits = BitConverter.ToUInt32(eBytes, 0);
                    _assembly.AppendLine($"    LD {resultReg}, {eBits:X8}");
                    _currentAddress += 9; // LD fr, nnnn = 9 bytes
                    break;
                // String functions
                case "LEN":
                    return EmitLenFunction(func, resultReg);
                case "LEFT":
                    return EmitLeftFunction(func, resultReg);
                case "RIGHT":
                    return EmitRightFunction(func, resultReg);
                case "MID":
                    return EmitMidFunction(func, resultReg);
                case "CHR":
                    return EmitChrFunction(func, resultReg);
                case "ASC":
                    return EmitAscFunction(func, resultReg);
                case "VAL":
                    return EmitValFunction(func, resultReg);
                case "STR":
                    return EmitStrFunction(func, resultReg);
                case "STRING":
                    return EmitStringFunction(func, resultReg);
                case "INSTR":
                    return EmitInstrFunction(func, resultReg);
                case "UCASE":
                    return EmitUcaseFunction(func, resultReg);
                case "LCASE":
                    return EmitLcaseFunction(func, resultReg);
                case "TRIM":
                    return EmitTrimFunction(func, resultReg);
                case "LTRIM":
                    return EmitLtrimFunction(func, resultReg);
                case "RTRIM":
                    return EmitRtrimFunction(func, resultReg);
                default:
                    _errors.Add(new CompileError(func.Line, func.Column, $"Unsupported function: {func.FunctionName}"));
                    return "A";
            }

            return resultReg;
        }

        private void EmitPrint(PrintNode print)
        {
            if (print.AtPosition)
            {
                // PRINT AT x, y; expression
                string xReg = EmitExpression(print.AtX, VariableType.Integer);
                string yReg = EmitExpression(print.AtY, VariableType.Integer);
                
                _assembly.AppendLine($"    LD A, 0x14  ; DrawText function");
                _assembly.AppendLine($"    LD CD, {xReg}");
                _assembly.AppendLine($"    LD EF, {yReg}");
                
                // Font address
                if (print.FontAddr != null)
                {
                    string fontReg = EmitExpression(print.FontAddr, VariableType.Integer);
                    _assembly.AppendLine($"    LD GHI, {fontReg}");
                }
                else
                {
                    _assembly.AppendLine($"    LD GHI, 0  ; Use default font");
                }
                
                // Color
                if (print.Color != null)
                {
                    string colorReg = EmitExpression(print.Color, VariableType.Integer);
                    _assembly.AppendLine($"    LD J, {colorReg}");
                }
                else
                {
                    _assembly.AppendLine($"    LD J, 15  ; Default color");
                }
                
                // Process print items
                foreach (var item in print.Items)
                {
                    string exprReg = EmitExpression(item.Expression, VariableType.String);
                    _assembly.AppendLine($"    LD KLM, {exprReg}  ; String address");
                }
                
                // Flags, maxWidth, outline (if specified)
                if (print.Flags != null)
                {
                    string flagsReg = EmitExpression(print.Flags, VariableType.Integer);
                    _assembly.AppendLine($"    LD N, {flagsReg}");
                }
                if (print.MaxWidth != null)
                {
                    string widthReg = EmitExpression(print.MaxWidth, VariableType.Integer);
                    _assembly.AppendLine($"    LD OP, {widthReg}");
                }
                if (print.OutlineColor != null && print.OutlinePattern != null)
                {
                    string outColorReg = EmitExpression(print.OutlineColor, VariableType.Integer);
                    string outPatReg = EmitExpression(print.OutlinePattern, VariableType.Integer);
                    _assembly.AppendLine($"    LD Q, {outColorReg}");
                    _assembly.AppendLine($"    LD R, {outPatReg}");
                }
                
                _assembly.AppendLine($"    INT 0x01, A");
                _currentAddress += 30;
            }
            else if (print.TabPosition != null)
            {
                // PRINT TAB(n); expression
                string tabReg = EmitExpression(print.TabPosition, VariableType.Integer);
                // Tab implementation would need cursor position tracking
                _assembly.AppendLine($"    ; TAB({tabReg}) - cursor positioning");
                foreach (var item in print.Items)
                {
                    string exprReg = EmitExpression(item.Expression, VariableType.String);
                    _assembly.AppendLine($"    LD A, 0x12  ; DrawString function");
                    _assembly.AppendLine($"    LD BCD, {exprReg}");
                    _assembly.AppendLine($"    INT 0x01, A");
                }
                _currentAddress += 20;
            }
            else
            {
                // Regular PRINT - handle each item
                foreach (var item in print.Items)
                {
                    // Check if this is a string literal, integer, or expression
                    if (item.Expression is LiteralNode lit)
                    {
                        if (lit.Type == VariableType.String && lit.Value is string strVal)
                        {
                            // String literal - call DrawString directly
                            string strLabel = AllocateString(strVal);
                            _assembly.AppendLine($"    LD A, 0x12  ; DrawString function");
                            _assembly.AppendLine($"    LD BCD, 0  ; Default font");
                            _assembly.AppendLine($"    LD EFG, {strLabel}  ; String address");
                            _assembly.AppendLine($"    LD HI, 0  ; X position (default)");
                            _assembly.AppendLine($"    LD JK, 0  ; Y position (default)");
                            _assembly.AppendLine($"    LD L, 1  ; Color (default)");
                            _assembly.AppendLine($"    LD M, 2  ; Video page");
                            _assembly.AppendLine($"    INT 0x01, A  ; Execute interrupt");
                            _currentAddress += 30;
                        }
                        else if (lit.Type == VariableType.Integer && lit.Value is int intVal)
                        {
                            // Integer literal - call PrintInt with value in BCDE
                            _printIntEmitted = true;
                            _assembly.AppendLine($"    LD BCDE, {intVal}  ; Load integer value");
                            _assembly.AppendLine($"    CALL {_printIntLabel}");
                            _currentAddress += 15;
                        }
                    }
                    else if (item.Expression is VariableNode varNode)
                    {
                        SymbolInfo varInfo = _symbolTable.GetVariable(varNode.Name);
                        if (varInfo == null)
                        {
                            // Allocate new variable
                            VariableType varType = varNode.Name.EndsWith("$") ? VariableType.String : 
                                                 varNode.Name.EndsWith("#") ? VariableType.Float : VariableType.Integer;
                            string varLabel = _variableManager.GenerateVariableLabel(varNode.Name, varType);
                            varInfo = new SymbolInfo { Name = varNode.Name, Type = varType, Label = varLabel };
                            _symbolTable.AddVariable(varNode.Name, varInfo);
                            
                            // Declare variable in variable section
                            DeclareVariable(varInfo, null);
                        }
                        
                        if (varInfo.Type == VariableType.String)
                        {
                            // String variable - call DrawString directly
                            _assembly.AppendLine($"    LD A, 0x12  ; DrawString function");
                            _assembly.AppendLine($"    LD BCD, 0  ; Default font");
                            _assembly.AppendLine($"    LD EFG, {varInfo.Label}  ; String address");
                            _assembly.AppendLine($"    LD HI, 0  ; X position (default)");
                            _assembly.AppendLine($"    LD JK, 0  ; Y position (default)");
                            _assembly.AppendLine($"    LD L, 1  ; Color (default)");
                            _assembly.AppendLine($"    LD M, 2  ; Video page");
                            _assembly.AppendLine($"    INT 0x01, A  ; Execute interrupt");
                            _currentAddress += 30;
                        }
                        else
                        {
                            // Integer/float variable - call PrintInt
                            _printIntEmitted = true;
                            _assembly.AppendLine($"    LD BCDE, ({varInfo.Label})  ; Load variable value (32-bit)");
                            _assembly.AppendLine($"    CALL {_printIntLabel}");
                            _currentAddress += 15;
                        }
                    }
                    else
                    {
                        // Expression - evaluate and call PrintInt
                        _printIntEmitted = true;
                        string exprReg = EmitExpression(item.Expression, VariableType.Integer);
                        _assembly.AppendLine($"    LD BCDE, {exprReg}  ; Load expression result");
                        _assembly.AppendLine($"    CALL {_printIntLabel}");
                        ReleaseRegister(exprReg, VariableType.Integer);
                        _currentAddress += 15;
                    }
                }
            }
        }

        private void EmitIf(IfNode ifNode)
        {
            string condReg = EmitExpression(ifNode.Condition, VariableType.Integer);
            string elseLabel = $".else_{_labelCounter}";
            string endLabel = $".endif_{_labelCounter}";
            _labelCounter++;

            // Test condition - if condReg is 8-bit (A), use A directly; otherwise compare
            if (condReg == "A")
            {
                // Already an 8-bit boolean register
                _assembly.AppendLine($"    CP A, 0");
                _assembly.AppendLine($"    JR Z, {elseLabel}");
            }
            else
            {
                // 32-bit register, compare to 0
                _assembly.AppendLine($"    CP {condReg}, 0");
                _assembly.AppendLine($"    JR Z, {elseLabel}");
                ReleaseRegister(condReg, VariableType.Integer);
            }

            // THEN block
            foreach (var stmt in ifNode.ThenStatements)
            {
                EmitStatement(stmt);
            }

            if (ifNode.ElseStatements.Count > 0 || ifNode.ElseIfs.Count > 0)
            {
                _assembly.AppendLine($"    JP {endLabel}");
            }

            _assembly.AppendLine($"{elseLabel}");

            // ELSEIF blocks
            foreach (var elseif in ifNode.ElseIfs)
            {
                string elseifLabel = $".elseif_{_labelCounter}";
                _labelCounter++;
                string condReg2 = EmitExpression(elseif.Condition);
                _assembly.AppendLine($"    CP {condReg2}, 0");
                _assembly.AppendLine($"    JR Z, {elseifLabel}");
                foreach (var stmt in elseif.Statements)
                {
                    EmitStatement(stmt);
                }
                _assembly.AppendLine($"    JP {endLabel}");
                _assembly.AppendLine($"{elseifLabel}");
            }

            // ELSE block
            if (ifNode.ElseStatements.Count > 0)
            {
                foreach (var stmt in ifNode.ElseStatements)
                {
                    EmitStatement(stmt);
                }
            }

            _assembly.AppendLine($"{endLabel}");
        }

        private void EmitFor(ForNode forNode)
        {
            // Determine if this is a float loop (check if step is float or if start/end are float)
            bool isFloatLoop = false;
            if (forNode.Step != null)
            {
                if (forNode.Step is LiteralNode stepLit && stepLit.Type == VariableType.Float)
                    isFloatLoop = true;
            }
            if (!isFloatLoop && (forNode.Start is LiteralNode startLit && startLit.Type == VariableType.Float ||
                                 forNode.End is LiteralNode endLit && endLit.Type == VariableType.Float))
            {
                isFloatLoop = true;
            }

            // Allocate loop variable
            SymbolInfo loopVar = _symbolTable.GetVariable(forNode.VariableName);
            if (loopVar == null)
            {
                string label = _variableManager.GenerateVariableLabel(forNode.VariableName, isFloatLoop ? VariableType.Float : VariableType.Integer);
                loopVar = new SymbolInfo 
                { 
                    Name = forNode.VariableName, 
                    Type = isFloatLoop ? VariableType.Float : VariableType.Integer, 
                    Label = label
                };
                _symbolTable.AddVariable(forNode.VariableName, loopVar);
                
                // Declare variable in variable section
                DeclareVariable(loopVar, null);
            }

            string startLabel = $".for_start_{_labelCounter}";
            string endLabel = $".for_end_{_labelCounter}";
            string stepLabel = $".for_step_{_labelCounter}";
            _labelCounter++;

            if (isFloatLoop)
            {
                EmitForFloat(forNode, loopVar, startLabel, endLabel, stepLabel);
            }
            else
            {
                EmitForInteger(forNode, loopVar, startLabel, endLabel, stepLabel);
            }
        }

        private void EmitForInteger(ForNode forNode, SymbolInfo loopVar, string startLabel, string endLabel, string stepLabel)
        {
            // Initialize loop variable - use direct memory operation for literals
            if (forNode.Start is LiteralNode startLit && startLit.Value is int startVal)
            {
                // Direct memory load: LD (addr), value, 4 (32-bit)
                _assembly.AppendLine($"    LD ({loopVar.Label}), {startVal}, 4  ; Initialize loop variable (32-bit)");
            }
            else
            {
                // Expression - evaluate and store
                string startReg = EmitExpression(forNode.Start, VariableType.Integer);
                _assembly.AppendLine($"    LD ({loopVar.Label}), {startReg}  ; Initialize loop variable (32-bit)");
                ReleaseRegister(startReg, VariableType.Integer);
            }
            
            _assembly.AppendLine($"{startLabel}");

            // Check condition - use 32-bit registers
            string endReg = EmitExpression(forNode.End, VariableType.Integer);
            string loopValReg = GetTempRegister(VariableType.Integer);
            _assembly.AppendLine($"    LD {loopValReg}, ({loopVar.Label})  ; Load loop variable");
            _assembly.AppendLine($"    CP {loopValReg}, {endReg}  ; Compare 32-bit values");
            _assembly.AppendLine($"    JR GT, {endLabel}");
            ReleaseRegister(endReg, VariableType.Integer);
            // Keep loopValReg for increment

            // Loop body
            foreach (var stmt in forNode.Statements)
            {
                EmitStatement(stmt);
            }

            // Increment - use 32-bit operations
            // Note: loopValReg is already loaded from the condition check above, but we need to reload
            // it after the loop body since the body might have modified registers
            _assembly.AppendLine($"{stepLabel}");
            // Reload loop variable (it might have been modified or registers reused)
            _assembly.AppendLine($"    LD {loopValReg}, ({loopVar.Label})  ; Reload loop variable");
            
            if (forNode.Step != null)
            {
                if (forNode.Step is LiteralNode stepLit && stepLit.Value is int stepVal)
                {
                    // Direct increment
                    _assembly.AppendLine($"    ADD {loopValReg}, {stepVal}  ; Add step (32-bit)");
                    _assembly.AppendLine($"    LD ({loopVar.Label}), {loopValReg}  ; Store loop variable");
                }
                else
                {
                    string stepReg = EmitExpression(forNode.Step, VariableType.Integer);
                    _assembly.AppendLine($"    ADD {loopValReg}, {stepReg}  ; Add step (32-bit)");
                    _assembly.AppendLine($"    LD ({loopVar.Label}), {loopValReg}  ; Store loop variable");
                    ReleaseRegister(stepReg, VariableType.Integer);
                }
            }
            else
            {
                // Default step of 1
                _assembly.AppendLine($"    ADD {loopValReg}, 1  ; Increment by 1 (32-bit)");
                _assembly.AppendLine($"    LD ({loopVar.Label}), {loopValReg}  ; Store loop variable");
            }
            ReleaseRegister(loopValReg, VariableType.Integer);
            _assembly.AppendLine($"    JP {startLabel}");
            _assembly.AppendLine($"{endLabel}");
        }

        private void EmitForFloat(ForNode forNode, SymbolInfo loopVar, string startLabel, string endLabel, string stepLabel)
        {
            // Initialize loop variable - float
            if (forNode.Start is LiteralNode startLit && startLit.Value is float startVal)
            {
                // For float literals, we need to load the float value
                string startReg = EmitExpression(forNode.Start, VariableType.Float);
                _assembly.AppendLine($"    LD ({loopVar.Label}), {startReg}  ; Initialize loop variable (float)");
                ReleaseRegister(startReg, VariableType.Float);
            }
            else
            {
                string startReg = EmitExpression(forNode.Start, VariableType.Float);
                _assembly.AppendLine($"    LD ({loopVar.Label}), {startReg}  ; Initialize loop variable (float)");
                ReleaseRegister(startReg, VariableType.Float);
            }
            
            _assembly.AppendLine($"{startLabel}");

            // Check condition - use float registers
            string endReg = EmitExpression(forNode.End, VariableType.Float);
            string loopValReg = GetTempRegister(VariableType.Float);
            _assembly.AppendLine($"    LD {loopValReg}, ({loopVar.Label})  ; Load loop variable (float)");
            _assembly.AppendLine($"    CP {loopValReg}, {endReg}  ; Compare float values");
            _assembly.AppendLine($"    JR GT, {endLabel}");
            ReleaseRegister(endReg, VariableType.Float);

            // Loop body
            foreach (var stmt in forNode.Statements)
            {
                EmitStatement(stmt);
            }

            // Increment - use float operations
            _assembly.AppendLine($"{stepLabel}:");
            if (forNode.Step != null)
            {
                if (forNode.Step is LiteralNode stepLit && stepLit.Value is float stepVal)
                {
                    string stepReg = GetTempRegister(VariableType.Float);
                    // Load float literal into register
                    byte[] bytes = BitConverter.GetBytes(stepVal);
                    if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                    uint floatBits = BitConverter.ToUInt32(bytes, 0);
                    _assembly.AppendLine($"    LD {stepReg}, {floatBits:X8}  ; Load step value (float)");
                    _assembly.AppendLine($"    LD {loopValReg}, ({loopVar.Label})  ; Load loop variable");
                    _assembly.AppendLine($"    ADD {loopValReg}, {stepReg}  ; Add step (float)");
                    _assembly.AppendLine($"    LD ({loopVar.Label}), {loopValReg}  ; Store loop variable");
                    ReleaseRegister(stepReg, VariableType.Float);
                }
                else
                {
                    string stepReg = EmitExpression(forNode.Step, VariableType.Float);
                    _assembly.AppendLine($"    LD {loopValReg}, ({loopVar.Label})  ; Load loop variable");
                    _assembly.AppendLine($"    ADD {loopValReg}, {stepReg}  ; Add step (float)");
                    _assembly.AppendLine($"    LD ({loopVar.Label}), {loopValReg}  ; Store loop variable");
                    ReleaseRegister(stepReg, VariableType.Float);
                }
            }
            else
            {
                // Default step of 1.0
                string stepReg = GetTempRegister(VariableType.Float);
                _assembly.AppendLine($"    LD {stepReg}, 0x3F800000  ; Load 1.0 (float)");
                _assembly.AppendLine($"    LD {loopValReg}, ({loopVar.Label})  ; Load loop variable");
                _assembly.AppendLine($"    ADD {loopValReg}, {stepReg}  ; Add step (float)");
                _assembly.AppendLine($"    LD ({loopVar.Label}), {loopValReg}  ; Store loop variable");
                ReleaseRegister(stepReg, VariableType.Float);
            }
            ReleaseRegister(loopValReg, VariableType.Float);
            _assembly.AppendLine($"    JP {startLabel}");
            _assembly.AppendLine($"{endLabel}");
        }

        private void EmitWhile(WhileNode whileNode)
        {
            string startLabel = $".while_start_{_labelCounter}";
            string endLabel = $".while_end_{_labelCounter}";
            _labelCounter++;

            _assembly.AppendLine($"{startLabel}");
            string condReg = EmitExpression(whileNode.Condition);
            _assembly.AppendLine($"    CP {condReg}, 0");
            _assembly.AppendLine($"    JR Z, {endLabel}");

            foreach (var stmt in whileNode.Statements)
            {
                EmitStatement(stmt);
            }

            _assembly.AppendLine($"    JP {startLabel}");
            _assembly.AppendLine($"{endLabel}");
        }

        private void EmitGoto(GotoNode gotoNode)
        {
            if (_labelAddresses.ContainsKey(gotoNode.Label))
            {
                uint addr = _labelAddresses[gotoNode.Label];
                _assembly.AppendLine($"    JP {addr:X6}");
                _currentAddress += 7;
            }
            else
            {
                _forwardReferences.Add((gotoNode.Label, _currentAddress));
                _assembly.AppendLine($"    JP .{gotoNode.Label}  ; Forward reference");
                _currentAddress += 7;
            }
        }

        private void EmitGosub(GosubNode gosub)
        {
            if (_labelAddresses.ContainsKey(gosub.Label))
            {
                uint addr = _labelAddresses[gosub.Label];
                _assembly.AppendLine($"    CALL {addr:X6}");
                _currentAddress += 7;
            }
            else
            {
                _forwardReferences.Add((gosub.Label, _currentAddress));
                _assembly.AppendLine($"    CALL .{gosub.Label}  ; Forward reference");
                _currentAddress += 7;
            }
        }

        private void EmitReturn()
        {
            _assembly.AppendLine("    RET");
            _currentAddress += 1;
        }

        private void EmitEnd()
        {
            _assembly.AppendLine("    RET  ; END of program");
            _currentAddress += 2; // RET is typically 2 bytes
        }

        private void EmitCls(ClsNode cls)
        {
            _assembly.AppendLine("    LD A, 0x05  ; ClearVideoPage");
            if (cls.Page != null)
            {
                string pageReg = EmitExpression(cls.Page, VariableType.Integer);
                _assembly.AppendLine($"    LD B, {pageReg}");
            }
            else
            {
                _assembly.AppendLine("    LD B, 0");
            }
            _assembly.AppendLine("    LD C, 0  ; Color 0");
            _assembly.AppendLine("    INT 0x01, A");
            _currentAddress += 15;
        }

        private void EmitWait(WaitNode wait)
        {
            string framesReg = EmitExpression(wait.Frames, VariableType.Integer);
            _assembly.AppendLine($"    WAIT {framesReg}");
            _currentAddress += 5;
        }

        private void EmitSleep(SleepNode sleep)
        {
            // SLEEP uses TIME interrupt or WAIT with frame calculation
            string msReg = EmitExpression(sleep.Milliseconds, VariableType.Integer);
            // Convert milliseconds to frames (assuming 60 FPS: frames = ms / 16.67)
            _assembly.AppendLine($"    ; SLEEP {msReg} ms");
            _assembly.AppendLine($"    LD A, {msReg}");
            _assembly.AppendLine($"    DIV A, 17  ; Approximate frames (60 FPS)");
            _assembly.AppendLine($"    WAIT A");
            _currentAddress += 10;
        }

        private void EmitPlot(PlotNode plot)
        {
            // Load function ID FIRST (Register A) - this is the base register
            // This must be done first to avoid register collisions
            _assembly.AppendLine("    LD A, 0x20  ; Plot function");
            
            // Load page (8-bit) into Register+1 (B)
            _assembly.AppendLine("    LD B, 0  ; Page 0");
            
            // Load x coordinate (16-bit) into Register+2 (CD)
            // For literals, load directly; for expressions, evaluate and extract 16-bit value
            if (plot.X is LiteralNode xLit && xLit.Value is int xVal)
            {
                // Direct literal - load into 16-bit register CD
                _assembly.AppendLine($"    LD CD, {xVal}  ; X coordinate");
            }
            else
            {
                // Expression - evaluate into a register, then copy to CD
                string xReg = EmitExpression(plot.X, VariableType.Integer);
                // Copy value to 16-bit register CD
                _assembly.AppendLine($"    LD CD, {xReg}  ; X coordinate");
                // Release the temporary register
                ReleaseRegister(xReg, VariableType.Integer);
            }
            
            // Load y coordinate (16-bit) into Register+4 (EF)
            if (plot.Y is LiteralNode yLit && yLit.Value is int yVal)
            {
                // Direct literal - load into 16-bit register EF
                _assembly.AppendLine($"    LD EF, {yVal}  ; Y coordinate");
            }
            else
            {
                // Expression - evaluate into a register, then copy to EF
                string yReg = EmitExpression(plot.Y, VariableType.Integer);
                _assembly.AppendLine($"    LD EF, {yReg}  ; Y coordinate");
                ReleaseRegister(yReg, VariableType.Integer);
            }
            
            // Load color (8-bit) into Register+6 (G)
            if (plot.Color != null)
            {
                if (plot.Color is LiteralNode colorLit && colorLit.Value is int colorVal)
                {
                    // Direct literal - load into 8-bit register G
                    _assembly.AppendLine($"    LD G, {colorVal}  ; Color");
                }
                else
                {
                    // Expression - evaluate into a register, then copy to G
                    string colorReg = EmitExpression(plot.Color, VariableType.Integer);
                    _assembly.AppendLine($"    LD G, {colorReg}  ; Color");
                    ReleaseRegister(colorReg, VariableType.Integer);
                }
            }
            else
            {
                _assembly.AppendLine("    LD G, 15  ; Default color");
            }
            
            _assembly.AppendLine("    INT 0x01, A");
            _currentAddress += 25;
        }

        private void EmitLine(LineNode line)
        {
            string x1Reg = EmitExpression(line.X1, VariableType.Integer);
            string y1Reg = EmitExpression(line.Y1, VariableType.Integer);
            string x2Reg = EmitExpression(line.X2, VariableType.Integer);
            string y2Reg = EmitExpression(line.Y2, VariableType.Integer);
            _assembly.AppendLine("    LD A, 0x21  ; Line function");
            _assembly.AppendLine("    LD B, 0  ; Page 0");
            _assembly.AppendLine($"    LD CD, {x1Reg}");
            _assembly.AppendLine($"    LD EF, {y1Reg}");
            _assembly.AppendLine($"    LD GH, {x2Reg}");
            _assembly.AppendLine($"    LD IJ, {y2Reg}");
            if (line.Color != null)
            {
                string colorReg = EmitExpression(line.Color, VariableType.Integer);
                _assembly.AppendLine($"    LD K, {colorReg}");
            }
            else
            {
                _assembly.AppendLine("    LD K, 15  ; Default color");
            }
            _assembly.AppendLine("    INT 0x01, A");
            _currentAddress += 25;
        }

        private void EmitRectangle(RectangleNode rect)
        {
            string xReg = EmitExpression(rect.X, VariableType.Integer);
            string yReg = EmitExpression(rect.Y, VariableType.Integer);
            string wReg = EmitExpression(rect.Width, VariableType.Integer);
            string hReg = EmitExpression(rect.Height, VariableType.Integer);
            _assembly.AppendLine($"    LD A, {(rect.Filled ? "0x06" : "0x07")}  ; {(rect.Filled ? "DrawFilledRectangle" : "DrawRectangle")}");
            _assembly.AppendLine("    LD B, 0  ; Page 0");
            _assembly.AppendLine($"    LD CD, {xReg}");
            _assembly.AppendLine($"    LD EF, {yReg}");
            _assembly.AppendLine($"    LD GH, {wReg}");
            _assembly.AppendLine($"    LD IJ, {hReg}");
            if (rect.Color != null)
            {
                string colorReg = EmitExpression(rect.Color, VariableType.Integer);
                _assembly.AppendLine($"    LD K, {colorReg}");
            }
            else
            {
                _assembly.AppendLine("    LD K, 15  ; Default color");
            }
            _assembly.AppendLine("    INT 0x01, A");
            _currentAddress += 25;
        }

        private void EmitCircle(CircleNode circle)
        {
            string xReg = EmitExpression(circle.X, VariableType.Integer);
            string yReg = EmitExpression(circle.Y, VariableType.Integer);
            string rReg = EmitExpression(circle.Radius, VariableType.Integer);
            _assembly.AppendLine($"    LD A, {(circle.Filled ? "0x23" : "0x22")}  ; {(circle.Filled ? "DrawFilledEllipse" : "Ellipse")}");
            _assembly.AppendLine("    LD B, 0  ; Page 0");
            _assembly.AppendLine($"    LD CD, {xReg}");
            _assembly.AppendLine($"    LD EF, {yReg}");
            _assembly.AppendLine($"    LD GH, {rReg}  ; radiusX");
            _assembly.AppendLine($"    LD IJ, {rReg}  ; radiusY (same for circle)");
            if (circle.Color != null)
            {
                string colorReg = EmitExpression(circle.Color, VariableType.Integer);
                _assembly.AppendLine($"    LD K, {colorReg}");
            }
            else
            {
                _assembly.AppendLine("    LD K, 15  ; Default color");
            }
            _assembly.AppendLine("    INT 0x01, A");
            _currentAddress += 25;
        }

        private void EmitEllipse(EllipseNode ellipse)
        {
            string xReg = EmitExpression(ellipse.X, VariableType.Integer);
            string yReg = EmitExpression(ellipse.Y, VariableType.Integer);
            string rxReg = EmitExpression(ellipse.RadiusX, VariableType.Integer);
            string ryReg = EmitExpression(ellipse.RadiusY, VariableType.Integer);
            _assembly.AppendLine($"    LD A, {(ellipse.Filled ? "0x23" : "0x22")}  ; {(ellipse.Filled ? "DrawFilledEllipse" : "Ellipse")}");
            _assembly.AppendLine("    LD B, 0  ; Page 0");
            _assembly.AppendLine($"    LD CD, {xReg}");
            _assembly.AppendLine($"    LD EF, {yReg}");
            _assembly.AppendLine($"    LD GH, {rxReg}  ; radiusX");
            _assembly.AppendLine($"    LD IJ, {ryReg}  ; radiusY");
            if (ellipse.Color != null)
            {
                string colorReg = EmitExpression(ellipse.Color, VariableType.Integer);
                _assembly.AppendLine($"    LD K, {colorReg}");
            }
            else
            {
                _assembly.AppendLine("    LD K, 15  ; Default color");
            }
            _assembly.AppendLine("    INT 0x01, A");
            _currentAddress += 25;
        }

        private void EmitScreen(ScreenNode screen)
        {
            // SCREEN sets active page - may need interrupt or direct memory access
            string pageReg = EmitExpression(screen.Page, VariableType.Integer);
            _assembly.AppendLine($"    ; SCREEN {pageReg} - set active video page");
            _assembly.AppendLine($"    LD A, 0x03  ; ReadVideoAddress (placeholder)");
            _assembly.AppendLine($"    LD B, {pageReg}");
            _assembly.AppendLine($"    INT 0x01, A");
            _currentAddress += 10;
        }

        private void EmitVideo(VideoNode video)
        {
            string pagesReg = EmitExpression(video.Pages, VariableType.Integer);
            _assembly.AppendLine("    LD A, 0x02  ; SetVideoPagesCount");
            _assembly.AppendLine($"    LD B, {pagesReg}");
            _assembly.AppendLine("    INT 0x01, A");
            _currentAddress += 10;
        }

        private void EmitInk(InkNode ink)
        {
            // INK sets current drawing color - store in a variable for later use
            string colorReg = EmitExpression(ink.Color, VariableType.Integer);
            _assembly.AppendLine($"    ; INK {colorReg} - set current color");
            // Store in a compiler-managed variable
            _assembly.AppendLine($"    LD .currentColor, {colorReg}");
            _currentAddress += 10;
        }

        private void EmitPaper(PaperNode paper)
        {
            // PAPER sets background color
            string colorReg = EmitExpression(paper.Color, VariableType.Integer);
            _assembly.AppendLine($"    ; PAPER {colorReg} - set background color");
            _assembly.AppendLine($"    LD .currentPaper, {colorReg}");
            _currentAddress += 10;
        }

        private void ResolveForwardReferences()
        {
            // Forward references are resolved by label addresses
            // This is a simplified version
        }

        private int _tempRegisterCounter = 0;
        // 32-bit registers spaced by 4 positions to avoid overlap
        // ABCD (A-D), EFGH (E-H), IJKL (I-L), MNOP (M-P), QRST (Q-T), UVWX (U-X), YZAB (Y-B wraps)
        private readonly string[] _int32Registers = {
            "ABCD", "EFGH", "IJKL", "MNOP", "QRST", "UVWX", "YZAB"
        };
        private readonly string[] _floatRegisters = {
            "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7",
            "F8", "F9", "F10", "F11", "F12", "F13", "F14", "F15"
        };

        private string GetTempRegister(VariableType type)
        {
            if (type == VariableType.Float)
            {
                // Check if we have a reusable float register
                if (_availableFloatRegisters.Count > 0)
                {
                    return _availableFloatRegisters.Pop();
                }
                // Otherwise allocate a new one
                string reg = _floatRegisters[_tempRegisterCounter % _floatRegisters.Length];
                _tempRegisterCounter++;
                return reg;
            }
            else
            {
                // Check if we have a reusable integer register
                if (_availableIntRegisters.Count > 0)
                {
                    return _availableIntRegisters.Pop();
                }
                // Otherwise allocate a new one
                string reg = _int32Registers[_tempRegisterCounter % _int32Registers.Length];
                _tempRegisterCounter++;
                return reg;
            }
        }

        private void ReleaseRegister(string reg, VariableType type)
        {
            // Add register back to the pool for reuse
            if (type == VariableType.Float)
            {
                _availableFloatRegisters.Push(reg);
            }
            else
            {
                _availableIntRegisters.Push(reg);
            }
        }

        private string AllocateString(string str)
        {
            // Check if we've already allocated this string
            if (_stringLiterals.ContainsKey(str))
            {
                return _stringLiterals[str];
            }

            // Create label for string
            string label = $".string{_stringCounter}";
            _stringCounter++;
            
            // Add to data section (labels start with dot but don't end with colon)
            _dataSection.AppendLine($"{label}");
            _dataSection.Append($"    #DB ");
            
            // Escape string and add null terminator
            string escaped = str.Replace("\"", "\"\""); // Escape quotes
            _dataSection.AppendLine($"\"{escaped}\", 0");
            
            // Track this string
            _stringLiterals[str] = label;
            
            // Update variable manager (estimate size for address calculation)
            uint addr = _variableManager.GetVariableSectionEnd();
            _variableManager.SetCodeEndAddress(addr + (uint)str.Length + 1); // +1 for null terminator
            
            return label;
        }

        private string EmitStringExpression(ExpressionNode expr)
        {
            // For string expressions in PRINT, return either a label (for literals) or an address (for variables)
            // This allows us to use labels directly in LD instructions
            if (expr is LiteralNode lit)
            {
                if (lit.Type == VariableType.String && lit.Value is string strVal)
                {
                    // Return the label directly for string literals (e.g., ".string0")
                    return AllocateString(strVal);
                }
                else if (lit.Type == VariableType.Integer && lit.Value is int intVal)
                {
                    // Convert integer literal to string
                    return ConvertIntegerToString(intVal);
                }
            }
            else if (expr is VariableNode var)
            {
                SymbolInfo varInfo = _symbolTable.GetVariable(var.Name);
                if (varInfo == null)
                {
                    // Allocate new variable - determine type from variable name
                    VariableType varType = var.Name.EndsWith("$") ? VariableType.String : 
                                         var.Name.EndsWith("#") ? VariableType.Float : VariableType.Integer;
                    string varLabel = _variableManager.GenerateVariableLabel(var.Name, varType);
                    varInfo = new SymbolInfo
                    {
                        Name = var.Name,
                        Type = varType,
                        Label = varLabel
                    };
                    _symbolTable.AddVariable(var.Name, varInfo);
                    
                    // Declare variable in variable section
                    DeclareVariable(varInfo, null);
                }
                
                if (varInfo.Type == VariableType.String)
                {
                    // For string variables, return the label
                    return varInfo.Label;
                }
                else
                {
                    // For integer/float variables, convert to string
                    return ConvertVariableToString(varInfo);
                }
            }
            else
            {
                // For expressions, check if they're integer or string
                // For now, assume integer and convert to string
                return ConvertExpressionToString(expr);
            }
            
            // Fallback (should never reach here)
            return "0";
        }

        private string ConvertIntegerToString(int value)
        {
            // Allocate a temporary string buffer
            uint strAddr = _variableManager.GetVariableSectionEnd();
            _variableManager.SetCodeEndAddress(strAddr + 32); // Reserve space for string
            
            // Use IntToString interrupt to convert integer to string
            string valueReg = GetTempRegister(VariableType.Integer);
            string formatLabel = AllocateString("{0}"); // Format string for integer
            string formatAddr = _stringLiterals["{0}"];
            
            _assembly.AppendLine($"    LD A, 0x03  ; IntToString function");
            _assembly.AppendLine($"    LD {valueReg}, {value}  ; Integer value (32-bit)");
            _assembly.AppendLine($"    LD BCD, {formatLabel}  ; Format string address");
            _assembly.AppendLine($"    LD EFG, {strAddr:X6}  ; Target address");
            _assembly.AppendLine($"    INT 0x05, A  ; Convert integer to string");
            
            ReleaseRegister(valueReg, VariableType.Integer);
            
            return $"{strAddr:X6}";
        }

        private string ConvertVariableToString(SymbolInfo varInfo)
        {
            // Allocate a temporary string buffer
            uint strAddr = _variableManager.GetVariableSectionEnd();
            _variableManager.SetCodeEndAddress(strAddr + 32); // Reserve space for string
            
            // Use appropriate conversion interrupt based on type
            if (varInfo.Type == VariableType.Integer)
            {
                string valueReg = GetTempRegister(VariableType.Integer);
                string formatLabel = AllocateString("{0}"); // Format string
                
                _assembly.AppendLine($"    LD A, 0x03  ; IntToString function");
                _assembly.AppendLine($"    LD {valueReg}, ({varInfo.Label})  ; Load integer value (32-bit)");
                _assembly.AppendLine($"    LD BCD, {formatLabel}  ; Format string address");
                _assembly.AppendLine($"    LD EFG, {strAddr:X6}  ; Target address");
                _assembly.AppendLine($"    INT 0x05, A  ; Convert integer to string");
                
                ReleaseRegister(valueReg, VariableType.Integer);
            }
            else if (varInfo.Type == VariableType.Float)
            {
                string valueReg = GetTempRegister(VariableType.Float);
                string formatLabel = AllocateString("{0}"); // Format string
                
                _assembly.AppendLine($"    LD A, 0x01  ; FloatToString function");
                _assembly.AppendLine($"    LD B, 0  ; Float register index (will load into F0)");
                _assembly.AppendLine($"    LD {valueReg}, ({varInfo.Label})  ; Load float value");
                _assembly.AppendLine($"    LD F0, {valueReg}  ; Store in float register");
                _assembly.AppendLine($"    LD BCD, {formatLabel}  ; Format string address");
                _assembly.AppendLine($"    LD EFG, {strAddr:X6}  ; Target address");
                _assembly.AppendLine($"    INT 0x05, A  ; Convert float to string");
                
                ReleaseRegister(valueReg, VariableType.Float);
            }
            
            return $"{strAddr:X6}";
        }

        private string ConvertExpressionToString(ExpressionNode expr)
        {
            // Evaluate expression as integer, then convert to string
            string valueReg = EmitExpression(expr, VariableType.Integer);
            
            // Allocate a temporary string buffer
            uint strAddr = _variableManager.GetVariableSectionEnd();
            _variableManager.SetCodeEndAddress(strAddr + 32); // Reserve space for string
            
            string formatLabel = AllocateString("{0}"); // Format string
            
            _assembly.AppendLine($"    LD A, 0x03  ; IntToString function");
            _assembly.AppendLine($"    LD BCDE, {valueReg}  ; Integer value (32-bit)");
            _assembly.AppendLine($"    LD FGH, {formatLabel}  ; Format string address");
            _assembly.AppendLine($"    LD IJK, {strAddr:X6}  ; Target address");
            _assembly.AppendLine($"    INT 0x05, A  ; Convert integer to string");
            
            ReleaseRegister(valueReg, VariableType.Integer);
            
            return $"{strAddr:X6}";
        }

        private string EmitPrintIntSubroutine()
        {
            // Allocate string buffer if not already allocated
            if (_printIntBufferAddr == 0)
            {
                _printIntBufferAddr = _variableManager.GetVariableSectionEnd();
                _variableManager.SetCodeEndAddress(_printIntBufferAddr + 12); // Reserve 12 bytes for string buffer
            }
            
            // Allocate format string if not already allocated
            if (!_stringLiterals.ContainsKey("{0}"))
            {
                _stringFmtLabel = AllocateString("{0}");
            }
            else
            {
                _stringFmtLabel = _stringLiterals["{0}"];
            }
            
            StringBuilder helper = new StringBuilder();
            helper.AppendLine($"{_printIntLabel}");
            helper.AppendLine("    ; Input: BCDE contains the 32-bit integer to print");
            helper.AppendLine("    ; Use a fixed temporary buffer for string conversion");
            helper.AppendLine("    PUSH A, M  ; Save registers");
            helper.AppendLine("    ; 1. Convert Integer to String");
            helper.AppendLine("    LD A, 0x03  ; IntToString function ID");
            helper.AppendLine("    ; BCDE is already loaded with the value");
            helper.AppendLine($"    LD FGH, {_stringFmtLabel}  ; Format string \"{{0}}\"");
            helper.AppendLine($"    LD IJK, {_printIntBufferLabel}  ; Static scratch buffer for string");
            helper.AppendLine("    INT 0x05, A  ; Convert");
            helper.AppendLine("    ; 2. Call DrawString (Interrupt 0x01)");
            helper.AppendLine("    LD A, 0x12  ; DrawString function ID");
            helper.AppendLine("    LD BCD, 0  ; Default font");
            helper.AppendLine($"    LD EFG, {_printIntBufferLabel}  ; Address of converted string");
            helper.AppendLine("    LD HI, 0  ; X");
            helper.AppendLine("    LD JK, 0  ; Y");
            helper.AppendLine("    LD L, 1  ; Color");
            helper.AppendLine("    LD M, 2  ; Video Page");
            helper.AppendLine("    INT 0x01, A  ; Execute Draw");
            helper.AppendLine("    POP A, M");
            helper.AppendLine("    RET");
            return helper.ToString();
        }

        /// <summary>
        /// Declares a variable in the variable section.
        /// </summary>
        private void DeclareVariable(SymbolInfo varInfo, string initialValue)
        {
            _variableSection.AppendLine($"{varInfo.Label}");
            
            if (varInfo.Type == VariableType.String && !string.IsNullOrEmpty(initialValue))
            {
                // String with initial value
                string escaped = initialValue.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
                _variableSection.AppendLine($"    #DB \"{escaped}\", 0");
            }
            else if (varInfo.Type == VariableType.String)
            {
                // String without initial value - allocate space
                _variableSection.AppendLine($"    #DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0  ; Reserve 16 bytes for string");
            }
            else
            {
                // Integer or float - 4 bytes initialized to 0
                _variableSection.AppendLine($"    #DB 0, 0, 0, 0");
            }
        }

        /// <summary>
        /// Determines the type of an expression.
        /// </summary>
        private VariableType GetExpressionType(ExpressionNode expr)
        {
            switch (expr)
            {
                case LiteralNode lit:
                    return lit.Type;
                case VariableNode var:
                    SymbolInfo varInfo = _symbolTable.GetVariable(var.Name);
                    if (varInfo != null)
                        return varInfo.Type;
                    // Infer from variable name suffix
                    if (var.Name.EndsWith("$"))
                        return VariableType.String;
                    if (var.Name.EndsWith("#"))
                        return VariableType.Float;
                    return VariableType.Integer;
                case FunctionCallNode func:
                    // String-returning functions
                    string funcName = func.FunctionName.ToUpper();
                    if (funcName == "LEFT" || funcName == "RIGHT" || funcName == "MID" || 
                        funcName == "CHR" || funcName == "STR" || funcName == "STRING" ||
                        funcName == "UCASE" || funcName == "LCASE" || funcName == "TRIM" ||
                        funcName == "LTRIM" || funcName == "RTRIM")
                        return VariableType.String;
                    // Integer-returning functions
                    if (funcName == "LEN" || funcName == "ASC" || funcName == "INSTR")
                        return VariableType.Integer;
                    // Numeric-returning functions
                    if (funcName == "VAL")
                        return VariableType.Float; // VAL can return float
                    return VariableType.Integer; // Default
                case BinaryExpressionNode bin:
                    // For binary expressions, check if it's string concatenation
                    if (bin.Operator == TokenType.PLUS)
                    {
                        VariableType leftType = GetExpressionType(bin.Left);
                        VariableType rightType = GetExpressionType(bin.Right);
                        if (leftType == VariableType.String || rightType == VariableType.String)
                            return VariableType.String;
                    }
                    // Default to integer for numeric operations
                    return VariableType.Integer;
                default:
                    return VariableType.Integer;
            }
        }

        /// <summary>
        /// Emits code for string concatenation.
        /// </summary>
        private string EmitStringConcatenation(ExpressionNode left, ExpressionNode right, string leftReg, string rightReg)
        {
            // Allocate temporary buffer for result
            string bufferLabel = $".strcat_buf_{_stringHelperCounter}";
            _stringHelperCounter++;
            
            // Add buffer to data section
            _dataSection.AppendLine($"{bufferLabel}");
            _dataSection.AppendLine("    #DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0  ; 32 bytes for concatenated string");
            
            // Mark that we need the string concatenation helper
            _stringHelpersEmitted["StrCat"] = true;
            
            // Call string concatenation helper
            // Parameters: leftReg (24-bit string address), rightReg (24-bit string address), bufferLabel (24-bit result address)
            _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
            _assembly.AppendLine($"    LD FGH, {leftReg}  ; First string address");
            _assembly.AppendLine($"    LD IJK, {rightReg}  ; Second string address");
            _assembly.AppendLine($"    LD LMN, {bufferLabel}  ; Result buffer address");
            _assembly.AppendLine($"    CALL .StrCat");
            _assembly.AppendLine($"    POP A, U  ; Restore registers");
            
            ReleaseRegister(leftReg, VariableType.String);
            ReleaseRegister(rightReg, VariableType.String);
            
            // Return the buffer label as a string address
            string resultReg = GetTempRegister(VariableType.String);
            _assembly.AppendLine($"    LD {resultReg}, {bufferLabel}");
            _currentAddress += 50; // Approximate size
            
            return resultReg;
        }

        /// <summary>
        /// Emits code for LEN function - returns string length.
        /// </summary>
        private string EmitLenFunction(FunctionCallNode func, string resultReg)
        {
            if (func.Arguments.Count != 1)
            {
                _errors.Add(new CompileError(func.Line, func.Column, "LEN requires 1 argument"));
                return resultReg;
            }
            
            string strReg = EmitExpression(func.Arguments[0], VariableType.String);
            
            // Mark that we need the string length helper
            _stringHelpersEmitted["StrLen"] = true;
            
            // Call string length helper
            _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
            _assembly.AppendLine($"    LD FGH, {strReg}  ; String address");
            _assembly.AppendLine($"    CALL .StrLen");
            _assembly.AppendLine($"    LD {resultReg}, A  ; Length result");
            _assembly.AppendLine($"    POP A, U  ; Restore registers");
            
            ReleaseRegister(strReg, VariableType.String);
            _currentAddress += 30;
            
            return resultReg;
        }

        /// <summary>
        /// Emits code for LEFT$ function - returns leftmost n characters.
        /// </summary>
        private string EmitLeftFunction(FunctionCallNode func, string resultReg)
        {
            if (func.Arguments.Count != 2)
            {
                _errors.Add(new CompileError(func.Line, func.Column, "LEFT$ requires 2 arguments"));
                return resultReg;
            }
            
            string strReg = EmitExpression(func.Arguments[0], VariableType.String);
            string nReg = EmitExpression(func.Arguments[1], VariableType.Integer);
            
            // Allocate temporary buffer
            string bufferLabel = $".strleft_buf_{_stringHelperCounter}";
            _stringHelperCounter++;
            _dataSection.AppendLine($"{bufferLabel}");
            _dataSection.AppendLine("    #DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0  ; 32 bytes");
            
            _stringHelpersEmitted["StrLeft"] = true;
            
            _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
            _assembly.AppendLine($"    LD FGH, {strReg}  ; String address");
            _assembly.AppendLine($"    LD BCDE, {nReg}  ; Number of characters");
            _assembly.AppendLine($"    LD IJK, {bufferLabel}  ; Result buffer");
            _assembly.AppendLine($"    CALL .StrLeft");
            _assembly.AppendLine($"    POP A, U  ; Restore registers");
            
            ReleaseRegister(strReg, VariableType.String);
            ReleaseRegister(nReg, VariableType.Integer);
            
            string resultStrReg = GetTempRegister(VariableType.String);
            _assembly.AppendLine($"    LD {resultStrReg}, {bufferLabel}");
            _currentAddress += 40;
            
            return resultStrReg;
        }

        /// <summary>
        /// Emits code for RIGHT$ function - returns rightmost n characters.
        /// </summary>
        private string EmitRightFunction(FunctionCallNode func, string resultReg)
        {
            if (func.Arguments.Count != 2)
            {
                _errors.Add(new CompileError(func.Line, func.Column, "RIGHT$ requires 2 arguments"));
                return resultReg;
            }
            
            string strReg = EmitExpression(func.Arguments[0], VariableType.String);
            string nReg = EmitExpression(func.Arguments[1], VariableType.Integer);
            
            string bufferLabel = $".strright_buf_{_stringHelperCounter}";
            _stringHelperCounter++;
            _dataSection.AppendLine($"{bufferLabel}");
            _dataSection.AppendLine("    #DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0  ; 32 bytes");
            
            _stringHelpersEmitted["StrRight"] = true;
            
            _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
            _assembly.AppendLine($"    LD FGH, {strReg}  ; String address");
            _assembly.AppendLine($"    LD BCDE, {nReg}  ; Number of characters");
            _assembly.AppendLine($"    LD IJK, {bufferLabel}  ; Result buffer");
            _assembly.AppendLine($"    CALL .StrRight");
            _assembly.AppendLine($"    POP A, U  ; Restore registers");
            
            ReleaseRegister(strReg, VariableType.String);
            ReleaseRegister(nReg, VariableType.Integer);
            
            string resultStrReg = GetTempRegister(VariableType.String);
            _assembly.AppendLine($"    LD {resultStrReg}, {bufferLabel}");
            _currentAddress += 40;
            
            return resultStrReg;
        }

        /// <summary>
        /// Emits code for MID$ function - returns substring.
        /// </summary>
        private string EmitMidFunction(FunctionCallNode func, string resultReg)
        {
            if (func.Arguments.Count < 2 || func.Arguments.Count > 3)
            {
                _errors.Add(new CompileError(func.Line, func.Column, "MID$ requires 2 or 3 arguments"));
                return resultReg;
            }
            
            string strReg = EmitExpression(func.Arguments[0], VariableType.String);
            string startReg = EmitExpression(func.Arguments[1], VariableType.Integer);
            string lengthReg = null;
            if (func.Arguments.Count == 3)
            {
                lengthReg = EmitExpression(func.Arguments[2], VariableType.Integer);
            }
            
            string bufferLabel = $".strmid_buf_{_stringHelperCounter}";
            _stringHelperCounter++;
            _dataSection.AppendLine($"{bufferLabel}");
            _dataSection.AppendLine("    #DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0  ; 32 bytes");
            
            _stringHelpersEmitted["StrMid"] = true;
            
            _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
            _assembly.AppendLine($"    LD FGH, {strReg}  ; String address");
            _assembly.AppendLine($"    LD BCDE, {startReg}  ; Start position (1-based)");
            if (lengthReg != null)
            {
                _assembly.AppendLine($"    LD EFGH, {lengthReg}  ; Length");
            }
            else
            {
                _assembly.AppendLine($"    LD EFGH, 0xFFFFFFFF  ; Length = -1 (to end)");
            }
            _assembly.AppendLine($"    LD IJK, {bufferLabel}  ; Result buffer");
            _assembly.AppendLine($"    CALL .StrMid");
            _assembly.AppendLine($"    POP A, U  ; Restore registers");
            
            ReleaseRegister(strReg, VariableType.String);
            ReleaseRegister(startReg, VariableType.Integer);
            if (lengthReg != null)
                ReleaseRegister(lengthReg, VariableType.Integer);
            
            string resultStrReg = GetTempRegister(VariableType.String);
            _assembly.AppendLine($"    LD {resultStrReg}, {bufferLabel}");
            _currentAddress += 45;
            
            return resultStrReg;
        }

        /// <summary>
        /// Emits code for CHR$ function - converts ASCII code to character.
        /// </summary>
        private string EmitChrFunction(FunctionCallNode func, string resultReg)
        {
            if (func.Arguments.Count != 1)
            {
                _errors.Add(new CompileError(func.Line, func.Column, "CHR$ requires 1 argument"));
                return resultReg;
            }
            
            string codeReg = EmitExpression(func.Arguments[0], VariableType.Integer);
            
            string bufferLabel = $".strchr_buf_{_stringHelperCounter}";
            _stringHelperCounter++;
            _dataSection.AppendLine($"{bufferLabel}");
            _dataSection.AppendLine("    #DB 0, 0  ; 2 bytes for single character + null");
            
            _stringHelpersEmitted["StrChr"] = true;
            
            _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
            _assembly.AppendLine($"    LD BCDE, {codeReg}  ; ASCII code");
            _assembly.AppendLine($"    LD FGH, {bufferLabel}  ; Result buffer");
            _assembly.AppendLine($"    CALL .StrChr");
            _assembly.AppendLine($"    POP A, U  ; Restore registers");
            
            ReleaseRegister(codeReg, VariableType.Integer);
            
            string resultStrReg = GetTempRegister(VariableType.String);
            _assembly.AppendLine($"    LD {resultStrReg}, {bufferLabel}");
            _currentAddress += 35;
            
            return resultStrReg;
        }

        /// <summary>
        /// Emits code for ASC function - returns ASCII code of first character.
        /// </summary>
        private string EmitAscFunction(FunctionCallNode func, string resultReg)
        {
            if (func.Arguments.Count != 1)
            {
                _errors.Add(new CompileError(func.Line, func.Column, "ASC requires 1 argument"));
                return resultReg;
            }
            
            string strReg = EmitExpression(func.Arguments[0], VariableType.String);
            
            _stringHelpersEmitted["StrAsc"] = true;
            
            _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
            _assembly.AppendLine($"    LD FGH, {strReg}  ; String address");
            _assembly.AppendLine($"    CALL .StrAsc");
            _assembly.AppendLine($"    LD {resultReg}, A  ; ASCII code result");
            _assembly.AppendLine($"    POP A, U  ; Restore registers");
            
            ReleaseRegister(strReg, VariableType.String);
            _currentAddress += 30;
            
            return resultReg;
        }

        /// <summary>
        /// Emits code for VAL function - converts string to number.
        /// </summary>
        private string EmitValFunction(FunctionCallNode func, string resultReg)
        {
            if (func.Arguments.Count != 1)
            {
                _errors.Add(new CompileError(func.Line, func.Column, "VAL requires 1 argument"));
                return resultReg;
            }
            
            string strReg = EmitExpression(func.Arguments[0], VariableType.String);
            
            _stringHelpersEmitted["StrVal"] = true;
            
            _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
            _assembly.AppendLine($"    LD FGH, {strReg}  ; String address");
            _assembly.AppendLine($"    CALL .StrVal");
            _assembly.AppendLine($"    LD {resultReg}, BCDE  ; Numeric result (32-bit)");
            _assembly.AppendLine($"    POP A, U  ; Restore registers");
            
            ReleaseRegister(strReg, VariableType.String);
            _currentAddress += 30;
            
            return resultReg;
        }

        /// <summary>
        /// Emits code for STR$ function - converts number to string.
        /// </summary>
        private string EmitStrFunction(FunctionCallNode func, string resultReg)
        {
            if (func.Arguments.Count != 1)
            {
                _errors.Add(new CompileError(func.Line, func.Column, "STR$ requires 1 argument"));
                return resultReg;
            }
            
            // Determine if argument is integer or float
            VariableType argType = GetExpressionType(func.Arguments[0]);
            string numReg = EmitExpression(func.Arguments[0], argType);
            
            string bufferLabel = $".strstr_buf_{_stringHelperCounter}";
            _stringHelperCounter++;
            _dataSection.AppendLine($"{bufferLabel}");
            _dataSection.AppendLine("    #DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0  ; 12 bytes");
            
            // Use existing IntToString or FloatToString interrupt
            string formatLabel = AllocateString("{0}");
            
            if (argType == VariableType.Float)
            {
                _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
                _assembly.AppendLine($"    LD A, 0x01  ; FloatToString function");
                _assembly.AppendLine($"    LD B, 0  ; Float register index (F0)");
                _assembly.AppendLine($"    LD F0, {numReg}  ; Load float value");
                _assembly.AppendLine($"    LD FGH, {formatLabel}  ; Format string");
                _assembly.AppendLine($"    LD IJK, {bufferLabel}  ; Target buffer");
                _assembly.AppendLine($"    INT 0x05, A  ; Convert");
                _assembly.AppendLine($"    POP A, U  ; Restore registers");
            }
            else
            {
                _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
                _assembly.AppendLine($"    LD A, 0x03  ; IntToString function");
                _assembly.AppendLine($"    LD BCDE, {numReg}  ; Integer value");
                _assembly.AppendLine($"    LD FGH, {formatLabel}  ; Format string");
                _assembly.AppendLine($"    LD IJK, {bufferLabel}  ; Target buffer");
                _assembly.AppendLine($"    INT 0x05, A  ; Convert");
                _assembly.AppendLine($"    POP A, U  ; Restore registers");
            }
            
            ReleaseRegister(numReg, argType);
            
            string resultStrReg = GetTempRegister(VariableType.String);
            _assembly.AppendLine($"    LD {resultStrReg}, {bufferLabel}");
            _currentAddress += 40;
            
            return resultStrReg;
        }

        /// <summary>
        /// Emits code for STRING$ function - repeats character n times.
        /// </summary>
        private string EmitStringFunction(FunctionCallNode func, string resultReg)
        {
            if (func.Arguments.Count != 2)
            {
                _errors.Add(new CompileError(func.Line, func.Column, "STRING$ requires 2 arguments"));
                return resultReg;
            }
            
            string nReg = EmitExpression(func.Arguments[0], VariableType.Integer);
            string charReg = EmitExpression(func.Arguments[1], VariableType.String);
            
            string bufferLabel = $".strstring_buf_{_stringHelperCounter}";
            _stringHelperCounter++;
            _dataSection.AppendLine($"{bufferLabel}");
            _dataSection.AppendLine("    #DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0  ; 32 bytes");
            
            _stringHelpersEmitted["StrString"] = true;
            
            _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
            _assembly.AppendLine($"    LD BCDE, {nReg}  ; Number of repetitions");
            _assembly.AppendLine($"    LD FGH, {charReg}  ; Character string address");
            _assembly.AppendLine($"    LD IJK, {bufferLabel}  ; Result buffer");
            _assembly.AppendLine($"    CALL .StrString");
            _assembly.AppendLine($"    POP A, U  ; Restore registers");
            
            ReleaseRegister(nReg, VariableType.Integer);
            ReleaseRegister(charReg, VariableType.String);
            
            string resultStrReg = GetTempRegister(VariableType.String);
            _assembly.AppendLine($"    LD {resultStrReg}, {bufferLabel}");
            _currentAddress += 40;
            
            return resultStrReg;
        }

        /// <summary>
        /// Emits code for INSTR function - finds substring position.
        /// </summary>
        private string EmitInstrFunction(FunctionCallNode func, string resultReg)
        {
            if (func.Arguments.Count < 2 || func.Arguments.Count > 3)
            {
                _errors.Add(new CompileError(func.Line, func.Column, "INSTR requires 2 or 3 arguments"));
                return resultReg;
            }
            
            string startReg = null;
            string str1Reg, str2Reg;
            
            if (func.Arguments.Count == 3)
            {
                startReg = EmitExpression(func.Arguments[0], VariableType.Integer);
                str1Reg = EmitExpression(func.Arguments[1], VariableType.String);
                str2Reg = EmitExpression(func.Arguments[2], VariableType.String);
            }
            else
            {
                str1Reg = EmitExpression(func.Arguments[0], VariableType.String);
                str2Reg = EmitExpression(func.Arguments[1], VariableType.String);
            }
            
            _stringHelpersEmitted["StrInstr"] = true;
            
            _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
            if (startReg != null)
            {
                _assembly.AppendLine($"    LD BCDE, {startReg}  ; Start position");
            }
            else
            {
                _assembly.AppendLine($"    LD BCDE, 1  ; Start position (default)");
            }
            _assembly.AppendLine($"    LD FGH, {str1Reg}  ; First string address");
            _assembly.AppendLine($"    LD IJK, {str2Reg}  ; Second string address");
            _assembly.AppendLine($"    CALL .StrInstr");
            _assembly.AppendLine($"    LD {resultReg}, BCDE  ; Position result (0 if not found)");
            _assembly.AppendLine($"    POP A, U  ; Restore registers");
            
            if (startReg != null)
                ReleaseRegister(startReg, VariableType.Integer);
            ReleaseRegister(str1Reg, VariableType.String);
            ReleaseRegister(str2Reg, VariableType.String);
            _currentAddress += 40;
            
            return resultReg;
        }

        /// <summary>
        /// Emits code for UCASE$ function - converts to uppercase.
        /// </summary>
        private string EmitUcaseFunction(FunctionCallNode func, string resultReg)
        {
            if (func.Arguments.Count != 1)
            {
                _errors.Add(new CompileError(func.Line, func.Column, "UCASE$ requires 1 argument"));
                return resultReg;
            }
            
            string strReg = EmitExpression(func.Arguments[0], VariableType.String);
            
            string bufferLabel = $".strucase_buf_{_stringHelperCounter}";
            _stringHelperCounter++;
            _dataSection.AppendLine($"{bufferLabel}");
            _dataSection.AppendLine("    #DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0  ; 32 bytes");
            
            _stringHelpersEmitted["StrUcase"] = true;
            
            _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
            _assembly.AppendLine($"    LD FGH, {strReg}  ; String address");
            _assembly.AppendLine($"    LD IJK, {bufferLabel}  ; Result buffer");
            _assembly.AppendLine($"    CALL .StrUcase");
            _assembly.AppendLine($"    POP A, U  ; Restore registers");
            
            ReleaseRegister(strReg, VariableType.String);
            
            string resultStrReg = GetTempRegister(VariableType.String);
            _assembly.AppendLine($"    LD {resultStrReg}, {bufferLabel}");
            _currentAddress += 35;
            
            return resultStrReg;
        }

        /// <summary>
        /// Emits code for LCASE$ function - converts to lowercase.
        /// </summary>
        private string EmitLcaseFunction(FunctionCallNode func, string resultReg)
        {
            if (func.Arguments.Count != 1)
            {
                _errors.Add(new CompileError(func.Line, func.Column, "LCASE$ requires 1 argument"));
                return resultReg;
            }
            
            string strReg = EmitExpression(func.Arguments[0], VariableType.String);
            
            string bufferLabel = $".strlcase_buf_{_stringHelperCounter}";
            _stringHelperCounter++;
            _dataSection.AppendLine($"{bufferLabel}");
            _dataSection.AppendLine("    #DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0  ; 32 bytes");
            
            _stringHelpersEmitted["StrLcase"] = true;
            
            _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
            _assembly.AppendLine($"    LD FGH, {strReg}  ; String address");
            _assembly.AppendLine($"    LD IJK, {bufferLabel}  ; Result buffer");
            _assembly.AppendLine($"    CALL .StrLcase");
            _assembly.AppendLine($"    POP A, U  ; Restore registers");
            
            ReleaseRegister(strReg, VariableType.String);
            
            string resultStrReg = GetTempRegister(VariableType.String);
            _assembly.AppendLine($"    LD {resultStrReg}, {bufferLabel}");
            _currentAddress += 35;
            
            return resultStrReg;
        }

        /// <summary>
        /// Emits code for TRIM$ function - removes leading and trailing whitespace.
        /// </summary>
        private string EmitTrimFunction(FunctionCallNode func, string resultReg)
        {
            if (func.Arguments.Count != 1)
            {
                _errors.Add(new CompileError(func.Line, func.Column, "TRIM$ requires 1 argument"));
                return resultReg;
            }
            
            string strReg = EmitExpression(func.Arguments[0], VariableType.String);
            
            string bufferLabel = $".strtrim_buf_{_stringHelperCounter}";
            _stringHelperCounter++;
            _dataSection.AppendLine($"{bufferLabel}");
            _dataSection.AppendLine("    #DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0  ; 32 bytes");
            
            _stringHelpersEmitted["StrTrim"] = true;
            
            _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
            _assembly.AppendLine($"    LD FGH, {strReg}  ; String address");
            _assembly.AppendLine($"    LD IJK, {bufferLabel}  ; Result buffer");
            _assembly.AppendLine($"    CALL .StrTrim");
            _assembly.AppendLine($"    POP A, U  ; Restore registers");
            
            ReleaseRegister(strReg, VariableType.String);
            
            string resultStrReg = GetTempRegister(VariableType.String);
            _assembly.AppendLine($"    LD {resultStrReg}, {bufferLabel}");
            _currentAddress += 35;
            
            return resultStrReg;
        }

        /// <summary>
        /// Emits code for LTRIM$ function - removes leading whitespace.
        /// </summary>
        private string EmitLtrimFunction(FunctionCallNode func, string resultReg)
        {
            if (func.Arguments.Count != 1)
            {
                _errors.Add(new CompileError(func.Line, func.Column, "LTRIM$ requires 1 argument"));
                return resultReg;
            }
            
            string strReg = EmitExpression(func.Arguments[0], VariableType.String);
            
            string bufferLabel = $".strltrim_buf_{_stringHelperCounter}";
            _stringHelperCounter++;
            _dataSection.AppendLine($"{bufferLabel}");
            _dataSection.AppendLine("    #DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0  ; 32 bytes");
            
            _stringHelpersEmitted["StrLtrim"] = true;
            
            _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
            _assembly.AppendLine($"    LD FGH, {strReg}  ; String address");
            _assembly.AppendLine($"    LD IJK, {bufferLabel}  ; Result buffer");
            _assembly.AppendLine($"    CALL .StrLtrim");
            _assembly.AppendLine($"    POP A, U  ; Restore registers");
            
            ReleaseRegister(strReg, VariableType.String);
            
            string resultStrReg = GetTempRegister(VariableType.String);
            _assembly.AppendLine($"    LD {resultStrReg}, {bufferLabel}");
            _currentAddress += 35;
            
            return resultStrReg;
        }

        /// <summary>
        /// Emits code for RTRIM$ function - removes trailing whitespace.
        /// </summary>
        private string EmitRtrimFunction(FunctionCallNode func, string resultReg)
        {
            if (func.Arguments.Count != 1)
            {
                _errors.Add(new CompileError(func.Line, func.Column, "RTRIM$ requires 1 argument"));
                return resultReg;
            }
            
            string strReg = EmitExpression(func.Arguments[0], VariableType.String);
            
            string bufferLabel = $".strrtrim_buf_{_stringHelperCounter}";
            _stringHelperCounter++;
            _dataSection.AppendLine($"{bufferLabel}");
            _dataSection.AppendLine("    #DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0  ; 32 bytes");
            
            _stringHelpersEmitted["StrRtrim"] = true;
            
            _assembly.AppendLine($"    PUSH A, U  ; Preserve registers");
            _assembly.AppendLine($"    LD FGH, {strReg}  ; String address");
            _assembly.AppendLine($"    LD IJK, {bufferLabel}  ; Result buffer");
            _assembly.AppendLine($"    CALL .StrRtrim");
            _assembly.AppendLine($"    POP A, U  ; Restore registers");
            
            ReleaseRegister(strReg, VariableType.String);
            
            string resultStrReg = GetTempRegister(VariableType.String);
            _assembly.AppendLine($"    LD {resultStrReg}, {bufferLabel}");
            _currentAddress += 35;
            
            return resultStrReg;
        }

        /// <summary>
        /// Emits a string helper subroutine.
        /// </summary>
        private string EmitStringHelper(string helperName)
        {
            StringBuilder helper = new StringBuilder();
            
            switch (helperName)
            {
                case "StrLen":
                    helper.AppendLine(".StrLen");
                    helper.AppendLine("    ; Input: FGH = string address (24-bit)");
                    helper.AppendLine("    ; Output: A = string length (8-bit)");
                    helper.AppendLine("    PUSH B, U  ; Preserve registers");
                    helper.AppendLine("    LD A, 0  ; Initialize length counter");
                    helper.AppendLine("    LD BCD, FGH  ; Copy string address");
                    helper.AppendLine(".StrLen_loop");
                    helper.AppendLine("    LD B, (BCD)  ; Load byte from string");
                    helper.AppendLine("    CP B, 0  ; Check for null terminator");
                    helper.AppendLine("    JR Z, .StrLen_done");
                    helper.AppendLine("    INC A  ; Increment length");
                    helper.AppendLine("    INC BCD  ; Move to next byte");
                    helper.AppendLine("    JP .StrLen_loop");
                    helper.AppendLine(".StrLen_done");
                    helper.AppendLine("    POP B, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    break;
                    
                case "StrCat":
                    helper.AppendLine(".StrCat");
                    helper.AppendLine("    ; Input: FGH = first string address, IJK = second string address, LMN = result buffer");
                    helper.AppendLine("    ; Concatenates two strings into result buffer");
                    helper.AppendLine("    PUSH A, U  ; Preserve registers");
                    helper.AppendLine("    ; Copy first string");
                    helper.AppendLine("    LD BCD, FGH  ; Source");
                    helper.AppendLine("    LD EFG, LMN  ; Destination");
                    helper.AppendLine(".StrCat_copy1");
                    helper.AppendLine("    LD A, (BCD)  ; Load byte");
                    helper.AppendLine("    LD (EFG), A  ; Store byte");
                    helper.AppendLine("    CP A, 0  ; Check for null");
                    helper.AppendLine("    JR Z, .StrCat_copy2");
                    helper.AppendLine("    INC BCD  ; Next source byte");
                    helper.AppendLine("    INC EFG  ; Next dest byte");
                    helper.AppendLine("    JP .StrCat_copy1");
                    helper.AppendLine(".StrCat_copy2");
                    helper.AppendLine("    ; Copy second string (EFG already points to end of first string)");
                    helper.AppendLine("    LD BCD, IJK  ; Source");
                    helper.AppendLine(".StrCat_copy2_loop");
                    helper.AppendLine("    LD A, (BCD)  ; Load byte");
                    helper.AppendLine("    LD (EFG), A  ; Store byte");
                    helper.AppendLine("    CP A, 0  ; Check for null");
                    helper.AppendLine("    JR Z, .StrCat_done");
                    helper.AppendLine("    INC BCD  ; Next source byte");
                    helper.AppendLine("    INC EFG  ; Next dest byte");
                    helper.AppendLine("    JP .StrCat_copy2_loop");
                    helper.AppendLine(".StrCat_done");
                    helper.AppendLine("    POP A, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    break;
                    
                case "StrLeft":
                    helper.AppendLine(".StrLeft");
                    helper.AppendLine("    ; Input: FGH = string address, BCDE = number of characters, IJK = result buffer");
                    helper.AppendLine("    PUSH A, U  ; Preserve registers");
                    helper.AppendLine("    LD BCD, FGH  ; Source string");
                    helper.AppendLine("    LD EFG, IJK  ; Destination buffer");
                    helper.AppendLine("    LD HI, 0  ; Character counter");
                    helper.AppendLine(".StrLeft_loop");
                    helper.AppendLine("    CP HI, CD  ; Compare counter with count");
                    helper.AppendLine("    JR GTE, .StrLeft_done");
                    helper.AppendLine("    LD A, (BCD)  ; Load character");
                    helper.AppendLine("    CP A, 0  ; Check for null");
                    helper.AppendLine("    JR Z, .StrLeft_done");
                    helper.AppendLine("    LD (EFG), A  ; Store character");
                    helper.AppendLine("    INC BCD  ; Next source");
                    helper.AppendLine("    INC EFG  ; Next dest");
                    helper.AppendLine("    INC HI  ; Increment counter");
                    helper.AppendLine("    JP .StrLeft_loop");
                    helper.AppendLine(".StrLeft_done");
                    helper.AppendLine("    LD A, 0  ; Null terminator");
                    helper.AppendLine("    LD (EFG), A");
                    helper.AppendLine("    POP A, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    break;
                    
                case "StrRight":
                    helper.AppendLine(".StrRight");
                    helper.AppendLine("    ; Input: FGH = string address, BCDE = number of characters, IJK = result buffer");
                    helper.AppendLine("    PUSH A, U  ; Preserve registers");
                    helper.AppendLine("    ; First, find string length");
                    helper.AppendLine("    LD BCD, FGH  ; String address");
                    helper.AppendLine("    LD HI, 0  ; Length counter");
                    helper.AppendLine(".StrRight_len");
                    helper.AppendLine("    LD A, (BCD)  ; Load byte");
                    helper.AppendLine("    CP A, 0  ; Check for null");
                    helper.AppendLine("    JR Z, .StrRight_start");
                    helper.AppendLine("    INC HI  ; Increment length");
                    helper.AppendLine("    INC BCD  ; Next byte");
                    helper.AppendLine("    JP .StrRight_len");
                    helper.AppendLine(".StrRight_start");
                    helper.AppendLine("    ; Calculate start position: length - count");
                    helper.AppendLine("    LD JK, HI  ; Length");
                    helper.AppendLine("    SUB JK, CD  ; Subtract count");
                    helper.AppendLine("    ; If result < 0, start at 0");
                    helper.AppendLine("    CP JK, 0");
                    helper.AppendLine("    JR LT, .StrRight_start0");
                    helper.AppendLine("    JP .StrRight_copy");
                    helper.AppendLine(".StrRight_start0");
                    helper.AppendLine("    LD JK, 0  ; Start at beginning");
                    helper.AppendLine(".StrRight_copy");
                    helper.AppendLine("    ; Copy from position JK to end");
                    helper.AppendLine("    LD BCD, FGH  ; Source string");
                    helper.AppendLine("    ADD BCD, JK  ; Add offset");
                    helper.AppendLine("    LD EFG, IJK  ; Destination");
                    helper.AppendLine(".StrRight_loop");
                    helper.AppendLine("    LD A, (BCD)  ; Load character");
                    helper.AppendLine("    LD (EFG), A  ; Store character");
                    helper.AppendLine("    CP A, 0  ; Check for null");
                    helper.AppendLine("    JR Z, .StrRight_done");
                    helper.AppendLine("    INC BCD  ; Next source");
                    helper.AppendLine("    INC EFG  ; Next dest");
                    helper.AppendLine("    JP .StrRight_loop");
                    helper.AppendLine(".StrRight_done");
                    helper.AppendLine("    POP A, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    break;
                    
                case "StrMid":
                    helper.AppendLine(".StrMid");
                    helper.AppendLine("    ; Input: FGH = string address, BCDE = start position (1-based), EFGH = length (-1 for to end), IJK = result buffer");
                    helper.AppendLine("    PUSH A, U  ; Preserve registers");
                    helper.AppendLine("    ; Skip to start position (convert 1-based to 0-based)");
                    helper.AppendLine("    LD BCD, FGH  ; Source string");
                    helper.AppendLine("    SUB BCDE, 1  ; Convert to 0-based");
                    helper.AppendLine("    ADD BCD, BCDE  ; Add offset");
                    helper.AppendLine("    LD EFG, IJK  ; Destination");
                    helper.AppendLine("    LD HI, 0  ; Character counter");
                    helper.AppendLine(".StrMid_loop");
                    helper.AppendLine("    ; Check if we've reached the length limit");
                    helper.AppendLine("    CP EFGH, 0xFFFFFFFF  ; Check if length is -1");
                    helper.AppendLine("    JR NE, .StrMid_checklen");
                    helper.AppendLine("    JP .StrMid_copy");
                    helper.AppendLine(".StrMid_checklen");
                    helper.AppendLine("    CP HI, GH  ; Compare counter with length");
                    helper.AppendLine("    JR GTE, .StrMid_done");
                    helper.AppendLine(".StrMid_copy");
                    helper.AppendLine("    LD A, (BCD)  ; Load character");
                    helper.AppendLine("    CP A, 0  ; Check for null");
                    helper.AppendLine("    JR Z, .StrMid_done");
                    helper.AppendLine("    LD (EFG), A  ; Store character");
                    helper.AppendLine("    INC BCD  ; Next source");
                    helper.AppendLine("    INC EFG  ; Next dest");
                    helper.AppendLine("    INC HI  ; Increment counter");
                    helper.AppendLine("    JP .StrMid_loop");
                    helper.AppendLine(".StrMid_done");
                    helper.AppendLine("    LD A, 0  ; Null terminator");
                    helper.AppendLine("    LD (EFG), A");
                    helper.AppendLine("    POP A, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    break;
                    
                case "StrChr":
                    helper.AppendLine(".StrChr");
                    helper.AppendLine("    ; Input: BCDE = ASCII code, FGH = result buffer");
                    helper.AppendLine("    PUSH A, U  ; Preserve registers");
                    helper.AppendLine("    LD A, CD  ; Get ASCII code (8-bit)");
                    helper.AppendLine("    LD (FGH), A  ; Store character");
                    helper.AppendLine("    LD A, 0  ; Null terminator");
                    helper.AppendLine("    INC FGH");
                    helper.AppendLine("    LD (FGH), A");
                    helper.AppendLine("    POP A, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    break;
                    
                case "StrAsc":
                    helper.AppendLine(".StrAsc");
                    helper.AppendLine("    ; Input: FGH = string address");
                    helper.AppendLine("    ; Output: A = ASCII code of first character (0 if empty)");
                    helper.AppendLine("    PUSH B, U  ; Preserve registers");
                    helper.AppendLine("    LD BCD, FGH  ; String address");
                    helper.AppendLine("    LD A, (BCD)  ; Load first character");
                    helper.AppendLine("    POP B, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    break;
                    
                case "StrVal":
                    helper.AppendLine(".StrVal");
                    helper.AppendLine("    ; Input: FGH = string address");
                    helper.AppendLine("    ; Output: BCDE = numeric value (32-bit)");
                    helper.AppendLine("    ; Note: This is a simplified implementation. Full VAL would parse floats too.");
                    helper.AppendLine("    PUSH A, U  ; Preserve registers");
                    helper.AppendLine("    LD BCD, FGH  ; String address");
                    helper.AppendLine("    LD BCDE, 0  ; Initialize result");
                    helper.AppendLine("    LD A, 1  ; Sign (1 = positive, -1 = negative)");
                    helper.AppendLine("    ; Check for negative sign");
                    helper.AppendLine("    LD HI, (BCD)  ; Load first character");
                    helper.AppendLine("    CP HI, 0x2D  ; Check for '-'");
                    helper.AppendLine("    JR NE, .StrVal_parse");
                    helper.AppendLine("    LD A, -1  ; Negative");
                    helper.AppendLine("    INC BCD  ; Skip '-'");
                    helper.AppendLine(".StrVal_parse");
                    helper.AppendLine("    LD JK, (BCD)  ; Load character");
                    helper.AppendLine("    CP JK, 0  ; Check for null");
                    helper.AppendLine("    JR Z, .StrVal_done");
                    helper.AppendLine("    ; Check if digit (0x30-0x39)");
                    helper.AppendLine("    CP JK, 0x30  ; '0'");
                    helper.AppendLine("    JR LT, .StrVal_done");
                    helper.AppendLine("    CP JK, 0x3A  ; '9' + 1");
                    helper.AppendLine("    JR GTE, .StrVal_done");
                    helper.AppendLine("    ; Convert digit to number");
                    helper.AppendLine("    SUB JK, 0x30  ; Subtract '0'");
                    helper.AppendLine("    MUL BCDE, 10  ; Multiply result by 10");
                    helper.AppendLine("    ADD BCDE, JK  ; Add digit");
                    helper.AppendLine("    INC BCD  ; Next character");
                    helper.AppendLine("    JP .StrVal_parse");
                    helper.AppendLine(".StrVal_done");
                    helper.AppendLine("    ; Apply sign");
                    helper.AppendLine("    CP A, -1");
                    helper.AppendLine("    JR NE, .StrVal_ret");
                    helper.AppendLine("    NEG BCDE  ; Negate result");
                    helper.AppendLine(".StrVal_ret");
                    helper.AppendLine("    POP A, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    break;
                    
                case "StrString":
                    helper.AppendLine(".StrString");
                    helper.AppendLine("    ; Input: BCDE = number of repetitions, FGH = character string address, IJK = result buffer");
                    helper.AppendLine("    PUSH A, U  ; Preserve registers");
                    helper.AppendLine("    ; Get first character from string");
                    helper.AppendLine("    LD BCD, FGH  ; Character string");
                    helper.AppendLine("    LD A, (BCD)  ; Load first character");
                    helper.AppendLine("    LD EFG, IJK  ; Result buffer");
                    helper.AppendLine("    LD HI, 0  ; Counter");
                    helper.AppendLine(".StrString_loop");
                    helper.AppendLine("    CP HI, CD  ; Compare counter with count");
                    helper.AppendLine("    JR GTE, .StrString_done");
                    helper.AppendLine("    LD (EFG), A  ; Store character");
                    helper.AppendLine("    INC EFG  ; Next position");
                    helper.AppendLine("    INC HI  ; Increment counter");
                    helper.AppendLine("    JP .StrString_loop");
                    helper.AppendLine(".StrString_done");
                    helper.AppendLine("    LD A, 0  ; Null terminator");
                    helper.AppendLine("    LD (EFG), A");
                    helper.AppendLine("    POP A, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    break;
                    
                case "StrInstr":
                    helper.AppendLine(".StrInstr");
                    helper.AppendLine("    ; Input: BCDE = start position (1-based), FGH = first string, IJK = second string");
                    helper.AppendLine("    ; Output: BCDE = position (1-based, 0 if not found)");
                    helper.AppendLine("    PUSH A, U  ; Preserve registers");
                    helper.AppendLine("    ; Convert start to 0-based and skip to position");
                    helper.AppendLine("    LD BCD, FGH  ; First string");
                    helper.AppendLine("    SUB BCDE, 1  ; Convert to 0-based");
                    helper.AppendLine("    ADD BCD, BCDE  ; Add offset");
                    helper.AppendLine("    LD EFG, IJK  ; Second string (search string)");
                    helper.AppendLine("    LD HI, (EFG)  ; Load first character of search string");
                    helper.AppendLine("    CP HI, 0  ; Check if search string is empty");
                    helper.AppendLine("    JR Z, .StrInstr_notfound");
                    helper.AppendLine("    LD JK, BCDE  ; Save start position for result");
                    helper.AppendLine(".StrInstr_outer");
                    helper.AppendLine("    LD A, (BCD)  ; Load character from first string");
                    helper.AppendLine("    CP A, 0  ; Check for end of string");
                    helper.AppendLine("    JR Z, .StrInstr_notfound");
                    helper.AppendLine("    CP A, HI  ; Compare with first char of search");
                    helper.AppendLine("    JR NE, .StrInstr_next");
                    helper.AppendLine("    ; Potential match - check rest of search string");
                    helper.AppendLine("    LD LMN, BCD  ; Save position in first string");
                    helper.AppendLine("    LD OPQ, EFG  ; Save position in search string");
                    helper.AppendLine("    INC LMN  ; Next char in first string");
                    helper.AppendLine("    INC OPQ  ; Next char in search string");
                    helper.AppendLine(".StrInstr_match");
                    helper.AppendLine("    LD A, (OPQ)  ; Load from search string");
                    helper.AppendLine("    CP A, 0  ; Check for end of search");
                    helper.AppendLine("    JR Z, .StrInstr_found");
                    helper.AppendLine("    LD R, (LMN)  ; Load from first string");
                    helper.AppendLine("    CP R, 0  ; Check for end of first string");
                    helper.AppendLine("    JR Z, .StrInstr_notfound");
                    helper.AppendLine("    CP A, R  ; Compare characters");
                    helper.AppendLine("    JR NE, .StrInstr_next");
                    helper.AppendLine("    INC LMN  ; Next in first");
                    helper.AppendLine("    INC OPQ  ; Next in search");
                    helper.AppendLine("    JP .StrInstr_match");
                    helper.AppendLine(".StrInstr_found");
                    helper.AppendLine("    ; Found! Calculate 1-based position");
                    helper.AppendLine("    SUB BCD, FGH  ; Calculate offset");
                    helper.AppendLine("    ADD BCDE, 1  ; Convert to 1-based");
                    helper.AppendLine("    LD BCDE, BCD  ; Result");
                    helper.AppendLine("    POP A, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    helper.AppendLine(".StrInstr_next");
                    helper.AppendLine("    INC BCD  ; Next position in first string");
                    helper.AppendLine("    INC JK  ; Increment position counter");
                    helper.AppendLine("    JP .StrInstr_outer");
                    helper.AppendLine(".StrInstr_notfound");
                    helper.AppendLine("    LD BCDE, 0  ; Not found");
                    helper.AppendLine("    POP A, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    break;
                    
                case "StrUcase":
                    helper.AppendLine(".StrUcase");
                    helper.AppendLine("    ; Input: FGH = string address, IJK = result buffer");
                    helper.AppendLine("    PUSH A, U  ; Preserve registers");
                    helper.AppendLine("    LD BCD, FGH  ; Source");
                    helper.AppendLine("    LD EFG, IJK  ; Destination");
                    helper.AppendLine(".StrUcase_loop");
                    helper.AppendLine("    LD A, (BCD)  ; Load character");
                    helper.AppendLine("    CP A, 0  ; Check for null");
                    helper.AppendLine("    JR Z, .StrUcase_done");
                    helper.AppendLine("    ; Check if lowercase letter (a-z: 0x61-0x7A)");
                    helper.AppendLine("    CP A, 0x61  ; 'a'");
                    helper.AppendLine("    JR LT, .StrUcase_store");
                    helper.AppendLine("    CP A, 0x7B  ; 'z' + 1");
                    helper.AppendLine("    JR GTE, .StrUcase_store");
                    helper.AppendLine("    SUB A, 0x20  ; Convert to uppercase");
                    helper.AppendLine(".StrUcase_store");
                    helper.AppendLine("    LD (EFG), A  ; Store character");
                    helper.AppendLine("    INC BCD  ; Next source");
                    helper.AppendLine("    INC EFG  ; Next dest");
                    helper.AppendLine("    JP .StrUcase_loop");
                    helper.AppendLine(".StrUcase_done");
                    helper.AppendLine("    LD A, 0  ; Null terminator");
                    helper.AppendLine("    LD (EFG), A");
                    helper.AppendLine("    POP A, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    break;
                    
                case "StrLcase":
                    helper.AppendLine(".StrLcase");
                    helper.AppendLine("    ; Input: FGH = string address, IJK = result buffer");
                    helper.AppendLine("    PUSH A, U  ; Preserve registers");
                    helper.AppendLine("    LD BCD, FGH  ; Source");
                    helper.AppendLine("    LD EFG, IJK  ; Destination");
                    helper.AppendLine(".StrLcase_loop");
                    helper.AppendLine("    LD A, (BCD)  ; Load character");
                    helper.AppendLine("    CP A, 0  ; Check for null");
                    helper.AppendLine("    JR Z, .StrLcase_done");
                    helper.AppendLine("    ; Check if uppercase letter (A-Z: 0x41-0x5A)");
                    helper.AppendLine("    CP A, 0x41  ; 'A'");
                    helper.AppendLine("    JR LT, .StrLcase_store");
                    helper.AppendLine("    CP A, 0x5B  ; 'Z' + 1");
                    helper.AppendLine("    JR GTE, .StrLcase_store");
                    helper.AppendLine("    ADD A, 0x20  ; Convert to lowercase");
                    helper.AppendLine(".StrLcase_store");
                    helper.AppendLine("    LD (EFG), A  ; Store character");
                    helper.AppendLine("    INC BCD  ; Next source");
                    helper.AppendLine("    INC EFG  ; Next dest");
                    helper.AppendLine("    JP .StrLcase_loop");
                    helper.AppendLine(".StrLcase_done");
                    helper.AppendLine("    LD A, 0  ; Null terminator");
                    helper.AppendLine("    LD (EFG), A");
                    helper.AppendLine("    POP A, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    break;
                    
                case "StrTrim":
                    helper.AppendLine(".StrTrim");
                    helper.AppendLine("    ; Input: FGH = string address, IJK = result buffer");
                    helper.AppendLine("    ; Removes leading and trailing whitespace");
                    helper.AppendLine("    PUSH A, U  ; Preserve registers");
                    helper.AppendLine("    ; First, find start (skip leading whitespace)");
                    helper.AppendLine("    LD BCD, FGH  ; String address");
                    helper.AppendLine(".StrTrim_skip_lead");
                    helper.AppendLine("    LD A, (BCD)  ; Load character");
                    helper.AppendLine("    CP A, 0  ; Check for null");
                    helper.AppendLine("    JR Z, .StrTrim_empty");
                    helper.AppendLine("    CP A, 0x20  ; Space");
                    helper.AppendLine("    JR Z, .StrTrim_skip_lead_inc");
                    helper.AppendLine("    CP A, 0x09  ; Tab");
                    helper.AppendLine("    JR Z, .StrTrim_skip_lead_inc");
                    helper.AppendLine("    CP A, 0x0A  ; Newline");
                    helper.AppendLine("    JR Z, .StrTrim_skip_lead_inc");
                    helper.AppendLine("    CP A, 0x0D  ; Carriage return");
                    helper.AppendLine("    JR Z, .StrTrim_skip_lead_inc");
                    helper.AppendLine("    JP .StrTrim_copy");
                    helper.AppendLine(".StrTrim_skip_lead_inc");
                    helper.AppendLine("    INC BCD  ; Skip whitespace");
                    helper.AppendLine("    JP .StrTrim_skip_lead");
                    helper.AppendLine(".StrTrim_copy");
                    helper.AppendLine("    ; Find end of string (for trailing whitespace)");
                    helper.AppendLine("    LD EFG, BCD  ; Start of non-whitespace");
                    helper.AppendLine("    LD HI, BCD  ; End pointer");
                    helper.AppendLine("    LD JK, BCD  ; Last non-whitespace");
                    helper.AppendLine(".StrTrim_find_end");
                    helper.AppendLine("    LD A, (HI)  ; Load character");
                    helper.AppendLine("    CP A, 0  ; Check for null");
                    helper.AppendLine("    JR Z, .StrTrim_found_end");
                    helper.AppendLine("    CP A, 0x20  ; Space");
                    helper.AppendLine("    JR Z, .StrTrim_find_end_inc");
                    helper.AppendLine("    CP A, 0x09  ; Tab");
                    helper.AppendLine("    JR Z, .StrTrim_find_end_inc");
                    helper.AppendLine("    CP A, 0x0A  ; Newline");
                    helper.AppendLine("    JR Z, .StrTrim_find_end_inc");
                    helper.AppendLine("    CP A, 0x0D  ; Carriage return");
                    helper.AppendLine("    JR Z, .StrTrim_find_end_inc");
                    helper.AppendLine("    LD JK, HI  ; Update last non-whitespace");
                    helper.AppendLine(".StrTrim_find_end_inc");
                    helper.AppendLine("    INC HI  ; Next character");
                    helper.AppendLine("    JP .StrTrim_find_end");
                    helper.AppendLine(".StrTrim_found_end");
                    helper.AppendLine("    ; Copy from EFG to JK");
                    helper.AppendLine("    LD LMN, IJK  ; Destination");
                    helper.AppendLine(".StrTrim_copy_loop");
                    helper.AppendLine("    CP EFG, JK  ; Compare with end");
                    helper.AppendLine("    JR GT, .StrTrim_done");
                    helper.AppendLine("    LD A, (EFG)  ; Load character");
                    helper.AppendLine("    LD (LMN), A  ; Store character");
                    helper.AppendLine("    INC EFG  ; Next source");
                    helper.AppendLine("    INC LMN  ; Next dest");
                    helper.AppendLine("    JP .StrTrim_copy_loop");
                    helper.AppendLine(".StrTrim_empty");
                    helper.AppendLine("    LD EFG, IJK  ; Destination");
                    helper.AppendLine("    LD A, 0  ; Null terminator");
                    helper.AppendLine("    LD (EFG), A");
                    helper.AppendLine("    POP A, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    helper.AppendLine(".StrTrim_done");
                    helper.AppendLine("    LD A, 0  ; Null terminator");
                    helper.AppendLine("    LD (LMN), A");
                    helper.AppendLine("    POP A, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    break;
                    
                case "StrLtrim":
                    helper.AppendLine(".StrLtrim");
                    helper.AppendLine("    ; Input: FGH = string address, IJK = result buffer");
                    helper.AppendLine("    ; Removes leading whitespace");
                    helper.AppendLine("    PUSH A, U  ; Preserve registers");
                    helper.AppendLine("    LD BCD, FGH  ; Source");
                    helper.AppendLine("    ; Skip leading whitespace");
                    helper.AppendLine(".StrLtrim_skip");
                    helper.AppendLine("    LD A, (BCD)  ; Load character");
                    helper.AppendLine("    CP A, 0  ; Check for null");
                    helper.AppendLine("    JR Z, .StrLtrim_done");
                    helper.AppendLine("    CP A, 0x20  ; Space");
                    helper.AppendLine("    JR Z, .StrLtrim_skip_inc");
                    helper.AppendLine("    CP A, 0x09  ; Tab");
                    helper.AppendLine("    JR Z, .StrLtrim_skip_inc");
                    helper.AppendLine("    CP A, 0x0A  ; Newline");
                    helper.AppendLine("    JR Z, .StrLtrim_skip_inc");
                    helper.AppendLine("    CP A, 0x0D  ; Carriage return");
                    helper.AppendLine("    JR Z, .StrLtrim_skip_inc");
                    helper.AppendLine("    JP .StrLtrim_copy");
                    helper.AppendLine(".StrLtrim_skip_inc");
                    helper.AppendLine("    INC BCD  ; Skip whitespace");
                    helper.AppendLine("    JP .StrLtrim_skip");
                    helper.AppendLine(".StrLtrim_copy");
                    helper.AppendLine("    LD EFG, IJK  ; Destination");
                    helper.AppendLine(".StrLtrim_loop");
                    helper.AppendLine("    LD A, (BCD)  ; Load character");
                    helper.AppendLine("    LD (EFG), A  ; Store character");
                    helper.AppendLine("    CP A, 0  ; Check for null");
                    helper.AppendLine("    JR Z, .StrLtrim_done");
                    helper.AppendLine("    INC BCD  ; Next source");
                    helper.AppendLine("    INC EFG  ; Next dest");
                    helper.AppendLine("    JP .StrLtrim_loop");
                    helper.AppendLine(".StrLtrim_done");
                    helper.AppendLine("    POP A, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    break;
                    
                case "StrRtrim":
                    helper.AppendLine(".StrRtrim");
                    helper.AppendLine("    ; Input: FGH = string address, IJK = result buffer");
                    helper.AppendLine("    ; Removes trailing whitespace");
                    helper.AppendLine("    PUSH A, U  ; Preserve registers");
                    helper.AppendLine("    LD BCD, FGH  ; Source");
                    helper.AppendLine("    LD EFG, IJK  ; Destination");
                    helper.AppendLine("    LD HI, EFG  ; Last non-whitespace position");
                    helper.AppendLine(".StrRtrim_loop");
                    helper.AppendLine("    LD A, (BCD)  ; Load character");
                    helper.AppendLine("    CP A, 0  ; Check for null");
                    helper.AppendLine("    JR Z, .StrRtrim_done");
                    helper.AppendLine("    LD (EFG), A  ; Store character");
                    helper.AppendLine("    CP A, 0x20  ; Space");
                    helper.AppendLine("    JR Z, .StrRtrim_next");
                    helper.AppendLine("    CP A, 0x09  ; Tab");
                    helper.AppendLine("    JR Z, .StrRtrim_next");
                    helper.AppendLine("    CP A, 0x0A  ; Newline");
                    helper.AppendLine("    JR Z, .StrRtrim_next");
                    helper.AppendLine("    CP A, 0x0D  ; Carriage return");
                    helper.AppendLine("    JR Z, .StrRtrim_next");
                    helper.AppendLine("    LD HI, EFG  ; Update last non-whitespace");
                    helper.AppendLine(".StrRtrim_next");
                    helper.AppendLine("    INC BCD  ; Next source");
                    helper.AppendLine("    INC EFG  ; Next dest");
                    helper.AppendLine("    JP .StrRtrim_loop");
                    helper.AppendLine(".StrRtrim_done");
                    helper.AppendLine("    ; Null terminate at last non-whitespace");
                    helper.AppendLine("    INC HI  ; After last non-whitespace");
                    helper.AppendLine("    LD A, 0  ; Null terminator");
                    helper.AppendLine("    LD (HI), A");
                    helper.AppendLine("    POP A, U  ; Restore registers");
                    helper.AppendLine("    RET");
                    break;
                    
                default:
                    return ""; // Unknown helper
            }
            
            return helper.ToString();
        }

        public uint GetRunAddress()
        {
            return _codeStartAddress;
        }
    }
}

