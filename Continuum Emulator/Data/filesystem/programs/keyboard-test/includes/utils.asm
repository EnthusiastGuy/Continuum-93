
.ConvertByteToString
		PUSH A, F
		LD BCD, .NumberStringBuffer     ; Where to deposit the resulting string
        LD A, (.byte_to_convert)
		LD E, 3	; We're working with 3 digits here
		ADD BCD, E		; Start from the end of the number string backwards

.convert_byte_string_next_digit
		DIV A, 10, F	; F will contain the least significant number
		ADD F, 48
		DEC BCD
		LD (BCD), F
		DEC E
		CP E, 0
		JR NZ, .convert_byte_string_next_digit
		POP A, F
		RET

.byte_to_convert
        #DB 0
