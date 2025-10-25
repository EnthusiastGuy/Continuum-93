.InitializeClock
    LD N, 0x03                          ; ReadClock
    LD O, 0x00                          ; Milliseconds
    LD PQR, .ClockTimeMsPrevious        ; Destination address
    INT 0x00, N                         ; Machine interrupt
    RET

; This routine calculates the delta time between cycles
; Sets the A register to 1 if the desired frame rate has been reached
.HasUpdateTimePassed
    LD N, 0x03                          ; ReadClock
    LD O, 0x00                          ; Milliseconds
    LD PQR, .ClockTimeMs                ; Set the destination address to .ClockTimeMs
    INT 0x00, N                         ; Machine interrupt
    LD FGHI, (.ClockTimeMsPrevious)     ; Load the previous clock time
    LD JKLM, (.ClockTimeMs)             ; Load the new clock time

    SUB JKLM, FGHI                      ; Calculate the delta between the current and previous clock ms
    LD OPQR, (.TargetFrameTime)         ; Get the value of the target frame rate
    CP JKLM, OPQR                       ; Compare the delta with the target frame time
    JR GT, .updateClockTick             ; If the  delta is GTE than the target, we jump
    RESF Z                              ; The frame rate has not been reached, so reset flag Z
    RET

.updateClockTick
    ; The frame rate has been reached
    LD JKLM, (.ClockTimeMs)             ; Load the new clock time again
    LD (.ClockTimeMsPrevious), JKLM     ; Store the new time as the previous for next iteration
    SETF Z                              ; Sets flag zero
    RET


; Frame rate related memory
.TargetFrameTime
    ; We prepend 3 extra bytes of zeroes, since we need to compare to an 4 byte register
    #DB 0, 0, 0, 16                     ; 16 ms between updates per frame should give approx. 60 FPS
    ;#DB 0, 0, 0x3E8                    ; 1000 ms
.ClockTimeMs
    #DB 0x00000000                      ; 32 bits for clock ms
.ClockTimeMsPrevious
    #DB 0x00000000                      ; 32 bits for clock ms