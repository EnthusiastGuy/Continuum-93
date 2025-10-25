; Expects the input value as a byte in Z register
; Returns oscilated value back in Z register

; INPUT     OUTPUT
; 0         0
; 1         2
; 2         4
; ...
; 126       252
; 127       254     -> peak
; 128       252
; 129       250
; ...
; 243       4
; 254       2
; 255       0
.GetOscillation
    CP Z, 128
    JR GTE, .oscillationDecremental
    MUL Z, 2
    RET
.oscillationDecremental
    PUSH A
    SUB Z, 128
    MUL Z, 2
    LD A, Z
    LD Z, 254
    SUB Z, A
    POP A
    RET
