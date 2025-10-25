
; Sets video to 3 pages, clears them
; Draws the background of the OS
; Copies the custom palette across all 3 video pages' palettes
.InitVideo
		LD A, 0x02 	; SetVideoPagesCount interrupt
		LD B, 3 	; set video pages to 3.
		INT 0x01, A ; Trigger interrupt Video with SetVideoPagesCount

		CALLR .SetVideoManualControlMode
		CALLR .SetVideoLayersVisibility
		
		CALLR .ClearAllLayers
		
		LD A, 0x06 ; DrawFilledRectangle
		LD B, 0 ; select video page 0 to draw on
		LD CD, 1 ; x
		LD EF, 1 ; y
		LD GH, 478 ; width
		LD IJ, 268 ; height
		LD K, (.Color_lines) ; color
		INT 0x01, A ; Trigger interrupt Video
		
		LD CD, 2 ; x
		LD EF, 2 ; y
		LD GH, 180 ; width
		LD IJ, 266 ; height
		LD K, (.Color_background) ; color
		INT 0x01, A ; Trigger interrupt Video
		
		LD CD, 183 ; x
		LD EF, 2 ; y
		LD GH, 295 ; width
		LD IJ, 266 ; height
		INT 0x01, A ; Trigger interrupt Video
		
		; Draw the right side title background
		LD CD, 183 ; x
		LD EF, 2 ; y
		LD GH, 295 ; width
		LD IJ, 13 ; height
		LD K, (.Color_info_title_background) ; color
		INT 0x01, A ; Trigger interrupt Video
		
		CALLR .SetPalettes

		RET

.ClearAllLayers
		PUSH AB
		LD AB, 0x000F	; Clear video page 0 (background) with a solid color
		CALLR .ClearScreen
		INC A		; Increment to page 1
		LD B, 0		; Clear with transparent
		CALLR .ClearScreen
		INC A		; Increment to page 2
		CALLR .ClearScreen
		POP AB
		RET

; We copy a custom defined small palette to all 3 used video pages palette pointers
.SetPalettes
		LD EFG, 48			; We'll copy 16 colors multiplied by 3 bytes for R, G and B
		LD HIJ, .Palette16_fantasy_kitchen	; Point to where we store the palette

		LD A, 0x04 	; ReadVideoPaletteAddress
		LD B, 2 	; the video page
		INT 0x01, A ; Trigger interrupt Video

		ADD BCD, 3	; First color is transparent for all video pages except the first
		MEMC HIJ, BCD, EFG

		LD KLM, BCD

		LD B, 1 	; Get palette for video page 1
		INT 0x01, A ; Trigger interrupt Video

		ADD BCD, 3	; First color is transparent for all video pages except the first
		MEMC HIJ, BCD, EFG

		LD NOP, BCD

		LD B, 0		; Get palette for video page 0
		INT 0x01, A ; Trigger interrupt Video

		ADD BCD, 3	; We still place the colors offset to maintain the same color codes across pages
		MEMC HIJ, BCD, EFG

		LD QRS, BCD

		RET

; Clears the specified video page with specified color
; input A: the video page which needs clearing (1-8)
; input B: the color which will be used to fill that memory page (0 - transparent or 1 - 255).
.ClearScreen
		PUSH Z
		LD Z, 0x05 	; ClearVideoPage
		INT 0x01, Z ; Trigger interrupt Video
		POP Z
		RET

; Draws the provided list item, or overflow marker
; Input: HI - the x coordinate of the top-left corner of the character;
; Input: EFG - the source address of the null terminated string to be drawn;
; Input: K - the y coordinate of the top-left corner of the character.
.DrawListTextItem
		PUSH A, P
		LD O, (.FontHeight)			; Font height
		MUL JK, O					; y
		ADD JK, 3					; y offset
		LD A, 0x12 					; DrawText
		LD BCD, .FontData 			; the source address (in RAM) of the font to be used.
		LD L, (.FileListColor) 		; the color used to draw the string
		LD M, 2 					; the video page on which we draw the string (1-8)
		LD NO, 155					; width limit
		INT 0x01, A 				; Trigger interrupt Video
		LD (.Overflow), C			; Mark whether an overflow was present
		LD (.LastDrawnX), AB		; Last drawn X
		POP A, P
		RET

; Called after the selection bar position has been modified so it gets
; updated on the screen
.RefreshSelection
		LD AB, 0x0100	; Video page and color to clear
		CALLR .ClearScreen
		CALLR .SelectionBar
		CALLR .DisplayRecordInfo
		RET

.DrawMessage
		LD A, 0x06 ; DrawFilledRectangle
		LD B, 2 ; select video page 0 to draw on
		LD CD, 120 ; x
		LD EF, 108 ; y
		LD GH, 240 ; width
		LD IJ, 54 ; height
		LD K, (.Color_message_background) ; color
		INT 0x01, A ; Trigger interrupt Video

		LD A, 0x07     ; DrawRectangle
		LD B, 2     ; the video page on which we draw the rectangle (0 - 7)
		LD CD, 122     ; the x coordinate of the top-left corner of the rectangle as a signed 16-bit number.
		LD EF, 110     ; the y coordinate of the top-left corner of the rectangle as a signed 16-bit number.
		LD GH, 236     ; the width of the rectangle.
		LD IJ, 50     ; the height of the rectangle.
		LD K, (.Color_message_text)     ; the color of the rectangle.
		INT 0x01, A       ; Trigger interrupt Video

		LD A, 0x12     ; DrawString
		LD BCD, .FontData     ; the source address (in RAM) of the font to be used.
		LD EFG, .MessageTextErrorsFound     ; the source address (in RAM) of the null terminated string to be drawn.
		LD HI, 128     ; the x coordinate of the top-left corner of the sprite as a signed 16-bit number.
		LD JK, 130     ; the y coordinate of the top-left corner of the sprite as a signed 16-bit number.
		LD L, (.Color_message_text)     ; the color used to draw the string
		LD M, 2     ; the video page on which we draw the string (0 - 7)
		LD NO, 0     ; the maximum width in pixels of the text to be drawn. Everything else is clipped.
		INT 0x01, A       ; Trigger interrupt Video

		RET


.SetVideoManualControlMode
		; Sets the video buffer control mode to manual
		LD A, 0x33
		LD B, 0b00000000
		INT 0x01, A

		RET

.SetVideoAutoControlMode
		; Sets the video buffer control mode to auto
		LD A, 0x33
		LD B, 0b11111111
		INT 0x01, A

		RET

.SetVideoLayersVisibility
		; Sets all layers to visible
		LD A, 0x31
		LD B, 0b11111111
		INT 0x01, A

		RET
