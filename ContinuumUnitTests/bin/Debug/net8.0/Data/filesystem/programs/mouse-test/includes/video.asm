


.InitVideo
		LD A, 0x02 	; SetVideoPagesCount interrupt
		LD B, 3 	; set video pages to 3.
		INT 0x01, A ; Trigger interrupt Video with SetVideoPagesCount
		CALLR .SetPalettes

        RET

.SetPalettes
		LD EFG, 48			; We'll copy 16 colors multiplied by 3 bytes for R, G and B
		LD HIJ, .Palette16_bubblegum	; Point to where we store the palette

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

.HandleMouseDraw
		LD A, 1					; Clear the dot cursor page
		LD B, 0					; Clear with transparent
		CALLR .ClearScreen

		LD XYZ, 480
		LD D, 3					; set "Read mouse state" mode
		INT 2, D				; Trigger Read mouse state interrupt
		MUL XYZ, GH				; y position
		ADD XYZ, EF				; x position
		LD ABC, (.VideoPage2Pointer)		; Video memory pointer to where we draw
		LD MNO, (.VideoPage1Pointer)		; Video memory pointer to where we show the dot cursor
		ADD ABC, XYZ
		ADD MNO, XYZ
		LD (MNO), K

		CP I, 1					; mouse button state

		JR NZ, .draw_pixel_exit
		LD (ABC), K				; Put pixel on screen if mouse lButton is pressed
.draw_pixel_exit
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

.VideoPage1Pointer
		#DB 0xFC0B80

.VideoPage2Pointer
		#DB 0xFA1140

.PointVideoOffset
		#DB 0x000000