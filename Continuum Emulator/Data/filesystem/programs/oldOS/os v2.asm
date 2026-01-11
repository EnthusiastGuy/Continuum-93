; An OS first attempt...

		#ORG 0
		REGS 0
		LD YZ, 0	; This will store the length of the command string
		LD X, 0
		
		;load font file to the .FontData pointer
		LD A, 7				; File load
		LD BCD, .FontFile
		LD EFG, .FontData		; Deposit font at this address
		INT 4, A
		
		CALLR .DrawSplash
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
		LD (BCD), 0, 1
		
		INC X	;Refresh mark
		
		JR .ScanKeypressExit
		
.NotBackspace

		CP A, 13	; Enter
		JR NZ, .NotEnter
		
		CALLR .ClearCommand
		
		CALLR .InterpretCommand
		
		LD YZ, 0
		
.NotEnter

		CP YZ, 79				; Maximum 80 characters
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
		
		CALLR .ClearPage1
		
		LD EFG, .CommandBuffer
		LD HI, 2
		LD JK, 260
		LD L, 0x77
		LD M, 1
		CALLR .DrawString
		
		PUSH AB
		ER
		REGS 0
		POP VW
		RET

.ClearPage1
		PUSH A, C		; Push registers A through C (that is A, B and C)
		LD A, 5			; "Clear page function selector"
		LD B, 1			; Page to clear
		LD C, 0			; Color to clear with
		INT 1, A		; Video/Clear page
		POP A, C		; Retrieve registers A through C (A, B, C)
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
		LD L, 0xEC
		JR .DrawCaret
.TransparentCaret
		LD L, 0x0
.DrawCaret
		LD EFG, .Caret
		LD JK, 262
		LD M, 1
		CALLR .DrawString
		
		REGS 0
		RET

.DrawSplash
		REGS 5
		
		LD EFG, .ContinuumWelcomeLine
		LD HI, 145 ; X
		LD JK, 30 ; Y
		LD L, 0xEC ; color
		LD M, 0 ; video page
		CALLR .DrawString
		
		REGS 0
		RET
		
.InterpretCommand
		REGS 5
		LD EFG, .StringUnknown
		LD HI, 3 ; X
		LD JK, 250 ; Y
		LD L, 0xDE ; color
		LD M, 0 ; video page
		CALLR .DrawString
		REGS 0
		RET
		
.DrawString
		LD A, 18
		LD BCD, .FontData

		INT 1, A			; Draw text
		RET

.FontFile
		#DB "fonts\SlickAntsContour.font", 0
.Caret
		#DB "_", 0

.CommandBuffer	; Reserve some space here (80 chars) for the command buffer
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0

.Clock
		#DB 0, 0, 0, 0			
		
.ContinuumWelcomeLine
		#DB "CONTINUUM 93 (c) 2022 Enthusiast Guy", 0
		
.StringUnknown
		#DB "Unknown command", 0

.FontData	; 952 bytes will be occupied from here for the font data. We can explicitly reserve them as below
			; so we can write more code after this block, if we need to. Explicitly reserving also help measure
			; exactly how much bytes your code takes.
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		#DB 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		
; End