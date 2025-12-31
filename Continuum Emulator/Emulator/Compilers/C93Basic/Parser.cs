using System;
using System.Collections.Generic;
using System.Linq;

namespace Continuum93.Emulator.Compilers.C93Basic
{
    /// <summary>
    /// Parser for building AST from tokens.
    /// </summary>
    public class Parser
    {
        private List<Token> _tokens;
        private int _position;
        private SymbolTable _symbolTable;
        public List<CompileError> Errors { get; private set; } = new List<CompileError>();

        public Parser(SymbolTable symbolTable)
        {
            _symbolTable = symbolTable;
        }

        public ProgramNode Parse(List<Token> tokens)
        {
            _tokens = tokens;
            _position = 0;
            Errors.Clear();

            ProgramNode program = new ProgramNode();

            while (!IsAtEnd())
            {
                if (Current().Type == TokenType.NEWLINE)
                {
                    Advance();
                    continue;
                }

                if (Current().Type == TokenType.EOF)
                    break;

                try
                {
                    StatementNode stmt = ParseStatement();
                    if (stmt != null)
                    {
                        program.Statements.Add(stmt);
                    }
                }
                catch (Exception ex)
                {
                    Errors.Add(new CompileError(Current().Line, Current().Column, $"Parse error: {ex.Message}"));
                    // Try to recover by skipping to next line
                    while (!IsAtEnd() && Current().Type != TokenType.NEWLINE && Current().Type != TokenType.EOF)
                    {
                        Advance();
                    }
                    // Don't skip the statement - try to continue parsing
                }
            }

            return program;
        }

        private StatementNode ParseStatement()
        {
            if (IsAtEnd()) return null;

            // Skip newlines
            while (Current().Type == TokenType.NEWLINE && !IsAtEnd())
            {
                Advance();
            }

            if (IsAtEnd()) return null;

            // Check for label
            if (Current().Type == TokenType.IDENTIFIER && Peek().Type == TokenType.COLON)
            {
                string labelName = Current().Value;
                int line = Current().Line;
                Advance(); // identifier
                Advance(); // colon
                _symbolTable.AddLabel(labelName, 0); // Address will be set later
                return new LabelNode { Name = labelName, Line = line };
            }

            Token token = Current();

            switch (token.Type)
            {
                case TokenType.LET:
                case TokenType.IDENTIFIER when IsAssignment():
                    return ParseAssignment();
                case TokenType.DIM:
                    return ParseDim();
                case TokenType.IF:
                    return ParseIf();
                case TokenType.FOR:
                    return ParseFor();
                case TokenType.WHILE:
                    return ParseWhile();
                case TokenType.REPEAT:
                    return ParseRepeat();
                case TokenType.GOTO:
                    return ParseGoto();
                case TokenType.GOSUB:
                    return ParseGosub();
                case TokenType.RETURN:
                    Advance();
                    return new ReturnNode { Line = token.Line };
                case TokenType.EXIT:
                    return ParseExit();
                case TokenType.CONTINUE:
                    return ParseContinue();
                case TokenType.PRINT:
                    return ParsePrint();
                case TokenType.INPUT:
                    return ParseInput();
                case TokenType.CLS:
                    return ParseCls();
                case TokenType.END:
                    Advance();
                    return new EndNode { Line = token.Line };
                case TokenType.STOP:
                    Advance();
                    return new StopNode { Line = token.Line };
                case TokenType.WAIT:
                    return ParseWait();
                case TokenType.SLEEP:
                    return ParseSleep();
                case TokenType.PLOT:
                    return ParsePlot();
                case TokenType.LINE:
                    return ParseLine();
                case TokenType.RECTANGLE:
                    return ParseRectangle();
                case TokenType.CIRCLE:
                    return ParseCircle();
                case TokenType.ELLIPSE:
                    return ParseEllipse();
                case TokenType.SCREEN:
                    return ParseScreen();
                case TokenType.VIDEO:
                    return ParseVideo();
                case TokenType.INK:
                    return ParseInk();
                case TokenType.PAPER:
                    return ParsePaper();
                case TokenType.BEEP:
                    return ParseBeep();
                case TokenType.PLAY:
                    return ParsePlay();
                case TokenType.LOAD:
                    return ParseLoad();
                case TokenType.FONT:
                    return ParseFont();
                case TokenType.LAYER:
                    return ParseLayer();
                case TokenType.SPRITE:
                    return ParseSprite();
                case TokenType.LOCATE:
                    return ParseLocate();
                case TokenType.DATA:
                    return ParseData();
                case TokenType.POKE:
                case TokenType.POKE16:
                case TokenType.POKE24:
                case TokenType.POKE32:
                    return ParsePoke(token.Type);
                case TokenType.MEMCOPY:
                    return ParseMemCopy();
                case TokenType.MEMFILL:
                    return ParseMemFill();
                case TokenType.REM:
                case TokenType.COMMENT:
                    // Skip comments
                    while (!IsAtEnd() && Current().Type != TokenType.NEWLINE && Current().Type != TokenType.EOF)
                        Advance();
                    return null;
                default:
                    Errors.Add(new CompileError(token.Line, token.Column, $"Unexpected token: {token.Type}"));
                    Advance();
                    return null;
            }
        }

