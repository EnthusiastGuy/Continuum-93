#include ..\..\lib\c93-keyboard.asm
#include includes\tiles.asm

; Tile drawing demo

	#ORG 0x80000

; Video initialization
    LD A, 0x02              ; SetVideoPagesCount
    LD B, 2                 ; set 2 video pages
    INT 0x01, A             ; Trigger interrupt Video with function SetVideoPagesCount

    LD A, 0x05     			; ClearVideoPage
	LD B, 0x00     			; the video page which needs clearing (0 - 7)
	LD C, 0x0     			; the color which will be used to fill that memory page (0 - transparent or 1 - 255).
	INT 0x01, A       		; Trigger interrupt Video
    INC B
    INT 0x01, A             ; .. also, for the second video page

    ; Obtain the memory location of the palette belonging to the first video page, the one that will render the background
    LD W, 0x04              ; ReadVideoPaletteAddress
    LD X, 0                 ; the video page palette of which start address we need to find (0 - 7)
    INT 0x01, W             ; Trigger interrupt Video with function ReadVideoPaletteAddress
    ; XYZ now contains the pointer to layer 0's palette address

    ; Load the tileset
    LD A, 0x34
    LD BCD, .TilesPath
    LD EFG, XYZ             ; Load the palette directly to video layer 0's palette
    LD HIJ, .TilesData      ; Load the image data directly into the video buffer since it matches the w/h
    LD KLMN, 0              ; the RGBA color that will be interpreted as the transparent color from the provided RGB.
    INT 0x04, A

    ; Obtain the memory location of the palette belonging to the second video page now
    LD A, 0x04              ; ReadVideoPaletteAddress
    LD B, 1                 ; the video page palette of which start address we need to find (0 - 7)
    INT 0x01, A             ; Trigger interrupt Video with function ReadVideoPaletteAddress
    ; BCD now contains the pointer to layer 1's palette address

    MEMC XYZ, BCD, 768      ; We duplicate the palette onto the second layer since we'll be using the same tilesheet.
    ;LD24 (.DrawParamsTileMapSource), .TilesData

.Repeat
	CALLR .InputUpdate
	LD A, 27				; Escape key
	CALLR .InputKeyPressed
	CP A, 1
	JR Z, .Exit
	
    ; Code here
    ;LD XYZ, .TilesLibrary
    ;LD AB, (XYZ)
    ;LD (.DrawParamsTileOriginX), AB
    ;ADD XYZ, 2
    ;LD AB, (XYZ)
    ;LD (.DrawParamsTileOriginY), AB
    ;ADD XYZ, 2
    ;LD AB, (XYZ)
    ;LD (.DrawParamsTileMapWidth), AB
    ;ADD XYZ, 2
    ;LD AB, (XYZ)
    ;LD (.DrawParamsTileMapHeight), AB

    CALL .DrawTile


	JP .Repeat
.Exit
	RET


.DrawTile
    PUSH A, Z

    LDREGS A, V, (.DrawParamsInterrupt)     ; Load draw params
    INT 0x01, A                             ; Trigger drawing
    

    POP A, Z
    RET

.DrawParamsInterrupt
    #DB 0x0E
.DrawParamsTileMapSource
    #DB .TilesData
.DrawParamsTileMapWidth
    #DB 0073
.DrawParamsTileOriginX
    #DB 0x0000
.DrawParamsTileOriginY
    #DB 0x0000
.DrawParamsTileWH
    #DB 0x000A, 0x000A
.DrawParamsVideoPage
    #DB 0x00
.DrawParamsTileXY
    #DB 0x000A, 0x000A
.DrawParamsEffects
    #DB 0b00000000
.DrawParamsTilingH
    #DB 0x00
.DrawParamsTilingV
    #DB 0x00
    

.TilesPath
    #DB "programs\tile-draw-demo\tiles.png", 0


.Backup
    LD A, 0x0E ; DrawTileMapSprite
    LD BCD, 0 ; the source address (in RAM) of the tile map.
    LD EF, 0 ; the width of the tilemap in pixels.
    LD GH, 0 ; the x position of the sprite within the tile map as a signed 16-bit number.
    LD IJ, 0 ; the y position of the sprite within the tile map as a signed 16-bit number.
    LD KL, 0 ; the width of the sprite within the tile map.
    LD MN, 0 ; the height of the sprite within the tile map.
    LD O, 0 ; the target video page where the sprite is to be drawn.
    LD PQ, 0 ; the target x coordinate where the sprite is to be drawn.
    LD RS, 0 ; the target y coordinate where the sprite is to be drawn.
    LD T, 0 ; the effects to be applied to the sprite that is being drawn (flip, tiling, rotation).
    INT 0x01, A ; Trigger interrupt Video
    RET

.TilesData
    #DB [5329] 0