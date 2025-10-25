#include ..\..\filesystem\lib\c93-palettes.asm

; Defines the colors to be used in the UI. Note that the values here are actual indexes
; of colors defined in each video palette.

.Color_lines
		#DB 0x0E	; Default 0x0E

.Color_background
		#DB 0x03	; Default 0x03

.Color_info_title_background
		#DB 0x0E	; Default 0x0E

.Color_directoryname_text
		#DB 0x0F	; Default 0x0F

.Color_filename_text
		#DB 0x07	; Default 0x07

.Color_selection_bar
		#DB 0x0A	; Default 0x0A

.Color_message_background
		#DB 0x06

.Color_message_text
		#DB 0x03
