#include os-color-theme.asm

; Holds some state data for the OS such as where the filenames are stored, what is the
; currently selected file, scroll index, path and so on.

.FileListColor
		#DB 0x05			; A placeholder for storing the temporary color of the next
							; entry to be drawn in the list. Color changes depending on
							; whether it's a directory or file

.Overflow
		#DB 0x00			; A memory flag indicating if last file/directory name draw
							; overflowed or not
		
.LastDrawnX
		#DB 0x0000			; Stores the last X value that was plotted during a text draw
							; when text overflowed. Used to represent the overflow ellipsis

.RecordIndex
		#DB 0x0000			; Current selected record index

.ScrollIndex
		#DB 0x0000			; Current scroll index

.IndexBufferPointer
		#DB 0

.LastRecordIndexBuffer
		#DB [48] 0x0000		; This holds the historical state of what the record index was
							; before the user switched directories. Used when returning from
							; a directory to retrieve the old state

.LastScrollIndexBuffer
		#DB [48] 0x0000		; This holds the historical state of what the scroll index was
							; before the user switched directories. Used when returning from
							; a directory to retrieve the old state

.CurrentPath
		#DB [256] 0			; We store the current relative directory path

.CurrentFileFullPath
		#DB [256] 0			; We store the full current directory path

.CurrentFileSize
		#DB [16] 0			; string representation of the file size

.Records
		#DB [65536] 0		; This should be enough

.CurrentRecord
		#DB 0x000000		; the pointer to the current record

.CurrentRecordType
		#DB 0				; 0 - directory, 1 - file

.TotalEntriesCount
		#DB 0x000000		; All directories/files count

.DirectoryEntriesCount
		#DB 0x000000		; How many directories are there in the current view

.FileEntriesCount
		#DB 0x000000		; How many files are there in the current view

.FileEntriesStartAddress
		#DB 0x000000		; Where the file entries names start

.OverflowMarker
		#DB "[...]", 0		; What will be displayed when a file/directory name is too long

.StringFileSize
		#DB "Size:", 0		; Text to be shown in the information panel for the file size

.MessageTextErrorsFound
		#DB "Cannot compile, errors found. ESC continues.", 0

.ErrorsFoundCompiling
		#DB 0