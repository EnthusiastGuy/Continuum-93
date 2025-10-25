#include ..\..\lib\c93-keyboard.asm
#include ..\..\lib\c93-palettes.asm
#include includes\video.asm

; Some silly "crayon" drawing tool

		#ORG 0x80000

		CALLR .InitVideo
		LD K, 0xAA				; Point color
.Loop
		CALLR .InputUpdate
		CALLR .InputNoStateChange
		CP A, 1
		JR Z, .no_key_input

		LD A, 27				; Escape key
		CALLR .InputKeyPressed
		CP A, 1
		JR Z, .exit

.no_key_input
		CALLR .HandleMouseDraw
		JR .Loop
.exit
		RET
