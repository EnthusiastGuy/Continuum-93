; This is where buffers and data reside

.MainWindowTopLeftX
	#DB 0x0000

.MainWindowTopLeftY
	#DB 0x0000

.MainWindowWidth
	#DB 480

.MainWindowHeight
	#DB 270

.FontFile
		#DB "fonts\SlickAntsContour.font", 0
		
.FontData	; 952 bytes will be occupied from here for the font data. We can explicitly reserve them as below
			; so we can write more code after this block, if we need to. Explicitly reserving also help measure
			; exactly how much bytes your code takes.
		#DB [952] 0
		
.NumberStringBuffer
		#DB [4] 0

.KeyboardBuffer
		#DB [64] 0
		
.Title
		#DB "Keyboard state V1.1", 0

.InfoLine1
		#DB "Press key combinations to see their codes and", 0

.InfoLine2
		#DB "corresponding information displayed to the left.", 0
		
.InfoLine3
		#DB "See the Continuum Interrupts manual, section:", 0
		
.InfoLine4
		#DB "'Input / Read Keyboard Pressed Keys As Codes'", 0

.InfoLine5
		#DB "and also the user manual, the Keyboard section.", 0
		
.InfoLine6
		#DB "To exit and return to OS, press LShift + Esc", 0

.Keys
		#DB .Key000, .Key001, .Key002, .Key003, .Key004, .Key005, .Key006, .Key007, .Key008, .Key009, .Key010, .Key011, .Key012, .Key013, .Key014, .Key015
		#DB .Key016, .Key017, .Key018, .Key019, .Key020, .Key021, .Key022, .Key023, .Key024, .Key025, .Key026, .Key027, .Key028, .Key029, .Key030, .Key031
		#DB .Key032, .Key033, .Key034, .Key035, .Key036, .Key037, .Key038, .Key039, .Key040, .Key041, .Key042, .Key043, .Key044, .Key045, .Key046, .Key047
		#DB .Key048, .Key049, .Key050, .Key051, .Key052, .Key053, .Key054, .Key055, .Key056, .Key057, .Key058, .Key059, .Key060, .Key061, .Key062, .Key063
		#DB .Key064, .Key065, .Key066, .Key067, .Key068, .Key069, .Key070, .Key071, .Key072, .Key073, .Key074, .Key075, .Key076, .Key077, .Key078, .Key079
		#DB .Key080, .Key081, .Key082, .Key083, .Key084, .Key085, .Key086, .Key087, .Key088, .Key089, .Key090, .Key091, .Key092, .Key093, .Key094, .Key095
		#DB .Key096, .Key097, .Key098, .Key099, .Key100, .Key101, .Key102, .Key103, .Key104, .Key105, .Key106, .Key107, .Key108, .Key109, .Key110, .Key111
		#DB .Key112, .Key113, .Key114, .Key115, .Key116, .Key117, .Key118, .Key119, .Key120, .Key121, .Key122, .Key123, .Key124, .Key125, .Key126, .Key127
		#DB .Key128, .Key129, .Key130, .Key131, .Key132, .Key133, .Key134, .Key135, .Key136, .Key137, .Key138, .Key139, .Key140, .Key141, .Key142, .Key143
		#DB .Key144, .Key145, .Key146, .Key147, .Key148, .Key149, .Key150, .Key151, .Key152, .Key153, .Key154, .Key155, .Key156, .Key157, .Key158, .Key159
		#DB .Key160, .Key161, .Key162, .Key163, .Key164, .Key165, .Key166, .Key167, .Key168, .Key169, .Key170, .Key171, .Key172, .Key173, .Key174, .Key175
		#DB .Key176, .Key177, .Key178, .Key179, .Key180, .Key181, .Key182, .Key183, .Key184, .Key185, .Key186, .Key187, .Key188, .Key189, .Key190, .Key191
		#DB .Key192, .Key193, .Key194, .Key195, .Key196, .Key197, .Key198, .Key199, .Key200, .Key201, .Key202, .Key203, .Key204, .Key205, .Key206, .Key207
		#DB .Key208, .Key209, .Key210, .Key211, .Key212, .Key213, .Key214, .Key215, .Key216, .Key217, .Key218, .Key219, .Key220, .Key221, .Key222, .Key223
		#DB .Key224, .Key225, .Key226, .Key227, .Key228, .Key229, .Key230, .Key231, .Key232, .Key233, .Key234, .Key235, .Key236, .Key237, .Key238, .Key239
		#DB .Key240, .Key241, .Key242, .Key243, .Key244, .Key245, .Key246, .Key247, .Key248, .Key249, .Key250, .Key251, .Key252, .Key253, .Key254, .Key255

