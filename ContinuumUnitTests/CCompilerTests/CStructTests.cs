using static ContinuumUnitTests.CCompilerTests.CTestHarness;

namespace ContinuumUnitTests.CCompilerTests
{
    [Collection("CCompiler")]
    public class CStructTests
    {
        [Fact]
        public void MembersAndPackedLayout() =>
            Assert.Equal(1, Eval(@"
#include <stddef.h>
struct S { char c; int i; short s; char tail[3]; };
int main(void) {
    struct S v;
    v.c = -1; v.i = 100000; v.s = -2; v.tail[2] = 9;
    return v.c == -1 && v.i == 100000 && v.s == -2 && v.tail[2] == 9
        && sizeof(struct S) == 10 && offsetof(struct S, i) == 1 && offsetof(struct S, s) == 5 && offsetof(struct S, tail) == 7;
}"));

        [Fact]
        public void NestedStructsAndArrow() =>
            Assert.Equal(3 + 4 * 10 + 7 * 100, Eval(@"
typedef struct { int x, y; } Point;
typedef struct { Point min, max; int tag; } Rect;
int area(const Rect *r) { return (r->max.x - r->min.x) * (r->max.y - r->min.y); }
int main(void) {
    Rect r;
    Rect *p = &r;
    p->min.x = 1; p->min.y = 2; r.max.x = 4; r.max.y = 6;
    r.tag = 7;
    return (r.max.x - r.min.x) + (p->max.y - p->min.y) * 10 + p->tag * 100 + area(p) * 0;
}"));

        [Fact]
        public void StructInitializersAndArraysOfStructs() =>
            Assert.Equal(1, Eval(@"
struct Item { const char *name; int qty; int price[2]; };
struct Item items[] = { { ""apple"", 3, {10, 11} }, { ""pear"", 5 } };
int main(void) {
    struct Item local = { ""fig"", 2, {7, 8} };
    struct Item partial = { 0 };
    return items[0].qty == 3 && items[0].price[1] == 11 && items[1].qty == 5 && items[1].price[0] == 0
        && items[1].name[1] == 'e' && local.price[1] == 8 && local.name[0] == 'f'
        && partial.qty == 0 && partial.name == 0 && sizeof(items) / sizeof(items[0]) == 2;
}"));

        [Fact]
        public void StructAssignmentCopiesAllBytes() =>
            Assert.Equal(1, Eval(@"
struct V { int a; char b; int c[3]; };
struct V g;
int main(void) {
    struct V x = { 1, 2, {3, 4, 5} };
    struct V y;
    struct V *py = &y;
    y = x;
    x.c[1] = 99;                 /* copy is independent */
    g = *py;
    struct V z = g;              /* initialization from another struct */
    return y.a == 1 && y.b == 2 && y.c[1] == 4 && y.c[2] == 5 && g.c[0] == 3 && z.c[2] == 5 && x.c[1] == 99;
}"));

        [Fact]
        public void Unions() =>
            Assert.Equal(1, Eval(@"
union U { unsigned int word; unsigned char bytes[4]; short half; };
int main(void) {
    union U u;
    u.word = 0x11223344u;
    return sizeof(union U) == 4 && u.bytes[0] == 0x11 && u.bytes[3] == 0x44 && u.half == 0x1122;
}"));

        [Fact]
        public void AnonymousUnionMembers() =>
            Assert.Equal(1, Eval(@"
struct Value { int kind; union { int i; char c; }; };
int main(void) { struct Value v; v.kind = 1; v.i = 0x41424344; return v.c == 0x41 && sizeof(struct Value) == 8; }"));

        [Fact]
        public void LinkedListWithMalloc() =>
            Assert.Equal(55 + 10 * 1000, Eval(@"
#include <stdlib.h>
struct Node { int value; struct Node *next; };
struct Node *push(struct Node *head, int v) {
    struct Node *n = malloc(sizeof(struct Node));
    n->value = v;
    n->next = head;
    return n;
}
int main(void) {
    struct Node *list = 0;
    for (int i = 1; i <= 10; i++) list = push(list, i);
    int sum = 0, count = 0;
    for (struct Node *p = list; p; p = p->next) { sum += p->value; count++; }
    return sum + count * 1000;
}"));

        [Fact]
        public void BinaryTreeRecursion() =>
            Assert.Equal(7 * 100 + 3, Eval(@"
#include <stdlib.h>
typedef struct Tree { int key; struct Tree *left, *right; } Tree;
Tree *insert(Tree *t, int key) {
    if (!t) { t = calloc(1, sizeof(Tree)); t->key = key; return t; }
    if (key < t->key) t->left = insert(t->left, key); else t->right = insert(t->right, key);
    return t;
}
int count(Tree *t) { return t ? 1 + count(t->left) + count(t->right) : 0; }
int depth(Tree *t) { if (!t) return 0; int l = depth(t->left), r = depth(t->right); return 1 + (l > r ? l : r); }
int main(void) {
    int keys[] = { 50, 30, 70, 20, 40, 60, 80 };
    Tree *root = 0;
    for (int i = 0; i < 7; i++) root = insert(root, keys[i]);
    return count(root) * 100 + depth(root);
}"));

        [Fact]
        public void ForwardDeclaredAndMutuallyRecursiveStructs() =>
            Assert.Equal(42, Eval(@"
struct B;
struct A { struct B *b; int v; };
struct B { struct A *a; int v; };
int main(void) { struct A a; struct B b; a.b = &b; b.a = &a; a.v = 40; b.v = 2; return a.b->a->v + a.b->v; }"));

        [Fact]
        public void StructPointersInFunctionsModifyCaller() =>
            Assert.Equal(1, Eval(@"
typedef struct { int balance; int ops; } Account;
void deposit(Account *a, int n) { a->balance += n; a->ops++; }
int main(void) { Account acc = { 100, 0 }; deposit(&acc, 50); deposit(&acc, -30); return acc.balance == 120 && acc.ops == 2; }"));

        [Fact]
        public void EnumsAndTypedefs() =>
            Assert.Equal(1, Eval(@"
typedef unsigned char byte;
typedef byte buffer[4];
enum Dir { NORTH, EAST = 10, SOUTH, WEST = EAST * 3 };
int main(void) {
    buffer b = { 1, 2, 3, 255 };
    enum Dir d = SOUTH;
    return NORTH == 0 && d == 11 && WEST == 30 && sizeof(buffer) == 4 && b[3] == 255 && sizeof(enum Dir) == 4;
}"));

        [Fact]
        public void BlockScopedStructTags() =>
            Assert.Equal(2 + 10, Eval(@"
struct T { int a; };
int main(void) {
    struct T outer = { 2 };
    int r;
    { struct T { char x[10]; } inner; r = (int)sizeof(inner); }
    return outer.a + r;
}"));
    }
}
