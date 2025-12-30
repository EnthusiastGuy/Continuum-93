#ORG 0x80000

	LD A, 0x02 	; SetVideoPagesCount interrupt
	LD B, 8 	; set video pages to 8.
	INT 0x01, A ; Trigger interrupt Video with SetVideoPagesCount

	ld a, 0x40
	ld bcd, .fontPath
	ld efg, .fontData
	int 0x04, a ; load font


	LD X, 0
.ClearLoop
	LD A, 0x05 ; ClearVideoPage
	LD B, X ; the video page which needs clearing (0-7)
	LD C, 0 ; the color which will be used to fill that memory page (0-transparent or 1-255).
	INT 0x01, A ; Trigger interrupt Video

	INC X
	CP X, 8
	JR NZ, .ClearLoop


    ; Layer 0
	ld a, 0x14 
	ld bcd, .fontData
	ld efg, .test0
	ld m, 0
	ld hijk, 0                    
	ld l, 4             
	ld no, 100
	ld p, 0b00111000    
	int 0x01, a 

    ; Layer 1
	ld a, 0x14 
	ld bcd, .fontData
	ld efg, .test1
	ld m, 1
	ld hijk, 0                    
	ld l, 4             
	ld no, 100
	ld p, 0b00111000   
	int 0x01, a 

    ; Layer 2
	ld a, 0x14 
	ld bcd, .fontData
	ld efg, .test2
	ld m, 2
	ld hijk, 0                    
	ld l, 4             
	ld no, 100
	ld p, 0b00111000 
	int 0x01, a 

    ; Layer 3
	ld a, 0x14 
	ld bcd, .fontData
	ld efg, .test3
	ld m, 3
	ld hijk, 0                    
	ld l, 4             
	ld no, 100
	ld p, 0b00111000  
	int 0x01, a 

    ; Layer 4
	ld a, 0x14 
	ld bcd, .fontData
	ld efg, .test4
	ld m, 4
	ld hijk, 0                    
	ld l, 4             
	ld no, 100
	ld p, 0b00111000   
	int 0x01, a 

    ; Layer 5
	ld a, 0x14 
	ld bcd, .fontData
	ld efg, .test5
	ld m, 5
	ld hijk, 0                    
	ld l, 4             
	ld no, 100
	ld p, 0b00111000  
	int 0x01, a 

    ; Layer 6
	ld a, 0x14 
	ld bcd, .fontData
	ld efg, .test6
	ld m, 6
	ld hijk, 0                    
	ld l, 4             
	ld no, 100
	ld p, 0b00111000
	int 0x01, a 

    ; Layer 7
	ld a, 0x14 
	ld bcd, .fontData
	ld efg, .test7
	ld m, 7
	ld hi, 0                    
	ld jk, 50
	ld l, 4             
	ld no, 100
	ld p, 0b00111000
	int 0x01, a 

.InfLoop
	JR .InfLoop


.fontPath
	#DB "fonts/Adventure 9x9.png", 0



.test0
	#DB "Layer 0", 0
.test1
	#DB "Layer 1", 0
.test2
	#DB "Layer 2", 0
.test3
	#DB "Layer 3", 0
.test4
	#DB "Layer 4", 0
.test5
	#DB "Layer 5", 0
.test6
	#DB "Layer 6", 0
.test7
	#DB "Layer 7", 0
	
.fontData
	#DB [0xF00] 0
	