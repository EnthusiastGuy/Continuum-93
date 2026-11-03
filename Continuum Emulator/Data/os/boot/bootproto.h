/*
 * Continuum 93 boot protocol - shared by the boot selector (boot.c) and the Halcyon OS.
 *
 * Memory map of the 512 KB system area (user programs load at 0x080000 and above):
 *   0x000000 - 0x03FFFF   operating system image (Halcyon code + data, or Q OS)
 *   0x040000 - 0x04EFFF   Halcyon C stack (top 0x04F000, grows down)
 *   0x050000 - 0x06FFFF   Halcyon workspace (fonts, directory listings, document buffers)
 *   0x070000 - 0x07EFFF   boot selector image and stack (top 0x07F000)
 *   0x07FD00 - 0x07FEFF   boot mailbox (below)
 *
 * Halcyon never calls user programs itself: it leaves a request in the mailbox and returns to the boot
 * selector, which runs the program with every register saved, then checks whether the program overwrote
 * the OS image. If the image is intact Halcyon resumes exactly where it was; otherwise it is rebuilt.
 */

#ifndef BOOTPROTO_H
#define BOOTPROTO_H

#define BOOT_MAGIC          0x43393342u         /* "C93B" */

#define BOOT_CMD_NONE       0
#define BOOT_CMD_RUN        1                   /* run mailbox path, then resume the OS */
#define BOOT_CMD_MENU       2                   /* return to the boot menu */
#define BOOT_CMD_SHUTDOWN   3                   /* turn the machine off */

typedef struct
{
    unsigned int magic;
    int command;
    int resume;                 /* set by the selector: 1 = OS image intact, resume; 0 = fresh start */
    unsigned int image_end;     /* end of the OS image (for the integrity check) */
    unsigned int checksum;
    int last_result;            /* 0 = ok, >0 = number of build errors, -1 = file not found */
    char path[200];             /* program to run (relative to the filesystem root) */
    char reopen[200];           /* folder to show again after a cold restart */
} BootMailbox;

#define BOOT_MAILBOX ((BootMailbox *)0x07FD00)

#endif
