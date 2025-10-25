; Loads font file to the .FontData pointer

.LoadFont
		LD A, 7				; File load
		LD BCD, .FontFile
		LD EFG, .FontData	; Deposit font at this address
		INT 4, A
		
		RET


.FontFile
		#DB "fonts\SlickAntsContour.font", 0
		;#DB "fonts\DoctorJack.font", 0
		
.FontData	; font data buffer.
		#DB 0
.FontHeight
		#DB 0
.FontDataLeft
		#DB [1140] 0	; or 950 for a height of 9px
		