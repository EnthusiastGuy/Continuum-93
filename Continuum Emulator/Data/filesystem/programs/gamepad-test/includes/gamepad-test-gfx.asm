; Draws the controllers, buttons and represents their state
.DrawAllControllers
    LD A, 0                     ; Current controller index. This register must not be tainted along this loop

    CALLR .DrawProgramName      ; We draw the title. Since the DrawText draws on video page 2 that gets cleared often, we keep this here instead of drawing static (can be improved upon)
    CALLR .GetControllerNames   ; Gets the controller names from the interrupt and deposits them in memory for later

.next_controller_draw
    LD B, A                         ; We use B to help calculate the current controller drawing frame within the screen
    ADD B, B                        ; Multiply B by 2 since it will point to a 16-bit coordinate
    LD DEF, .controller_x_offset    ; Point to the X coordinate buffer
    ADD DEF, B                      ; Position ourselves to the correct X coordinate
    LD GH, (DEF)                    ; Load the X value in a 16-bit register
    LD (.spriteX), GH               ; ... so we can put it in a temporary place we know it's the X for our frame
    LD DEF, .controller_y_offset    ; Same here, point to the Y coordinate buffer
    ADD DEF, B                      ; Position ourselves to the correct Y coordinate
    LD GH, (DEF)                    ; Load the Y value in a 16-bit register
    LD (.spriteY), GH               ; ... so we can put it too in a temporary place we know it's the Y for our frame

    CALLR .IsControllerConnected    ; We determine if the current index has a controller connected
    JR NZ, .DrawInsertController        ; If not, we jump to draw the "INSERT CONTROLLER" message and continue the loop
    
    ; Draw a background for the controller name
    LD CDEF, 0xFFC70055             ; x, y
    LD GH, 236                      ; width
    LD IJ, 9                        ; height
    LD K, 3                         ; color
    CALLR .DrawRectangle

    CALLR .DrawCurrentControllerName    ; Draw the current controller name as reported by the information interrupt
    CALLR .DrawController               ; Draw the background controller sprite on top of which we'll be drawing buttons

; Starting here we'll basically check for every button/thumbstick/trigger to determine their state
; and depending on it we'll draw the representation of that state on top of the controller sprite
; at specified coordinates. This is a long and boring sequence.
    CALLR .IsButtonADown    ; A already contains the controller index
    JR NZ, .btn_A_not_pressed
    LD GHIJ, 0x000E0055     ; sprite x, y
    LD KLMN, 0x000B000C     ; sprite width, height
    LD PQ, 85   ; target x offset
    LD RS, 30   ; target y offset
    CALLR .DrawTile

.btn_A_not_pressed
    CALLR .IsButtonBDown    ; A already contains the controller index
    JR NZ, .btn_B_not_pressed
    LD GHIJ, 0x000E0055     ; sprite x, y
    LD KLMN, 0x000B000C     ; sprite width, height
    LD PQ, 94   ; target x offset
    LD RS, 22   ; target y offset
    CALLR .DrawTile

.btn_B_not_pressed
    CALLR .IsButtonXDown    ; A already contains the controller index
    JR NZ, .btn_X_not_pressed
    LD GHIJ, 0x000E0055     ; sprite x, y
    LD KLMN, 0x000B000C     ; sprite width, height
    LD PQ, 76   ; target x offset
    LD RS, 22   ; target y offset
    CALLR .DrawTile

.btn_X_not_pressed
    CALLR .IsButtonYDown    ; A already contains the controller index
    JR NZ, .btn_Y_not_pressed
    LD GHIJ, 0x000E0055     ; sprite x, y
    LD KLMN, 0x000B000C     ; sprite width, height
    LD PQ, 85   ; target x offset
    LD RS, 14   ; target y offset
    CALLR .DrawTile

.btn_Y_not_pressed
    CALLR .IsButtonDPadUpDown    ; A already contains the controller index
    JR NZ, .btn_DPUp_not_pressed
    LD GHIJ, 0x00440057     ; sprite x, y
    LD KLMN, 0x00080006     ; sprite width, height
    LD PQ, 39   ; target x offset
    LD RS, 36   ; target y offset
    CALLR .DrawTile

