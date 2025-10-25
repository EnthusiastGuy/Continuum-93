#include "includes\keyb.asm"
#include "includes\data.asm"

	#ORG 80000

	LD A, 0x05     			; ClearVideoPage
	LD B, 0x00     			; the video page which needs clearing (0 - 7)
	LD C, 0x00     			; the color which will be used to fill that memory page (0 - transparent or 1 - 255).
	INT 0x01, A       		; Trigger interrupt Video

	LD A, 0x24     			; LinePath
	LD BCD, .Points     	; the RAM address where the points are stored.
	LD E, 0x00     			; the video page on which we draw (0 - 7)
	LD F, 0xAA     			; the fill color of the ellipse.
	INT 0x01, A       		; Trigger interrupt Video

.Repeat
	CALLR .InputUpdate
	LD A, 27				; Escape key
	CALLR .InputKeyPressed
	RETIF Z
	JR .Repeat