.Key000
		#DB "None", 0

.Key001
		#DB "Left mouse button", 0

.Key002
		#DB "Right mouse button", 0

.Key003
		#DB "Cancel", 0

.Key004
		#DB "Middle mouse button", 0

.Key005
		#DB "X button 1", 0

.Key006
		#DB "X button 2", 0

.Key007
		#DB "unassigned", 0

.Key008
		#DB "Back", 0

.Key009
		#DB "Tab", 0

.Key010
		#DB "reserved", 0

.Key011
		#DB "reserved", 0

.Key012
		#DB "Clear", 0

.Key013
		#DB "Enter", 0

.Key014
		#DB "", 0

.Key015
		#DB "", 0

.Key016
		#DB "Any Shift", 0

.Key017
		#DB "Any Ctrl", 0

.Key018
		#DB "Any Alt", 0

.Key019
		#DB "Pause", 0

.Key020
		#DB "Caps Lock", 0

.Key021
		#DB "Kana", 0

.Key022
		#DB "", 0

.Key023
		#DB "Junja", 0

.Key024
		#DB "Final", 0

.Key025
		#DB "Kanji", 0

.Key026
		#DB "", 0

.Key027
		#DB "Esc", 0

.Key028
		#DB "Ime Convert", 0

.Key029
		#DB "Ime No Convert", 0

.Key030
		#DB "Accept", 0

.Key031
		#DB "Mode change", 0

.Key032
		#DB "Space", 0

.Key033
		#DB "Page up", 0

.Key034
		#DB "Page down", 0

.Key035
		#DB "End", 0

.Key036
		#DB "Home", 0

.Key037
		#DB "Left", 0

.Key038
		#DB "Up", 0

.Key039
		#DB "Right", 0

.Key040
		#DB "Down", 0

.Key041
		#DB "Select", 0

.Key042
		#DB "Print", 0

.Key043
		#DB "Execute", 0

.Key044
		#DB "Print screen", 0

.Key045
		#DB "Insert", 0

.Key046
		#DB "Delete", 0

.Key047
		#DB "Help", 0

.Key048
		#DB "0", 0

.Key049
		#DB "1", 0

.Key050
		#DB "2", 0

.Key051
		#DB "3", 0

.Key052
		#DB "4", 0

.Key053
		#DB "5", 0

.Key054
		#DB "6", 0

.Key055
		#DB "7", 0

.Key056
		#DB "8", 0

.Key057
		#DB "9", 0

.Key058
		#DB "", 0

.Key059
		#DB "", 0
		
.Key060
		#DB "", 0

.Key061
		#DB "", 0

.Key062
		#DB "", 0

.Key063
		#DB "", 0

.Key064
		#DB "unassigned", 0

.Key065
		#DB "A", 0

.Key066
		#DB "B", 0

.Key067
		#DB "C", 0

.Key068
		#DB "D", 0

.Key069
		#DB "E", 0

.Key070
		#DB "F", 0

.Key071
		#DB "G", 0

.Key072
		#DB "H", 0

.Key073
		#DB "I", 0

.Key074
		#DB "J", 0

.Key075
		#DB "K", 0

.Key076
		#DB "L", 0

.Key077
		#DB "M", 0

.Key078
		#DB "N", 0

.Key079
		#DB "O", 0

.Key080
		#DB "P", 0

