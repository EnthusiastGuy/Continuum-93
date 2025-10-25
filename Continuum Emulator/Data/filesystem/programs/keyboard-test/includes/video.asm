
.InitializeVideo
        LD A, 0x02 	; SetVideoPagesCount
		LD B, 3 	; set video pages to 3.
		INT 0x01, A ; Trigger interrupt Video

        LD (.clear_screen_page), 0x02, 1		; We need to clear this newly created page due to garbage during memory allocation.
		CALLR .ClearScreen

		; load font file to the .FontData pointer
		LD A, 7					; File load
		LD BCD, .FontFile
		LD EFG, .FontData		; Deposit font at this address
		INT 0x04, A

		LD A, 0x06 ; DrawFilledRectangle
		LD B, 0 ; the video page on which we draw the rectangle (1-8)
		LD CD, (.MainWindowTopLeftX) ; the x coordinate of the top-left corner of the rectangle.
		LD EF, (.MainWindowTopLeftY) ; the y coordinate of the top-left corner of the rectangle.
		LD GH, (.MainWindowWidth) ; the width of the rectangle.
		LD IJ, (.MainWindowHeight) ; the height of the rectangle.
		LD K, 200 ; the fill color of the rectangle. Note there is no border, so a transparent color here will make the rectangle invisible.
		INT 0x01, A ; Trigger interrupt Video
		
		LD CD, 1 ; the x coordinate of the top-left corner of the rectangle.
		LD EF, 11 ; the y coordinate of the top-left corner of the rectangle.
		LD GH, 478 ; the width of the rectangle.
		LD IJ, 258 ; the height of the rectangle.
		LD K, 0 ; the fill color of the rectangle. Note there is no border, so a transparent color here will make the rectangle invisible.
		INT 0x01, A ; Trigger interrupt Video
		
		LD CD, 196 ; the x coordinate of the top-left corner of the rectangle.
		LD EF, 102 ; the y coordinate of the top-left corner of the rectangle.
		LD GH, 278 ; the width of the rectangle.
		LD IJ, 88 ; the height of the rectangle.
		LD K, 200 ; the fill color of the rectangle. Note there is no border, so a transparent color here will make the rectangle invisible.
		INT 0x01, A ; Trigger interrupt Video
		
		; Draw the info texts on the screen
		LD (.DrawKeyColor), 1, 1
		LD (.DrawKeyVideoPage), 2, 1
		LD XYZ, .Title
		LD (.DrawKeySource), XYZ
		LD (.DrawKeyYLow), 0, 1
		LD (.DrawKeyX), 8, 2
		CALL .DrawText

		LD XYZ, .InfoLine1
		LD (.DrawKeySource), XYZ
		LD (.DrawKeyYLow), 14, 1
		LD (.DrawKeyX), 208, 2
		CALL .DrawText
		
		LD XYZ, .InfoLine2
		LD (.DrawKeySource), XYZ
		LD (.DrawKeyYLow), 15, 1
		CALL .DrawText

		LD XYZ, .InfoLine3
		LD (.DrawKeySource), XYZ
		LD (.DrawKeyYLow), 17, 1
		CALL .DrawText
		
		LD XYZ, .InfoLine4
		LD (.DrawKeySource), XYZ
		LD (.DrawKeyYLow), 18, 1
		CALL .DrawText

		LD XYZ, .InfoLine5
		LD (.DrawKeySource), XYZ
		LD (.DrawKeyYLow), 19, 1
		CALL .DrawText

		LD XYZ, .InfoLine6
		LD (.DrawKeySource), XYZ
		LD (.DrawKeyYLow), 21, 1
		CALL .DrawText

        RET

.ClearScreen
		PUSH A, C
		LD A, 0x05 ; ClearVideoPage
		LD B, (.clear_screen_page)
		LD C, (.clear_screen_color) ; the color which will be used to fill that memory page (0 - transparent or 1 - 255).
		INT 0x01, A ; Trigger interrupt Video
		POP A, C
		RET
.clear_screen_page
		#DB 0
.clear_screen_color
		#DB 0

.DrawText
		PUSH A, O
		LD EFG, (.DrawKeySource)
		LD HI, (.DrawKeyX)
		LD JK, (.DrawKeyY)
		MUL JK, 8
		ADD JK, 2
		LD A, 0x12					; DrawText
		LD BCD, .FontData			; the source address (in RAM) of the font to be used.
		LD L, (.DrawKeyColor)		; the color used to draw the string
		LD M, (.DrawKeyVideoPage) 	; the video page on which we draw the string (1-8)
		INT 0x01, A					; Trigger interrupt Video
		POP A, O
		RET

.DrawKeyColor
		#DB 0
.DrawKeyVideoPage
		#DB 0
.DrawKeyX
		#DB 0x0000
.DrawKeyY
		#DB 0x00
.DrawKeyYLow
		#DB 0x00
.DrawKeySource
		#DB 0x000000
