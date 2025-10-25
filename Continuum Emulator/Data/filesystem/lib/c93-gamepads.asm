; Handles gamepads input. Provides methods to get the state of all buttons/thumbs/triggers etc

.GamepadsInputUpdate
    PUSH A, Z
    
    ; Copy current state to the previous state buffer
    LD ABC, .gamepads_input_CSB
    LD DEF, .gamepads_input_PSB
    MEMC ABC, DEF, 41
    MEMF ABC, 41, 0         ; Clear the input buffer to not have rezidues

    ; Grab current state
    LD Z, 0x14              ; ReadGamePadsState, ABC already contains the target address
    INT 0x02, Z             ; Trigger interrupt `Input/Read GamePads State`

    POP A, Z
    RET

; checks whether any button on any controller is in down state
; output:   Z flag is set if true
.IsAnyButtonOnAnyControllerDown
    PUSH A, Z
    LD B, (.gamepads_input_CSB)
    AND B, 0b11110000
    CP B, 0
    INVF Z
    POP A, Z
    RET
	
; checks whether any button on controller 0 is in down state
; output:   Z flag is set if true
.IsAnyButtonOnController0Down
    PUSH A, Z
    LD B, (.gamepads_input_CSB)
    AND B, 0b00010000
    CP B, 0
    INVF Z
    POP A, Z
    RET

; checks whether a controller identified by its index is connected
; input:    A - the index of the controller to check connection on
; output:   Z flag is set if true
.IsControllerConnected
    PUSH A, Z
    LD B, (.gamepads_input_CSB)
    BIT B, A
    POP A, Z
    RET

; checks if button DPad Up is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonDPadUpDown
    PUSH A, Z
    LD X, 0b00000001
    LD Y, 1
    CALLR .is_state_button_down
    POP A, Z
    RET
	
; checks if button DPad Up has just been pressed on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonDPadUpPressed
    PUSH A, Z
    LD X, 0b00000001
    LD Y, 1
    CALLR .is_state_button_pressed
    POP A, Z
    RET

; checks if button DPad Down is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonDPadDownDown
    PUSH A, Z
    LD X, 0b00000010
    LD Y, 1
    CALLR .is_state_button_down
    POP A, Z
    RET
	
; checks if button DPad Down has just been pressed on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonDPadDownPressed
    PUSH A, Z
    LD X, 0b00000010
    LD Y, 1
    CALLR .is_state_button_pressed
    POP A, Z
    RET

; checks if button DPad Left is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonDPadLeftDown
    PUSH A, Z
    LD X, 0b00000100
    LD Y, 1
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if button DPad Right is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonDPadRightDown
    PUSH A, Z
    LD X, 0b00001000
    LD Y, 1
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if button A is in down state on specified controller
; input:    A - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonADown
    PUSH A, Z
    LD X, 0b00010000
    LD Y, 1
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if button A has just been pressed on specified controller
; input:    A - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonAPressed
    PUSH A, Z
    LD X, 0b00010000
    LD Y, 1
    CALLR .is_state_button_pressed
    POP A, Z
    RET

; checks if button B is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonBDown
    PUSH A, Z
    LD X, 0b00100000
    LD Y, 1
    CALLR .is_state_button_down
    POP A, Z
    RET
	
; checks if button B has just been pressed on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonBPressed
    PUSH A, Z
    LD X, 0b00100000
    LD Y, 1
    CALLR .is_state_button_pressed
    POP A, Z
    RET

; checks if button X is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonXDown
    PUSH A, Z
    LD X, 0b01000000
    LD Y, 1
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if button Y is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonYDown
    PUSH A, Z
    LD X, 0b10000000
    LD Y, 1
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if left thumb joystick is pointing up on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsThumbLeftPointingUp
    PUSH A, Z
    LD X, 0b00000001
    LD Y, 2
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if left thumb joystick is pointing down on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsThumbLeftPointingDown
    PUSH A, Z
    LD X, 0b00000010
    LD Y, 2
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if left thumb joystick is pointing left on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsThumbLeftPointingLeft
    PUSH A, Z
    LD X, 0b00000100
    LD Y, 2
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if left thumb joystick is pointing right on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsThumbLeftPointingRight
    PUSH A, Z
    LD X, 0b00001000
    LD Y, 2
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if right thumb joystick is pointing up on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsThumbRightPointingUp
    PUSH A, Z
    LD X, 0b00010000
    LD Y, 2
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if right thumb joystick is pointing down on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsThumbRightPointingDown
    PUSH A, Z
    LD X, 0b00100000
    LD Y, 2
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if right thumb joystick is pointing left on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsThumbRightPointingLeft
    PUSH A, Z
    LD X, 0b01000000
    LD Y, 2
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if right thumb joystick is pointing right on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsThumbRightPointingRight
    PUSH A, Z
    LD X, 0b10000000
    LD Y, 2
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if button BigButton is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonBigDown
    PUSH A, Z
    LD X, 0b01000000
    LD Y, 3
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if button Back is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonBackDown
    PUSH A, Z
    LD X, 0b00100000
    LD Y, 3
    CALLR .is_state_button_down
    POP A, Z
    RET
	
