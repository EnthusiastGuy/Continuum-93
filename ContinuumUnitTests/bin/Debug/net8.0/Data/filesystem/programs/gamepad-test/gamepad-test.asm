#include ..\..\lib\c93-gamepads.asm
#include ..\..\lib\c93-keyboard.asm
#include includes\gamepad-test-init.asm
#include includes\gamepad-test-input.asm
#include includes\gamepad-test-gfx.asm

    #ORG 0x080000       ; Start after 512k (which are reserved for the OS)

    CALLR .Initialize   ; Setting up some things first
    
.MainLoop
    LD AB, 0x0200       ; Clear page 2 with transparent
	CALLR .ClearScreen
    LD AB, 0x0100       ; Clear page 1 with transparent
	CALLR .ClearScreen

    CALLR .GamepadsInputEvaluate    ; Get the input from gamepads
    CALLR .CheckIfUserWantsToExit   ; Exit to OS if user so commands
    JR Z, .Exit

    CALLR .DrawAllControllers       ; Draw the current state of the controllers

    JR .MainLoop                    ; And so on and on...

.Exit
    RET     ; Back to OS