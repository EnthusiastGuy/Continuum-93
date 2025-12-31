using System.Collections.Generic;

namespace Continuum93.Emulator.Compilers.C93Basic
{
    // Base AST node
    public abstract class ASTNode
    {
        public int Line { get; set; }
        public int Column { get; set; }
    }

    // Program node
    public class ProgramNode : ASTNode
    {
        public List<StatementNode> Statements { get; set; } = new List<StatementNode>();
    }

    // Statement nodes
    public abstract class StatementNode : ASTNode { }

    public class LabelNode : StatementNode
    {
        public string Name { get; set; }
    }

    public class AssignmentNode : StatementNode
    {
        public VariableNode Variable { get; set; }
        public ExpressionNode Expression { get; set; }
    }

    public class DimNode : StatementNode
    {
        public string VariableName { get; set; }
        public List<ExpressionNode> Dimensions { get; set; } = new List<ExpressionNode>();
        public VariableType Type { get; set; }
    }

    public class IfNode : StatementNode
    {
        public ExpressionNode Condition { get; set; }
        public List<StatementNode> ThenStatements { get; set; } = new List<StatementNode>();
        public List<ElseIfNode> ElseIfs { get; set; } = new List<ElseIfNode>();
        public List<StatementNode> ElseStatements { get; set; } = new List<StatementNode>();
    }

    public class ElseIfNode : StatementNode
    {
        public ExpressionNode Condition { get; set; }
        public List<StatementNode> Statements { get; set; } = new List<StatementNode>();
    }

    public class ForNode : StatementNode
    {
        public string VariableName { get; set; }
        public ExpressionNode Start { get; set; }
        public ExpressionNode End { get; set; }
        public ExpressionNode Step { get; set; }
        public List<StatementNode> Statements { get; set; } = new List<StatementNode>();
    }

    public class WhileNode : StatementNode
    {
        public ExpressionNode Condition { get; set; }
        public List<StatementNode> Statements { get; set; } = new List<StatementNode>();
    }

    public class RepeatNode : StatementNode
    {
        public ExpressionNode Condition { get; set; }
        public List<StatementNode> Statements { get; set; } = new List<StatementNode>();
    }

    public class GotoNode : StatementNode
    {
        public string Label { get; set; }
    }

    public class GosubNode : StatementNode
    {
        public string Label { get; set; }
    }

    public class ReturnNode : StatementNode { }

    public class ExitForNode : StatementNode { }
    public class ExitWhileNode : StatementNode { }
    public class ContinueForNode : StatementNode { }
    public class ContinueWhileNode : StatementNode { }

    public class PrintNode : StatementNode
    {
        public List<PrintItem> Items { get; set; } = new List<PrintItem>();
        public bool AtPosition { get; set; }
        public ExpressionNode AtX { get; set; }
        public ExpressionNode AtY { get; set; }
        public ExpressionNode FontAddr { get; set; }
        public ExpressionNode Color { get; set; }
        public ExpressionNode Flags { get; set; }
        public ExpressionNode MaxWidth { get; set; }
        public ExpressionNode OutlineColor { get; set; }
        public ExpressionNode OutlinePattern { get; set; }
        public ExpressionNode TabPosition { get; set; }
    }

    public class PrintItem
    {
        public ExpressionNode Expression { get; set; }
        public bool Semicolon { get; set; }
        public bool Comma { get; set; }
    }

    public class InputNode : StatementNode
    {
        public ExpressionNode Prompt { get; set; }
        public VariableNode Variable { get; set; }
    }

    public class ClsNode : StatementNode
    {
        public ExpressionNode Page { get; set; }
    }

    public class EndNode : StatementNode { }
    public class StopNode : StatementNode { }
    public class WaitNode : StatementNode
    {
        public ExpressionNode Frames { get; set; }
    }
    public class SleepNode : StatementNode
    {
        public ExpressionNode Milliseconds { get; set; }
    }

    // Command nodes (graphics, audio, etc.)
    public class PlotNode : StatementNode
    {
        public ExpressionNode X { get; set; }
        public ExpressionNode Y { get; set; }
        public ExpressionNode Color { get; set; }
    }

    public class LineNode : StatementNode
    {
        public ExpressionNode X1 { get; set; }
        public ExpressionNode Y1 { get; set; }
        public ExpressionNode X2 { get; set; }
        public ExpressionNode Y2 { get; set; }
        public ExpressionNode Color { get; set; }
    }

    public class RectangleNode : StatementNode
    {
        public ExpressionNode X { get; set; }
        public ExpressionNode Y { get; set; }
        public ExpressionNode Width { get; set; }
        public ExpressionNode Height { get; set; }
        public ExpressionNode Color { get; set; }
        public bool Filled { get; set; }
    }

    public class CircleNode : StatementNode
    {
        public ExpressionNode X { get; set; }
        public ExpressionNode Y { get; set; }
        public ExpressionNode Radius { get; set; }
        public ExpressionNode Color { get; set; }
        public bool Filled { get; set; }
    }

