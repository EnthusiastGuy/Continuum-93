; Clock, for future implementation

.RefreshClock
    PUSH A, Z
    LD A, 0x03                  ; Read Clock
    LD B, 0x00                  ; Milliseconds mode
    LD CDE, .SessionEllapsedMS  ; destination address
    INT 0x00, A                 ; Trigger interrupt `Machine/Read Clock Milliseconds`
    POP A, Z
    RET

.SessionEllapsedMS
    #DB 0x00000000