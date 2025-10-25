#include os-execution.asm

.KeyboardInputEvaluate

        CALLR .InputUpdate				; Saves the current state of the keys pressed
		CALLR .GamepadsInputUpdate              ; Update gamepad states

		CALLR .InputNoStateChange
		CP A, 1
		JR Z, .keyboard_input_evaluate_no_input

		LD A, 27	; Escape key
		CALLR .InputKeyPressed
		CP A, 1
		CALLR Z, .ClearMessage

		LD X, (.ErrorsFoundCompiling)
		CP X, 0
		JR NZ, .gamepad_input_evaluate_no_input
		
		; Keyboard handling
		LD A, 38	; Up arrow key
		CALLR .InputKeyPressed
		CP A, 1
		CALLR Z, .ArrowUp
		
		LD A, 40	; Down arrow key
		CALLR .InputKeyPressed
		CP A, 1
		CALLR Z, .ArrowDown

		LD A, 116	; F5 key
		CALLR .InputKeyPressed
		CP A, 1
		CALLR Z, .KeyF5Pressed

        LD A, 13    ; Enter key
        CALLR .InputKeyPressed
        CP A, 1
        CALLR Z, .KeyEnterPressed

		LD A, 8		; Backspace key
		CALLR .InputKeyPressed
        CP A, 1
        CALLR Z, .KeyBackspacePressed

		; Check for a sequence of characters
		LD K, 0
		LD A, 163	; Right control
		CALLR .InputKeyPressed
		LD K, A
		LD A, 46	; Delete
		CALLR .InputKeyPressed
		ADD K, A
		CP K, 2
        CALLR Z, .LRCtrlDelPressed

.keyboard_input_evaluate_no_input

		LD A, 0		; Identify controller zero
		CALLR .IsAnyButtonOnController0Down
		JR NZ, .gamepad_input_evaluate_no_input

		CALLR .IsButtonDPadUpPressed
		CALLR Z, .ArrowUp

		CALLR .IsButtonDPadDownPressed
		CALLR Z, .ArrowDown

		CALLR .IsButtonAPressed
		CALLR Z, .KeyEnterPressed

		CALLR .IsButtonBPressed
		CALLR Z, .KeyBackspacePressed

		CALLR .IsButtonBackPressed
		CALLR Z, .KeyF5Pressed


.gamepad_input_evaluate_no_input
        RET


.ArrowUp
		LD AB, (.RecordIndex)
		CP AB, 0
		JR Z, .arrow_up_top
		DEC AB
		LD (.RecordIndex), AB
        CALLR .RefreshSelection
.ArrowUpExit
		RET

.arrow_up_top
		LD AB, (.ScrollIndex)
		CP AB, 0
		JR Z, .ArrowUpExit
		DEC AB
		LD (.ScrollIndex), AB
		CALLR .clear_and_redraw_list
		RET

.ArrowDown
		LD AB, (.RecordIndex)
		LD CD, (.ScrollIndex)
		LD EFG, (.TotalEntriesCount)

		LD XYZ, .TotalEntriesCount
		ADD CD, AB
		INC CD
		CP CD, FG				; Checks if reached the end of the list if shorter than available window
		JR GTE, .ArrowDownExit 
		
		CP AB, 28
		JR Z, .arrow_down_bottom
		INC AB
		LD (.RecordIndex), AB
        CALLR .RefreshSelection
.ArrowDownExit		
		RET

.arrow_down_bottom
		LD AB, (.ScrollIndex)
		ADD AB, 29
		LD CDE, (.TotalEntriesCount)
		CP AB, DE
		JR GTE, .ArrowDownExit
		LD AB, (.ScrollIndex)
		INC AB
		LD (.ScrollIndex), AB
		CALLR .clear_and_redraw_list
		RET

.KeyF5Pressed
		LD A, 0x10
		INT 0x00, A
		RET

.clear_and_redraw_list
		LD AB, 0x0200	; Video page and color to clear
		CALLR .ClearScreen
		CALLR .ListRecords
		CALLR .RefreshSelection
		CALLR .DisplayRecordInfo
		RET

.KeyEnterPressed
        ; Enter selected folder or attempt to run current file

		LD A, (.CurrentRecordType)
		CP A, 0
		JR NZ, .key_enter_pressed_on_file
		CALLR .AppendDirectoryToPath
		CALLR .push_record_and_scroll_indexes
		CALLR .GetDirectoryRecords
		CALLR .clear_and_redraw_list
		RET
		
.key_enter_pressed_on_file
		CALLR .CompileAndExecuteASMFile
        RET

.KeyBackspacePressed
		CALLR .IsCurrentPathRoot
		CP A, 1
		JR Z, .key_back_space_pressed_exit

		CALLR .RemoveLastDirectoryFromPath
		CALLR .pop_record_and_scroll_indexes
		CALLR .GetDirectoryRecords
		CALLR .clear_and_redraw_list
		
.key_back_space_pressed_exit
		RET

.LRCtrlDelPressed
		LD A, 0x20		; Shutdown
		INT 0x00, A
		RET				; Well, theoretically this will never be reached


; TODO move these two from here
.push_record_and_scroll_indexes
		LD A, (.IndexBufferPointer)
		LD CDE, .LastRecordIndexBuffer
		LD FGH, .LastScrollIndexBuffer
		ADD CDE, A
		ADD FGH, A
		INC A
		INC A
		LD (.IndexBufferPointer), A
		LD AB, (.RecordIndex)
		LD (CDE), AB
		LD AB, (.ScrollIndex)
		LD (FGH), AB
		LD (.RecordIndex), 0x0000, 2
		LD (.ScrollIndex), 0x0000, 2

		RET

.pop_record_and_scroll_indexes
		LD A, (.IndexBufferPointer)
		LD CDE, .LastRecordIndexBuffer
		LD FGH, .LastScrollIndexBuffer
		DEC A
		DEC A
		LD (.IndexBufferPointer), A
		ADD CDE, A
		ADD FGH, A
		LD AB, (CDE)
		LD (.RecordIndex), AB
		LD AB, (FGH)
		LD (.ScrollIndex), AB

		RET
