#include ..\..\lib\c93-keyboard.asm
; 3D Cube draw

	#ORG 0x80000

	LD F0, -0.1		; rx -> rotation angle x
	LD F1, 40.0		; l
	LD F2, 200.0		; fs

	; Sets the video buffer control mode to manual
    LD A, 0x33
    LD B, 0b00000000
    INT 0x01, A

.Repeat
	CALLR .InputUpdate
	LD A, 27				; Escape key
	CALLR .InputKeyPressed
	CP A, 1
	JR Z, .Exit

	LD A, 0x05     ; ClearVideoPage
	LD B, 1     ; the video page which needs clearing (1-8)
	LD C, 0     ; the color which will be used to fill that memory page (0 - transparent or 1 - 255).
	INT 0x01, A       ; Trigger interrupt Video

	CALLR .InitCube

	ADD F0, 0.01		; rx = rx + 0.1
	COS F3, F0		; c -> cos(rx)
	SIN F4, F0		; s -> sin(rx)
	
	LD B, 0
.ComputeCube
	;F5 -> xt
	;F6 -> yt

	LD I, B
	MUL I, 4

	LD JKL, .X
	LD MNO, .Y
	LD PQR, .Z
	LD STU, .vX
	LD VWX, .vY
	
	ADD JKL, I	; x(b)
	ADD MNO, I	; y(b)
	ADD PQR, I	; z(b)
	ADD STU, I	; vx(b)
	ADD VWX, I	; vy(b)
	
	; Rotation on X axes: yt = y(b): y(b) = c*yt-s*z(b): z(b) = s*yt + c*z(b)
	; yt = y(b)
	LD F6, (MNO)	
	
	; y(b) = c*yt-s*z(b)
	LD F8, (PQR)	; get z(b)
	MUL F8, F4		; z(b) * s
	
	LD F7, F6		; yt
	MUL F7, F3		; yt * c
	SUB F7, F8		; yt * c - z(b) * s
	
	LD (MNO), F7	; y(b) = yt * c - z(b) * s
	
	; z(b) = s*yt + c*z(b)
	LD F8, (PQR)	; get z(b)
	MUL F8, F3		; z(b) * c
	
	LD F7, F6		; yt
	MUL F7, F4		; yt * s
	
	ADD F7, F8		; yt * s + z(b) * c
	
	LD (PQR), F7	; z(b) = yt * s + z(b) * c
	
	
	; Rotation on y axes: xt = x(b): x(b) = c*xt+s*z(b): z(b)=-s*xt+c*z(b)
	LD F5, (JKL)	; xt = x(b)
	
	LD F8, (PQR)	; get z(b)
	MUL F8, F4		; z(b) * s
	
	LD F7, F5		; xt
	MUL F7, F3		; xt * c
	ADD F7, F8		; xt * c + z(b) * s
	
	LD (JKL), F7	; x(b) = xt * c + z(b) * s
	
	LD F8, (PQR)	; get z(b)
	MUL F8, F3		; z(b) * c
	
	LD F7, F5		; xt
	MUL F7, F4		; xt * s
	
	SUB F8, F7		; z(b) * c - xt * s
	
	LD (PQR), F8	; z(b) = z(b) * c - xt * s
	
	; Rotation on z axes: xt = x(b) :x(b)=c*xt-s*y(b) : y(b) = s*xt+c*y(b)
	LD F5, (JKL)	; xt = x(b)
	
	LD F8, (MNO)	; get y(b)
	MUL F8, F4		; y(b) * s
	
	LD F7, F5		; xt
	MUL F7, F3		; xt * c
	SUB F7, F8		; xt * c - y(b) * s
	
	LD (JKL), F7	; x(b) = xt * c - y(b) * s
	
	LD F8, (MNO)	; get y(b)
	MUL F8, F3		; y(b) * c
	
	LD F7, F5		; xt
	MUL F7, F4		; xt * s
	
	ADD F7, F8		; xt * s + y(b) * c
	
	LD (MNO), F7	; y(b) = xt * s + y(b) * c
	
	; Perspective projection
	LD F7, (MNO)	; z(b)
	ADD F7, F2		; z(b) + fs
	LD F8, 6000.0	
	DIV F8, F7		; f = 6000.0 / (z(b) + fs) -> perspective projection value

	LD F9, (JKL)	; x(b)
	MUL F9, F8		; x(b) * f

	ADD F9, 320.0	; x(b) * f + 320

	
	LD (STU), F9	; vx(b) = x(b) * f + 320

	LD F9, (MNO)	; y(b)
	MUL F9, F8		; y(b) * f

	LD F8, 140.0
	ADD F8, F9

	LD (VWX), F8	; vy(b) = 240 - y(b) * f

	INC B
	CP B, 8
	JR LT, .ComputeCube
	
	; line (vx(1), vy(1))-(vx(2),vy(2))
	LD A, 0
	CALLR .GetVX
	LD CD, XY
	CALLR .GetVY
	LD EF, XY
	LD A, 1
	CALLR .GetVX
	LD GH, XY
	CALLR .GetVY
	LD IJ, XY

	CALLR .DrawCubeLine

	LD A, 1
	CALLR .GetVX
	LD CD, XY
	CALLR .GetVY
	LD EF, XY
	LD A, 2
	CALLR .GetVX
	LD GH, XY
	CALLR .GetVY
	LD IJ, XY

	CALLR .DrawCubeLine

	LD A, 2
	CALLR .GetVX
	LD CD, XY
	CALLR .GetVY
	LD EF, XY
	LD A, 3
	CALLR .GetVX
	LD GH, XY
	CALLR .GetVY
	LD IJ, XY

	CALLR .DrawCubeLine

	LD A, 3
	CALLR .GetVX
	LD CD, XY
	CALLR .GetVY
	LD EF, XY
	LD A, 0
	CALLR .GetVX
	LD GH, XY
	CALLR .GetVY
	LD IJ, XY

	CALLR .DrawCubeLine

	LD A, 4
	CALLR .GetVX
	LD CD, XY
	CALLR .GetVY
	LD EF, XY
	LD A, 5
	CALLR .GetVX
	LD GH, XY
	CALLR .GetVY
	LD IJ, XY

	CALLR .DrawCubeLine

	LD A, 5
	CALLR .GetVX
	LD CD, XY
	CALLR .GetVY
	LD EF, XY
	LD A, 6
	CALLR .GetVX
	LD GH, XY
	CALLR .GetVY
	LD IJ, XY

	CALLR .DrawCubeLine

	LD A, 6
	CALLR .GetVX
	LD CD, XY
	CALLR .GetVY
	LD EF, XY
	LD A, 7
	CALLR .GetVX
	LD GH, XY
	CALLR .GetVY
	LD IJ, XY

	CALLR .DrawCubeLine

	LD A, 7
	CALLR .GetVX
	LD CD, XY
	CALLR .GetVY
	LD EF, XY
	LD A, 4
	CALLR .GetVX
	LD GH, XY
	CALLR .GetVY
	LD IJ, XY

	CALLR .DrawCubeLine

	LD A, 0
	CALLR .GetVX
	LD CD, XY
	CALLR .GetVY
	LD EF, XY
	LD A, 4
	CALLR .GetVX
	LD GH, XY
	CALLR .GetVY
	LD IJ, XY

	CALLR .DrawCubeLine

	LD A, 3
	CALLR .GetVX
	LD CD, XY
	CALLR .GetVY
	LD EF, XY
	LD A, 7
	CALLR .GetVX
	LD GH, XY
	CALLR .GetVY
	LD IJ, XY

	CALLR .DrawCubeLine

	LD A, 1
	CALLR .GetVX
	LD CD, XY
	CALLR .GetVY
	LD EF, XY
	LD A, 5
	CALLR .GetVX
	LD GH, XY
	CALLR .GetVY
	LD IJ, XY

	CALLR .DrawCubeLine

	LD A, 2
	CALLR .GetVX
	LD CD, XY
	CALLR .GetVY
	LD EF, XY
	LD A, 6
	CALLR .GetVX
	LD GH, XY
	CALLR .GetVY
	LD IJ, XY

	CALLR .DrawCubeLine

	VDL 0b00000010  ; Manually draw the video frame to the render buffer

	WAIT 15

	JR .Repeat
	
