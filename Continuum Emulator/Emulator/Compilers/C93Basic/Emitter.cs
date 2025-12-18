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

        public List<CompileError> Errors => _errors;

        public Emitter(SymbolTable symbolTable, VariableManager variableManager)
        {
            _symbolTable = symbolTable;
            _variableManager = variableManager;
            _errors = new List<CompileError>();
            _labelAddresses = new Dictionary<string, uint>();
            _forwardReferences = new List<(string, uint)>();
        }

        public void SetCodeStartAddress(uint address)
        {
            _codeStartAddress = address;
            _currentAddress = address;
        }

        public string Emit(ProgramNode program)
        {
            _assembly = new StringBuilder();
            _labelCounter = 0;
            _errors.Clear();
            _labelAddresses.Clear();
            _forwardReferences.Clear();

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
                    uint varAddr = _variableManager.AllocateVariable(dim.Type, dims);
                    _symbolTable.AddVariable(dim.VariableName, new SymbolInfo
                    {
                        Name = dim.VariableName,
                        Type = dim.Type,
                        Address = varAddr,
                        ArrayDimensions = dims
                    });
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
                    _assembly.AppendLine($".{label.Name}:");
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
            // Get variable address
            SymbolInfo varInfo = _symbolTable.GetVariable(assign.Variable.Name);
            if (varInfo == null)
            {
                // Allocate new variable
                uint addr = _variableManager.AllocateVariable(assign.Variable.Type);
                varInfo = new SymbolInfo
                {
                    Name = assign.Variable.Name,
                    Type = assign.Variable.Type,
                    Address = addr
                };
                _symbolTable.AddVariable(assign.Variable.Name, varInfo);
            }

            // Evaluate expression and store result
            string exprReg = EmitExpression(assign.Expression, assign.Variable.Type);

            // Use LD to store variable (LD (addr), reg or fr)
            if (assign.Variable.Type == VariableType.Integer)
            {
                _assembly.AppendLine($"    LD ({varInfo.Address:X6}), {exprReg}");
                _currentAddress += 7; // LD (addr), rrrr = 7 bytes (32-bit)
            }
            else if (assign.Variable.Type == VariableType.Float)
            {
                // For floats, exprReg should be a float register (F0-F15)
                _assembly.AppendLine($"    LD ({varInfo.Address:X6}), {exprReg}");
                _currentAddress += 7; // LD (addr), fr = 7 bytes
            }
            else // String
            {
                // For strings, we need to copy the string data
                // exprReg contains the string address, copy to variable address
                _assembly.AppendLine($"    MEMC {exprReg}, {varInfo.Address:X6}, 256");
                _currentAddress += 10; // Approximate
            }
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
                // Store string in data section and return address
                uint strAddr = AllocateString(strVal);
                _assembly.AppendLine($"    LD {reg}, {strAddr:X6}");
                _currentAddress += 7;
            }
            
            return reg;
        }

        private string EmitVariableLoad(VariableNode var, VariableType targetType)
        {
            SymbolInfo varInfo = _symbolTable.GetVariable(var.Name);
            if (varInfo == null)
            {
                // Allocate new variable
                uint addr = _variableManager.AllocateVariable(var.Type);
                varInfo = new SymbolInfo
                {
                    Name = var.Name,
                    Type = var.Type,
                    Address = addr
                };
                _symbolTable.AddVariable(var.Name, varInfo);
            }

            string reg = GetTempRegister(targetType);

            if (var.Indices.Count == 0)
            {
                // Simple variable
            if (varInfo.Type == VariableType.Integer)
            {
                _assembly.AppendLine($"    LD {reg}, ({varInfo.Address:X6})");
                _currentAddress += 7; // LD rrrr, (nnn) = 7 bytes
            }
            else if (varInfo.Type == VariableType.Float)
            {
                _assembly.AppendLine($"    LD {reg}, ({varInfo.Address:X6})");
                _currentAddress += 7; // LD fr, (nnn) = 7 bytes
            }
            else // String
            {
                // String address is stored directly (24-bit address)
                _assembly.AppendLine($"    LD {reg}, {varInfo.Address:X6}");
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
                    _assembly.AppendLine($"    LD {reg}, {varInfo.Address:X6}");
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
                        _assembly.AppendLine($"    LD {reg}, ({varInfo.Address:X6})");
                    }
                    else if (varInfo.Type == VariableType.Float)
                    {
                        _assembly.AppendLine($"    LD {reg}, ({varInfo.Address:X6})");
                    }
                    else
                    {
                        _assembly.AppendLine($"    LD {reg}, {varInfo.Address:X6}");
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
            string resultReg = GetTempRegister(targetType);

            switch (bin.Operator)
            {
                case TokenType.PLUS:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    ADD {resultReg}, {rightReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.MINUS:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    SUB {resultReg}, {rightReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.MULTIPLY:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    MUL {resultReg}, {rightReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.DIVIDE:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    DIV {resultReg}, {rightReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.INT_DIVIDE:
                    // Integer division - use DIV then truncate
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    DIV {resultReg}, {rightReg}");
                    _assembly.AppendLine($"    INT {resultReg}, {resultReg}");
                    _currentAddress += 15;
                    break;
                case TokenType.MODULO:
                    // MOD - use DIV and get remainder
                    string tempReg = GetTempRegister(targetType);
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    LD {tempReg}, {rightReg}");
                    _assembly.AppendLine($"    DIV {resultReg}, {tempReg}");
                    // Remainder is in resultReg after DIV
                    _currentAddress += 15;
                    break;
                case TokenType.POWER:
                    // Use POW instruction for floats, or implement for integers
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    POW {resultReg}, {rightReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.EQUAL:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    CP {resultReg}, {rightReg}");
                    _assembly.AppendLine($"    LD {resultReg}, 1");
                    _assembly.AppendLine($"    JR Z, .eq_{_labelCounter}");
                    _assembly.AppendLine($"    LD {resultReg}, 0");
                    _assembly.AppendLine($".eq_{_labelCounter}:");
                    _labelCounter++;
                    _currentAddress += 25;
                    break;
                case TokenType.NOT_EQUAL:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    CP {resultReg}, {rightReg}");
                    _assembly.AppendLine($"    LD {resultReg}, 1");
                    _assembly.AppendLine($"    JR NZ, .ne_{_labelCounter}");
                    _assembly.AppendLine($"    LD {resultReg}, 0");
                    _assembly.AppendLine($".ne_{_labelCounter}:");
                    _labelCounter++;
                    _currentAddress += 25;
                    break;
                case TokenType.LESS_THAN:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    CP {resultReg}, {rightReg}");
                    _assembly.AppendLine($"    LD {resultReg}, 1");
                    _assembly.AppendLine($"    JR C, .lt_{_labelCounter}");
                    _assembly.AppendLine($"    LD {resultReg}, 0");
                    _assembly.AppendLine($".lt_{_labelCounter}:");
                    _labelCounter++;
                    _currentAddress += 25;
                    break;
                case TokenType.GREATER_THAN:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    CP {resultReg}, {rightReg}");
                    _assembly.AppendLine($"    LD {resultReg}, 1");
                    _assembly.AppendLine($"    JR G, .gt_{_labelCounter}");
                    _assembly.AppendLine($"    LD {resultReg}, 0");
                    _assembly.AppendLine($".gt_{_labelCounter}:");
                    _labelCounter++;
                    _currentAddress += 25;
                    break;
                case TokenType.LESS_EQUAL:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    CP {resultReg}, {rightReg}");
                    _assembly.AppendLine($"    LD {resultReg}, 1");
                    _assembly.AppendLine($"    JR C, .le_{_labelCounter}");
                    _assembly.AppendLine($"    JR Z, .le_{_labelCounter}");
                    _assembly.AppendLine($"    LD {resultReg}, 0");
                    _assembly.AppendLine($".le_{_labelCounter}:");
                    _labelCounter++;
                    _currentAddress += 30;
                    break;
                case TokenType.GREATER_EQUAL:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    CP {resultReg}, {rightReg}");
                    _assembly.AppendLine($"    LD {resultReg}, 1");
                    _assembly.AppendLine($"    JR G, .ge_{_labelCounter}");
                    _assembly.AppendLine($"    JR Z, .ge_{_labelCounter}");
                    _assembly.AppendLine($"    LD {resultReg}, 0");
                    _assembly.AppendLine($".ge_{_labelCounter}:");
                    _labelCounter++;
                    _currentAddress += 30;
                    break;
                case TokenType.AND:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    AND {resultReg}, {rightReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.OR:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    OR {resultReg}, {rightReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.XOR:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    XOR {resultReg}, {rightReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.NAND:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    NAND {resultReg}, {rightReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.NOR:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    NOR {resultReg}, {rightReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.XNOR:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    XNOR {resultReg}, {rightReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.IMPLY:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    IMPLY {resultReg}, {rightReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.SHL:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    SL {resultReg}, {rightReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.SHR:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    SR {resultReg}, {rightReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.ROL:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    RL {resultReg}, {rightReg}");
                    _currentAddress += 10;
                    break;
                case TokenType.ROR:
                    _assembly.AppendLine($"    LD {resultReg}, {leftReg}");
                    _assembly.AppendLine($"    RR {resultReg}, {rightReg}");
                    _currentAddress += 10;
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
                // String functions - use interrupts
                case "LEN":
                    if (func.Arguments.Count != 1) throw new Exception("LEN requires 1 argument");
                    string lenStr = EmitExpression(func.Arguments[0], VariableType.String);
                    _assembly.AppendLine($"    LD A, 0x01  ; String length function (placeholder)");
                    _assembly.AppendLine($"    LD BCD, {lenStr}");
                    _assembly.AppendLine($"    INT 0x05, A");
                    _assembly.AppendLine($"    LD {resultReg}, A");
                    _currentAddress += 15;
                    break;
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
                // Regular PRINT
                foreach (var item in print.Items)
                {
                    string exprReg = EmitExpression(item.Expression, VariableType.String);
                    _assembly.AppendLine($"    LD A, 0x12  ; DrawString function");
                    _assembly.AppendLine($"    LD BCD, {exprReg}  ; String address");
                    _assembly.AppendLine($"    INT 0x01, A");
                    _currentAddress += 15;
                }
            }
        }

        private void EmitIf(IfNode ifNode)
        {
            string condReg = EmitExpression(ifNode.Condition);
            string elseLabel = $".else_{_labelCounter}";
            string endLabel = $".endif_{_labelCounter}";
            _labelCounter++;

            // Test condition
            _assembly.AppendLine($"    CP {condReg}, 0");
            _assembly.AppendLine($"    JR Z, {elseLabel}");

            // THEN block
            foreach (var stmt in ifNode.ThenStatements)
            {
                EmitStatement(stmt);
            }

            if (ifNode.ElseStatements.Count > 0 || ifNode.ElseIfs.Count > 0)
            {
                _assembly.AppendLine($"    JP {endLabel}");
            }

            _assembly.AppendLine($"{elseLabel}:");

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
                _assembly.AppendLine($"{elseifLabel}:");
            }

            // ELSE block
            if (ifNode.ElseStatements.Count > 0)
            {
                foreach (var stmt in ifNode.ElseStatements)
                {
                    EmitStatement(stmt);
                }
            }

            _assembly.AppendLine($"{endLabel}:");
        }

        private void EmitFor(ForNode forNode)
        {
            // Allocate loop variable
            SymbolInfo loopVar = _symbolTable.GetVariable(forNode.VariableName);
            if (loopVar == null)
            {
                uint addr = _variableManager.AllocateVariable(VariableType.Integer);
                loopVar = new SymbolInfo { Name = forNode.VariableName, Type = VariableType.Integer, Address = addr };
                _symbolTable.AddVariable(forNode.VariableName, loopVar);
            }

            string startLabel = $".for_start_{_labelCounter}";
            string endLabel = $".for_end_{_labelCounter}";
            string stepLabel = $".for_step_{_labelCounter}";
            _labelCounter++;

            // Initialize loop variable
            string startReg = EmitExpression(forNode.Start);
            _assembly.AppendLine($"    LD ({loopVar.Address:X6}), {startReg}");
            _assembly.AppendLine($"{startLabel}:");

            // Check condition
            string endReg = EmitExpression(forNode.End);
            _assembly.AppendLine($"    LD A, ({loopVar.Address:X6})");
            _assembly.AppendLine($"    CP A, {endReg}");
            _assembly.AppendLine($"    JR G, {endLabel}");

            // Loop body
            foreach (var stmt in forNode.Statements)
            {
                EmitStatement(stmt);
            }

            // Increment
            _assembly.AppendLine($"{stepLabel}:");
            string step = forNode.Step != null ? EmitExpression(forNode.Step) : "1";
            _assembly.AppendLine($"    LD A, ({loopVar.Address:X6})");
            _assembly.AppendLine($"    ADD A, {step}");
            _assembly.AppendLine($"    LD ({loopVar.Address:X6}), A");
            _assembly.AppendLine($"    JP {startLabel}");
            _assembly.AppendLine($"{endLabel}:");
        }

        private void EmitWhile(WhileNode whileNode)
        {
            string startLabel = $".while_start_{_labelCounter}";
            string endLabel = $".while_end_{_labelCounter}";
            _labelCounter++;

            _assembly.AppendLine($"{startLabel}:");
            string condReg = EmitExpression(whileNode.Condition);
            _assembly.AppendLine($"    CP {condReg}, 0");
            _assembly.AppendLine($"    JR Z, {endLabel}");

            foreach (var stmt in whileNode.Statements)
            {
                EmitStatement(stmt);
            }

            _assembly.AppendLine($"    JP {startLabel}");
            _assembly.AppendLine($"{endLabel}:");
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
            string xReg = EmitExpression(plot.X, VariableType.Integer);
            string yReg = EmitExpression(plot.Y, VariableType.Integer);
            _assembly.AppendLine("    LD A, 0x20  ; Plot function");
            _assembly.AppendLine("    LD B, 0  ; Page 0");
            _assembly.AppendLine($"    LD CD, {xReg}");
            _assembly.AppendLine($"    LD EF, {yReg}");
            if (plot.Color != null)
            {
                string colorReg = EmitExpression(plot.Color, VariableType.Integer);
                _assembly.AppendLine($"    LD G, {colorReg}");
            }
            else
            {
                _assembly.AppendLine("    LD G, 15  ; Default color");
            }
            _assembly.AppendLine("    INT 0x01, A");
            _currentAddress += 20;
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

        private string GetTempRegister(VariableType type)
        {
            // Simple register allocation - use A for integers, F0 for floats
            return type == VariableType.Float ? "F0" : "A";
        }

        private uint AllocateString(string str)
        {
            // Allocate string in data section
            uint addr = _variableManager.GetVariableSectionEnd();
            _variableManager.SetCodeEndAddress(addr + (uint)str.Length + 1); // +1 for null terminator
            return addr;
        }

        public uint GetRunAddress()
        {
            return _codeStartAddress;
        }
    }
}

