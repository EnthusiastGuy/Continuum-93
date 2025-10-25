Continuum

TODO
- build compiler's ability to build at custom addresses [ok]
- set palette location for each page [ok];
- implement save/load interrupt [ok]


Interrupts:
- GetMemoryBankCount()
- SetVideoPages(n), n = {1, ..., 8}
- SetVideoBankAndAddressForPage(n, b, aaa)
- GetVideoAddressForPage(n)
- GetVideoBankForPage(n)

Video assignement procedure:
- Continuum starts with a default setup of 2 video pages and 2 palettes;
- Video RAM is assigned to the endmost free part of the memory;
-

File system Interrupt API
========================

Supported files cannot exceed 4Gb of size;

Directory listing
------------------

Requires:
- source directory path pointer: pointer to a null terminated string containing the
directory path;
- listing mode (simple/complex/full);
- pointer to a memory location where to dump the listing;

File size
----------

Requires:
- source file path pointer: pointer to a null terminated string containing the file path;
- destination 32bit register where the file size will be deposited (max 4gb);

File Save
----------

Requires:
- destination file path pointer: pointer to a null terminated string containing the file path;
- source memory start address (0 - 0xFFFFFF);
- source memory length (0 - 0xFFFFFF);

The full content of the specified memory area will be saved to the specified file path.

If the file does not exist, it will be created. If it exists it will be overwritten.

File Save at
-------------
Requires:
- destination file path pointer - pointer to a null terminated string containing the file path;
- destination file address pointer - pointer to where in the file should the block of memory
 be placed;
- start address (0 - 0xFFFFFF);
- length (0 - 0xFFFFFF);

The content of the specified memory area will be saved to the specified file path
at the specified file pointer, overwriting anything there. If the pointer+length exceeds
the existing file's size, the file will be resized accordingly and zero bytes will be
inserted if needed.

File Load
----------

Requires:
- source file path pointer - pointer to a null terminated string containing the file path;
- address - the address where the file will be loaded at;

Attempts to load the full contents of the file into memory at the optionally provided address.
If the file size exceeds the available RAM size, no loading will occur and an error should
be issued.

File Load from
---------------

Requires:
- source file path pointer - pointer to a null terminated string containing the file path;
- source file address pointer - pointer to where in the file should the data be loaded from;
- source file size - the number of bytes to be loaded;
- [optional] address - the address where the file will be loaded at. If this address is not
provided, zero is assumed;

Attempts to load the specified contents of the file to the specified address.
Errors should be issued on: no file, file too short.


