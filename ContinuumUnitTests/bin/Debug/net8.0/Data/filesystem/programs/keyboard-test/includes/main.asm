
.Start
		LD H, 0			; Keeps a previous number of keys pressed
		LD (.clear_screen_page), 1, 1
		CALLR .ClearScreen

.KeysState
		LD BCD, .KeyboardBuffer	; Load keyboard state at this address
		LD A, 0x10 				; ReadKeyboardState
		INT 0x02, A				; Trigger interrupt Input. After this A contains the length of the buffer
								; and (BCD) the pressed keycodes
		CP A, 0					; Check if the returned buffer length is zero, meaning no keys pressed
		JP Z, .Start			; ... if so, go back to clear and scan again
		CP A, H					; Check if the same amount of keys is pressed since the last time checked
		JP Z, .KeysState		; ... if so, go back to scan again
		LD H, A					; Update the number of keys pressed in the previous pass
		
		LD (.clear_screen_page), 1, 1
		CALLR .ClearScreen

		LD IJK, (BCD)
		CP IJK, 0x101BA0	; This checks the keys buffer for the exit sequence (16 any Shift, 27 Escape, 160 LShift)
		JR NZ, .NoExit

		RET					; Sequence found, exiting	
		
.NoExit
.NextKey
		LD W, (BCD)					; Get current key code from state buffer
		LD (.byte_to_convert), W
		CALL .ConvertByteToString	; Convert W to string and store the characters at .NumberStringBuffer
		
		LD X, A
		INC X

		LD EFG, .NumberStringBuffer
		LD (.DrawKeyColor), 254, 1
		LD (.DrawKeyVideoPage), 1, 1
		LD (.DrawKeySource), EFG
		LD (.DrawKeyYLow), X
		LD (.DrawKeyX), 8, 2
		CALL .DrawText		; Draw the key code

		LD V, 0
		MUL VW, 3			; |
		LD XYZ, .Keys		; |
		ADD XYZ, VW			; |
		LD XYZ, (XYZ)		; -> Gets the address of the corresponding info text
		
		LD (.DrawKeySource), XYZ
		LD (.DrawKeyX), 32, 2
		
		CALL .DrawText		; Draws the info text for the curent key code
		
		INC BCD
		DEC A
		CP A, 0
		JR NZ, .NextKey
		JP .KeysState
