
#ORG 0x80000

    JR .Start

.ScreenW
    #DB 256

.ScreenH
    #DB 192

; Simple 3D point structure

.Point1X
    #DB 50

.Point1Y
    #DB 50

.Point1Z
    #DB 100

.Point2X
    #DB 100

.Point2Y
    #DB 150

.Point2Z
    #DB 150

.Point3X
    #DB 150

.Point3Y
    #DB 50

.Point3Z
    #DB 200

; Camera parameters
.CameraZ
    #DB 200

; Define a simple texture (8x8 checkerboard)
    #DB 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00
    #DB 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF
    #DB 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00
    #DB 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF
    #DB 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00
    #DB 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF
    #DB 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00
    #DB 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF



.Start
    LD A, 0x02 	; SetVideoPagesCount
    LD B, 1 	; We'll be using one single video layer
    INT 0x01, A ; Trigger interrupt Video

.Loop
    CALLR .ClearScreen

    JR .Loop


.Project3DPoints
    ; Point 1
    LD A, (.Point1Z)

    RET

.ApplyPerspective
    LD A, (.Point1Z)

    
    RET


.ClearScreen
    PUSH A, C
    LD A, 0x05 ; ClearVideoPage
    LD B, 0         ; Layer to clear
    LD C, 0 ; the color which will be used to fill that memory page (0 - transparent or 1 - 255).
    INT 0x01, A ; Trigger interrupt Video
    POP A, C
    RET