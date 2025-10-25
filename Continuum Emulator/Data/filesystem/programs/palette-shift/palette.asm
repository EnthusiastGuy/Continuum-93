#include ..\..\lib\c93-keyboard.asm

	#ORG 0x20000

	LD Z, 4
	LD A, 2
	INT 1, Z
	; ABC should now have the address of the palette for video page 1
	LD (.VideoAddress), ABC
.Repeat
	CALLR .InputUpdate
	LD A, 27				; Escape key
	CALLR .InputKeyPressed
	CP A, 1
	JR Z, .exit
	
	LD DEF, (.VideoAddress)
	LD GH, 48	; Target only the first 16 colors
.ShiftPalette
	INC (DEF)	; Increment the colors in the palette
	INC DEF
	DEC GH
	CP GH, 0
	JP NZ, .ShiftPalette
	JP .Repeat

.exit
	RET

.VideoAddress
	#DB 0x000000