    public class EllipseNode : StatementNode
    {
        public ExpressionNode X { get; set; }
        public ExpressionNode Y { get; set; }
        public ExpressionNode RadiusX { get; set; }
        public ExpressionNode RadiusY { get; set; }
        public ExpressionNode Color { get; set; }
        public bool Filled { get; set; }
    }

    public class ScreenNode : StatementNode
    {
        public ExpressionNode Page { get; set; }
    }

    public class VideoNode : StatementNode
    {
        public ExpressionNode Pages { get; set; }
    }

    public class InkNode : StatementNode
    {
        public ExpressionNode Color { get; set; }
    }

    public class PaperNode : StatementNode
    {
        public ExpressionNode Color { get; set; }
    }

    public class BeepNode : StatementNode
    {
        public ExpressionNode Duration { get; set; }
        public ExpressionNode Pitch { get; set; }
        public ExpressionNode Volume { get; set; }
    }

    public class PlayNode : StatementNode
    {
        public ExpressionNode Address { get; set; }
    }

    public class LoadFontNode : StatementNode
    {
        public ExpressionNode Filename { get; set; }
        public ExpressionNode Address { get; set; }
    }

    public class FontNode : StatementNode
    {
        public ExpressionNode Address { get; set; }
    }

    public class FontColorNode : StatementNode
    {
        public ExpressionNode Color { get; set; }
    }

    public class FontFlagsNode : StatementNode
    {
        public ExpressionNode Flags { get; set; }
    }

    public class FontMaxWidthNode : StatementNode
    {
        public ExpressionNode MaxWidth { get; set; }
    }

    public class FontOutlineNode : StatementNode
    {
        public ExpressionNode Color { get; set; }
        public ExpressionNode Pattern { get; set; }
    }

    public class LayerShowNode : StatementNode
    {
        public ExpressionNode Layer { get; set; }
    }

    public class LayerHideNode : StatementNode
    {
        public ExpressionNode Layer { get; set; }
    }

    public class LayerVisibilityNode : StatementNode
    {
        public ExpressionNode Mask { get; set; }
    }

    public class SpriteNode : StatementNode
    {
        public ExpressionNode X { get; set; }
        public ExpressionNode Y { get; set; }
        public ExpressionNode Address { get; set; }
        public ExpressionNode Width { get; set; }
        public ExpressionNode Height { get; set; }
        public ExpressionNode Page { get; set; }
    }

    public class LocateNode : StatementNode
    {
        public ExpressionNode X { get; set; }
        public ExpressionNode Y { get; set; }
    }

    public class InkeyNode : ExpressionNode
    {
        public bool IsString { get; set; } // INKEY$ vs INKEY
    }

    public class MouseXNode : ExpressionNode { }
    public class MouseYNode : ExpressionNode { }
    public class MouseButtonNode : ExpressionNode
    {
        public ExpressionNode Button { get; set; }
    }

    public class DataNode : StatementNode
    {
        public List<ExpressionNode> Values { get; set; } = new List<ExpressionNode>();
    }

    public class PeekNode : ExpressionNode
    {
        public ExpressionNode Address { get; set; }
        public int Size { get; set; } // 1, 16, 24, or 32
    }

    public class PokeNode : StatementNode
    {
        public ExpressionNode Address { get; set; }
        public ExpressionNode Value { get; set; }
        public int Size { get; set; } // 1, 16, 24, or 32
    }

    public class MemCopyNode : StatementNode
    {
        public ExpressionNode Source { get; set; }
        public ExpressionNode Dest { get; set; }
        public ExpressionNode Length { get; set; }
    }

    public class MemFillNode : StatementNode
    {
        public ExpressionNode Address { get; set; }
        public ExpressionNode Length { get; set; }
        public ExpressionNode Value { get; set; }
    }

    public class VarPtrNode : ExpressionNode
    {
        public VariableNode Variable { get; set; }
    }

    public class TimeNode : ExpressionNode { }
    public class TicksNode : ExpressionNode { }

    // Expression nodes
    public abstract class ExpressionNode : ASTNode { }

    public class BinaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public TokenType Operator { get; set; }
        public ExpressionNode Right { get; set; }
    }

    public class UnaryExpressionNode : ExpressionNode
    {
        public TokenType Operator { get; set; }
        public ExpressionNode Operand { get; set; }
    }

    public class LiteralNode : ExpressionNode
    {
        public object Value { get; set; }
        public VariableType Type { get; set; }
    }

    public class VariableNode : ExpressionNode
    {
        public string Name { get; set; }
        public List<ExpressionNode> Indices { get; set; } = new List<ExpressionNode>();
        public VariableType Type { get; set; }
    }

    public class FunctionCallNode : ExpressionNode
    {
        public string FunctionName { get; set; }
        public List<ExpressionNode> Arguments { get; set; } = new List<ExpressionNode>();
    }

    public enum VariableType
    {
        Integer,
        Float,
        String
    }
}

