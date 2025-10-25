
#ORG 0x80000

	LD Z, 0x05 	; ClearVideoPage
	LD AB, 0x00DA	; Clear video page 0 (background) with a solid color
	INT 0x01, Z ; Trigger interrupt Video




	LD BC, 100
.loopMain

	CALL .mainLoop
	DEC BC
	LD A, B
	OR A, C
	JR NZ, .loopMain

.mainLoop
	PUSH BC
	LD BC, 0xFFFF

.loop
	CALL .somework

	DEC BC
	LD A, B
	OR A, C
	JR NZ, .loop
	POP BC
	RET

.somework
	;PUSH AB
	LD D, 0
	LD E, 12
	LD H, D
	LD I, E
	ADD HI, DE
	XOR A, A
	;POP AB
	RET