        private bool IsAssignment()
        {
            int savePos = _position;
            if (Current().Type == TokenType.IDENTIFIER)
            {
                Advance();
                if (Current().Type == TokenType.EQUAL || Current().Type == TokenType.LBRACKET)
                {
                    _position = savePos;
                    return true;
                }
            }
            _position = savePos;
            return false;
        }

        private AssignmentNode ParseAssignment()
        {
            int line = Current().Line;
            if (Current().Type == TokenType.LET)
                Advance();

            // Parse variable name
            Token varToken = ExpectIdentifier();
            VariableNode var = new VariableNode 
            { 
                Name = varToken.Value, 
                Type = GetVariableType(varToken.Value), 
                Line = varToken.Line 
            };
            
            // Check for array indices
            if (Match(TokenType.LBRACKET))
            {
                do
                {
                    var.Indices.Add(ParseExpression());
                } while (Match(TokenType.COMMA));
                Expect(TokenType.RBRACKET);
            }
            
            // Expect EQUAL token - must consume it
            Expect(TokenType.EQUAL);
            
            // Now parse the expression - should see INTEGER token next
            ExpressionNode expr = ParseExpression();
            return new AssignmentNode { Variable = var, Expression = expr, Line = line };
        }

        private DimNode ParseDim()
        {
            int line = Current().Line;
            Advance(); // DIM
            string varName = ExpectIdentifier().Value;
            VariableType type = GetVariableType(varName);

            DimNode dim = new DimNode { VariableName = varName, Type = type, Line = line };
            Expect(TokenType.LPAREN);
            
            do
            {
                dim.Dimensions.Add(ParseExpression());
            } while (Match(TokenType.COMMA));
            
            Expect(TokenType.RPAREN);
            return dim;
        }

        private IfNode ParseIf()
        {
            int line = Current().Line;
            Advance(); // IF
            ExpressionNode condition = ParseExpression();
            Expect(TokenType.THEN);
            
            IfNode ifNode = new IfNode { Condition = condition, Line = line };

            // Skip whitespace/newlines after THEN
            while (Current().Type == TokenType.NEWLINE)
            {
                Advance();
            }

            // Check for single-line IF (statement on same line as THEN)
            if (Current().Type == TokenType.EOF || Current().Type == TokenType.ENDIF ||
                (Current().Type == TokenType.END && Peek().Type == TokenType.IF))
            {
                return ifNode;
            }

            // Multi-line IF - parse THEN block
            // Parse statements until we find END IF, ELSE, or ELSEIF
            while (Current().Type != TokenType.ENDIF && Current().Type != TokenType.ELSE && 
                   Current().Type != TokenType.ELSEIF && Current().Type != TokenType.EOF &&
                   !(Current().Type == TokenType.END && Peek().Type == TokenType.IF))
            {
                if (Current().Type == TokenType.NEWLINE)
                {
                    Advance();
                    continue;
                }
                StatementNode stmt = ParseStatement();
                if (stmt != null)
                    ifNode.ThenStatements.Add(stmt);
            }

            // Check for ELSEIF
            if (Match(TokenType.ELSEIF))
                {
                    do
                    {
                        ExpressionNode elseifCond = ParseExpression();
                        Expect(TokenType.THEN);
                        ElseIfNode elseif = new ElseIfNode { Condition = elseifCond };
                        while (Current().Type != TokenType.ENDIF && Current().Type != TokenType.ELSE && 
                               Current().Type != TokenType.ELSEIF && Current().Type != TokenType.EOF &&
                               !(Current().Type == TokenType.END && Peek().Type == TokenType.IF))
                        {
                            if (Current().Type == TokenType.NEWLINE)
                            {
                                Advance();
                                continue;
                            }
                            StatementNode stmt = ParseStatement();
                            if (stmt != null)
                                elseif.Statements.Add(stmt);
                        }
                        ifNode.ElseIfs.Add(elseif);
                    } while (Match(TokenType.ELSEIF));
                }

            // Check for ELSE
            if (Match(TokenType.ELSE))
            {
                while (Current().Type != TokenType.ENDIF && Current().Type != TokenType.EOF &&
                       !(Current().Type == TokenType.END && Peek().Type == TokenType.IF))
                {
                    if (Current().Type == TokenType.NEWLINE)
                    {
                        Advance();
                        continue;
                    }
                    StatementNode stmt = ParseStatement();
                    if (stmt != null)
                        ifNode.ElseStatements.Add(stmt);
                }
            }

            // Check for END IF (two tokens) or ENDIF (single token)
            if (Current().Type == TokenType.END && Peek().Type == TokenType.IF)
            {
                Advance(); // END
                Advance(); // IF
            }
            else if (Match(TokenType.ENDIF))
            {
                // Single token ENDIF - already consumed by Match
            }

            return ifNode;
        }