.btn_DPUp_not_pressed
    CALLR .IsButtonDPadDownDown    ; A already contains the controller index
    JR NZ, .btn_DPDown_not_pressed
    LD GHIJ, 0x00440064     ; sprite x, y
    LD KLMN, 0x00080006     ; sprite width, height
    LD PQ, 39   ; target x offset
    LD RS, 49   ; target y offset
    CALLR .DrawTile

.btn_DPDown_not_pressed
    CALLR .IsButtonDPadLeftDown    ; A already contains the controller index
    JR NZ, .btn_DPLeft_not_pressed
    LD GHIJ, 0x003E005D     ; sprite x, y
    LD KLMN, 0x00060007     ; sprite width, height
    LD PQ, 33   ; target x offset
    LD RS, 42   ; target y offset
    CALLR .DrawTile

.btn_DPLeft_not_pressed
    CALLR .IsButtonDPadRightDown    ; A already contains the controller index
    JR NZ, .btn_DPRight_not_pressed
    LD GHIJ, 0x004C005D     ; sprite x, y
    LD KLMN, 0x00060007     ; sprite width, height
    LD PQ, 47   ; target x offset
    LD RS, 42   ; target y offset
    CALLR .DrawTile

.btn_DPRight_not_pressed
    CALLR .IsButtonBigDown    ; A already contains the controller index
    JR NZ, .btn_big_not_pressed
    LD GHIJ, 0x00000055     ; sprite x, y
    LD KLMN, 0x000E000E     ; sprite width, height
    LD PQ, 53   ; target x offset
    LD RS, 14   ; target y offset
    CALLR .DrawTile

.btn_big_not_pressed
    CALLR .IsButtonBackDown    ; A already contains the controller index
    JR NZ, .btn_back_not_pressed
    LD GHIJ, 0x00190055     ; sprite x, y
    LD KLMN, 0x000A0004     ; sprite width, height
    LD PQ, 40   ; target x offset
    LD RS, 14   ; target y offset
    CALLR .DrawTile

.btn_back_not_pressed
    CALLR .IsButtonStartDown    ; A already contains the controller index
    JR NZ, .btn_start_not_pressed
    LD GHIJ, 0x00190055     ; sprite x, y
    LD KLMN, 0x000A0004     ; sprite width, height
    LD PQ, 70   ; target x offset
    LD RS, 14   ; target y offset
    CALLR .DrawTile

.btn_start_not_pressed
    CALLR .IsButtonLTDown    ; A already contains the controller index
    JR NZ, .btn_lt_not_pressed
    LD GHIJ, 0x00310057     ; sprite x, y
    LD KLMN, 0x00050002     ; sprite width, height
    LD PQ, 34   ; target x offset
    LD RS, 2   ; target y offset
    CALLR .DrawTile

.btn_lt_not_pressed
    CALLR .IsButtonRTDown    ; A already contains the controller index
    JR NZ, .btn_rt_not_pressed
    LD GHIJ, 0x00580057     ; sprite x, y
    LD KLMN, 0x00050003     ; sprite width, height
    LD PQ, 81   ; target x offset
    LD RS, 2   ; target y offset
    CALLR .DrawTile

.btn_rt_not_pressed
    CALLR .IsButtonLBDown    ; A already contains the controller index
    JR NZ, .btn_lb_not_pressed
    LD GHIJ, 0x00240059     ; sprite x, y
    LD KLMN, 0x00180009     ; sprite width, height
    LD PQ, 21   ; target x offset
    LD RS, 4   ; target y offset
    CALLR .DrawTile

.btn_lb_not_pressed
    CALLR .IsButtonRBDown    ; A already contains the controller index
    JR NZ, .btn_rb_not_pressed
    LD GHIJ, 0x0055005A     ; sprite x, y
    LD KLMN, 0x0018000A     ; sprite width, height
    LD PQ, 78   ; target x offset
    LD RS, 5   ; target y offset
    CALLR .DrawTile

.btn_rb_not_pressed
    LD F0, 5.1
    