; checks if button Back has just been pressed on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonBackPressed
    PUSH A, Z
    LD X, 0b00100000
    LD Y, 3
    CALLR .is_state_button_pressed
    POP A, Z
    RET

; checks if button Start is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonStartDown
    PUSH A, Z
    LD X, 0b00010000
    LD Y, 3
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if button Right Trigger is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonRTDown
    PUSH A, Z
    LD X, 0b00001000
    LD Y, 3
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if button Left Trigger is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonLTDown
    PUSH A, Z
    LD X, 0b00000100
    LD Y, 3
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if button Left Button is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonLBDown
    PUSH A, Z
    LD X, 0b00000001
    LD Y, 3
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if button Right Button is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsButtonRBDown
    PUSH A, Z
    LD X, 0b00000010
    LD Y, 3
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if button Left Thumbstick is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsLeftThumbPressed
    PUSH A, Z
    LD X, 0b00000001
    LD Y, 4
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if button Right Thumbstick is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsRightThumbPressed
    PUSH A, Z
    LD X, 0b00000010
    LD Y, 4
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if any of A, B, X or Y is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsAnyABXYPressed
    PUSH A, Z
    LD X, 0b00000100
    LD Y, 4
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if any DPad button is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsAnyDPadPressed
    PUSH A, Z
    LD X, 0b00001000
    LD Y, 4
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if any L/R Shoulder button is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsAnyShoulderPressed
    PUSH A, Z
    LD X, 0b00010000
    LD Y, 4
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if any L/R Trigger button is in down state on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsAnyTriggerPressed
    PUSH A, Z
    LD X, 0b00100000
    LD Y, 4
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if the left thumbstick is moved in any direction on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsLeftThumbMoved
    PUSH A, Z
    LD X, 0b01000000
    LD Y, 4
    CALLR .is_state_button_down
    POP A, Z
    RET

; checks if the right thumbstick is moved in any direction on specified controller
; input:    A register - the index of the controller to check button on
; output:   Z flag is set if true
.IsRightThumbMoved
    PUSH A, Z
    LD X, 0b10000000
    LD Y, 4
    CALLR .is_state_button_down
    POP A, Z
    RET

; gets the value of Left Trigger
; input:    A register - the index of the controller to check button on
; output:   Z register - the requested value (signed)
.GetLeftTriggerValue
    PUSH A, Y
    LD Y, 5
    CALLR .get_state_value
    POP A, Y
    RET

; gets the value of Right Trigger
; input:    A register - the index of the controller to check button on
; output:   Z register - the requested value (signed)
.GetRightTriggerValue
    PUSH A, Y
    LD Y, 6
    CALLR .get_state_value
    POP A, Y
    RET

; gets the value of Left Thumb X
; input:    A register - the index of the controller to check button on
; output:   Z register - the requested value (signed)
.GetLeftThumbXValue
    PUSH A, Y
    LD Y, 7
    CALLR .get_state_value
    POP A, Y
    RET

; gets the value of Left Thumb Y
; input:    A register - the index of the controller to check button on
; output:   Z register - the requested value (signed)
.GetLeftThumbYValue
    PUSH A, Y
    LD Y, 8
    CALLR .get_state_value
    POP A, Y
    RET

; gets the value of Right Thumb X
; input:    A register - the index of the controller to check button on
; output:   Z register - the requested value (signed)
.GetRightThumbXValue
    PUSH A, Y
    LD Y, 9
    CALLR .get_state_value
    POP A, Y
    RET

; gets the value of Right Thumb Y
; input:    A register - the index of the controller to check button on
; output:   Z register - the requested value (signed)
.GetRightThumbYValue
    PUSH A, Y
    LD Y, 10
    CALLR .get_state_value
    POP A, Y
    RET


; ==================== Private utility functions ====================