        private ForNode ParseFor()
        {
            int line = Current().Line;
            Advance(); // FOR
            string varName = ExpectIdentifier().Value;
            Expect(TokenType.EQUAL);
            ExpressionNode start = ParseExpression();
            Expect(TokenType.TO);
            ExpressionNode end = ParseExpression();
            ExpressionNode step = null;
            if (Match(TokenType.STEP))
            {
                step = ParseExpression();
            }

            ForNode forNode = new ForNode { VariableName = varName, Start = start, End = end, Step = step, Line = line };

            // Parse loop body
            while (Current().Type != TokenType.NEXT && Current().Type != TokenType.EOF)
            {
                if (Current().Type == TokenType.NEWLINE)
                {
                    Advance();
                    continue;
                }
                StatementNode stmt = ParseStatement();
                if (stmt != null)
                    forNode.Statements.Add(stmt);
            }

            if (Match(TokenType.NEXT))
            {
                // Optional variable name after NEXT
                if (Current().Type == TokenType.IDENTIFIER)
                    Advance();
            }

            return forNode;
        }

        private WhileNode ParseWhile()
        {
            int line = Current().Line;
            Advance(); // WHILE
            ExpressionNode condition = ParseExpression();

            WhileNode whileNode = new WhileNode { Condition = condition, Line = line };

            while (Current().Type != TokenType.WEND && Current().Type != TokenType.EOF)
            {
                if (Current().Type == TokenType.NEWLINE)
                {
                    Advance();
                    continue;
                }
                StatementNode stmt = ParseStatement();
                if (stmt != null)
                    whileNode.Statements.Add(stmt);
            }

            Expect(TokenType.WEND);
            return whileNode;
        }

        private RepeatNode ParseRepeat()
        {
            int line = Current().Line;
            Advance(); // REPEAT

            RepeatNode repeatNode = new RepeatNode { Line = line };

            while (Current().Type != TokenType.UNTIL && Current().Type != TokenType.EOF)
            {
                if (Current().Type == TokenType.NEWLINE)
                {
                    Advance();
                    continue;
                }
                StatementNode stmt = ParseStatement();
                if (stmt != null)
                    repeatNode.Statements.Add(stmt);
            }

            Expect(TokenType.UNTIL);
            repeatNode.Condition = ParseExpression();
            return repeatNode;
        }

        private GotoNode ParseGoto()
        {
            int line = Current().Line;
            Advance(); // GOTO
            string label = ExpectIdentifier().Value;
            return new GotoNode { Label = label, Line = line };
        }

        private GosubNode ParseGosub()
        {
            int line = Current().Line;
            Advance(); // GOSUB
            string label = ExpectIdentifier().Value;
            return new GosubNode { Label = label, Line = line };
        }

        private StatementNode ParseExit()
        {
            int line = Current().Line;
            Advance(); // EXIT
            Token token = Expect(TokenType.FOR, TokenType.WHILE);
            if (token.Type == TokenType.FOR)
                return new ExitForNode { Line = line };
            else
                return new ExitWhileNode { Line = line };
        }

        private StatementNode ParseContinue()
        {
            int line = Current().Line;
            Advance(); // CONTINUE
            Token token = Expect(TokenType.FOR, TokenType.WHILE);
            if (token.Type == TokenType.FOR)
                return new ContinueForNode { Line = line };
            else
                return new ContinueWhileNode { Line = line };
        }

        private PrintNode ParsePrint()
        {
            int line = Current().Line;
            Advance(); // PRINT

            PrintNode print = new PrintNode { Line = line };

            // Check for PRINT AT
            if (Match(TokenType.AT))
            {
                print.AtPosition = true;
                print.AtX = ParseExpression();
                Expect(TokenType.COMMA);
                print.AtY = ParseExpression();
                Expect(TokenType.SEMICOLON);
            }
            // Check for PRINT TAB
            else if (Match(TokenType.TAB))
            {
                Expect(TokenType.LPAREN);
                print.TabPosition = ParseExpression();
                Expect(TokenType.RPAREN);
                Expect(TokenType.SEMICOLON);
            }

            // Parse print items
            while (Current().Type != TokenType.NEWLINE && Current().Type != TokenType.EOF)
            {
                if (Current().Type == TokenType.SEMICOLON || Current().Type == TokenType.COMMA)
                {
                    bool semicolon = Current().Type == TokenType.SEMICOLON;
                    Advance();
                    if (Current().Type == TokenType.NEWLINE || Current().Type == TokenType.EOF)
                        break;
                    print.Items.Add(new PrintItem { Expression = ParseExpression(), Semicolon = semicolon, Comma = !semicolon });
                }
                else
                {
                    ExpressionNode expr = ParseExpression();
                    bool semicolon = Match(TokenType.SEMICOLON);
                    bool comma = Match(TokenType.COMMA);
                    print.Items.Add(new PrintItem { Expression = expr, Semicolon = semicolon, Comma = comma });
                }

                // Check for USING clause
                if (Match(TokenType.USING))
                {
                    print.FontAddr = ParseExpression();
                    if (Match(TokenType.COMMA))
                    {
                        print.Color = ParseExpression();
                        if (Match(TokenType.COMMA))
                        {
                            print.Flags = ParseExpression();
                            if (Match(TokenType.COMMA))
                            {
                                print.MaxWidth = ParseExpression();
                                if (Match(TokenType.COMMA))
                                {
                                    print.OutlineColor = ParseExpression();
                                    if (Match(TokenType.COMMA))
                                    {
                                        print.OutlinePattern = ParseExpression();
                                    }
                                }
                            }
                        }
                    }
                }

                if (Current().Type == TokenType.NEWLINE || Current().Type == TokenType.EOF)
                    break;
            }

            return print;
        }

