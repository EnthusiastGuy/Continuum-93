using System.Text;
using static ContinuumUnitTests.CCompilerTests.CTestHarness;

namespace ContinuumUnitTests.CCompilerTests
{
    /// <summary>
    /// Differential fuzzing: random C expressions are evaluated by a C# model of C semantics (32-bit int,
    /// two's complement wrap-around) and by compiled code running on the emulator; every result must match.
    /// Undefined behavior is avoided by construction (divisors are forced into 1..255, shift counts into 0..31).
    /// </summary>
    [Collection("CCompiler")]
    public class CDifferentialTests
    {
        private sealed class Expr
        {
            public string C = "";
            public bool Unsigned;
            public uint Value;   // Bit pattern of the value (as int or unsigned int)
        }

        private sealed class Gen
        {
            private readonly Random _rnd;
            private readonly (string Name, bool Unsigned, uint Value)[] _vars;

            public Gen(int seed, (string, bool, uint)[] vars)
            {
                _rnd = new Random(seed);
                _vars = vars;
            }

            public Expr Make(int depth)
            {
                if (depth <= 0 || _rnd.Next(10) < 2)
                    return Leaf();

                return _rnd.Next(12) switch
                {
                    0 => Unary(depth),
                    1 => CastExpr(depth),
                    2 => Compare(depth),
                    3 => Logical(depth),
                    4 => Ternary(depth),
                    5 => Shift(depth),
                    6 => Division(depth),
                    _ => Arith(depth)
                };
            }

            private Expr Leaf()
            {
                if (_rnd.Next(3) == 0)
                {
                    var v = _vars[_rnd.Next(_vars.Length)];
                    return new Expr { C = v.Name, Unsigned = v.Unsigned, Value = v.Value };
                }

                switch (_rnd.Next(4))
                {
                    case 0:
                        {
                            int small = _rnd.Next(-300, 300);
                            return new Expr { C = small < 0 ? $"({small})" : small.ToString(), Value = (uint)small };
                        }
                    case 1:
                        {
                            uint u = (uint)_rnd.NextInt64(0, 0x1_0000_0000);
                            return new Expr { C = $"0x{u:X}u", Unsigned = true, Value = u };
                        }
                    case 2:
                        {
                            int big = (int)_rnd.NextInt64(int.MinValue + 1L, int.MaxValue);
                            return new Expr { C = big < 0 ? $"({big})" : big.ToString(), Value = (uint)big };
                        }
                    default:
                        {
                            int[] edge = [0, 1, -1, int.MaxValue, 255, 256, 65535, 32767, -32768, -128, 127];
                            int e = edge[_rnd.Next(edge.Length)];
                            return new Expr { C = e < 0 ? $"({e})" : e.ToString(), Value = (uint)e };
                        }
                }
            }

            private static bool Common(Expr a, Expr b) => a.Unsigned || b.Unsigned;

            private Expr Arith(int depth)
            {
                Expr a = Make(depth - 1), b = Make(depth - 1);
                string[] ops = ["+", "-", "*", "&", "|", "^"];
                string op = ops[_rnd.Next(ops.Length)];
                uint v = op switch
                {
                    "+" => a.Value + b.Value,
                    "-" => a.Value - b.Value,
                    "*" => a.Value * b.Value,
                    "&" => a.Value & b.Value,
                    "|" => a.Value | b.Value,
                    _ => a.Value ^ b.Value
                };
                return new Expr { C = $"({a.C} {op} {b.C})", Unsigned = Common(a, b), Value = v };
            }

