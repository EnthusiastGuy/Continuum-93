#include ..\..\lib\c93-keyboard.asm

; Sprite drawing. Given resolution is always 480 x 270, 8bpp

	#ORG 0x10000	; Code will be assembled at memory location 0x10000
	
.Restart
	
	CALLR .InputUpdate

	LD A, 27				; Escape key
	CALLR .InputKeyPressed
	CP A, 1
	JR Z, .exit
	

	LD D, 3		; Interrupt function selector 3
	INT 2, D	; Interrupt 2, function 3 (ReadMouseState)
				; Gets mouse x and y into EF and GH
				; (why EF and GH? Because they follow after the given D)

	CP GH, 270	; Lower mouse limit
	JR GT, .Restart	; If mouse y is greater than 270,
					; go back to ReadMouseState

	CP GH, 00	; Upper mouse limit
	JR LT, .Restart	; If mouse y is less than 20,
					; go back to ReadMouseState

	CP EF, 480	; Right-most mouse limit
	JR GT, .Restart	; If mouse x is greater than 460,
					; go back to ReadMouseState

	CP EF, 00	; Left-most mouse limit
	JR LT, .Restart	; If mouse x is less than 20,
					; go back to ReadMouseState

	
	; These 4 lines basically check whether the x, y are different
	; from the last coordinate used and if so, they prevent
	; redrawing
	CP MN, EF	
	JR NZ, .FoundDifferentXY
	CP OP, GH
	JR NZ, .FoundDifferentXY
	
	JR .Restart	; go back to ReadMouseState
.FoundDifferentXY
	
	LD MN, EF	; Save X to MN
	LD OP, GH	; Save y to OP

	LD W, 5		; Interrupt function selector 5: ClearVideoPage
	LD X, 0		; Video page to clear (0 - 8)
	LD Y, 0		; Color to clear with
	
	INT 1, W	; Trigger interrupt 1, function 5: Video/ClearVideoPage
	
	LD D, 0		; video page
	LD Z, 16	; Draw function
	LD ABC, .GfxData	; source of the icon data
	LD IJ, 7	; mouse icon width
	LD KL, 12	; mouse icon height
	INT 1, Z	; Trigger interrupt 1, function 16: DrawSprite
				; ABC contains the source icon, D the video page, EF, GH coords
				; IJ and KL contain the width and height
				
	JR .Restart	; Start over
.exit
	RET

	#ORG 0x20000
.GfxData	; The icon (1 byte per pixel, 7px x 12px icon)
	#DB 0xA6, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	#DB 0xA6, 0xA6, 0x00, 0x00, 0x00, 0x00, 0x00
	#DB 0xA6, 0x15, 0xA6, 0x00, 0x00, 0x00, 0x00
	#DB 0xA6, 0x15, 0x15, 0xA6, 0x00, 0x00, 0x00
	#DB 0xA6, 0x15, 0x15, 0x15, 0xA6, 0x00, 0x00
	#DB 0xA6, 0x15, 0x15, 0x15, 0x15, 0xA6, 0x00
	#DB 0xA6, 0x15, 0x15, 0x15, 0x15, 0x15, 0xA6
	#DB 0xA6, 0x15, 0x15, 0x15, 0xA6, 0xA6, 0xA6
	#DB 0xA6, 0x15, 0xA6, 0x15, 0xA6, 0x00, 0x00
	#DB 0xA6, 0xA6, 0x00, 0xA6, 0x15, 0xA6, 0x00
	#DB 0x00, 0x00, 0x00, 0xA6, 0x15, 0xA6, 0x00
	#DB 0x00, 0x00, 0x00, 0x00, 0xA6, 0xA6, 0x00
	