        private InputNode ParseInput()
        {
            int line = Current().Line;
            Advance(); // INPUT
            ExpressionNode prompt = null;
            if (Current().Type == TokenType.STRING)
            {
                prompt = ParseExpression();
                if (Match(TokenType.SEMICOLON))
                {
                    // Optional semicolon
                }
            }
            VariableNode var = ParseVariable();
            return new InputNode { Prompt = prompt, Variable = var, Line = line };
        }

        private ClsNode ParseCls()
        {
            int line = Current().Line;
            Advance(); // CLS
            ExpressionNode page = null;
            if (Current().Type != TokenType.NEWLINE && Current().Type != TokenType.EOF)
            {
                page = ParseExpression();
            }
            return new ClsNode { Page = page, Line = line };
        }

        private WaitNode ParseWait()
        {
            int line = Current().Line;
            Advance(); // WAIT
            ExpressionNode frames = ParseExpression();
            return new WaitNode { Frames = frames, Line = line };
        }

        private SleepNode ParseSleep()
        {
            int line = Current().Line;
            Advance(); // SLEEP
            ExpressionNode ms = ParseExpression();
            return new SleepNode { Milliseconds = ms, Line = line };
        }

        private PlotNode ParsePlot()
        {
            int line = Current().Line;
            Advance(); // PLOT
            ExpressionNode x = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode y = ParseExpression();
            ExpressionNode color = null;
            if (Match(TokenType.COMMA))
            {
                color = ParseExpression();
            }
            return new PlotNode { X = x, Y = y, Color = color, Line = line };
        }

        private LineNode ParseLine()
        {
            int line = Current().Line;
            Advance(); // LINE
            ExpressionNode x1 = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode y1 = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode x2 = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode y2 = ParseExpression();
            ExpressionNode color = null;
            if (Match(TokenType.COMMA))
            {
                color = ParseExpression();
            }
            return new LineNode { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2, Color = color, Line = line };
        }

        private StatementNode ParseRectangle()
        {
            int line = Current().Line;
            Advance(); // RECTANGLE
            bool filled = Match(TokenType.FILLED);
            ExpressionNode x = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode y = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode width = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode height = ParseExpression();
            ExpressionNode color = null;
            if (Match(TokenType.COMMA))
            {
                color = ParseExpression();
            }
            return new RectangleNode { X = x, Y = y, Width = width, Height = height, Color = color, Filled = filled, Line = line };
        }

        private StatementNode ParseCircle()
        {
            int line = Current().Line;
            Advance(); // CIRCLE
            bool filled = Match(TokenType.FILLED);
            ExpressionNode x = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode y = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode radius = ParseExpression();
            ExpressionNode color = null;
            if (Match(TokenType.COMMA))
            {
                color = ParseExpression();
            }
            return new CircleNode { X = x, Y = y, Radius = radius, Color = color, Filled = filled, Line = line };
        }

        private StatementNode ParseEllipse()
        {
            int line = Current().Line;
            Advance(); // ELLIPSE
            bool filled = Match(TokenType.FILLED);
            ExpressionNode x = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode y = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode radiusX = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode radiusY = ParseExpression();
            ExpressionNode color = null;
            if (Match(TokenType.COMMA))
            {
                color = ParseExpression();
            }
            return new EllipseNode { X = x, Y = y, RadiusX = radiusX, RadiusY = radiusY, Color = color, Filled = filled, Line = line };
        }

        private StatementNode ParseScreen()
        {
            int line = Current().Line;
            Advance(); // SCREEN
            ExpressionNode page = ParseExpression();
            return new ScreenNode { Page = page, Line = line };
        }

        private StatementNode ParseVideo()
        {
            int line = Current().Line;
            Advance(); // VIDEO
            ExpressionNode pages = ParseExpression();
            return new VideoNode { Pages = pages, Line = line };
        }

