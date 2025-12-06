; Continuum OS V1

		#ORG 0
		REGS 0
		LD YZ, 0	; Length of the command string
		LD X, 0
		
		;load font file to address
		LD A, 7				; File load
		LD BCD, .FontFile
		LD EFG, 0x2000		; Deposit font at this address

		INT 4, A
.Loop
		WAIT 1
		CALL .ScanKeypress
		PUSH VW
		CALL .RefreshCaret
		CP X, 0
		JR Z, .Loop
		LD X, 0
		CALL .PrintCommand
		
		JP .Loop
		
.ScanKeypress
		LD A, 1
		INT 2, A
		
		CP A, 0		; A should now have the character if any, otherwise zero
		JR Z, .ScanKeypressExit
		
		CP A, 8		; Backspace
		JR NZ, .NotBackspace
		
		CP YZ, 0
		JR Z, .ScanKeypressExit	; Not allowed to backspace further if line is empty
		
		LD BCD, .CommandBuffer
		DEC YZ
		ADD BCD, YZ
		LD (BCD), 0
		
		INC X	;Refresh mark
		
		JR .ScanKeypressExit
		
.NotBackspace

		CP A, 13	; Enter
		JR NZ, .NotEnter
		
		CALL .InterpretCommand
		CALL .ClearCommand
		LD YZ, 0
		
.NotEnter

		CP YZ, 79
		JR Z, .ScanKeypressExit	; Not allowed to type very long commands

		LD BCD, .CommandBuffer
		ADD BCD, YZ
		LD (BCD), A
		INC YZ
		INC X
		
.ScanKeypressExit
		RET
		
.ClearCommand

		REGS 2	; Switch to register bank 2
		LD ABC, .CommandBuffer
		MEMF ABC, 80, 0
		REGS 0	; Switch back to register bank 0
		
		RET
		
.PrintCommand
		REGS 1
		DR
		
		LD A, 5
		LD B, 1
		LD C, 0
		INT 1, A			; Clear screen
		
		LD A, 18
		LD BCD, 0x2000
		LD EFG, .CommandBuffer
		LD HI, 2
		LD JK, 260
		LD L, 0x77
		LD M, 1
		
		INT 1, A			; Draw text
		PUSH AB
		ER
		REGS 0
		POP VW
		RET

.InterpretCommand
		; Takes data at .CommandBuffer and attempts to interpret it
		REGS 4
		
		LD DEF, .Commands
.RepeatSearch
		LD ABC, .CommandBuffer
		
.CompareNextCommandCharacter
		CP (ABC), 32		; Space
		JR Z, .Match
		CP (ABC), (DEF)
		JR NZ, .NoMatch
		INC ABC
		INC DEF
		JR .CompareNextCommandCharacter
		
.NoMatch
		; Retry until no more commands to check
		FIND (DEF), 0
		INC DEF
		CP (DEF), 0
		
		JR Z, .InterpretCommandExit					; Not found
		INC DEF
		JR .RepeatSearch
		
.Match
		
		INC DEF		; Move to the command index pointer
		LD D, (DEF)	; Gets the command index in D

		CP D, 1
		JR Z, .InterpretColor
		CP D, 2
		JR Z, .InterpretLoad

.InterpretColor
		LD EFG, .ColorString
		JR .DrawInterpretConclusion
.InterpretLoad
		LD EFG, .LoadString
		JR .DrawInterpretConclusion

.DrawInterpretConclusion

		;MEMF 16771216, 100, 0xAA

		LD A, 18
		LD BCD, 0x2000
		LD HI, 0
		LD JK, 0
		LD L, 0xAA
		LD M, 0
		INT 1, A			; Draw text

.InterpretCommandExit
		REGS 0
		RET
		
.RefreshCaret
		; Get clock
		REGS 3
		POP HI		; Gets the last pixel after the end of the printed text
		INC HI		; ... and adds another pixel
		
		LD A, 3
		LD B, 0
		LD CDE, .Clock
		
		INT 0, A	; Get number of total ms since start into .Clock
		
		ADD CDE, 3
		LD F, 128
		CP F, (CDE)
		JR LT, .TransparentCaret
		LD L, 0x77
		JR .DrawCaret
.TransparentCaret
		LD L, 0x0
.DrawCaret
		LD A, 18
		LD BCD, 0x2000
		LD EFG, .Caret
		LD JK, 262
		
		LD M, 1
		INT 1, A			; Draw text

		REGS 0
		RET

.FontFile
		#DB "fonts\SlickAntsContour.font", 0
.Caret
		#DB "_", 0

.CommandBuffer	; Reserve some space here for commands
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0

.Clock
		#DB 0, 0, 0, 0
		
.Commands
		#DB "color", 0, 1
		#DB "load", 0, 2
		#DB 0, 0			; End
		
.ColorString
		#DB "Attempting to interpret color command.", 0
		
.LoadString
		#DB "Attempting to interpret load command.", 0
		
