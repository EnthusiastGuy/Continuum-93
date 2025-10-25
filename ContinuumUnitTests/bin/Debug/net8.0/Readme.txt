Continuum 93
============
Release version



Introduction
------------

Continuum 93 is a fantasy computer and emulator of a classic retro computer designed for
retro games, able to run assembly code programs. This is a beta release of this project.
For any issues not covered by this readme or provided manuals, please reach out to the
Discord channel: https://discord.gg/6AYkhnzxD9

Hardware (the virtual one)
--------------------------

The CPU is virtual, running at a frequency proportional with your CPU single core
performance. Depending on your machine power it can go from 2-3 Mhz (or less) up to
500 Mhz or even more.
It can run assembly instructions and has quite a generos amount of registers to work
with, divided on register pages. The addressing mode is 24-bit and that reveals the
RAM which has 16 MB. The video RAM sits at the end of the regular RAM and takes up
128k per layer (up to 8 layers).

Please read the provided PDF documentation for further details on hardware, addressing,
registers, assembly instruction reference and some code examples. Also observe the code
examples in the Data\filesystem\programs directory. They can be loaded using the existing
operating system that Continuum loads into. This OS is a visual file browser and most
examples are in the 'programs' directory. Just go in the directory of each individual
program and execute the .asm file by positioning the selector on it and pressing Enter.
Please see the User Manual, the Operating System section for more details.

Also check the Data\init.cfg file for other settings.


Installation
------------

Windows

Not much to it, just unzip the contents of the provided package. Since you're reading this,
you probaly already did.

The default operating system is able to load and execute *.asm files.
The source of the OS starts with the 'q.asm' located in the 'Data\os' directory.
Feel free to disect it or change it. Any questions, go ahead and ask on the forum.

Raspberry Pi OS (64 bit)

Unzip the package anywhere you choose, for instance $home/Documents/Continuum/
Open a terminal, navigate to your unzipped Continuum directory and type

	sudo chmod +x ./Continuum93

This will allow you to execute the emulator, otherwise you might get a
"Permission denied" error. To run the application, type:

	./Continuum93



Writing code
------------

As of August 2023, the new preferred way to code for Continuum is to use Visual
Studio Code, since now you can have your code split across separate files and
a project directory now makes more sense. Make sure you install the 'ASM Code Lens'
extension for Visual Studio Code. It will help with syntax highlighting. Since
Continuum uses a proprietary ASM instruction set, it won't be perfect, but it
should be enough for now.

You may also keep using Notepad++ with the Vibrant Ink style enabled.
Find the "Continuum93 Assembly.xml" user language file provided in the
"Support/Notepad++ user defined language" directory and copy that in your
Notepad++ User defined language directory. To find where that is, open Notepad++,
click the Language menu option then "User defined language >" and then select
"Open User Defined Language folder" (tested on Notepad++ V8.4.9 64 bit)

If the syntax highlighting acts weird, make sure you select the
"Language/Continuum93 assembly" language file while editing your source code.

Once that's out of the way, simply create some .asm files and start typing code. I
recommend creating them in the Data/filesystem/programs/[your-project-dir]
directory and then run them from Continuum operating system.


Debugging
---------

Continuum Tools is available in the same directory and now it's upgraded and better
than ever. You can use it for debugging. Please read the "Continuum Tools Manual.pdf"
manual provided in the "Support" directory. Continuum tools is currently only
available for Windows environments.


Bugs
----

Probably some of them lurking around. While I do have over 3400 unit tests covering lots
of functionality of the assembler, registers, execution, interrupts and application, I
would expect some more of them that I (most likely) missed. While I'm on a permanent hunt
for them, feel free to report any that you find in the #support channel here:
https://discord.com/channels/1192833782974259380/1192929513491943485

