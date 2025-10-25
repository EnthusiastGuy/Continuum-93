; Retrieves the list of directories and files stored in the .CurrentPath. The retrieved entries
; are stored at .Records with the following structure:
; dir1\0dir2\0dir3\0 ... dirX\0\0file1\0file2\0file3\0 ... fileY\0\0
; Observe that at the end of the directory list you will find two zero bytes as well as at
; the end of the file list.
; The interrupt also returns the total entries, directory entries, file entries and the memory
; address where the files start, for convenience. We also store those.
.GetDirectoryRecords
		
		PUSH A, M
		
		LD A, 0x15				; ListDirectoriesAndFilesInDirectory
		LD BCD, .CurrentPath	; the address of the null terminated string which represents the directory path.
		LD EFG, .Records		; the address where the directory directories listing will be dumped as null terminated strings.
		MEMF EFG, 0xFFFF, 0		; Clears the directory contents buffer
		INT 0x04, A				; Trigger interrupt Filesystem

		LD (.TotalEntriesCount), BCD			; number of total entries found

		

		LD (.DirectoryEntriesCount), EFG		; number of directories found
		LD (.FileEntriesCount), HIJ				; number of files found
		LD (.FileEntriesStartAddress), KLM		; memory address where the file listing starts
		
		POP A, M
		RET

; This displays the current directory listing starting with folders then files
.ListRecords
		CALLR .GetStartRecord	; Will return the record address in ABC
		LD HI, 4				; x
		LD JK, 0				; y
		LD G, 0					; Flag for the directory -> files
		CP (ABC), 0				; Just in case there are no folders
		JR NZ, .NextRecord
		INC ABC
.NextRecord
		LD EFG, ABC
		LD OPQ, (.FileEntriesStartAddress)
		LD RST, .Records
		ADD OPQ, RST
		LD U, (.Color_directoryname_text)
		LD (.FileListColor), U	; Directory color
		CP EFG, OPQ
		JR LT, .keep_dir_color
		LD U, (.Color_filename_text)
		LD (.FileListColor), U	; File color
.keep_dir_color
		CALL .DrawListTextItem
		CALL .TestOverflow
		CP K, 28
		JR Z, .ListLimitReached
		INC K
		FIND (ABC), 0
		INC ABC
		CP (ABC), 0
		JR NZ, .NextRecord
		
		INC ABC
		INC G
		CP G, 2		; flag to indicate listing files as well
		JR NZ, .NextRecord

.ListLimitReached
		RET

; Tests whether the text entry for the currently drawn item is larger than the available space
; If so, it draws a marker next to it '[...]' that lets the user know the name is truncated.
.TestOverflow
		PUSH A, Z
		LD O, (.Overflow)
		CP O, 0x01
		JP NZ, .OverflowNegative
		LD EFG, .OverflowMarker
		LD HI, 160					; Draw this at the rightmost part of the list
		CALL .DrawListTextItem
.OverflowNegative
		POP A, Z
		RET

; Gets what is the first record to be displayed. Since the user is able to 
; scroll down, this needs to be used
; Output: ABC - start record address
.GetStartRecord
		PUSH D, G
		LD FG, 0
		LD DE, (.ScrollIndex)
		LD ABC, .Records
.LoopFindRecord
		CP FG, DE
		JR Z, .FoundRecordStart
		INC FG
		
		FIND (ABC), 0
		INC ABC
		CP (ABC), 0
		JR Z, .FoundDoubleMarker
		JR .LoopFindRecord

.FoundRecordStart
		POP D, G
		RET

.FoundDoubleMarker
		INC ABC
		CP (ABC), 0
		JR Z, .FoundRecordStart
		JR .LoopFindRecord

; Identifies the current record by the record index and scroll index
.GetRecordNameAndType
		PUSH A, Z
		LD AB, (.RecordIndex)
		LD CD, (.ScrollIndex)
		ADD AB, CD
		LD FG, 0

		LD CDE, .Records
		CP (CDE), 0					; In case we have no directories, skip empty byte
		JR NZ, .get_record_name_and_type_next
		INC CDE

.get_record_name_and_type_next
		CP AB, FG
		JR Z, .get_record_name_and_type_found

		FIND (CDE), 0
		INC CDE
		CP (CDE), 0
		INC FG
		JR NZ, .get_record_name_and_type_next
		INC CDE
		JR .get_record_name_and_type_next

.get_record_name_and_type_found
		LD (.CurrentRecord), CDE	; CDE contains the pointer to the correct record

		LD FGH, (.FileEntriesStartAddress)
		LD IJK, .Records
		ADD FGH, IJK
		CP CDE, OPQ
		LD A, 0
		JR LT, .get_record_name_and_type_found_dir
		LD A, 1
.get_record_name_and_type_found_dir
		LD (.CurrentRecordType), A

		POP A, Z
		RET

; Unifies the current path with the current record so it produces the file path
.CopyDirectoryPathToFullPath
		PUSH A, J
		LD ABC, .CurrentPath
		LD DEF, .CurrentFileFullPath
		MEMF DEF, 256, 0
		LD GHI, (.CurrentRecord)

.copy_directory_path_to_full_path_loop
		CP (ABC), 0
		JR Z, .copy_file_name
		LD J, (ABC)
		LD (DEF), J
		INC ABC
		INC DEF
		JR .copy_directory_path_to_full_path_loop

.copy_file_name
		LD J, (GHI)
		LD (DEF), J
		INC GHI
		INC DEF
		CP J, 0
		JR NZ, .copy_file_name
		POP A, J
		RET

; Adds the current selected directory to the global path
.AppendDirectoryToPath
		PUSH A, Z

		LD ABC, .CurrentPath
		FIND (ABC), 0

		LD DEF, (.CurrentRecord)
.append_directory_to_path_loop
		LD G, (DEF)
		LD (ABC), G

		INC ABC
		INC DEF

		CP (DEF), 0
		JR NZ, .append_directory_to_path_loop
		LD (ABC), 0x5C00, 2			; Backslash and zero to terminate the string
		POP A, Z
		RET

; Removes the last directory added from the global path
.RemoveLastDirectoryFromPath
		PUSH A, D

		LD ABC, .CurrentPath

		LD D, 0xFF
		ADD ABC, D				; Start searching from the end

.remove_last_directory_from_path_loop
		CP (ABC), 0x5C			; look for backslash
		JR Z, .remove_last_directory_from_path_found_slash
		DEC ABC
		DEC D
		CP D, 0					; we don't need to bother checkig the first character of the path. Can't be a backslash
		JR NZ, .remove_last_directory_from_path_loop

.remove_last_directory_from_path_found_slash
		LD (ABC), 0, 1
		DEC ABC
		DEC D
		CP (ABC), 0x5C
		JR Z, .remove_last_directory_from_path_exit
		CP D, 0
		JR NZ, .remove_last_directory_from_path_found_slash

.remove_last_directory_from_path_exit
		CP (ABC), 0x5C
		JR Z, .remove_last_directory_from_path_no_root
		LD (ABC), 0, 1
.remove_last_directory_from_path_no_root

		POP A, D
		RET



; Checks whether the current path is the root path
; Output: A - 1 if true, 0 if false
.IsCurrentPathRoot
		LD A, (.CurrentPath)
		CP A, 0
		JR Z, .is_current_path_root_true
		RET
.is_current_path_root_true
		LD A, 1
		RET

; Input: ABC - pointer to the file path string
; Output: ABCD - the file size, in bytes
.GetFileSize
		PUSH Z
		LD Z, 0x20 ; GetFileSize
		INT 0x04, Z ; Trigger interrupt Filesystem
		POP Z
		RET
