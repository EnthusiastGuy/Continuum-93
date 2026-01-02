#include ..\..\lib\c93-keyboard.asm

; Video fill test

	#ORG 0x80000

	LD Z, 50	; Some rectangle dimensions and limits
	LD M, 3
	LD B, 1
	LD CDEF, 0xFFA0FFA0	; x, y
	LD GH, 100
	LD IJ, 56
	LD K, 0xAB
	LD L, 7
.Repeat
	CALLR .InputUpdate
	LD A, 27				; Escape key
	CALLR .InputKeyPressed
	CP A, 1
	JR Z, .Exit
	
	LD A, 8
	INT 1, A	; DrawFilledRectangle at
				; subsequent registers coordinates

	LD A, 9
	LD K, 0xFF
	INT 1, A	; DrawFilledRectangle at
				; subsequent registers coordinates
	
	LD NO, 380
	INT 3, M	; Random into register NO
	LD CD, NO
	LD NO, 210
	INT 3, M	; Random
	LD EF, NO
	
	LD K, 0
	INT 3, K	; Random again
	INC K
	CP Z, 0
	JP Z, .Skip
	DEC Z
	JP .Repeat
.Skip
	LD B, 0
	JP .Repeat
.Exit
	RET