.Exit
	RET

; Input: A - the index (0 - 7)
; Output: XY - the vX value
.GetVX
	PUSH A, D
	MUL A, 4
	LD BCD, .vX
	ADD BCD, A
	LD F1, (BCD)
	LD XY, F1
	POP A, D
	RET

; Input: A - the index (0 - 7)
; Output: XY - the vY value
.GetVY
	PUSH A, D
	MUL A, 4
	LD BCD, .vY
	ADD BCD, A
	LD F1, (BCD)
	LD XY, F1
	POP A, D
	RET


.DrawCubeLine
	
	LD A, 0x21     ; Line
	LD B, 1     ; the video page on which we plot (1-8)
	LD K, 0x33     ; the color of the line.
	INT 0x01, A       ; Trigger interrupt Video
	
	RET

.InitCube
	LD F10, -1.0
	LD F11, 1.0
	LD ABC, .X
	LD DEF, .Y
	LD GHI, .Z
	; XYZ(1)
	LD (ABC), F10
	LD (DEF), F10
	LD (GHI), F10
	CALLR .IncrementXYZOffsets
	; XYZ(2)
	LD (ABC), F10
	LD (DEF), F11
	LD (GHI), F10
	CALLR .IncrementXYZOffsets
	; XYZ(3)
	LD (ABC), F11
	LD (DEF), F11
	LD (GHI), F10
	CALLR .IncrementXYZOffsets
	; XYZ(4)
	LD (ABC), F11
	LD (DEF), F10
	LD (GHI), F10
	CALLR .IncrementXYZOffsets
	; XYZ(5)
	LD (ABC), F10
	LD (DEF), F10
	LD (GHI), F11
	CALLR .IncrementXYZOffsets
	; XYZ(6)
	LD (ABC), F10
	LD (DEF), F11
	LD (GHI), F11
	CALLR .IncrementXYZOffsets
	; XYZ(7)
	LD (ABC), F11
	LD (DEF), F11
	LD (GHI), F11
	CALLR .IncrementXYZOffsets
	; XYZ(8)
	LD (ABC), F11
	LD (DEF), F10
	LD (GHI), F11
	CALLR .IncrementXYZOffsets
	
	RET

.IncrementXYZOffsets
	ADD ABC, 4
	ADD DEF, 4
	ADD GHI, 4
	RET

	#DB "Storing X:"
.X
	#DB [8] 0x00000000

	#DB "Storing Y:"
.Y
	#DB [8] 0x00000000

	#DB "Storing Z:"
.Z
	#DB [8] 0x00000000
	
	#DB "Storing vX:"
.vX
	#DB [8] 0x00000000

	#DB "Storing vY:"
.vY
	#DB [8] 0x00000000
