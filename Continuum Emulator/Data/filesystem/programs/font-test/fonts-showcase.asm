#include ..\..\lib\c93-keyboard.asm

	#ORG 0x080000       ; Start after 512k (which are reserved for the OS)

    LD A, 0x02              ; SetVideoPagesCount
    LD B, 1                 ; set 1 video page
    INT 0x01, A             ; Trigger interrupt Video with function SetVideoPagesCount

    LD A, 0x05     			; ClearVideoPage
	LD B, 0x00     			; the video page which needs clearing (0 - 7)
	LD C, 0x85     			; the color which will be used to fill that memory page (0 - transparent or 1 - 255).
	INT 0x01, A       		; Trigger interrupt Video

    LD A, 0x40
    LD BCD, .FontPath
    LD EFG, .FontData
    INT 0x04, A

    LDREGS A, R, (.DrawTextDrawParams)
    INT 0x01, A

.Repeat
    CALLR .InputUpdate              ; Update the keyboard input buffers to be able to determine changes

	LD A, 27				        ; Escape key
	CALLR .InputKeyPressed          ; Check if the keycode in A has just been pressed
	RETIF Z                         ; Exit if pressed

    JR .Repeat


.DrawTextDrawParams
    ; DrawText, font data, text, x, y, color, video page, max width, flags, outline color, outline pattern
    #DB 0x14, .FontData, .Text, 0080, 00100, 0x00, 0x00, 00320, 0b00111000, 0xFF, 0b11111111


.Text
    #DB "Ti VA AV Te Tc To Ts Tr. Fa Fc Fd Fe Fg Fi Fj Fm Fn Fo Fp Fq Fr Fs Fu Fv Fw Fx Fy Fz", 0
    ;A quick, jumpy, vexed maze, now! Fight bold-wizard by the gold box... 25801", 0

.FontPath
    #DB "fonts\ModernDOS-8x15.png", 0

.FontData
    #DB 0