; Draw left trigger value
    LD CDEF, 0x0000FFEB     ; x, y
    LD GH, 8                ; width
    LD IJ, 52               ; height
    LD K, 1                 ; color
    CALLR .DrawRectangle

    LD CDEF, 0x0001FFEC     ; x, y
    LD GH, 6                ; width
    LD IJ, 50               ; height
    LD K, 4                 ; color
    CALLR .DrawRectangle
    
    CALLR .GetLeftTriggerValue

    DIV Z, F0  ; Normalizes from (0 - 255) to (0 - 50)
    LD J, Z
    LD CDEF, 0x0001001E     ; x, y
    LD GH, 6                ; width
    LD K, 5                 ; color
    SUB EF, Z
    CALLR .DrawRectangle

; Draw right trigger value
    LD CDEF, 0x0070FFEB     ; x, y
    LD GH, 8                ; width
    LD IJ, 52
    LD K, 1                 ; color
    CALLR .DrawRectangle

    LD CDEF, 0x0071FFEC     ; x, y
    LD GH, 6                ; width
    LD IJ, 50
    LD K, 4                 ; color
    CALLR .DrawRectangle

    CALLR .GetRightTriggerValue

    DIV Z, F0  ; Normalizes from (0 - 255) to (0 - 50)
    LD J, Z
    LD CDEF, 0x0071001E     ; x, y
    LD GH, 6                ; width
    LD K, 5                 ; color
    SUB EF, Z
    CALLR .DrawRectangle

; Draw left thumb value

    LD CDEF, 0xFFC8FFEB     ; x, y
    LD GH, 53                ; width
    LD IJ, 53
    LD K, 1                 ; color
    CALLR .DrawRectangle

    LD CDEF, 0xFFC9FFEC     ; x, y
    LD GH, 51                ; width
    LD IJ, 51
    LD K, 4                 ; color
    CALLR .DrawRectangle

    CALLR .GetLeftThumbXValue
    DIV Z, F0
    LD (.left_thumb_x), Z                 ; Save leftX for later
    LD CD, 0xFFC7
    ADD CD, Z    
    CALLR .GetLeftThumbYValue
    DIV Z, F0
    LD (.left_thumb_y), Z                 ; Save leftY for later
    LD EF, 30
    SUB EF, Z

; Draw cross
    LD GH, 5                ; width
    LD J, 1                 ; height
    LD K, 5                 ; color
    CALLR .DrawRectangle

    LD GH, 1
    LD IJ, 5
    ADD CD, 2
    SUB EF, 2
    CALLR .DrawRectangle

; Draw right thumb value

    LD CDEF, 0x007C001E     ; x, y
    LD GH, 53                ; width
    LD IJ, 53
    LD K, 1                 ; color
    CALLR .DrawRectangle

    LD CDEF, 0x007D001F     ; x, y
    LD GH, 51                ; width
    LD IJ, 51
    LD K, 4                 ; color
    CALLR .DrawRectangle

    CALLR .GetRightThumbXValue
    DIV Z, F0
    LD (.right_thumb_x), Z                 ; Save leftX for later
    LD CD, 123
    ADD CD, Z    
    CALLR .GetRightThumbYValue
    DIV Z, F0
    LD (.right_thumb_y), Z                 ; Save leftX for later
    LD EF, 81
    SUB EF, Z

; Draw cross
    LD GH, 5                ; width
    LD J, 1                 ; height
    LD K, 5                 ; color
    CALLR .DrawRectangle

    LD GH, 1
    LD IJ, 5
    ADD CD, 2
    SUB EF, 2
    CALLR .DrawRectangle

; Display the thumbsticks depending on state
; Left thumbstick
    LD GH, 0
    CALLR .IsLeftThumbPressed
    JR NZ, .left_thumb_not_pressed
    INC H
    
.left_thumb_not_pressed
    RL H, 1
    CALLR .IsLeftThumbMoved
    JR NZ, .left_thumb_not_moved
    INC H
.left_thumb_not_moved
    MUL GH, 15      ; Sprite X
    LD IJ, 99       ; Sprite Y

    LD KLMN, 0x000F000F     ; sprite width, height
    LD PQ, 22   ; target x offset
    LD T, (.left_thumb_x)
    DIV T, 12
    ADD PQ, T
    LD RS, 22   ; target y offset
    LD T, (.left_thumb_y)
    DIV T, 12
    SUB RS, T

    CALLR .DrawTile

