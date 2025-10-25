; Keyboard test program. Monitors and displays any combination of simultaneously pressed keys that
; can be registered

#include includes\video.asm
#include includes\data.asm
#include includes\utils.asm
#include includes\main.asm


	#ORG 0x80000

	CALLR .InitializeVideo
	JR .Start
