#ORG 0x080000

	ld a, 0x02
	ld b, 1
	int 0x01, a ; set 1 video page

	ld a, 0x04
	dec b
	int 0x01, a 
	memc .pallete, bcd, 48 ; set our pallete

	ld a, 0x40
	ld bcd, .fontPath
	ld efg, .fontData
	int 0x04, a ; load font

	ld abc, 0x050000
	int 0x01, a ; clear 1st video page

	ld xy, 0
	ld abcd, 0
	ld efg, .test
	ld m, 0
	ld no, 96
	ld p, 0b010111
	ld qr, 0
.loop_010
	ld hijk, abcd       
	ld a, 0x14 
	ld bcd, .fontData
	mul hi, x           
	mul jk, y           
	ld l, x             
	add l, y
	inc l            
	and l, 0b1111       
	int 0x01, a 
	inc x
	cp x, 10 
	jp ne, .loop_010      
	;inc y
	;cp y, 5             
	;jr ne, -13
.superloop
	jp .superloop

	DEBUG
	RET

.pallete ; total bytes -> 16 * 3 = 48 bytes
	#DB 0,   0,   0   ; (0) Black
	#DB 170, 0,   0   ; (1) Red
	#DB 0,   170, 0   ; (2) Green
	#DB 170, 85,  0   ; (3) Yellow
	#DB 0,   0,   170 ; (4) Blue
	#DB 170, 0,   170 ; (5) Magenta
	#DB 0,   170, 170 ; (6) Cyan
	#DB 170, 170, 170 ; (7) White
	#DB 85,  85,  85  ; (8) Bright Black
	#DB 255, 85,  85  ; (9) Bright Red
	#DB 85,  255, 85  ; (10) Bright Green
	#DB 255, 255, 85  ; (11) Bright Yellow
	#DB 85,  85,  255 ; (12) Bright Blue
	#DB 255, 85,  255 ; (13) Bright Magenta
	#DB 85,  255, 255 ; (14) Bright Cyan
	#DB 255, 255, 255 ; (15) Bright White

.fontPath
	#DB "programs\delete-test\font3.png", 0

.fontData
	#DB [0x800] 0

.test
	#DB "XXXX$ XXXX$X", 0