; This holds some preparatory methods ran at the start-up of the program to set things up

.Initialize
    LD A, 0x02 	                ; Interrupt selector, function "Set video pages count"
    LD B, 3 	                ; set video pages to 3.
    INT 0x01, A                 ; Execute interrupt 0x01 (Video) with function 0x02 (SetVideoPagesCount)
    
    LD AB, 0x000F               ; Clear page 0 with a solid color
	CALLR .ClearScreen
    CALLR .LoadFont
    CALLR .SetVideoAutoControlMode  ; We want to control when the drawing needs to be done
    CALLR .SetStaticText            ; Draw some static stuff
    CALLR .LoadSprites
    RET

; Loads font file to the .FontData pointer
.LoadFont
    LD A, 0x07				    ; Interrupt selector, function "File load"
    LD BCD, .FontFile           ; What is the font file path to load
    LD EFG, .FontData		    ; Deposit font at this address
    INT 0x04, A                 ; Execute interrupt 0x04 (filesystem) with function 0x30 (File load)
    RET

; Sets the video buffer mode to manual for all video pages
.SetVideoAutoControlMode
    LD A, 0x33
    LD B, 0b00000000
    INT 0x01, A
    RET

.SetStaticText
    CALLR .DrawTitleBar
    RET

; Loads the tilemap with all sprites
.LoadSprites
    LD A, 0x30                  ; Interrupt selector, function "Load image and palette"
    LD BCD, .TilemapFilePath    ; Address to where the path of the tilemap is located
    LD EFG, 0x85000;            ; Target address for the tilemap load
    INT 0x04, A                 ; Execute interrupt 0x4 (filesystem) with function 0x30 (Load image and palette)
    LD YZ, 0                    ; Since A contains the length of the palette in number of colors
    MUL ZA, 3                   ;   we use this trick to calculate the length of the palette in bytes
    SUB EFG, ZA                 ;   so we can localize where it is in memory. Remember, palette is loaded before the tilemap
    MEMC EFG, 0xFA0B40, YZA     ; Copy the palette to video page 1's palette
    MEMC EFG, 0xFA0840, YZA     ; ... and also to video page 2's palette
    RET
