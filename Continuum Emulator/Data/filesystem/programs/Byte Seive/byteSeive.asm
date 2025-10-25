; Byte Seive implementation for Continuum 93
; https://en.wikipedia.org/wiki/Byte_Sieve
; https://archive.org/details/byte-magazine-1981-09/page/n181

#include ..\..\lib\c93-keyboard.asm

#ORG 0x80000


	LD A, 0x02              ; SetVideoPagesCount
    LD B, 1                 ; set 1 video page
    INT 0x01, A             ; Trigger interrupt Video with function SetVideoPagesCount

    LD A, 0x05     			; ClearVideoPage
	LD B, 0x00     			; the video page which needs clearing (0 - 7)
	LD C, 0x85     			; the color which will be used to fill that memory page (0 - transparent or 1 - 255).
	INT 0x01, A       		; Trigger interrupt Video

	LD A, 0x40
    LD BCD, .FontPath
    LD EFG, .FontData
    INT 0x04, A				; Load font

	CALL .DrawText


	LD CDE, .StartClock
	CALL .SetTime

; Start the sieve ======================================
	LD EFGH, 0			; prime
	LD MNOP, 0			; k

	LD A, 200			; 200 iterations
.nextIteration
	LD IJKL, 0			; count = 0;
	
	LD XYZ, 8191
	LD BCD, .flags		; 

	; Fill the sieve with 1s
.populateNext
	LD (BCD), 1, 1			;
	INC BCD
	DEC XYZ
	CP XYZ, 0
	JR NE, .populateNext	;

	LD BCD, .flags		;
	LD XYZ, 0

.loop3
	CP (BCD), 1			;
	JR NE, .notPrime		;
	LD EFGH, 3			; EFGH = 3
	ADD EFGH, XYZ		; EFGH += i
	ADD EFGH, XYZ		; EFGH += i => EFGH = 3 + 2*i

	LD MNOP, EFGH		; MNOP = prime
	ADD MNOP, XYZ		; MNOP = prime + i => matches 'k'
.while
	CP MNOP, 8190
	JR GT, .exit
	LD QRS, .flags
	ADD QRS, NOP
	LD (QRS), 0, 1
	ADD MNOP, EFGH
	JR .while
.exit
	INC IJKL

.notPrime
	INC BCD
	INC XYZ
	CP XYZ, 8191
	JR LT, .loop3		; loop until 8190
	DEC A
	CP A, 0
	JR NE, .nextIteration	; loop until 10 iterations

	LD (.PrimeCount), IJKL	; Store the result

; End the sieve ======================================

	LD CDE, .EndClock
	CALL .SetTime

	; Display results
	LD BCDE, (.EndClock)
	LD FGHI, (.StartClock)

	SUB BCDE, FGHI

	LD A, 0x02
	LD FGH, .TimeStringFormat
	LD IJK, .TimeString
	INT 0x05, A

	LD BCDE, (.PrimeCount)

	LD FGH, .PrimeCountStringFormat
	LD IJK, .PrimeCountString
	INT 0x05, A

	CALL .DrawTimeText
	CALL .DrawPrimeText
	
.HandleEscWait
    CALLR .InputUpdate

    LD A, 27	; Esc key
	CALL .InputKeyPressed
	CP A, 1
	
    RETIF Z

    WAIT 16
    JP .HandleEscWait



.DrawText
	LD A, 0x14
	LDREGS A, R, (.DrawTextDrawParams)
	INT 0x01, A
	RET

.DrawTimeText
	LD A, 0x14
	LDREGS A, R, (.DrawTimeTextDrawParams)
	INT 0x01, A
	RET

.DrawPrimeText
	LD A, 0x14
	LDREGS A, R, (.DrawPrimesTextDrawParams)
	INT 0x01, A
	RET

.SetTime
	LD A, 0x03
	LD B, 0x00
	
	INT 0x00, A
	RET



.flags
	#DB [8191] 0x00	; Reserve 8191 bytes for the sieve

.FontPath
    #DB "programs\Byte Seive\RealityOneWideFont.png", 0

.DrawTextDrawParams
    ; DrawText, font data, text, x, y, color, video page, max width, flags, outline color, outline pattern
    #DB 0x14, .FontData, .TextIterations, 0080, 00100, 0x00, 0x00, 00320, 0b00111000, 0xFF, 0b11111111

.DrawTimeTextDrawParams
    ; DrawText, font data, text, x, y, color, video page, max width, flags, outline color, outline pattern
    #DB 0x14, .FontData, .TimeString, 0080, 00140, 0x00, 0x00, 00320, 0b00111000, 0xFF, 0b11111111

.DrawPrimesTextDrawParams
    ; DrawText, font data, text, x, y, color, video page, max width, flags, outline color, outline pattern
    #DB 0x14, .FontData, .PrimeCountString, 0080, 00120, 0x00, 0x00, 00320, 0b00111000, 0xFF, 0b11111111

.TextIterations
    #DB "200 iterations, running...", 0

.PrimeCount
	#DB 0x00, 0x00, 0x00, 0x00

.StartClock
	#DB 0x00000000

.EndClock
	#DB 0x00000000

.PrimeCountStringFormat
	#DB "'Primes found:' 0 '' per loop", 0

.PrimeCountString
	#DB [32] 0x00

.TimeStringFormat
	#DB "'Elapsed ms:' 0 ''", 0

.TimeString
	#DB [32] 0x00

.FontData
    #DB [10000] 0		; Always reserve sufficient space, especially when using #include