; Right thumbstick
    LD GH, 0
    CALLR .IsRightThumbPressed
    JR NZ, .right_thumb_not_pressed
    INC H
    
.right_thumb_not_pressed
    RL H, 1
    CALLR .IsRightThumbMoved
    JR NZ, .right_thumb_not_moved
    INC H
.right_thumb_not_moved
    MUL GH, 15      ; Sprite X
    LD IJ, 99       ; Sprite Y

    LD KLMN, 0x000F000F     ; sprite width, height
    LD PQ, 62   ; target x offset
    LD T, (.right_thumb_x)
    DIV T, 12
    ADD PQ, T
    LD RS, 39   ; target y offset
    LD T, (.right_thumb_y)
    DIV T, 12
    SUB RS, T

    CALLR .DrawTile
; Finished the boring job of displaying the state for current controller

.no_controller_draw
    INC A           ; We move to the next controller
    CP A, 4         ; ... but we check if this was the last one
    JR NZ, .next_controller_draw    ; ... if not, we continue

    VDL 0b00000111  ; Being done with drawing we signal that we want to manually draw video pages 0, 1 and 2
    RET

; Draws the "INSERT CONTROLLER" message which is yet another fancy sprite
.DrawInsertController
    LD GHIJ, 0x00000072     ; sprite x, y
    LD KLMN, 0x00790009     ; sprite width, height
    LD PQ, 1    ; target x offset
    LD RS, 32   ; target y offset
    CALLR .DrawTile
    JR .no_controller_draw  ; Since we jumped here, we continue jumping to continue the loop

; Draws the program name on the top bar
.DrawProgramName
    LD EFG, .program_name   ; text to draw
    LD HI, 1                ; x
    LD JK, 1                ; y
    CALLR .DrawText
    RET

; Draws the current controller name. Before calling this, a call to .GetControllerNames must be made
; This is not the most efficient way since we're calling .GetControllerNames everytime when we should only
; call it on new controller connected.
; input:    A register - the index that represents the current controller
.DrawCurrentControllerName
    PUSH A, Z
    LD B, A
    LD EFG, .ControllerNames        ; We point to the buffer where we loaded the controller names
.search_text
    CP B, 0                         ; If we landed on the current index to represent
    JR Z, .text_available           ; ... then we go to prepare coordinates and draw the text
    INC EFG                         ; otherwise, increment across the buffer
    CP (EFG), 0                     ; ... until we find the next zero string terminator
    JR NZ, .next                    ; if we didn't found the zero terminator, keep searching
    DEC B                           ; decrement the index pointer
    INC EFG                         ; increment one more time to jump over the terminator and land on printable text
.next
    JR .search_text                 ; repeat
.text_available
    LD HI, (.spriteX)               ; prepare the current frame X coordinate
    LD JK, (.spriteY)               ; and the Y coordinate
    SUB HI, 56                      ; Position X, Y relative to where we
    ADD JK, 86                      ; want to draw
    CALLR .DrawText                 ; Draw the found string which would be the controller name for the specified index
    POP A, Z
    RET

.DrawController
    PUSH A, Z
    LD A, 0x0E      ; Interrupt selector, function "Draw tile map sprite"
    LD BCD, 0x85000 ; Tilemap address
    LD EF, 121      ; Tilemap width
    LD GH, 0        ; Sprite x origin
    LD IJ, 0        ; Sprite y origin
    LD KL, 121      ; Sprite width
    LD MN, 85       ; Sprite height
    LD O, 1         ; Video page to draw to
    LD PQ, (.spriteX)   ; Specify current area coordinate X
    LD RS, (.spriteY)   ; Specify current area coordinate Y
    LD T, 0         ; No tile effects
    INT 0x01, A     ; Execute interrupt 0x01 (Video) with function 0x0E (Draw tile map sprite)
    POP A, Z
    RET

; Generic implementation that draws tiles on video page 2 (mostly dynamic tiles)
; Input:    GH and IJ - x and y of the sprite to draw as localized on the tilemap
;           KL and MN - width and height of the sprite to draw
;           PQ and RS - the x and y of where to draw the specified sprite
.DrawTile
    PUSH A, Z
    LD A, 0x0E      ; Interrupt selector, function "Draw tile map sprite"
    LD BCD, 0x85000 ; Tilemap address
    LD EF, 121      ; Tilemap width
    LD O, 2         ; Target video page
    ADD PQ, (.spriteX)    ; Adjust the relative offset of the current frame to the provided X
    ADD RS, (.spriteY)    ; ... and Y
    LD T, 0         ; No tile effects
    INT 0x01, A     ; Execute interrupt 0x01 (Video) with function 0x0E (Draw tile map sprite)
    POP A, Z
    RET