; generic functionality that checks whether the bit pointed by the X
; register is set thereby indicating respective button is down
; input:    A contains the controller id (0 - 3)
;           X contains the binary sequence to point to button bit
; output:   Z flag is set if bit is set
.is_state_button_down
    LD BCD, .gamepads_input_CSB
    ADD BCD, Y
    MUL A, 10
    ADD BCD, A
    LD A, (BCD)
    INV A
    AND A, X
    CP A, 0
    RET

; generic functionality that checks whether the bit pointed by the X
; register is set in current state but reset in previous thereby indicating
; respective button has just been pressed
; input:    A contains the controller id (0 - 3)
;           X contains the binary sequence to point to button bit
; output:   Z flag is set if bit is set
.is_state_button_pressed
    LD BCD, .gamepads_input_CSB
    ADD BCD, Y
    MUL A, 10
    ADD BCD, A
    LD A, (BCD)
	INV A
    AND A, X
	ADD BCD, 41	; Add 41 bytes so we get to the "previous" buffer
	LD B, (BCD)
	AND B, X
	ADD A, B
    CP A, 0
    RET

; generic functionality that returns a specified value pointed at by
; the y register from the current state.
; input:    A register contains the controller id (0 - 3)
;           Y register contains the offset to the byte per controller state
; output:   Z register contains the return value
.get_state_value
    LD BCD, .gamepads_input_CSB
    ADD BCD, Y
    MUL A, 10
    ADD BCD, A
    LD Z, (BCD)
    RET
	
; generic functionality that returns a specified value pointed at by
; the y register from the previous state.
; input:    A register contains the controller id (0 - 3)
;           Y register contains the offset to the byte per controller state
; output:   Z register contains the return value
.get_prev_state_value
    LD BCD, .gamepads_input_PSB
    ADD BCD, Y
    MUL A, 10
    ADD BCD, A
    LD Z, (BCD)
    RET



; ==================== State buffers structure ====================

; - 1 connection and status byte:
; ANY[n] - is any button pressed on controller [n]
; CON[n] - is controller [n] connected
; Yes: 1, No: 0
;   b7  |    b6  |    b5  |    b4  |    b3  |    b2  |    b1  |    b0
;-----------------------------------------------------------------------
; ANY3  |  ANY2  |  ANY1  |  ANY0  |  CON3  |  CON2  |  CON1  |  CON0

; For each controller
; 10 bytes per controller (max 4 controllers) consisting of:

; - first byte for DPad + ABXY butons pressed:
;
;   b7  |    b6  |    b5  |    b4  |    b3  |    b2  |    b1  |    b0
;-----------------------------------------------------------------------
; Btn.Y |  Btn.X |  Btn.B |  Btn.A |DP.Right| DP.Left| DP.Down|  DP.Up

; - second byte for ThumbStick left and right. They register just the simple
; direction, without the progressive value:
;
;     b7    |      b6    |      b5    |      b4    |      b3    |      b2    |      b1    |      b0
;------------------------------------------------------------------------------------------------------
;TRght.Right| TRght.Left | TRght.Down |  TRght.Up  | TLft.Right |  TLft.Left |  TLft.Down |  TLft.Up

; - third byte for misc buttons:
; None means that no button is pressed, the inverse of "any button pressed"
;
;     b7    |      b6    |      b5    |      b4    |      b3    |      b2    |      b1    |      b0
;------------------------------------------------------------------------------------------------------
;    None   |  BigButton |     Back   |    Start   |  RTrigger  |  LTrigger  |  RShoulder |  LShoulder

; - fourth byte for left/right stick presses and misc flags
; b0:   Left stick is pressed
; b1:   Right stick is pressed
; b2:   Any of A, B, X, Y pressed
; b3:   Any of the DPad buttons pressed
; b4:   Any of L/R Shoulder pressed
; b5:   Any of L/R Trigger pressed
; b6:   Any left thumb direction actioned
; b7:   Any right thumb direction actioned

; - fifth byte contains the Left Trigger Z value (0 to 255)
; - sixth byte contains the Right Trigger Z value (0 to 255)
; - seventh byte contains the left thumb X value (-128 to 127)
; - eight byte contains the left thumb Y value (-128 to 127)
; - ninth byte contains the right thumb X value (-128 to 127)
; - tenth byte contains the right thumb Y value (-128 to 127)

.gamepads_input_CSB      ; current state buffer
    #DB [41] 0

.gamepads_input_PSB      ; previous state buffer
    #DB [41] 0