.Key081
		#DB "Q", 0

.Key082
		#DB "R", 0

.Key083
		#DB "S", 0

.Key084
		#DB "T", 0

.Key085
		#DB "U", 0

.Key086
		#DB "V", 0

.Key087
		#DB "W", 0

.Key088
		#DB "X", 0

.Key089
		#DB "Y", 0

.Key090
		#DB "Z", 0

.Key091
		#DB "Left windows", 0

.Key092
		#DB "Right windows", 0

.Key093
		#DB "Apps", 0

.Key094
		#DB "reserved", 0

.Key095
		#DB "Sleep", 0

.Key096
		#DB "Numpad 0", 0

.Key097
		#DB "Numpad 1", 0

.Key098
		#DB "Numpad 2", 0

.Key099
		#DB "Numpad 3", 0

.Key100
		#DB "Numpad 4", 0

.Key101
		#DB "Numpad 5", 0

.Key102
		#DB "Numpad 6", 0

.Key103
		#DB "Numpad 7", 0

.Key104
		#DB "Numpad 8", 0

.Key105
		#DB "Numpad 9", 0

.Key106
		#DB "Multiply", 0

.Key107
		#DB "Add", 0

.Key108
		#DB "Separator", 0

.Key109
		#DB "Subtract", 0

.Key110
		#DB "Decimal", 0

.Key111
		#DB "Divide", 0

.Key112
		#DB "F1", 0

.Key113
		#DB "F2", 0

.Key114
		#DB "F3", 0

.Key115
		#DB "F4", 0

.Key116
		#DB "F5", 0

.Key117
		#DB "F6", 0

.Key118
		#DB "F7", 0

.Key119
		#DB "F8", 0

.Key120
		#DB "F9", 0

.Key121
		#DB "F10", 0

.Key122
		#DB "F11", 0

.Key123
		#DB "F12", 0

.Key124
		#DB "F13", 0

.Key125
		#DB "F14", 0

.Key126
		#DB "F15", 0

.Key127
		#DB "F16", 0

.Key128
		#DB "F17", 0

.Key129
		#DB "F18", 0

.Key130
		#DB "F19", 0

.Key131
		#DB "F20", 0

.Key132
		#DB "F21", 0

.Key133
		#DB "F22", 0

.Key134
		#DB "F23", 0

.Key135
		#DB "F24", 0

.Key136
		#DB "unassigned", 0

.Key137
		#DB "unassigned", 0

.Key138
		#DB "unassigned", 0

.Key139
		#DB "unassigned", 0

.Key140
		#DB "unassigned", 0

.Key141
		#DB "unassigned", 0

.Key142
		#DB "unassigned", 0

.Key143
		#DB "unassigned", 0

.Key144
		#DB "Num lock", 0

.Key145
		#DB "Scroll", 0

.Key146
		#DB "OEM Nec Equal", 0

.Key147
		#DB "OEM Fj Masshou", 0

.Key148
		#DB "OEM Fj Touroku", 0

.Key149
		#DB "OEM Fj Loya", 0

.Key150
		#DB "OEM Fj Roya", 0

.Key151
		#DB "unassigned", 0

.Key152
		#DB "unassigned", 0

.Key153
		#DB "unassigned", 0

.Key154
		#DB "unassigned", 0

.Key155
		#DB "unassigned", 0

.Key156
		#DB "unassigned", 0

.Key157
		#DB "unassigned", 0

.Key158
		#DB "unassigned", 0

.Key159
		#DB "unassigned", 0
		
.Key160
		#DB "Left Shift", 0

.Key161
		#DB "Right Shift", 0

.Key162
		#DB "Left Ctrl", 0

.Key163
		#DB "Right Ctrl", 0

.Key164
		#DB "Left Alt", 0

.Key165
		#DB "Right Alt", 0

.Key166
		#DB "Browser back", 0

.Key167
		#DB "Browser forward", 0

.Key168
		#DB "Browser refresh", 0

.Key169
		#DB "Browser stop", 0

