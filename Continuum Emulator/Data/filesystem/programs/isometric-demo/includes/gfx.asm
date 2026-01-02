; Functionality related to graphics

.InitializeGFX

    CALLR .SetVideoAutoControlMode  ; Set the video mode so we can control WHEN we actually draw on the screen with VDL

    LD A, 0x02              ; SetVideoPagesCount
    LD B, 3                 ; set 3 video pages
    INT 0x01, A             ; Trigger interrupt Video with function SetVideoPagesCount

    ; The functionality of clearing the active (second) video page has been moved to a dedicated function since
    ; this is called every frame render.
    CALLR .ClearActivePage
    INC B                   ; .. since the function above sets the necessary data, we simply increment and clear the next page also
    INT 0x01, A             ; Clear third video page

    ; Obtain the memory location of the palette belonging to the first video page, the one that will render the background
    LD W, 0x04              ; ReadVideoPaletteAddress
    LD X, 0                 ; the video page palette of which start address we need to find (0 - 7)
    INT 0x01, W             ; Trigger interrupt Video with function ReadVideoPaletteAddress
    ; XYZ now contains the pointer to layer 0's palette address

    ; Loading background directly in the video buffer. This can be done this way since the width
    ; of the image to be loaded matches the width of the video frame. If this would have not been the
    ; case, the proper way is to load in memory and then draw them with DrawSprite (int 0x01, fn 0x10)
    LD A, 0x33
    LD BCD, .PathBackground
    LD EFG, XYZ             ; Load the palette directly to video layer 0's palette
    LD HIJ, 0xFE05C0        ; Load the image data directly into the video buffer since it matches the w/h
    INT 0x04, A

    ; We repeat this process for the second video page that will actually draw the tiles since we need to load the palettes
    ; obtained from the png tilesets.
    LD W, 0x04              ; ReadVideoPaletteAddress
    LD X, 1                 ; the video page palette of which start address we need to find (0 - 7)
    INT 0x01, W             ; Trigger interrupt Video with function ReadVideoPaletteAddress
    ; XYZ now contains the pointer to layer 1's palette address

    ; Load the tiles pngs. This is a more complicated way of doing things. All tiles could fit nicely in one single tileset, but for the sake of
    ; demonstrating how merging works, we are going to load two tilesets with two different transparency keys and have them merged in second
    ; video layer's palette.

    ; Load the first tileset
    LD A, 0x34              ; Load8BitPngWithCustomTransparency
    LD BCD, .PathTilesA     ; the source address (in RAM) of the null terminated string which represents the desired path to the image file to be processed.
    LD EFG, XYZ             ; the pointer to the address (in RAM) where the PNG palette and transparency data will be loaded.
    LD HIJ, .TilesAData     ; the pointer to the address (in RAM) where the PNG pixel data will be loaded.
    LD KLMN, 0              ; the RGBA color that will be interpreted as the transparent color from the provided RGB.
    INT 0x04, A             ; Trigger interrupt Filesystem
    
    LD H, F                 ; Transfer the number of loaded palette colors to register H used with the interrupt below since we need that for the merging process

    ; Load the second tileset as merged to the first
    LD A, 0x36              ; Merge8BitPngWithCustomTransparency
    LD BCD, .PathTilesB     ; the source address (in RAM) of the null terminated string which represents the desired path to the image file to be processed.
    LD EFG, XYZ             ; the pointer to the address (in RAM) where the PNG palette and transparency data will be loaded.
    ; H already contains the number of existing colors on the color palette that will be merged into.
    LD IJK, .TilesBData     ; the pointer to the address (in RAM) where the PNG pixel data will be loaded.
    LD LMNO, 0xFF00FFFF     ; the RGBA color that will be interpreted as the transparent color from the provided RGB.
    INT 0x04, A             ; Trigger interrupt Filesystem

    RET


; Sets the video buffer mode to manual for all video pages
.SetVideoAutoControlMode
    LD A, 0x33
    LD B, 0b00000000
    INT 0x01, A
    RET


; Drawing the actual tiles on the second video layer, above the background.
; X and Y below represent the isometric coordinates. They will be later converted to cartesian coordinates.
.DrawScene
    LD Y, 0                     ; Start on vertical with the top row
.NextY
    LD X, 0                     ; Start on horizontal
.NextX
    LD UV, 0                    ; Here we're picking up the tile index from the .TerrainLayout defined below. To do that, we're
    LD W, Y                     ; converting the X and Y into a linear index. Since Y moves on isometric vertical,
    MUL UVW, 10                 ; we multiply it with 10 to land on the correct row, then
    ADD UVW, X                  ; we add X to the result so we can land on the correct tile.
    ADD UVW, .TerrainLayout     ; Now we have an index that needs to be added to the .TerrainLayout address offset
    LD W, (UVW)                 ; and getting the byte from that address lands us with the correct tile to place at this X, Y.

    CALLR .DrawSpriteIndex      ; We draw the sprite with the index stored in W
    INC X                       ; We move to the next tile on the horizontal
    CP X, 10                    ; until we reach the end
    JR NZ, .NextX
    INC Y                       ; ... then we move to the next row
    CP Y, 10                    ; until that's also finished
    JR NZ, .NextY

    RET


