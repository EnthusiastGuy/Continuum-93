; Points that draw a test geometrical figure

.Points
	#DB 0x0080, 0x0020 ; Start at top middle
	#DB 0x0090, 0x0030 ; Move to top right
	#DB 0x00A0, 0x0020 ; Right upper peak
	#DB 0x00B0, 0x0030 ; Right mid
	#DB 0x00A0, 0x0040 ; Back in, upper right inner corner
	#DB 0x00C0, 0x0060 ; Right outer mid
	#DB 0x00A0, 0x0080 ; Lower right peak
	#DB 0x00B0, 0x0090 ; Lower right outside
	#DB 0x00A0, 0x00A0 ; Lower right inner
	#DB 0x0080, 0x00C0 ; Bottom middle
	#DB 0x0060, 0x00A0 ; Lower left inner
	#DB 0x0050, 0x0090 ; Lower left outside
	#DB 0x0060, 0x0080 ; Lower left peak
	#DB 0x0040, 0x0060 ; Left outer mid
	#DB 0x0060, 0x0040 ; Back in, upper left inner corner
	#DB 0x0050, 0x0030 ; Left mid
	#DB 0x0060, 0x0020 ; Left upper peak
	#DB 0x0070, 0x0030 ; Top left
	#DB 0x0080, 0x0020 ; Back to top middle
	#DB 0x0090, 0x0050 ; Intersecting line to right
	#DB 0x0070, 0x0050 ; Across to left
	#DB 0x0070, 0x0070 ; Down to lower left inner corner
	#DB 0x0090, 0x0070 ; Across to lower right inner corner
	#DB 0x0090, 0x0050 ; Back up to start of intersecting line
	#DB 0x0080, 0x0020 ; End back at top middle
	#DB 0x0080, 0x0020 ; End