.Key170
		#DB "Browser search", 0

.Key171
		#DB "Browser favorites", 0

.Key172
		#DB "Browser home", 0

.Key173
		#DB "Volume mute", 0

.Key174
		#DB "Volume down", 0

.Key175
		#DB "Volume up", 0

.Key176
		#DB "Media next track", 0

.Key177
		#DB "Media prev track", 0

.Key178
		#DB "Media stop", 0

.Key179
		#DB "Media play/pause", 0

.Key180
		#DB "Launch mail", 0

.Key181
		#DB "Select media", 0

.Key182
		#DB "Launch app1", 0

.Key183
		#DB "Launch app2", 0

.Key184
		#DB "reserved", 0

.Key185
		#DB "reserved", 0

.Key186
		#DB "OEM semicolon", 0

.Key187
		#DB "OEM plus", 0

.Key188
		#DB "OEM comma", 0

.Key189
		#DB "OEM minus", 0

.Key190
		#DB "OEM period", 0

.Key191
		#DB "OEM question", 0

.Key192
		#DB "OEM tilde", 0

.Key193
		#DB "reserved", 0

.Key194
		#DB "reserved", 0

.Key195
		#DB "reserved", 0

.Key196
		#DB "reserved", 0

.Key197
		#DB "reserved", 0

.Key198
		#DB "reserved", 0

.Key199
		#DB "reserved", 0

.Key200
		#DB "reserved", 0

.Key201
		#DB "reserved", 0

.Key202
		#DB "Chat pad green", 0

.Key203
		#DB "Chat pad orange", 0

.Key204
		#DB "reserved", 0

.Key205
		#DB "reserved", 0

.Key206
		#DB "reserved", 0

.Key207
		#DB "reserved", 0

.Key208
		#DB "reserved", 0

.Key209
		#DB "reserved", 0

.Key210
		#DB "reserved", 0

.Key211
		#DB "reserved", 0

.Key212
		#DB "reserved", 0

.Key213
		#DB "reserved", 0

.Key214
		#DB "reserved", 0

.Key215
		#DB "reserved", 0

.Key216
		#DB "unassigned", 0

.Key217
		#DB "unassigned", 0

.Key218
		#DB "unassigned", 0

.Key219
		#DB "OEM open brackets", 0

.Key220
		#DB "OEM pipe", 0

.Key221
		#DB "OEM close brackets", 0

.Key222
		#DB "OEM quotes", 0

.Key223
		#DB "OEM 8", 0

.Key224
		#DB "reserved", 0

.Key225
		#DB "AX Key Japan", 0

.Key226
		#DB "OEM Back slash", 0

.Key227
		#DB "ICO Help", 0

.Key228
		#DB "ICO 00", 0

.Key229
		#DB "Process key", 0

.Key230
		#DB "ICO clear", 0

.Key231
		#DB "Packet", 0

.Key232
		#DB "unassigned", 0

.Key233
		#DB "OEM reset", 0

.Key234
		#DB "OEM jump", 0

.Key235
		#DB "OEM PA1", 0

.Key236
		#DB "OEM PA2", 0

.Key237
		#DB "OEM PA3", 0

.Key238
		#DB "OEM WS Ctrl", 0

.Key239
		#DB "OEM CU Sel", 0

.Key240
		#DB "OEM Attn", 0

.Key241
		#DB "OEM Finish", 0

.Key242
		#DB "OEM Copy", 0

.Key243
		#DB "OEM Auto", 0

.Key244
		#DB "OEM EN IW", 0

.Key245
		#DB "OEM Back Tab", 0

.Key246
		#DB "Attn", 0

.Key247
		#DB "Crsel", 0

.Key248
		#DB "Exsel", 0

.Key249
		#DB "Erase Eof", 0

.Key250
		#DB "Play", 0

.Key251
		#DB "Zoom", 0

.Key252
		#DB "No name", 0

.Key253
		#DB "PA1", 0

.Key254
		#DB "OEM Clear", 0

.Key255
		#DB "reserved", 0