; Accepts sprinte index in register W, draws to layer 1
.DrawSpriteIndex
    PUSH A, Z               ; Preserving the registers since we have an enclosing loop that uses some of them
    LD I, 0
    LD J, W                 ; IJ will have the value of W (that itself can be anything from 0 to 15)
    DIV IJ, 4, GH           ; since the tileset is organized over a 4 x 4 pattern, we get IJ (the Y on the tilesheet) as the result of division by 4
                            ; and the remainder of that into GH (which will be the X)

    MUL GH, 32              ; Since each tile is 32 pixels wide, we multiply the X obtained above by this number
    MUL IJ, 24              ; ... and since the height is 24, we do the same for Y effectively getting to the top left pixel of the sprite to draw

    ; Convert isometric coordinates to cartesian coordinates
    LD Z, Y
    LD Y, 9                 ; 10 is the maximum row width in isometric space, so that's 0-9 (10 items)
    SUB Y, Z                ; Reverse the X coordinate so we can draw in the correct Z order

    ; This below transforms the X, Y of the isometric grid into the X, Y of the rendering layer. The formulas were obtained by drawing an isometric grid on paper,
    ; obtaining the actual coordinates by trial-error, observing the pattern and determing the formulas below which sit above their respective implementations
    ; however, a bit different, probably from the way I drew the coordinates at a larger step on the math paper.
    
    ; Compute cartesian X. Original formula is Xs = 2*(Xi + Yi + 2) * 32px
    LD PQ, 2
    ADD PQ, X
    ADD PQ, Y
    MUL PQ, 16

    ; Compute cartesian Y. Original formula is Ys = (8 + Xi - Yi) * 24px
    LD RS, 8
    ADD RS, X
    SUB RS, Y
    MUL RS, 8
    
    ; Defining interrupt input
    LD A, 0x0E              ; DrawTileMapSprite function index
    LD BCD, .TilesAData     ; the source address (in RAM) of the tile map. Since the two tile sheets are conveniently one right after another
                            ; we can access them both just by playing with the coordinates
    LD EF, 128              ; the width of the tilemap in pixels. This is used by the drawtilemapsprite interrupt to understand when's the next row of pixels
                            ; so it can travel vertically for its next sprite's row
    ; GH and IJ (x and y) are already set from the index
    LD KL, 32               ; the width of the sprite within the tile map.
    LD MN, 24               ; the height of the sprite within the tile map.
    LD O, 1                 ; the target video page where the sprite is to be drawn.

    ADD PQ, 52              ; Add an arbitrary offset to the X so it centers the scene horizontally
    ADD RS, 59              ; Add an arbitrary offset to the Y so it centers the scene vertically

    LD Z, (.zOffset)        ; Taking an always-incrementing offset to obtain an oscilation
    LD W, X                 ; We involve both isometric X and Y at higher magnitude so we can
    MUL W, 10               ; get the "wave" effect on a diagonal. We could restrict to either X or Y and the wave
    ADD Z, W                ; will simply follow that path only. Therefore, we add both X and Y with a higher
    LD W, Y                 ; magnitude to Z, which is the offset.
    MUL W, 16
    ADD Z, W                ; Then we call the function that gets us the oscillation based on the provided value.
    CALLR .GetOscillation   ; This function is meant to yield smooth ending values even if the byte input is restarted
    DIV Z, 16               ; The result is placed back in Z and we make it smaller so the effect is more subtle
    ADD RS, Z               ; We now add the displacement result to RS which is (as seen above) the cartesian Y coordinate
    LD T, 0b00000000        ; reset effect bits. Bit 0 - flip horizontal, bit 1 - flip vertical. The rest of the bits are not used.
    INT 0x01, A             ; Trigger interrupt Video with the function index stored in A to draw the tile

    POP A, Z                ; We return the previous values to registers before
    RET                     ; exiting back to the loop

.ClearActivePage
    LD A, 0x05              ; ClearVideoPage function index
    LD B, 1                 ; Clear second video page
    LD C, 0                 ; the color which will be used to fill that memory page (0 - transparent).
    INT 0x01, A             ; Trigger interrupt Video with the function index stored in A to clear the page

    RET


; This will hold a value that will be incremented each frame by 1 until 255 then it will start over from 0.
; It will be further processed by GetOscillation to transform it into a range of 0, 2, ..., 252, 254, 252, ... 2, 0
; which will represent a linear oscilation.
.zOffset
    #DB 0x00

; Some demo tiles layout. Each cell must take values from 0x00 to 0x0F
; If any value specified here is outside this range, the program will not break, however, it will interpret as image whatever it finds in memory
; outside the bounds of the coherent image and you'll get either some noisy artifacts or if the memory reached is filled with zeroes, nothing visible at all
; since zeroes mean transparent colors for all layers except the first one.
.TerrainLayout
    #DB 0x01, 0x01, 0x07, 0x03, 0x02, 0x01, 0x04, 0x0C, 0x0C, 0x0C
    #DB 0x02, 0x06, 0x00, 0x0D, 0x0D, 0x00, 0x05, 0x04, 0x0E, 0x0C
    #DB 0x00, 0x02, 0x0D, 0x0D, 0x0C, 0x0D, 0x01, 0x04, 0x0C, 0x0C
    #DB 0x02, 0x05, 0x0D, 0x0C, 0x0C, 0x0C, 0x0D, 0x07, 0x04, 0x0C
    #DB 0x00, 0x05, 0x04, 0x0C, 0x0C, 0x0C, 0x0C, 0x0D, 0x05, 0x04
    #DB 0x07, 0x05, 0x04, 0x0C, 0x0C, 0x0F, 0x0C, 0x0D, 0x01, 0x06
    #DB 0x03, 0x05, 0x04, 0x04, 0x0C, 0x0F, 0x0C, 0x0C, 0x0D, 0x01
    #DB 0x01, 0x07, 0x05, 0x04, 0x0A, 0x0F, 0x0B, 0x0C, 0x0D, 0x00
    #DB 0x06, 0x02, 0x00, 0x05, 0x08, 0x08, 0x08, 0x0D, 0x03, 0x07
    #DB 0x01, 0x06, 0x07, 0x00, 0x09, 0x09, 0x09, 0x00, 0x02, 0x01
