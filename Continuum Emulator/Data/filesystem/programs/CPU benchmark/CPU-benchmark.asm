#include ..\..\lib\c93-keyboard.asm

	; Assemble this code starting at this address so it doesn't interfere with the OS
	#ORG 0x80000

	LD A, 0x02 	; SetVideoPagesCount interrupt
	LD B, 1 	; set a single video page
	INT 0x01, A ; Trigger interrupt Video with SetVideoPagesCount

	CALLR .ClearPage
	
	; Loading a font to be used
    LD A, 0x40
    LD BCD, .FontPath
    LD EFG, .FontData
    INT 0x04, A

	LDREGS A, R, (.DrawTextDrawParams)
	LD JK, 10
	LD EFG, .WaitMessage
	INT 0x01, A



	; Record the time at the start of the benchmark
	LD A, 0x03
	LD B, 0x00
	LD CDE, .StartTime
	INT 0x00, A
	
	; Run the benchmark 10 million times for accuracy
	LD XYZ, 10000000

; We create a simple benchmark that runs some simple instructions
; for which we know what the assigned CPU cycles are, resulting
; in a completely measurable execution cycles total. The benchmark
; below takes 28 CPU cycles per run
.BenchLoop
	LD XYZ, XYZ			; 6 cycles
	DEC XYZ				; 6 cycles
	CP XYZ, 0			; 8 cycles
	JR GT, .BenchLoop	; 8 cycles
	
	; Record the time at the end of the benchmark
	LD A, 0x03
	LD B, 0x00
	LD CDE, .EndTime
	INT 0x00, A
	
	LD ABCD, (.EndTime)
	LD EFGH, (.StartTime)
	
	LD F0, 280000000.0

	SUB ABCD, EFGH

	DIV F0, ABCD
	DIV F0, 1000.0
	
	; Convert to string
	LD A, 0x01
	LD B, 0
	LD CDE, .NumberFormat
	LD FGH, .TextString
	INT 0x05, A

	CALLR .ClearPage

	; Draw a nice rectangle
	LD A, 0x07
	LD B, 0
	LD CD, 80
	LD EF, 64
	LD GH, 320
	LD IJ, 100
	LD K, 0x06
	INT 0x01, A

	; Get the CPU designation name into the .CPUDesignation buffer
	LD A, 0xF0
	LD BC, F0
	LD DEF, .CPUDesignation
	INT 0x00, A

	LDREGS A, R, (.DrawTextDrawParams)
	INT 0x01, A

	ADD JK, 16
	LD EFG, .CPUDesignation
	INT 0x01, A

	LD JK, 10
	LD EFG, .DoneMessage
	INT 0x01, A

	LD JK, 250
	LD EFG, .ExitMessage
	INT 0x01, A

.HandleEscWait
    CALLR .InputUpdate

    LD A, 27	; Esc key
	CALLR .InputKeyPressed
	CP A, 1

    RETIF Z

    WAIT 16
    JP .HandleEscWait
	
.ClearPage
	LD Z, 0x05 	; ClearVideoPage
	LD AB, 0x00DA	; Clear video page 0 (background) with a solid color
	INT 0x01, Z ; Trigger interrupt Video
	RET
	
.WaitMessage
	#DB "Please wait up to a few minutes", 0

.DoneMessage
	#DB "Benchmark done", 0

.NumberFormat
	#DB "'CPU Frequency:' 0.00 'Mhz'", 0

.TextString
	#DB [40] 0

.CPUDesignation
	#DB [40] 0

.ExitMessage
	#DB "Press ESC to exit", 0

.DrawTextDrawParams
    ; DrawText, font data, text, x, y, color, video page, max width, flags, outline color, outline pattern
    #DB 0x14, .FontData, .TextString, 0080, 00100, 0x00, 0x00, 00320, 0b00111000, 0xFF, 0b11111111
	
.StartTime
	#DB 0x00000000
	
.EndTime
	#DB 0x00000000
	
.OffsetTime
	#DB 0x00000000

.FontPath
	#DB "programs\CPU benchmark\font.png", 0

.FontData

	#DB [5000] 0x00
