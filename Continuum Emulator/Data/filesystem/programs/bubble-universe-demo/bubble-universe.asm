#include ..\..\lib\c93-keyboard.asm

    #ORG 0x100000

    LD F0, 0.0              ; u
    LD F7, 0.0267369595     ; r (tau/235)
    LD F8, 0.0              ; x
    LD F9, 0.0              ; y
    LD F10, 0.0             ; v
    LD F11, 0.0             ; t
    LD F12, 0.0             ; oldV

    LD A, 0x02  ; SetVideoPagesCount
    LD B, 1     ; number of pages (1-8)
    INT 0x01, A ; Trigger interrupt Video
    CALLR .ClearScreen
    CALLR .GeneratePalette

    ; Sets the video buffer control mode to manual
    LD A, 0x33
    LD B, 0b00000000
    INT 0x01, A

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

    CALLR .ClearScreen
    
    LD A, 0     ; i
.Outer
    LD B, 0     ; j
.Inner
    LD F0, A    ; i
    ADD F0, F12 ; i + oldV
    LD F10, F0
    SIN F0      ; SIN(i+v)

    LD F1, F7
    MUL F1, A   ; r * i
    ADD F1, F8  ; r * i + x
    LD F2, F1   
    SIN F1      ; SIN(r * i + x)
    ADD F0, F1  ; u = SIN(i+v) + SIN(r*i+x)

    COS F10     ; COS(i+v)
    COS F2      ; COS(r*i+x)

    ADD F10, F2 ; v = COS(i+v) + COS(r*i+x)
    LD F12, F10 ; save v to oldV

    LD F8, F0
    ADD F8, F11 ; x = u + t

    ; Calculate the color to plot with
    LD R, A     ; i
    DIV R, 12   
    RL R, 4     ; set leftmost 4 bits as the red color determined by i
    LD S, B
    DIV S, 12
    OR R, S
    LD (.Color), R
    CALL .Plot

    INC B
    CP B, 192
    JR NZ, .Inner
    INC A
    CP A, 192
    JR NZ, .Outer

    ADD F11, 0.0015

    VDL 0b00000001  ; Manually draw the video frame to the render buffer

    JR .Repeat

.Plot
    PUSH A, Z

    LD A, 0x20
    LD B, 0     ; Video page
    MUL F0, 65.0
    LD CD, F0   ; x
    ADD CD, 240
    MUL F10, 65.0
    LD EF, F10  ; y
    ADD EF, 135
    LD G, (.Color)
    INT 0x01, A

    POP A, Z
    RET

.Color
    #DB 0x00

.ClearScreen
    PUSH A, Z
    LD A, 0
    LD B, 0     ; Black
    LD Z, 0x05 	; ClearVideoPage
    INT 0x01, Z ; Trigger interrupt Video
    POP A, Z
    RET

.GeneratePalette
    LD XYZ, 0xFE02C0    ; Palette address here

    LD (XYZ), 0, 3     ; Pitch black
    ADD XYZ, 3
    LD I, 0
    LD J, 0
.NextColor
    LD R, I         ; Green index
    LD G, J
    MUL R, 12
    MUL G, 12
    LD (XYZ), R
    INC XYZ
    LD (XYZ), G
    INC XYZ
    LD (XYZ), 99, 1
    INC XYZ
    INC I
    CP I, 16
    JR NZ, .NextColor
    LD I, 0
    INC J
    CP J, 16
    JR NZ, .NextColor
    RET

.exit
		RET