        private StatementNode ParseInk()
        {
            int line = Current().Line;
            Advance(); // INK
            ExpressionNode color = ParseExpression();
            return new InkNode { Color = color, Line = line };
        }

        private StatementNode ParsePaper()
        {
            int line = Current().Line;
            Advance(); // PAPER
            ExpressionNode color = ParseExpression();
            return new PaperNode { Color = color, Line = line };
        }

        private StatementNode ParseBeep()
        {
            int line = Current().Line;
            Advance(); // BEEP
            ExpressionNode duration = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode pitch = ParseExpression();
            ExpressionNode volume = null;
            if (Match(TokenType.COMMA))
            {
                volume = ParseExpression();
            }
            return new BeepNode { Duration = duration, Pitch = pitch, Volume = volume, Line = line };
        }

        private StatementNode ParsePlay()
        {
            int line = Current().Line;
            Advance(); // PLAY
            ExpressionNode address = ParseExpression();
            return new PlayNode { Address = address, Line = line };
        }

        private StatementNode ParseLoad()
        {
            int line = Current().Line;
            Advance(); // LOAD
            if (Match(TokenType.FONT))
            {
                // LOAD FONT filename$, address
                ExpressionNode filename = ParseExpression();
                Expect(TokenType.COMMA);
                ExpressionNode address = ParseExpression();
                return new LoadFontNode { Filename = filename, Address = address, Line = line };
            }
            else
            {
                // LOAD filename$ [, address]
                ExpressionNode filename = ParseExpression();
                ExpressionNode address = null;
                if (Match(TokenType.COMMA))
                {
                    address = ParseExpression();
                }
                // TODO: Implement regular LOAD command
                Errors.Add(new CompileError(line, Current().Column, "LOAD command not yet implemented (use LOAD FONT)"));
                return null;
            }
        }

        private StatementNode ParseFont()
        {
            int line = Current().Line;
            Advance(); // FONT
            if (Match(TokenType.COLOR))
            {
                ExpressionNode color = ParseExpression();
                return new FontColorNode { Color = color, Line = line };
            }
            else if (Match(TokenType.FLAGS))
            {
                ExpressionNode flags = ParseExpression();
                return new FontFlagsNode { Flags = flags, Line = line };
            }
            else if (Match(TokenType.MAXWIDTH))
            {
                ExpressionNode maxWidth = ParseExpression();
                return new FontMaxWidthNode { MaxWidth = maxWidth, Line = line };
            }
            else if (Match(TokenType.OUTLINE))
            {
                ExpressionNode color = ParseExpression();
                Expect(TokenType.COMMA);
                ExpressionNode pattern = ParseExpression();
                return new FontOutlineNode { Color = color, Pattern = pattern, Line = line };
            }
            else
            {
                // FONT address
                ExpressionNode address = ParseExpression();
                return new FontNode { Address = address, Line = line };
            }
        }

        private StatementNode ParseLayer()
        {
            int line = Current().Line;
            Advance(); // LAYER
            if (Match(TokenType.SHOW))
            {
                ExpressionNode layer = ParseExpression();
                return new LayerShowNode { Layer = layer, Line = line };
            }
            else if (Match(TokenType.HIDE))
            {
                ExpressionNode layer = ParseExpression();
                return new LayerHideNode { Layer = layer, Line = line };
            }
            else if (Match(TokenType.VISIBILITY))
            {
                ExpressionNode mask = ParseExpression();
                return new LayerVisibilityNode { Mask = mask, Line = line };
            }
            else
            {
                Errors.Add(new CompileError(line, Current().Column, "LAYER command requires SHOW, HIDE, or VISIBILITY"));
                return null;
            }
        }

        private StatementNode ParseSprite()
        {
            int line = Current().Line;
            Advance(); // SPRITE
            ExpressionNode x = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode y = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode address = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode width = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode height = ParseExpression();
            ExpressionNode page = null;
            if (Match(TokenType.COMMA))
            {
                page = ParseExpression();
            }
            return new SpriteNode { X = x, Y = y, Address = address, Width = width, Height = height, Page = page, Line = line };
        }

        private StatementNode ParseLocate()
        {
            int line = Current().Line;
            Advance(); // LOCATE
            ExpressionNode x = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode y = ParseExpression();
            return new LocateNode { X = x, Y = y, Line = line };
        }


        private ExpressionNode ParseMouse()
        {
            int line = Current().Line;
            // MOUSE already consumed by Match in ParsePrimary
            if (Match(TokenType.X))
            {
                return new MouseXNode { Line = line };
            }
            else if (Match(TokenType.Y))
            {
                return new MouseYNode { Line = line };
            }
            else if (Match(TokenType.BUTTON))
            {
                ExpressionNode button = ParseExpression();
                return new MouseButtonNode { Button = button, Line = line };
            }
            else
            {
                Errors.Add(new CompileError(line, Current().Column, "MOUSE requires X, Y, or BUTTON"));
                return null;
            }
        }

