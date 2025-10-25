; Points that draw a test geometrical figure (variant)

.Points
	#DB 0x0080, 0x0020 ; Start at top middle
	#DB 0x00A0, 0x0060 ; Draw right to mid-right
	#DB 0x0080, 0x00A0 ; Down to bottom middle
	#DB 0x0060, 0x0060 ; Left to mid-left, creating a central triangle
	#DB 0x0080, 0x0020 ; Back to top middle
	#DB 0x00C0, 0x0040 ; Extend rightwards from mid-right
	#DB 0x00A0, 0x0080 ; Down right to complete right outer triangle
	#DB 0x00A0, 0x0060 ; Back up to mid-right
	#DB 0x0040, 0x0040 ; Extend leftwards from mid-left
	#DB 0x0060, 0x0080 ; Down left to complete left outer triangle
	#DB 0x0060, 0x0060 ; Back up to mid-left
	#DB 0x0040, 0x0040 ; Connect to left extension
	#DB 0x00C0, 0x0040 ; Across to right extension, creating a horizontal line through the middle
	#DB 0x00B0, 0x0030 ; Move inwards from right for top right inner quadrilateral
	#DB 0x0070, 0x0030 ; Across to top left of inner quadrilateral
	#DB 0x0050, 0x0050 ; Down left to complete left side of quadrilateral
	#DB 0x0090, 0x0050 ; Across to right side of quadrilateral
	#DB 0x00B0, 0x0030 ; Back up to top right of quadrilateral
	#DB 0x0070, 0x0070 ; Inner diamond, start top
	#DB 0x0060, 0x0080 ; Inner diamond, left
	#DB 0x0080, 0x0090 ; Inner diamond, bottom
	#DB 0x00A0, 0x0080 ; Inner diamond, right
	#DB 0x0070, 0x0070 ; Back to inner diamond, top
	#DB 0x0080, 0x0020  ; Close figure back at top middle
	#DB 0x0080, 0x0020, 0x0080, 0x0020 ; End