#include ..\..\lib\c93-keyboard.asm
; Random lines draw

	#ORG 0x80000

	LD Z, 0	; Some rectangle dimensions and limits
	LD M, 3
	LD B, 0
	LD CDEF, 0x00A000A0	; x1, y1
	LD GHIJ, 0x00B000B0 ; x2, y2
	LD K, 0xAB
.Repeat
	CALLR .InputUpdate
	LD A, 27				; Escape key
	CALLR .InputKeyPressed
	CP A, 1
	JR Z, .Exit
		
	LD A, 0x22
	INT 1, A	; DrawFilledRectangle at
				; subsequent registers coordinates
	
	LD NO, 480
	INT 3, M	; Random into register NO
	LD CD, NO
    LD NO, 480
	INT 3, M	; Random into register NO
    LD GH, NO
	LD NO, 270
	INT 3, M	; Random
	LD EF, NO
    LD NO, 270
	INT 3, M	; Random
    LD IJ, NO
	
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