; Draws a rectangle
; Input:    CD and EF - x and y where to draw the rectangle
;           GH and IJ - the width and height of the rectangle
;           K will hold the color to draw the rectangle with
.DrawRectangle
    PUSH A, Z
    LD A, 0x06      ; Interrupt selector, function "Draw filled rectangle"
    LD B, 2         ; Target video page
    LD I, 0         ; IJ the height of the rectangle. Since we're working with small values, we can reset the I here and just play around with the provided J
    ADD CD, (.spriteX)    ; Again, adjust the relative offset of the current frame to the provided X
    ADD EF, (.spriteY)    ; ... and Y
    INT 0x01, A     ; Execute interrupt 0x01 (Video) with function 0x06 (Draw filled rectangle)
    POP A, Z
    RET

; Draw the title bar that's underneath the... title.
.DrawTitleBar
    PUSH A, Z
    LD A, 0x06      ; Interrupt selector, function "Draw filled rectangle"
    LD B, 0         ; Target video page
    LD GH, 480      ; The width of the title bar (in this case, full width)
    LD IJ, 8        ; IJ the height of the rectangle.
    LD K, 0x33      ; color of the title bar
    LD CD, 0        ; X position of the title bar
    LD EF, 0        ; Y position of the title bar
    INT 0x01, A     ; Execute interrupt 0x01 (Video) with function 0x06 (Draw filled rectangle)
    POP A, Z
    RET

; Draws a piece of text on the screen
; Input:    EFG - the address to the null terminates string that we want to draw
;           HI and JK - x and y of where to draw the text
.DrawText
    PUSH A, O
    LD A, 0x12					; Interrupt selector, function "Draw text"
    LD BCD, .FontData			; the source address (in RAM) of the font to be used.
    LD L, 1		                ; the color used to draw the string
    LD M, 2 	                ; the video page on which we draw the string
    LD NO, 233                  ; This will provide a horizontal limit for the text to draw. This limit is actually for the controller name
                                ; since that method also uses this and any other texts are shorter anyway
    INT 0x01, A					; Execute interrupt 0x01 (Video) with function 0x12 (Draw text)
    POP A, O
    RET

; Provides the controller names 
.GetControllerNames
    PUSH A, Z
    LD A, 0x16                  ; Interrupt selector, function "Read gamepads names"
    LD BCD, .ControllerNames    ; Where to deposit the names once retrieved
    INT 0x02, A                 ; Execute interrupt 0x02 (Input) with function 0x16 (Read gamepads names)
    POP A, Z
    RET

; Clears the specified video page with specified color
; Input:    A - the video page which needs clearing (0 - 7)
; Input:    B - the color which will be used to fill that memory page (0 - transparent or 1 - 255)
.ClearScreen
    PUSH Z
    LD Z, 0x05 	; ClearVideoPage
    INT 0x01, Z ; Trigger interrupt Video
    POP Z
    RET

; Here we start keeping some data either hardcoded, or some buffers to fill with data we obtain/calculate in realtime
.TilemapFilePath
    #DB "programs\gamepad-test\tiles\controller_tilesheet.png", 0

.PaletteSize
    #DB 0

.spriteX
    #DB 0x018A;

.spriteY
    #DB 0x00FF;

.controller_x_offset
    #DB 0x003C, 0x012C, 0x003C, 0x012C

.controller_y_offset
    #DB 0x0028, 0x0028, 0x00AF, 0x00AF

.left_thumb_x
    #DB 0x00

.left_thumb_y
    #DB 0x00

.right_thumb_x
    #DB 0x00

.right_thumb_y
    #DB 0x00

.program_name
    #DB "Gamepad test", 0

.FontFile
	#DB "fonts\SlickAntsContourSlim.font", 0

.FontData
	#DB [952] 0

.ControllerNames
	#DB [256] 0
    