            private Expr Division(int depth)
            {
                Expr a = Make(depth - 1), d = Make(depth - 1);
                bool mod = _rnd.Next(2) == 0;
                // Divisor is forced into 1..255 (keeps its signedness so the operation's signedness still varies)
                uint dv = (d.Value & 0xFF) | 1;
                string divisor = $"(({d.C} & 0xFF) | 1)";
                bool uns = Common(a, d);
                uint v;
                if (uns)
                    v = mod ? a.Value % dv : a.Value / dv;
                else
                    v = (uint)(mod ? (int)a.Value % (int)dv : (int)a.Value / (int)dv);
                return new Expr { C = $"({a.C} {(mod ? "%" : "/")} {divisor})", Unsigned = uns, Value = v };
            }

            private Expr Shift(int depth)
            {
                Expr a = Make(depth - 1), n = Make(depth - 1);
                int count = (int)(n.Value & 31);
                bool left = _rnd.Next(2) == 0;
                uint v = left ? a.Value << count : a.Unsigned ? a.Value >> count : (uint)((int)a.Value >> count);
                return new Expr { C = $"({a.C} {(left ? "<<" : ">>")} ({n.C} & 31))", Unsigned = a.Unsigned, Value = v };
            }

            private Expr Unary(int depth)
            {
                Expr a = Make(depth - 1);
                switch (_rnd.Next(3))
                {
                    case 0: return new Expr { C = $"(-{a.C})", Unsigned = a.Unsigned, Value = 0u - a.Value };
                    case 1: return new Expr { C = $"(~{a.C})", Unsigned = a.Unsigned, Value = ~a.Value };
                    default: return new Expr { C = $"(!{a.C})", Value = a.Value == 0 ? 1u : 0u };
                }
            }

            private Expr CastExpr(int depth)
            {
                Expr a = Make(depth - 1);
                switch (_rnd.Next(7))
                {
                    case 0: return new Expr { C = $"((char){a.C})", Value = (uint)(sbyte)a.Value };
                    case 1: return new Expr { C = $"((unsigned char){a.C})", Value = (byte)a.Value };
                    case 2: return new Expr { C = $"((short){a.C})", Value = (uint)(short)a.Value };
                    case 3: return new Expr { C = $"((unsigned short){a.C})", Value = (ushort)a.Value };
                    case 4: return new Expr { C = $"((int){a.C})", Value = a.Value };
                    case 5: return new Expr { C = $"((_Bool){a.C})", Value = a.Value != 0 ? 1u : 0u };
                    default: return new Expr { C = $"((unsigned int){a.C})", Unsigned = true, Value = a.Value };
                }
            }

            private Expr Compare(int depth)
            {
                Expr a = Make(depth - 1), b = Make(depth - 1);
                string[] ops = ["<", "<=", ">", ">=", "==", "!="];
                string op = ops[_rnd.Next(ops.Length)];
                bool uns = Common(a, b);
                int cmp = uns ? a.Value.CompareTo(b.Value) : ((int)a.Value).CompareTo((int)b.Value);
                bool r = op switch
                {
                    "<" => cmp < 0,
                    "<=" => cmp <= 0,
                    ">" => cmp > 0,
                    ">=" => cmp >= 0,
                    "==" => cmp == 0,
                    _ => cmp != 0
                };
                return new Expr { C = $"({a.C} {op} {b.C})", Value = r ? 1u : 0u };
            }

            private Expr Logical(int depth)
            {
                Expr a = Make(depth - 1), b = Make(depth - 1);
                bool and = _rnd.Next(2) == 0;
                bool r = and ? a.Value != 0 && b.Value != 0 : a.Value != 0 || b.Value != 0;
                return new Expr { C = $"({a.C} {(and ? "&&" : "||")} {b.C})", Value = r ? 1u : 0u };
            }

            private Expr Ternary(int depth)
            {
                Expr c = Make(depth - 1), a = Make(depth - 1), b = Make(depth - 1);
                return new Expr { C = $"({c.C} ? {a.C} : {b.C})", Unsigned = Common(a, b), Value = c.Value != 0 ? a.Value : b.Value };
            }
        }

