#include os-utils.asm

.SelectionBar
		PUSH A, L
		LD A, 0x06 ; DrawFilledRectangle
		LD B, 1 ; select video page 1 to draw on
		LD CD, 3 ; x
		LD EF, (.RecordIndex) ; y
		LD L, (.FontHeight)		; Get the font height from the font itself
		MUL EF, L				
		ADD EF, 12
		LD GH, 178 ; width
		LD IJ, 1 ; height	(9)
		LD K, (.Color_selection_bar) ; color
		INT 0x01, A ; Trigger interrupt Video
		POP A, L

		RET

.DisplayRecordInfo
		PUSH A, Z
		
		CALLR .GetRecordNameAndType
		LD EFG, (.CurrentRecord)		; .Current record for only filename
		LD H, (.CurrentRecordType)
		CALLR .clear_info_target_areas
		CALLR .DrawInfoTitle
		CP H, 0
		JR Z, .display_record_info_but_not_size
		LD HIJ, (.TotalEntriesCount)
		CP HIJ, 0
		JR Z, .display_record_info_but_not_size
		CALLR .CopyDirectoryPathToFullPath
		CALLR .DisplayFileSize
.display_record_info_but_not_size
		POP A, Z
		RET

.DrawInfoTitle
		PUSH A, N
		LD HI, 185
		LD JK, 3
		LD A, 0x12 					; DrawText
		LD BCD, .FontData 			; the source address (in RAM) of the font to be used.
		LD L, (.FileListColor) 		; the color used to draw the string
		LD M, 2						; the video page on which we draw the string (1-8)
		;LD N, 292					; width limit
		INT 0x01, A 				; Trigger interrupt Video
		LD (.LastDrawnX), AB		; Last drawn X
		POP A, N
		RET

.clear_info_target_areas
		PUSH A, Z
		LD A, 0x06 		; DrawFilledRectangle
		LD B, 2 		; select video page 0 to draw on
		LD CD, 185 		; x
		LD EF, 3 		; y
		LD GH, 292 		; width
		
		LD I, 0
		LD J, (.FontHeight)	; Get the font height from the font itself
		LD K, 0x00		; color
		INT 0x01, A 	; Trigger interrupt Video

		; Clear the filesize box
		LD CD, 190
		LD EF, 20
		INT 0x01, A 	; Trigger interrupt Video

		POP A, Z
		RET

.DisplayFileSize
		LD ABC, .CurrentFileFullPath
		CALLR .GetFileSize
		CALLR .UtilsGetCurrentFileSizeAsString
		
		LD HI, 226
		LD JK, 20
		LD A, 0x12 					; DrawText
		LD EFG, .CurrentFileSize
		LD BCD, .FontData 			; the source address (in RAM) of the font to be used.
		LD L, (.FileListColor) 		; the color used to draw the string
		LD M, 2						; the video page on which we draw the string (1-8)
		INT 0x01, A 				; Trigger interrupt Video

		LD A, 0x12 					; DrawText
		LD HI, 190
		LD EFG, .StringFileSize
		LD BCD, .FontData
		INT 0x01, A 				; Trigger interrupt Video

		RET
