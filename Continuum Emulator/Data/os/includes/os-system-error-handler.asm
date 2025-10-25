.HandleSystemError
    CALLR .InitVideo
    CALLR .ClearAllLayers

    ; Load font
    LD A, 0x40
    LD BCD, .HandleSystemErrorFontFile
    LD EFG, .HandleSystemErrorFontFileData
    INT 0x04, A

    LDREGS A, R, (.HandleSystemErrorTextDrawParams)

    GETVAR 0xFFFE, WXYZ     ; Get the last error
    SETVAR 0xFFFE, 0        ; Reset the error

    CP WXYZ, 0x10   ; Stack overflow
    JR Z, .HandleSystemErrorStackOverflow

    CP WXYZ, 0x11   ; Stack underflow
    JR Z, .HandleSystemErrorStackUnderflow

    LD EFG, .HandleSystemErrorMessageUnknown
    JP .HandleSYstemErrorPrintMessage


.HandleSystemErrorStackOverflow
    LD EFG, .HandleSystemErrorMessageStackOverflow
    JP .HandleSYstemErrorPrintMessage

.HandleSystemErrorStackUnderflow
    LD EFG, .HandleSystemErrorMessageStackUnderflow
    JP .HandleSYstemErrorPrintMessage



.HandleSYstemErrorPrintMessage
    INT 0x01, A

    VDL 0b00000111

.HandleSystemErrorEscWait
    CALLR .InputUpdate

    LD A, 27	; Esc key
	CALLR .InputKeyPressed
	CP A, 1

    JP Z, 0x000000 ; Restart OS

    WAIT 16
    JP .HandleSystemErrorEscWait


.HandleSystemErrorTextDrawParams
    ; DrawText, font data, text, x, y, color, video page, max width, flags, outline color, outline pattern
    #DB 0x14, .HandleSystemErrorFontFileData, .HandleSystemErrorMessageStackOverflow, 0080, 0080, 0x00, 0x00, 00320, 0b00111000, 0xFF, 0b11111111
    ; Flags: [unused], [unused], [outline], [wrap], [centered], [disable kerning], [monospace center], [monospace]
    

.HandleSystemErrorFontFile
	#DB "fonts\Adventure.png", 0

.HandleSystemErrorMessageUnknown
    #DB "Oops, something happened but we don't know what (unrecognized error). Press Esc and let's hope the OS still works down there somewhere...", 0

.HandleSystemErrorMessageStackOverflow
    #DB "An intolerable stack overflow has occurred. The system hasn't crashed, more like took a dramatic detour. Details of the error and its whereabouts can be found in the general log file. Press Escape to jump back to where the OS is already waiting, along with your programs, which are likely as perplexed as you and I are.", 0

.HandleSystemErrorMessageStackUnderflow
    #DB "An intolerable stack underflow has occurred. The system hasn't crashed, more like took a dramatic detour. Details of the error and its whereabouts can be found in the general log file. Press Escape to jump back to where the OS is already waiting, along with your programs, which are likely as perplexed as you and I are.", 0


.HandleSystemErrorFontFileData
    #DB [5000] 0