        private static (string, bool, uint)[] Variables(Random r) =>
        [
            ("i0", false, (uint)r.Next(-1000, 1000)),
            ("i1", false, (uint)r.Next()),
            ("i2", false, (uint)-r.Next()),
            ("u0", true, (uint)r.Next(0, 5000)),
            ("u1", true, (uint)r.NextInt64(0, 0x1_0000_0000)),
            ("g0", false, (uint)r.Next(-70000, 70000)),     // globals
            ("g1", true, (uint)r.NextInt64(0, 0x1_0000_0000)),
        ];

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        public void RandomExpressionsMatchReferenceSemantics(int seed)
        {
            const int count = 40;
            var vars = Variables(new Random(seed * 7919));
            var gen = new Gen(seed, vars);
            var exprs = Enumerable.Range(0, count).Select(i => gen.Make(2 + i % 4)).ToList();

            var src = new StringBuilder();
            src.AppendLine($"unsigned int results[{count}];");
            foreach (var (name, uns, value) in vars.Where(v => v.Item1.StartsWith("g")))
                src.AppendLine($"{(uns ? "unsigned int" : "int")} {name} = {(uns ? $"0x{value:X}u" : ((int)value).ToString())};");
            src.AppendLine("int main(void) {");
            foreach (var (name, uns, value) in vars.Where(v => !v.Item1.StartsWith("g")))
                src.AppendLine($"    {(uns ? "unsigned int" : "int")} {name} = {(uns ? $"0x{value:X}u" : $"({(int)value})")};");
            for (int i = 0; i < count; i++)
                src.AppendLine($"    results[{i}] = (unsigned int)({exprs[i].C});");
            src.AppendLine("    return 0;\n}");

            var r = Run(src.ToString(), maxSteps: 50_000_000);
            for (int i = 0; i < count; i++)
            {
                uint actual = r.GlobalUInt("results", i);
                Assert.True(exprs[i].Value == actual,
                    $"Mismatch for expression #{i}:\n  {exprs[i].C}\n  expected 0x{exprs[i].Value:X8}, got 0x{actual:X8}");
            }
        }

        [Theory]
        [InlineData(100)]
        [InlineData(200)]
        [InlineData(300)]
        public void RandomExpressionsAsConditionsMatchReference(int seed)
        {
            // Same expressions, but consumed by if/while conditions (exercises the branch code paths)
            const int count = 30;
            var vars = Variables(new Random(seed));
            var gen = new Gen(seed, vars);
            var exprs = Enumerable.Range(0, count).Select(i => gen.Make(1 + i % 4)).ToList();

            var src = new StringBuilder();
            src.AppendLine("unsigned int bits;");
            foreach (var (name, uns, value) in vars.Where(v => v.Item1.StartsWith("g")))
                src.AppendLine($"{(uns ? "unsigned int" : "int")} {name} = {(uns ? $"0x{value:X}u" : ((int)value).ToString())};");
            src.AppendLine("int main(void) {");
            foreach (var (name, uns, value) in vars.Where(v => !v.Item1.StartsWith("g")))
                src.AppendLine($"    {(uns ? "unsigned int" : "int")} {name} = {(uns ? $"0x{value:X}u" : $"({(int)value})")};");
            for (int i = 0; i < count; i++)
            {
                string form = (i % 3) switch
                {
                    0 => $"if ({exprs[i].C}) bits |= 1u << {i};",
                    1 => $"if (!({exprs[i].C})) {{ }} else bits |= 1u << {i};",
                    _ => $"{{ int once = 1; while (once && ({exprs[i].C})) {{ bits |= 1u << {i}; once = 0; }} }}"
                };
                src.AppendLine("    " + form);
            }
            src.AppendLine("    return 0;\n}");

            var r = Run(src.ToString(), maxSteps: 50_000_000);
            uint expected = 0;
            for (int i = 0; i < count; i++)
                if (exprs[i].Value != 0) expected |= 1u << i;
            Assert.Equal(expected, r.GlobalUInt("bits"));
        }
    }
}
