; The isometric tileset used with this demo has been designed by NullDreams
; please visit his page to see his more of his work here: https://nulldreams.itch.io

; This program exemplifies how you can load different 8 bit palette based PNG files with different color palettes and merge those into one of the
; video palettes that's meant to draw them. The usecase for such a situation is when you have some fixed tileset you need to render on the same
; layer as other tiles that can change depending on the level. As such, to not overflow the palette capacity you would slice the tiles on themes
; pertaining to each individual level and merge them using this mechanism each time you move to a different game level.

#include includes\keyboard.asm
#include includes\gfx.asm
#include includes\clock.asm
#include includes\utils.asm
#include includes\data.asm

    #ORG 0x80000                    ; We position our code outside the OS reserved area of 512k
    
    CALLR .InitializeGFX            ; Set video mode, load background and tiles

    VDL 0b00000001                  ; Signal to manually refresh first video page (first bit is set) since InitializeGFX
                                    ; already loads the backgound in memory for video page 1. Alternatively, the VDL below can
                                    ; be modified to do this refresh by changing it to VDL 00000011, but since the background
                                    ; never changes, it suffices to do it once here.

    CALLR .InitializeClock          ; Initialize the clock time
    LD (.zOffset), 0                ; This will be used to manage an oscilator used to sustain the main animation


.Repeat                             ; Main loop
    CALL .HasUpdateTimePassed       ; Updates the clock and also returns whether it passed the target frame time
    JR NZ, .Repeat                  ; We loop back until the allowed timeframe is reached

    CALLR .InputUpdate              ; Update the keyboard input buffers to be able to determine changes
    ADD (.zOffset), 7               ; Controls the speed of the animation. Higher values -> faster  (default 7)

	LD A, 27				        ; Escape key
	CALLR .InputKeyPressed          ; Check if the keycode in A has just been pressed
	RETIF Z                         ; Exit if pressed

    CALLR .Redraw                   ; Redraw whole scene

    JR .Repeat

.Redraw
    CALLR .ClearActivePage          ; Clear the layer where the active drawing takes place
    CALLR .DrawScene                ; Draw tiles

    VDL 0b00000010                  ; Signal to manually refresh second video page (second bit is set)
    RET


