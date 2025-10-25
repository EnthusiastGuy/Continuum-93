; Q os V0.1.1. This is meant to be more user friendly with visual feedback.
; Reserves memory area from 0x000000 to 0x07FFFF (512k) for future use.

#include includes\os-system-error-handler.asm
#include includes\os-video.asm
#include includes\os-font.asm
#include includes\os-filesystem.asm
#include includes\os-data.asm
#include ..\filesystem\lib\c93-keyboard.asm
#include ..\filesystem\lib\c93-gamepads.asm
#include includes\os-input.asm
#include includes\os-clock.asm
#include includes\os-ui.asm

		#ORG 0

		; Set the jump address in case of an error
		LD A, 0
		LD BCD, .HandleSystemError
		SETVAR 0xFFFF, ABCD

		CALLR .InitVideo			; Set 3 video pages, clear the background with solid, clear others with transparent, draw lines
		CALLR .LoadFont				; Loads the font into memory
		CALLR .GetDirectoryRecords	; Gets all directories and files in current directory

		CALLR .ListRecords			; Initial records display
		CALLR .SelectionBar			; Initial selection bar display
		CALLR .DisplayRecordInfo	; Initial display of the record info

.MainLoop
		LD X, (.ErrorsFoundCompiling)
		CP X, 0
		CALL NZ, .DrawMessage
		
		CALLR .KeyboardInputEvaluate

		VDL 0b00000111

		JR .MainLoop

		; Being an OS, this will never exit
