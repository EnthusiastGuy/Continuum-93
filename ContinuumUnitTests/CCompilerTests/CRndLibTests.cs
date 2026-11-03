using static ContinuumUnitTests.CCompilerTests.CTestHarness;

namespace ContinuumUnitTests.CCompilerTests
{
    /// <summary>
    /// Compiles real-world code: the 32-bit generators of rnd.h by Mattias Gustavsson
    /// (https://github.com/mattiasgustavsson/libs, rnd.h v1.0, dual licensed MIT / public domain - Unlicense).
    /// The C below is copied verbatim from rnd.h (only the float and 64-bit parts are omitted, since the
    /// compiler has no floating point or 64-bit integers). Results are compared with a C# port of the same code.
    /// </summary>
    [Collection("CCompiler")]
    public class CRndLibTests
    {
        // ---- Excerpt of rnd.h (declarations + implementation), unmodified apart from omitted float/64-bit parts ----
        private const string RndH = @"
#ifndef rnd_h
#define rnd_h

#ifndef RND_U32
    #define RND_U32 unsigned int
#endif

typedef struct rnd_well_t { RND_U32 state[ 17 ]; } rnd_well_t;
void rnd_well_seed( rnd_well_t* well, RND_U32 seed );
RND_U32 rnd_well_next( rnd_well_t* well );

typedef struct rnd_gamerand_t { RND_U32 state[ 2 ]; } rnd_gamerand_t;
void rnd_gamerand_seed( rnd_gamerand_t* gamerand, RND_U32 seed );
RND_U32 rnd_gamerand_next( rnd_gamerand_t* gamerand );

#endif /* rnd_h */

#ifdef RND_IMPLEMENTATION
#undef RND_IMPLEMENTATION

static RND_U32 rnd_internal_murmur3_avalanche32( RND_U32 h )
    {
    h ^= h >> 16;
    h *= 0x85ebca6b;
    h ^= h >> 13;
    h *= 0xc2b2ae35;
    h ^= h >> 16;
    return h;
    }


void rnd_well_seed( rnd_well_t* well, RND_U32 seed )
    {
    RND_U32 value = rnd_internal_murmur3_avalanche32( ( seed << 1U ) | 1U );
    well->state[ 16 ] = 0;
    well->state[ 0 ] = value ^ 0xf68a9fc1U;
    for( int i = 1; i < 16; ++i )
        well->state[ i ] = ( 0x6c078965U * ( well->state[ i - 1 ] ^ ( well->state[ i - 1 ] >> 30 ) ) + i );
    }


RND_U32 rnd_well_next( rnd_well_t* well )
    {
    RND_U32 a = well->state[ well->state[ 16 ] ];
    RND_U32 c = well->state[ ( well->state[ 16 ] + 13 ) & 15 ];
    RND_U32 b = a ^ c ^ ( a << 16 ) ^ ( c << 15 );
    c = well->state[ ( well->state[ 16 ] + 9 ) & 15 ];
    c ^= ( c >> 11 );
    a = well->state[ well->state[ 16 ] ] = b ^ c;
    RND_U32 d = a ^ ( ( a << 5 ) & 0xda442d24U );
    well->state[ 16 ] = (well->state[ 16 ] + 15 ) & 15;
    a = well->state[ well->state[ 16 ] ];
    well->state[ well->state[ 16 ] ] = a ^ b ^ d ^ ( a << 2 ) ^ ( b << 18 ) ^ ( c << 28 );
    return well->state[ well->state[ 16 ] ];
    }


void rnd_gamerand_seed( rnd_gamerand_t* gamerand, RND_U32 seed )
    {
    RND_U32 value = rnd_internal_murmur3_avalanche32( ( seed << 1U ) | 1U );
    gamerand->state[ 0 ] = value;
    gamerand->state[ 1 ] = value ^ 0x49616e42U;
    }


RND_U32 rnd_gamerand_next( rnd_gamerand_t* gamerand )
    {
    gamerand->state[ 0 ] = ( gamerand->state[ 0 ] << 16 ) + ( gamerand->state[ 0 ] >> 16 );
    gamerand->state[ 0 ] += gamerand->state[ 1 ];
    gamerand->state[ 1 ] += gamerand->state[ 0 ];
    return gamerand->state[ 0 ];
    }

#endif /* RND_IMPLEMENTATION */
";
        // ---- C# reference port ----

        private static uint Murmur3(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }

