.GamepadsInputEvaluate
    CALLR .GamepadsInputUpdate              ; Update gamepad states
    CALLR .InputUpdate
    
    RET

; Using the keyboard input only to check the ESC key so we can exit
; Returns Z flag set if ESC pressed
.CheckIfUserWantsToExit
    LD A, 27				; Escape key code
    CALLR .InputKeyPressed
    CP A, 1
    RET