        private StatementNode ParseData()
        {
            int line = Current().Line;
            Advance(); // DATA
            DataNode data = new DataNode { Line = line };
            do
            {
                data.Values.Add(ParseExpression());
            } while (Match(TokenType.COMMA));
            return data;
        }

        private ExpressionNode ParsePeek(TokenType tokenType)
        {
            int line = Current().Line;
            Advance(); // PEEK/PEEK16/PEEK24/PEEK32
            Expect(TokenType.LPAREN);
            ExpressionNode address = ParseExpression();
            Expect(TokenType.RPAREN);
            int size = tokenType == TokenType.PEEK ? 1 :
                       tokenType == TokenType.PEEK16 ? 16 :
                       tokenType == TokenType.PEEK24 ? 24 : 32;
            return new PeekNode { Address = address, Size = size, Line = line };
        }

        private StatementNode ParsePoke(TokenType tokenType)
        {
            int line = Current().Line;
            Advance(); // POKE/POKE16/POKE24/POKE32
            ExpressionNode address = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode value = ParseExpression();
            int size = tokenType == TokenType.POKE ? 1 :
                       tokenType == TokenType.POKE16 ? 16 :
                       tokenType == TokenType.POKE24 ? 24 : 32;
            return new PokeNode { Address = address, Value = value, Size = size, Line = line };
        }

        private StatementNode ParseMemCopy()
        {
            int line = Current().Line;
            Advance(); // MEMCOPY
            ExpressionNode source = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode dest = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode length = ParseExpression();
            return new MemCopyNode { Source = source, Dest = dest, Length = length, Line = line };
        }

        private StatementNode ParseMemFill()
        {
            int line = Current().Line;
            Advance(); // MEMFILL
            ExpressionNode address = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode length = ParseExpression();
            Expect(TokenType.COMMA);
            ExpressionNode value = ParseExpression();
            return new MemFillNode { Address = address, Length = length, Value = value, Line = line };
        }

        private ExpressionNode ParseVarPtr()
        {
            int line = Current().Line;
            // VARPTR already consumed by Match in ParsePrimary
            Expect(TokenType.LPAREN);
            VariableNode var = ParseVariable();
            Expect(TokenType.RPAREN);
            return new VarPtrNode { Variable = var, Line = line };
        }

        private ExpressionNode ParseTime()
        {
            int line = Current().Line;
            // TIME already consumed by Match in ParsePrimary
            return new TimeNode { Line = line };
        }

        private ExpressionNode ParseTicks()
        {
            int line = Current().Line;
            // TICKS already consumed by Match in ParsePrimary
            return new TicksNode { Line = line };
        }

        private ExpressionNode ParseExpression()
        {
            return ParseLogicalOr();
        }

        private ExpressionNode ParseLogicalOr()
        {
            ExpressionNode expr = ParseLogicalXor();
            while (Match(TokenType.OR))
            {
                Token op = _tokens[_position - 1];
                ExpressionNode right = ParseLogicalXor();
                expr = new BinaryExpressionNode { Left = expr, Operator = op.Type, Right = right, Line = expr.Line };
            }
            return expr;
        }

        private ExpressionNode ParseLogicalXor()
        {
            ExpressionNode expr = ParseLogicalAnd();
            while (Match(TokenType.XOR))
            {
                Token op = _tokens[_position - 1];
                ExpressionNode right = ParseLogicalAnd();
                expr = new BinaryExpressionNode { Left = expr, Operator = op.Type, Right = right, Line = expr.Line };
            }
            return expr;
        }

        private ExpressionNode ParseLogicalAnd()
        {
            ExpressionNode expr = ParseComparison();
            while (Match(TokenType.AND))
            {
                Token op = _tokens[_position - 1];
                ExpressionNode right = ParseComparison();
                expr = new BinaryExpressionNode { Left = expr, Operator = op.Type, Right = right, Line = expr.Line };
            }
            return expr;
        }

        private ExpressionNode ParseComparison()
        {
            ExpressionNode expr = ParseBitwise();
            while (Match(TokenType.EQUAL, TokenType.NOT_EQUAL, TokenType.LESS_THAN, TokenType.GREATER_THAN, 
                         TokenType.LESS_EQUAL, TokenType.GREATER_EQUAL))
            {
                Token op = _tokens[_position - 1];
                ExpressionNode right = ParseBitwise();
                expr = new BinaryExpressionNode { Left = expr, Operator = op.Type, Right = right, Line = expr.Line };
            }
            return expr;
        }

        private ExpressionNode ParseBitwise()
        {
            ExpressionNode expr = ParseAdditive();
            while (Match(TokenType.SHL, TokenType.SHR, TokenType.ROL, TokenType.ROR))
            {
                Token op = _tokens[_position - 1];
                ExpressionNode right = ParseAdditive();
                expr = new BinaryExpressionNode { Left = expr, Operator = op.Type, Right = right, Line = expr.Line };
            }
            return expr;
        }

