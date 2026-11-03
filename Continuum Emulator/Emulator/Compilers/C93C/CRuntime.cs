namespace Continuum93.Emulator.Compilers.C93C
{
    /// <summary>
    /// Built-in headers and the C runtime library. The library is plain C compiled together with the user's
    /// program; only functions reachable from main are emitted, and user definitions replace library ones.
    /// </summary>
    public static class CRuntime
    {
        public static string GetHeader(string name) => name switch
        {
            "stddef.h" => StdDef,
            "stdint.h" => StdInt,
            "stdbool.h" => StdBool,
            "limits.h" => Limits,
            "string.h" => StringH,
            "stdlib.h" => StdLib,
            "ctype.h" => CType,
            _ => null
        };

        private const string StdDef = @"
#ifndef __C93_STDDEF_H
#define __C93_STDDEF_H
typedef unsigned int size_t;
typedef int ptrdiff_t;
#define NULL ((void *)0)
#define offsetof(type, member) ((size_t)&((type *)0)->member)
#endif
";

        private const string StdInt = @"
#ifndef __C93_STDINT_H
#define __C93_STDINT_H
typedef signed char int8_t;
typedef unsigned char uint8_t;
typedef short int16_t;
typedef unsigned short uint16_t;
typedef int int32_t;
typedef unsigned int uint32_t;
typedef int intptr_t;
typedef unsigned int uintptr_t;
#define INT8_MIN (-128)
#define INT8_MAX 127
#define UINT8_MAX 255
#define INT16_MIN (-32768)
#define INT16_MAX 32767
#define UINT16_MAX 65535
#define INT32_MIN (-2147483647 - 1)
#define INT32_MAX 2147483647
#define UINT32_MAX 4294967295u
#endif
";

        private const string StdBool = @"
#ifndef __C93_STDBOOL_H
#define __C93_STDBOOL_H
#define bool _Bool
#define true 1
#define false 0
#endif
";

        private const string Limits = @"
#ifndef __C93_LIMITS_H
#define __C93_LIMITS_H
#define CHAR_BIT 8
#define SCHAR_MIN (-128)
#define SCHAR_MAX 127
#define UCHAR_MAX 255
#define CHAR_MIN (-128)
#define CHAR_MAX 127
#define SHRT_MIN (-32768)
#define SHRT_MAX 32767
#define USHRT_MAX 65535
#define INT_MIN (-2147483647 - 1)
#define INT_MAX 2147483647
#define UINT_MAX 4294967295u
#define LONG_MIN (-2147483647 - 1)
#define LONG_MAX 2147483647
#define ULONG_MAX 4294967295u
#endif
";

        private const string StringH = @"
#ifndef __C93_STRING_H
#define __C93_STRING_H
#include <stddef.h>
void *memset(void *dst, int c, size_t n);
void *memcpy(void *dst, const void *src, size_t n);
void *memmove(void *dst, const void *src, size_t n);
int memcmp(const void *a, const void *b, size_t n);
size_t strlen(const char *s);
char *strcpy(char *dst, const char *src);
char *strncpy(char *dst, const char *src, size_t n);
char *strcat(char *dst, const char *src);
int strcmp(const char *a, const char *b);
int strncmp(const char *a, const char *b, size_t n);
char *strchr(const char *s, int c);
#endif
";

        private const string StdLib = @"
#ifndef __C93_STDLIB_H
#define __C93_STDLIB_H
#include <stddef.h>
#define RAND_MAX 32767
void *malloc(size_t size);
void *calloc(size_t count, size_t size);
void free(void *p);
int abs(int x);
int atoi(const char *s);
int rand(void);
void srand(unsigned int seed);
#endif
";

        private const string CType = @"
#ifndef __C93_CTYPE_H
#define __C93_CTYPE_H
int isdigit(int c);
int isalpha(int c);
int isalnum(int c);
int isspace(int c);
int isupper(int c);
int islower(int c);
int toupper(int c);
int tolower(int c);
#endif
";

        public const string LibrarySource = @"
#include <stddef.h>
#include <string.h>
#include <stdlib.h>
#include <ctype.h>

/* The bump allocator hands out memory after the program image; free() is a no-op. */
extern char __heap_start[];
static char *__c93_heap_ptr;
static unsigned int __c93_rand_state = 1;

void *malloc(size_t size)
{
    char *p;
    if (__c93_heap_ptr == 0)
        __c93_heap_ptr = __heap_start;
    p = __c93_heap_ptr;
    __c93_heap_ptr += size;
    return p;
}

void *calloc(size_t count, size_t size)
{
    void *p = malloc(count * size);
    memset(p, 0, count * size);
    return p;
}

void free(void *p)
{
    (void)p;
}

void *memset(void *dst, int c, size_t n)
{
    unsigned char *d = dst;
    while (n--)
        *d++ = (unsigned char)c;
    return dst;
}

void *memcpy(void *dst, const void *src, size_t n)
{
    unsigned char *d = dst;
    const unsigned char *s = src;
    while (n--)
        *d++ = *s++;
    return dst;
}

void *memmove(void *dst, const void *src, size_t n)
{
    unsigned char *d = dst;
    const unsigned char *s = src;
    if (d < s)
    {
        while (n--)
            *d++ = *s++;
    }
    else
    {
        d += n;
        s += n;
        while (n--)
            *--d = *--s;
    }
    return dst;
}

int memcmp(const void *a, const void *b, size_t n)
{
    const unsigned char *x = a;
    const unsigned char *y = b;
    for (; n; n--, x++, y++)
        if (*x != *y)
            return *x - *y;
    return 0;
}

size_t strlen(const char *s)
{
    const char *p = s;
    while (*p)
        p++;
    return p - s;
}

char *strcpy(char *dst, const char *src)
{
    char *r = dst;
    while ((*dst++ = *src++) != 0)
        ;
    return r;
}

char *strncpy(char *dst, const char *src, size_t n)
{
    size_t i = 0;
    for (; i < n && src[i]; i++)
        dst[i] = src[i];
    for (; i < n; i++)
        dst[i] = 0;
    return dst;
}

char *strcat(char *dst, const char *src)
{
    strcpy(dst + strlen(dst), src);
    return dst;
}

int strcmp(const char *a, const char *b)
{
    while (*a && *a == *b)
    {
        a++;
        b++;
    }
    return (unsigned char)*a - (unsigned char)*b;
}

int strncmp(const char *a, const char *b, size_t n)
{
    for (; n; n--, a++, b++)
    {
        if (*a != *b)
            return (unsigned char)*a - (unsigned char)*b;
        if (!*a)
            return 0;
    }
    return 0;
}

char *strchr(const char *s, int c)
{
    for (;; s++)
    {
        if (*s == (char)c)
            return (char *)s;
        if (!*s)
            return 0;
    }
}

int abs(int x)
{
    return x < 0 ? -x : x;
}

int atoi(const char *s)
{
    int sign = 1, v = 0;
    while (isspace(*s))
        s++;
    if (*s == '-' || *s == '+')
    {
        if (*s == '-')
            sign = -1;
        s++;
    }
    while (isdigit(*s))
        v = v * 10 + (*s++ - '0');
    return sign * v;
}

int rand(void)
{
    __c93_rand_state = __c93_rand_state * 1103515245u + 12345u;
    return (__c93_rand_state >> 16) & 0x7FFF;
}

void srand(unsigned int seed)
{
    __c93_rand_state = seed;
}

int isdigit(int c) { return c >= '0' && c <= '9'; }
int isalpha(int c) { return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'); }
int isalnum(int c) { return isalpha(c) || isdigit(c); }
int isspace(int c) { return c == ' ' || c == '\t' || c == '\n' || c == '\r' || c == '\f' || c == '\v'; }
int isupper(int c) { return c >= 'A' && c <= 'Z'; }
int islower(int c) { return c >= 'a' && c <= 'z'; }
int toupper(int c) { return islower(c) ? c - 32 : c; }
int tolower(int c) { return isupper(c) ? c + 32 : c; }
";
    }
}