        private static uint[] WellSeed(uint seed)
        {
            uint[] s = new uint[17];
            uint value = Murmur3((seed << 1) | 1);
            s[16] = 0;
            s[0] = value ^ 0xf68a9fc1U;
            for (int i = 1; i < 16; ++i)
                s[i] = 0x6c078965U * (s[i - 1] ^ (s[i - 1] >> 30)) + (uint)i;
            return s;
        }

        private static uint WellNext(uint[] s)
        {
            uint a = s[s[16]];
            uint c = s[(s[16] + 13) & 15];
            uint b = a ^ c ^ (a << 16) ^ (c << 15);
            c = s[(s[16] + 9) & 15];
            c ^= c >> 11;
            a = s[s[16]] = b ^ c;
            uint d = a ^ ((a << 5) & 0xda442d24U);
            s[16] = (s[16] + 15) & 15;
            a = s[s[16]];
            s[s[16]] = a ^ b ^ d ^ (a << 2) ^ (b << 18) ^ (c << 28);
            return s[s[16]];
        }

        private static uint[] GamerandSeed(uint seed)
        {
            uint value = Murmur3((seed << 1) | 1);
            return [value, value ^ 0x49616e42U];
        }

        private static uint GamerandNext(uint[] s)
        {
            s[0] = (s[0] << 16) + (s[0] >> 16);
            s[0] += s[1];
            s[1] += s[0];
            return s[0];
        }

        private const int Count = 64;

        private static CRunResult RunWithRnd(string main) =>
            Run($"#define RND_IMPLEMENTATION\n#include \"rnd.h\"\n{main}", new() { ["rnd.h"] = RndH });

        [Theory]
        [InlineData(0u)]
        [InlineData(1u)]
        [InlineData(12345u)]
        [InlineData(0xDEADBEEFu)]
        [InlineData(0xFFFFFFFFu)]
        public void GamerandSequenceMatchesReference(uint seed)
        {
            var r = RunWithRnd($@"
RND_U32 out[{Count}];
int main(void) {{
    rnd_gamerand_t g;
    rnd_gamerand_seed(&g, {seed}u);
    for (int i = 0; i < {Count}; ++i) out[i] = rnd_gamerand_next(&g);
    return 0;
}}");
            uint[] state = GamerandSeed(seed);
            for (int i = 0; i < Count; i++)
                Assert.Equal(GamerandNext(state), r.GlobalUInt("out", i));
        }

        [Theory]
        [InlineData(0u)]
        [InlineData(42u)]
        [InlineData(0x80000000u)]
        [InlineData(987654321u)]
        public void WellSequenceMatchesReference(uint seed)
        {
            var r = RunWithRnd($@"
RND_U32 out[{Count}];
rnd_well_t well;
int main(void) {{
    rnd_well_seed(&well, {seed}u);
    for (int i = 0; i < {Count}; ++i) out[i] = rnd_well_next(&well);
    return 0;
}}");
            uint[] state = WellSeed(seed);
            for (int i = 0; i < Count; i++)
                Assert.Equal(WellNext(state), r.GlobalUInt("out", i));

            // The whole 17-word generator state must match too
            for (int i = 0; i < 17; i++)
                Assert.Equal(state[i], r.GlobalUInt("well", i));
        }

        [Fact]
        public void MurmurAvalancheMatchesReferenceForManyInputs()
        {
            var r = RunWithRnd($@"
RND_U32 out[256];
int main(void) {{
    RND_U32 x = 1;
    for (int i = 0; i < 256; ++i) {{ out[i] = rnd_internal_murmur3_avalanche32(x); x = x * 2654435761u + (RND_U32)i; }}
    return 0;
}}");
            uint x = 1;
            for (int i = 0; i < 256; i++)
            {
                Assert.Equal(Murmur3(x), r.GlobalUInt("out", i));
                x = x * 2654435761u + (uint)i;
            }
        }

        [Fact]
        public void HeaderCanBeIncludedTwiceWithoutImplementation()
        {
            var r = Run("#include \"rnd.h\"\n#include \"rnd.h\"\n#define RND_IMPLEMENTATION\n#include \"rnd.h\"\nint main(void) { rnd_gamerand_t g; rnd_gamerand_seed(&g, 7u); return (int)(rnd_gamerand_next(&g) & 0x7FFF); }",
                new() { ["rnd.h"] = RndH });
            uint[] s = GamerandSeed(7);
            Assert.Equal((int)(GamerandNext(s) & 0x7FFF), r.ReturnValue);
        }
    }
}
