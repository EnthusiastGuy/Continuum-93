#include ..\..\lib\c93-keyboard.asm

    #ORG 0x100000

    LD A, 0x02  ; SetVideoPagesCount
    LD B, 1     ; number of pages (1-8)
    INT 0x01, A ; Trigger interrupt Video
    CALLR .ClearScreen

    ; Sets the video buffer control mode to manual
    ;LD A, 0x33
    ;LD B, 0b00000000
    ;INT 0x01, A

.Repeat

    CALLR .InputUpdate
    CALLR .InputNoStateChange
    CP A, 1
    JR Z, .no_key_input

    LD A, 27				; Escape key
    CALLR .InputKeyPressed
    CP A, 1
    JR Z, .exit

.no_key_input

    ;CALLR .ClearScreen
    CALLR .DrawPerlinLine
    
    JR .Repeat

.DrawPerlinLine
    PUSH A, Z

    LD A, 0x26
    LD B, 0     ; Video page
    LD CD, 10   ; x1
    LD EF, 100  ; y1
    LD GH, 470  ; x2
    LD IJ, 80   ; y2
    LD K, (.Color)  ; Color
    LD LM, 1214 ; Seed1
    LD NO, 60   ; Pattern 1 min Y
    LD PQ, 120  ; Pattern 1 max Y
    LD R, 5     ; Pattern 1 zoom
    LD ST, 45   ; Pattern 1 offset

    LD UV, 1214 ; Seed1
    LD WX, 60   ; Pattern 1 min Y
    LD YZ, 120  ; Pattern 1 max Y
    LD R, 5     ; Pattern 1 zoom
    LD ST, 45   ; Pattern 1 offset


    INT 0x01, A

    POP A, Z
    RET

.Color
    #DB 0x0F

.ClearScreen
    PUSH A, Z
    LD A, 0
    LD B, 0     ; Black
    LD Z, 0x05 	; ClearVideoPage
    INT 0x01, Z ; Trigger interrupt Video
    POP A, Z
    RET

.exit
		RET