        private ExpressionNode ParseAdditive()
        {
            ExpressionNode expr = ParseMultiplicative();
            while (Match(TokenType.PLUS, TokenType.MINUS))
            {
                // Match called Advance(), so the matched token is at _position - 1
                Token op = _tokens[_position - 1];
                ExpressionNode right = ParseMultiplicative();
                expr = new BinaryExpressionNode { Left = expr, Operator = op.Type, Right = right, Line = expr.Line };
            }
            return expr;
        }

        private ExpressionNode ParseMultiplicative()
        {
            ExpressionNode expr = ParseUnary();
            while (Match(TokenType.MULTIPLY, TokenType.DIVIDE, TokenType.INT_DIVIDE, TokenType.MODULO))
            {
                // Match called Advance(), so the matched token is at _position - 1
                Token op = _tokens[_position - 1];
                ExpressionNode right = ParseUnary();
                expr = new BinaryExpressionNode { Left = expr, Operator = op.Type, Right = right, Line = expr.Line };
            }
            return expr;
        }

        private ExpressionNode ParseUnary()
        {
            if (Match(TokenType.MINUS, TokenType.NOT))
            {
                Token op = _tokens[_position - 1];
                ExpressionNode right = ParseUnary();
                return new UnaryExpressionNode { Operator = op.Type, Operand = right, Line = op.Line };
            }
            return ParsePower();
        }

        private ExpressionNode ParsePower()
        {
            ExpressionNode expr = ParsePrimary();
            if (Match(TokenType.POWER))
            {
                Token op = _tokens[_position - 1];
                ExpressionNode right = ParseUnary(); // Right-associative
                return new BinaryExpressionNode { Left = expr, Operator = op.Type, Right = right, Line = expr.Line };
            }
            return expr;
        }

        private ExpressionNode ParsePrimary()
        {
            if (Match(TokenType.LPAREN))
            {
                ExpressionNode expr = ParseExpression();
                Expect(TokenType.RPAREN);
                return expr;
            }

            if (Match(TokenType.INTEGER))
            {
                // Match called Advance(), so Previous() returns token before INTEGER
                // We need the INTEGER token itself, which is at _position - 1
                Token token = _tokens[_position - 1];
                try
                {
                    int value = ParseInteger(token.Value);
                    return new LiteralNode { Value = value, Type = VariableType.Integer, Line = token.Line };
                }
                catch (FormatException ex)
                {
                    throw new Exception($"Failed to parse integer from token '{token.Value}' (Type: {token.Type}) at line {token.Line}: {ex.Message}");
                }
            }

            if (Match(TokenType.FLOAT))
            {
                // Match called Advance(), so get the FLOAT token at _position - 1
                Token token = _tokens[_position - 1];
                try
                {
                    float value = ParseFloat(token.Value);
                    return new LiteralNode { Value = value, Type = VariableType.Float, Line = token.Line };
                }
                catch (FormatException ex)
                {
                    throw new Exception($"Failed to parse float from token '{token.Value}' (Type: {token.Type}) at line {token.Line}: {ex.Message}");
                }
            }

            if (Match(TokenType.STRING))
            {
                // Match called Advance(), so get the STRING token at _position - 1
                Token token = _tokens[_position - 1];
                string value = token.Value.Substring(1, token.Value.Length - 2); // Remove quotes
                return new LiteralNode { Value = value, Type = VariableType.String, Line = token.Line };
            }

            // Check for constants (PI, E) - these are tokenized as keywords but used as constants
            if (Match(TokenType.PI))
            {
                // PI is a constant, return as function call with no arguments
                return new FunctionCallNode { FunctionName = "PI", Line = _tokens[_position - 1].Line };
            }
            
            if (Match(TokenType.E))
            {
                // E is a constant, return as function call with no arguments
                return new FunctionCallNode { FunctionName = "E", Line = _tokens[_position - 1].Line };
            }

            // Check for MOUSE, TIME, TICKS as expressions
            if (Match(TokenType.MOUSE))
            {
                return ParseMouse();
            }

            if (Match(TokenType.TIME))
            {
                return ParseTime();
            }

            if (Match(TokenType.TICKS))
            {
                return ParseTicks();
            }

            // Check for INKEY as expression
            if (Match(TokenType.INKEY))
            {
                bool isString = false;
                if (Current().Type == TokenType.IDENTIFIER && Current().Value == "$")
                {
                    Advance(); // Consume $
                    isString = true;
                }
                return new InkeyNode { IsString = isString, Line = _tokens[_position - 1].Line };
            }

            // Check for PEEK, VARPTR as expressions
            if (Current().Type == TokenType.PEEK || Current().Type == TokenType.PEEK16 || 
                Current().Type == TokenType.PEEK24 || Current().Type == TokenType.PEEK32)
            {
                return ParsePeek(Current().Type);
            }

            if (Match(TokenType.VARPTR))
            {
                return ParseVarPtr();
            }

            // Check for function calls (tokenized as keywords like SIN, COS, etc.)
            TokenType[] functionTokens = {
                TokenType.ABS, TokenType.SGN, TokenType.INT, TokenType.FIX, TokenType.FLOOR, TokenType.CEIL, TokenType.ROUND,
                TokenType.SIN, TokenType.COS, TokenType.TAN, TokenType.ASIN, TokenType.ACOS, TokenType.ATAN, TokenType.ATAN2,
                TokenType.EXP, TokenType.LOG, TokenType.LOG10,
                TokenType.SQR, TokenType.POW, TokenType.ISQR,
                TokenType.MIN, TokenType.MAX,
                TokenType.RND, TokenType.RANDOMIZE,
                TokenType.LEN, TokenType.LEFT, TokenType.RIGHT, TokenType.MID, TokenType.CHR, TokenType.ASC,
                TokenType.VAL, TokenType.STR, TokenType.STRING_FUNC, TokenType.INSTR,
                TokenType.UCASE, TokenType.LCASE, TokenType.TRIM, TokenType.LTRIM, TokenType.RTRIM
            };
            
            if (functionTokens.Contains(Current().Type))
            {
                Token funcToken = Advance();
                string funcName = funcToken.Value.ToUpper();
                
                // Function call requires parentheses
                Expect(TokenType.LPAREN);
                FunctionCallNode func = new FunctionCallNode { FunctionName = funcName, Line = funcToken.Line };
                if (Current().Type != TokenType.RPAREN)
                {
                    do
                    {
                        func.Arguments.Add(ParseExpression());
                    } while (Match(TokenType.COMMA));
                }
                Expect(TokenType.RPAREN);
                return func;
            }

            // Check for function call or variable (identifier-based)
            if (Current().Type == TokenType.IDENTIFIER)
            {
                Token identifier = Advance();
                string name = identifier.Value;

                // Check if it's a function call
                if (Current().Type == TokenType.LPAREN)
                {
                    Advance(); // (
                    FunctionCallNode func = new FunctionCallNode { FunctionName = name, Line = identifier.Line };
                    if (Current().Type != TokenType.RPAREN)
                    {
                        do
                        {
                            func.Arguments.Add(ParseExpression());
                        } while (Match(TokenType.COMMA));
                    }
                    Expect(TokenType.RPAREN);
                    return func;
                }
                else
                {
                    // Variable
                    VariableNode var = new VariableNode { Name = name, Type = GetVariableType(name), Line = identifier.Line };
                    if (Match(TokenType.LBRACKET))
                    {
                        do
                        {
                            var.Indices.Add(ParseExpression());
                        } while (Match(TokenType.COMMA));
                        Expect(TokenType.RBRACKET);
                    }
                    return var;
                }
            }

            throw new Exception($"Unexpected token in expression: {Current().Type} (value: '{Current().Value}') at line {Current().Line}");
        }

