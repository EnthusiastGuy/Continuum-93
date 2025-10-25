
	#ORG 0x080000       ; Start after 512k (which are reserved for the OS)

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
    INT 0x04, A

    LD A, 0x14
    LD BCD, .FontData
    LD EFG, .HelloWorld2
    LD HI, 80           ; x
    LD JK, 40           ; y
    LD L, 0x0           ; color
    LD M, 0             ; video page
    LD NO, 320          ; max width
    LD P, 0b00111000    ; [unused], [unused], [outline], [wrap], [centered], [disable kerning], [monospace center], [monospace]
    LD Q, 0xFF          ; Outline color
    LD R, 0b00100100    ; Outline pattern

    INT 0x01, A


    LD A, 0x41          ; Copy rectangle
    LD B, 0             ; source video page
    LD CD, 80           ; source X
    LD EF, 50           ; source Y
    LD GH, 100          ; source width
    LD IJ, 100          ; source height
    LD K, 0             ; destination video page
    LD LM, 0            ; destination X
    LD NO, 200          ; destination Y
    LD PQ, 200          ; destination width
    LD RS, 200          ; destination height

    INT 0x01, A


    LD A, 0x25          ; Bezier path
    LD BCD, .PathData   ; path data
    LD E, 0             ; video page
    LD F, 4             ; brush width
    LD G, 6             ; filled segment size
    LD H, 3             ; empty segment size
    LD I, 0x9F          ; color
    LD J, 0xFF          ; outline color
    LD K, 0             ; start percent
    LD L, 1           ; end percent

    LD J, 0

.RepeatPath

    //INT 0x01, A
    INC L
    //WAIT 10
    CP L, 100
    JR NZ, .RepeatPath

    LD A, 0x28          ; Fill area or polygon
    LD B, 0             ; video page
    LD CD, 2            ; x
    LD EF, 2            ; y
    LD G, 0x55          ; fill color
    LD H, 0x00          ; border color
    LD I, 0b00000000    ; flags

    //INT 0x01, A




    LD A, 0x40          ; Scroll
    LD B, 0             ; video page
    LD CD, 240          ; x
    LD EF, 30           ; y
    LD GH, 100          ; width
    LD IJ, 50           ; height
    LD KL, -1           ; scroll X
    LD MN, 1            ; scroll Y
    LD O, 0b00000011    ; flags

    





.Repeat
    //INT 0x01, A         ; Video/Scroll
    WAIT 10
    JR .Repeat

    RET

.FontPath
    #DB "programs\font-test\RealityOneWideFont.png", 0

.HelloWorld2
    #DB "Hello world. This is a new way of displaying fonts with Continuum 93.", 0

.HelloWorld
	#DB "Hello world! @@ 1234567890 @@ . This is a new way of drawing text in Continuum 93 by using PNG defined fonts. It is using a complex method of rendering which also uses kerning, optional monospacing, optional centering glyphs when using monospacing, centering and wrapping. Font glyphs can be any size, not only 8bit. This text demo here also uses kerning! Here's a word that makes it easy to notice kerning -> AVIATOR", 0

.PathData
    #DB 0x0084, 0x0084, 0x0096, 0x0032, 0x00C8, 0x0064, 0x00FA, 0x0096, 0x012C, 0x0064, 0xFFFF

.FontData
    #DB 0


