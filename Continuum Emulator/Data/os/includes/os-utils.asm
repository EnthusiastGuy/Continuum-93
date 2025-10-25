
; Input: ABCD contains the number
.UtilsGetCurrentFileSizeAsString
		PUSH A, Z
		LD EFG, .CurrentFileSize    ; Target address
        MEMF EFG, 16, 0             ; Clears any previous values
		LD H, 10	                    ; Maximum 10 digits
		ADD EFG, H		            ; Start from the end of the number string backwards

.utils_convert_32bit_to_string_next_digit
		DIV ABCD, 10, XY	            ; X will contain the least significant digit
		ADD Y, 48
		DEC EFG
		LD (EFG), Y
		DEC H
		CP H, 0
		JR NZ, .utils_convert_32bit_to_string_next_digit

        CALLR .remove_current_file_size_padding

		POP A, Z
		RET

.remove_current_file_size_padding
        LD ABC, .CurrentFileSize
        LD DEF, ABC
.remove_current_file_size_padding_loop
        CP (ABC), 48                        ; Check for zero padding
        JR NZ, .remove_current_file_size_padding_found
        INC ABC
        JR .remove_current_file_size_padding_loop
.remove_current_file_size_padding_found
        LD G, (ABC)
        LD (DEF), G
        CP G, 0
        JR Z, .remove_current_file_size_padding_exit
        INC ABC
        INC DEF
        JR .remove_current_file_size_padding_found

.remove_current_file_size_padding_exit
        RET