        private VariableNode ParseVariable()
        {
            Token identifier = ExpectIdentifier();
            VariableNode var = new VariableNode { Name = identifier.Value, Type = GetVariableType(identifier.Value), Line = identifier.Line };
            if (Match(TokenType.LPAREN))
            {
                do
                {
                    var.Indices.Add(ParseExpression());
                } while (Match(TokenType.COMMA));
                Expect(TokenType.RPAREN);
            }
            return var;
        }

        private VariableType GetVariableType(string name)
        {
            if (name.EndsWith("$"))
                return VariableType.String;
            if (name.EndsWith("#"))
                return VariableType.Float;
            return VariableType.Integer;
        }

        private int ParseInteger(string value)
        {
            if (value.StartsWith("0x") || value.StartsWith("0X"))
                return Convert.ToInt32(value.Substring(2), 16);
            if (value.StartsWith("0b") || value.StartsWith("0B"))
                return Convert.ToInt32(value.Substring(2), 2);
            return int.Parse(value);
        }

        private float ParseFloat(string value)
        {
            return float.Parse(value);
        }

        // Helper methods
        private bool IsAtEnd() => Current().Type == TokenType.EOF;
        private Token Current() => _tokens[_position];
        private Token Advance() => _tokens[_position++];
        private Token Previous() => _position >= 2 ? _tokens[_position - 2] : _tokens[0];
        private Token Peek() => _position < _tokens.Count - 1 ? _tokens[_position + 1] : _tokens[_tokens.Count - 1];

        private bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Current().Type == type)
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private Token Expect(params TokenType[] types)
        {
            if (Match(types))
                return Previous();
            throw new Exception($"Expected one of {string.Join(", ", types)}, got {Current().Type}");
        }

        private Token ExpectIdentifier()
        {
            if (Current().Type == TokenType.IDENTIFIER)
                return Advance();
            throw new Exception($"Expected identifier, got {Current().Type}");
        }
    }
}

