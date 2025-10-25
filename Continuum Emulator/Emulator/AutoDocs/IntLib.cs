using System.Collections.Generic;

namespace Continuum93.Emulator.AutoDocs
{
    public static class IntLib
    {
        private static readonly List<IntFunction> functions = new();

        public static void Initialize()
        {
            // Machine
            AddFunction("Machine", "Machine Stop", "Stops the machine. This is unrecoverable since the instruction pointer will not advance anymore. The visible effect is that Continuum will freeze. You will need to restart the emulator.",
                new IntParam(true, 1, "0x00", "Machine"),
                new IntParam(true, 1, "0x00", "Stop")
            );

            AddFunction("Machine", "Machine Clear", "Clears the machine memory, registers, reinitializes video pages and palettes, sets rendering to enabled and running to true",
                new IntParam(true, 1, "0x00", "Machine"),
                new IntParam(true, 1, "0x01", "Clear")
            );

            AddFunction("Machine", "Read Clock Milliseconds", "Writes the number of milliseconds since the machine started at the specified address (32-bit response).",
                new IntParam(true, 1, "0x00", "Machine"),
                new IntParam(true, 1, "0x03", "Read Clock"),
                new IntParam(true, 1, "0x00", "Milliseconds"),
                new IntParam(true, 3, "", "destination address")
            );

            AddFunction("Machine", "Read Clock Ticks", "Writes the number of ticks since the machine started at the specified address. (64 bit response)",
                new IntParam(true, 1, "0x00", "Machine"),
                new IntParam(true, 1, "0x03", "Read Clock"),
                new IntParam(true, 1, "0x01", "Ticks"),
                new IntParam(true, 3, "", "destination address")
            );

            AddFunction("Machine", "ToggleFullscreen", "Attempts to toggle fulscreen on host system.",
                new IntParam(true, 1, "0x00", "Machine"),
                new IntParam(true, 1, "0x10", "ToggleFullscreen"),
                new IntParam(false, 1, "", "1 if resulting state is fullscreen, 0 if not.")
            );

            AddFunction("Machine", "ShutDown", "Issues a shutdown command that should exit the emulator.",
                new IntParam(true, 1, "0x00", "Machine"),
                new IntParam(true, 1, "0x20", "ShutDown")
            );

            AddFunction("Machine", "Build", "Builds continuum executable code from an assembly source file and loads the code blocks into memory. Returns the address of the code's entry point.",
                new IntParam(true, 1, "0x00", "Machine"),
                new IntParam(true, 1, "0xC0", "Build"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the assembly source file path."),
                new IntParam(false, 3, "", "the address to the program's entry point or 0xFFFFFF if it failed to find the file."),
                new IntParam(false, 1, "", "the number of errors (capped to 255) that the compile process produced.")
            );

            // Video
            AddFunction("Video", "Read Video Resolution", "Returns resolution width and height",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x00", "ReadVideoResolution"),
                new IntParam(false, 2, "", "resolution width", 1),
                new IntParam(false, 2, "", "resolution height", 1)
            );

            AddFunction("Video", "Read Video Pages Count", "Returns the number of video pages.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x01", "ReadVideoPagesCount"),
                new IntParam(false, 1, "", "available video pages.")
            );

            AddFunction("Video", "Set Video Pages Count", "Sets the number of video pages to be rendered by the engine. This resets palettes and may produce visual artifacts.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x02", "SetVideoPagesCount"),
                new IntParam(true, 1, "", "number of pages (1 - 8)")
            );

            AddFunction("Video", "Read Video Address", "Returns a pointer to the VRAM position where the specified video page starts at.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x03", "ReadVideoAddress"),
                new IntParam(true, 1, "", "the video page of which start address we need to find (0 - 7)"),
                new IntParam(false, 3, "", "the start address of the provided video page", 1)
            );

            AddFunction("Video", "Read Video Palette Address", "Returns a pointer to the VRAM position where the specified video page's pallete starts at.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x04", "ReadVideoPaletteAddress"),
                new IntParam(true, 1, "", "the video page palette of which start address we need to find (0 - 7)"),
                new IntParam(false, 3, "", "the start address of the palette for the provided video page", 1)
            );

            AddFunction("Video", "Clear Video Page", "Clears (fills) the specified video page with the provided color.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x05", "ClearVideoPage"),
                new IntParam(true, 1, "", "the video page which needs clearing (0 - 7)"),
                new IntParam(true, 1, "", "the color which will be used to fill that memory page (0 - transparent or 1 - 255).")
            );

            AddFunction("Video", "Draw Filled Rectangle", "Draws on the specified video page a filled rectangle at x, y with given width, height and fill color. Can draw partially even if coordinates meet offscreen space.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x06", "DrawFilledRectangle"),
                new IntParam(true, 1, "", "the video page on which we draw the rectangle (0 - 7)"),
                new IntParam(true, 2, "", "the x coordinate of the top-left corner of the rectangle as a signed 16-bit number."),
                new IntParam(true, 2, "", "the y coordinate of the top-left corner of the rectangle as a signed 16-bit number."),
                new IntParam(true, 2, "", "the width of the rectangle."),
                new IntParam(true, 2, "", "the height of the rectangle."),
                new IntParam(true, 1, "", "the fill color of the rectangle. Note there is no border, so a transparent color here will make the rectangle invisible.")
            );

            AddFunction("Video", "Draw Rectangle", "Draws on the specified video page a rectangle at x, y with given width, height and color. Can draw partially even if coordinates meet offscreen space.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x07", "DrawRectangle"),
                new IntParam(true, 1, "", "the video page on which we draw the rectangle (0 - 7)"),
                new IntParam(true, 2, "", "the x coordinate of the top-left corner of the rectangle as a signed 16-bit number."),
                new IntParam(true, 2, "", "the y coordinate of the top-left corner of the rectangle as a signed 16-bit number."),
                new IntParam(true, 2, "", "the width of the rectangle."),
                new IntParam(true, 2, "", "the height of the rectangle."),
                new IntParam(true, 1, "", "the color of the rectangle.")
            );

            AddFunction("Video", "Draw Filled Rounded Rectangle", "Draws on the specified video page a filled rectangle at x, y with given width, height, color and corner roundness value. Can draw partially even if coordinates meet offscreen space.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x08", "DrawFilledRoundedRectangle"),
                new IntParam(true, 1, "", "the video page on which we draw the rectangle (0 - 7)"),
                new IntParam(true, 2, "", "the x coordinate of the top-left corner of the rectangle as a signed 16-bit number."),
                new IntParam(true, 2, "", "the y coordinate of the top-left corner of the rectangle as a signed 16-bit number."),
                new IntParam(true, 2, "", "the width of the rectangle."),
                new IntParam(true, 2, "", "the height of the rectangle."),
                new IntParam(true, 1, "", "the color of the rectangle."),
                new IntParam(true, 1, "", "the roundness value of the corners.")
            );

            AddFunction("Video", "Draw Rounded Rectangle", "Draws on the specified video page a rectangle at x, y with given width, height, color and corner roundness value. Can draw partially even if coordinates meet offscreen space.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x09", "DrawRoundedRectangle"),
                new IntParam(true, 1, "", "the video page on which we draw the rectangle (0 - 7)"),
                new IntParam(true, 2, "", "the x coordinate of the top-left corner of the rectangle as a signed 16-bit number."),
                new IntParam(true, 2, "", "the y coordinate of the top-left corner of the rectangle as a signed 16-bit number."),
                new IntParam(true, 2, "", "the width of the rectangle."),
                new IntParam(true, 2, "", "the height of the rectangle."),
                new IntParam(true, 1, "", "the color of the rectangle."),
                new IntParam(true, 1, "", "the roundness value of the corners.")
            );

            AddFunction("Video", "Draw Tile Map Sprite", "Draws on the specified video page a sprite at x, y with given width, height. Can draw partially even if coordinates meet offscreen space.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x0E", "DrawTileMapSprite"),
                new IntParam(true, 3, "", "the source address (in RAM) of the tile map."),
                new IntParam(true, 2, "", "the width of the tilemap in pixels."),
                new IntParam(true, 2, "", "the x position of the sprite within the tile map as a signed 16-bit number."),
                new IntParam(true, 2, "", "the y position of the sprite within the tile map as a signed 16-bit number."),
                new IntParam(true, 2, "", "the width of the sprite within the tile map."),
                new IntParam(true, 2, "", "the height of the sprite within the tile map."),
                new IntParam(true, 1, "", "the target video page where the sprite is to be drawn."),
                new IntParam(true, 2, "", "the target x coordinate where the sprite is to be drawn."),
                new IntParam(true, 2, "", "the target y coordinate where the sprite is to be drawn."),
                new IntParam(true, 1, "", "the effects to be applied to the sprite that is being drawn (flip, tiling, rotation on 22.5 degrees increments)."),
                new IntParam(true, 1, "", "(optional) horizontal tiling value, how many times this repeats horizontally."),
                new IntParam(true, 1, "", "(optional) vertical tiling value, how many times this repeats vertically.")
            );

            AddFunction("Video", "Draw Sprite", "Draws on the specified video page a sprite at x, y with given width, height. Can draw partially even if coordinates meet offscreen space.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x10", "DrawSprite"),
                new IntParam(true, 3, "", "the source address (in RAM) of the sprite to be drawn."),
                new IntParam(true, 1, "", "the video page on which we draw the sprite (0 - 7)"),
                new IntParam(true, 2, "", "the x coordinate of the top-left corner of the sprite as a signed 16-bit number."),
                new IntParam(true, 2, "", "the y coordinate of the top-left corner of the sprite as a signed 16-bit number."),
                new IntParam(true, 2, "", "the width of the sprite."),
                new IntParam(true, 2, "", "the height of the sprite.")
            );

            AddFunction("Video", "Draw String", "Draws on the specified video page a null terminated string at x, y with given color. Can draw partially even if coordinates meet offscreen space.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x12", "DrawString"),
                new IntParam(true, 3, "", "the source address (in RAM) of the font to be used."),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string to be drawn."),
                new IntParam(true, 2, "", "the x coordinate of the top-left corner of the sprite as a signed 16-bit number."),
                new IntParam(true, 2, "", "the y coordinate of the top-left corner of the sprite as a signed 16-bit number."),
                new IntParam(true, 1, "", "the color used to draw the string"),
                new IntParam(true, 1, "", "the video page on which we draw the string (0 - 7)"),
                new IntParam(true, 2, "", "the maximum width in pixels of the text to be drawn. Everything else is clipped."),
                new IntParam(false, 2, "", "the pixel x coordinate right after the end of the drawn string."),
                new IntParam(false, 1, "", "0 if no x overflow occured within specified limits, 1 if overflow occured.")
            );

            AddFunction("Video", "Draw Text", "Draws on the specified video page a null terminated string at x, y with given color. Can draw partially even if coordinates meet offscreen space.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x14", "DrawText"),
                new IntParam(true, 3, "", "the source address (in RAM) of the font to be used."),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string to be drawn."),
                new IntParam(true, 2, "", "the x coordinate of the top-left corner of the sprite as a signed 16-bit number."),
                new IntParam(true, 2, "", "the y coordinate of the top-left corner of the sprite as a signed 16-bit number."),
                new IntParam(true, 1, "", "the color used to draw the string"),
                new IntParam(true, 1, "", "the video page on which we draw the string (0 - 7)"),
                new IntParam(true, 2, "", "the width in pixels of the bounding area for the text."),
                new IntParam(true, 1, "", "flags associated with how the text should be drawn"),
                new IntParam(true, 1, "", "optional outline colour"),
                new IntParam(true, 1, "", "optional outline pattern"),
                new IntParam(false, 2, "", "the total width of the rendered text."),
                new IntParam(false, 2, "", "the total height of the rendered text.")
            );

            AddFunction("Video", "Get Text Rectangle", "Simulates drawing on the specified video page a null terminated string at x, y with given color. Returns the width and height of the text block.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x15", "GetTextRectangle"),
                new IntParam(true, 3, "", "the source address (in RAM) of the font to be used."),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string to be drawn."),
                new IntParam(true, 2, "", "the x coordinate of the top-left corner of the sprite as a signed 16-bit number."),
                new IntParam(true, 2, "", "the y coordinate of the top-left corner of the sprite as a signed 16-bit number."),
                new IntParam(true, 1, "", "the color used to draw the string"),
                new IntParam(true, 1, "", "the video page on which we draw the string (0 - 7)"),
                new IntParam(true, 2, "", "the width in pixels of the bounding area for the text."),
                new IntParam(true, 1, "", "flags associated with how the text should be drawn"),
                new IntParam(true, 1, "", "optional outline colour"),
                new IntParam(true, 1, "", "optional outline pattern"),
                new IntParam(false, 2, "", "the total width of the rendered text."),
                new IntParam(false, 2, "", "the total height of the rendered text.")
            );

            AddFunction("Video", "Plot", "Plots a point on given videopage at x, y with given fill color.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x20", "Plot"),
                new IntParam(true, 1, "", "the video page on which we plot (0 - 7)"),
                new IntParam(true, 2, "", "the x coordinate of the point."),
                new IntParam(true, 2, "", "the y coordinate of the point."),
                new IntParam(true, 1, "", "the color of the point.")
            );

            AddFunction("Video", "Line", "Draws a line between x1, y1 and x2, y2 with given fill color.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x21", "Line"),
                new IntParam(true, 1, "", "the video page on which we plot (0 - 7)"),
                new IntParam(true, 2, "", "the x1 coordinate of the line."),
                new IntParam(true, 2, "", "the y1 coordinate of the line."),
                new IntParam(true, 2, "", "the x2 coordinate of the line."),
                new IntParam(true, 2, "", "the y2 coordinate of the line."),
                new IntParam(true, 1, "", "the color of the line.")
            );

            AddFunction("Video", "Ellipse", "Draws an ellipse at x, y with radiusX and radiusY with given color.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x22", "Ellipse"),
                new IntParam(true, 1, "", "the video page on which we plot (0 - 7)"),
                new IntParam(true, 2, "", "the x coordinate of the ellipse."),
                new IntParam(true, 2, "", "the y coordinate of the ellipse."),
                new IntParam(true, 2, "", "the radius X of the ellipse."),
                new IntParam(true, 2, "", "the radius Y of the ellipse."),
                new IntParam(true, 1, "", "the color of the ellipse.")
            );

            AddFunction("Video", "Filled Ellipse", "Draws a filled ellipse at x, y with radiusX and radiusY with given fill color.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x23", "FilledEllipse"),
                new IntParam(true, 1, "", "the video page on which we plot (0 - 7)"),
                new IntParam(true, 2, "", "the x coordinate of the ellipse."),
                new IntParam(true, 2, "", "the y coordinate of the ellipse."),
                new IntParam(true, 2, "", "the radius X of the ellipse."),
                new IntParam(true, 2, "", "the radius Y of the ellipse."),
                new IntParam(true, 1, "", "the fill color of the ellipse.")
            );

            AddFunction("Video", "Line Path", "Draws a set of connected lines based on an input provided at the specified RAM address.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x24", "LinePath"),
                new IntParam(true, 3, "", "the RAM address where the points are stored."),
                new IntParam(true, 1, "", "the video page on which we draw (0 - 7)"),
                new IntParam(true, 1, "", "the fill color.")

            );

            AddFunction("Video", "Bezier Path", "Draws a set of connected bezier paths based on an input provided at the specified RAM address. Can be dashed.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x25", "BezierPath"),
                new IntParam(true, 3, "", "the RAM address where the points are stored."),
                new IntParam(true, 1, "", "the video page on which we draw (0 - 7)"),
                new IntParam(true, 1, "", "the width of the drawing brush in pixels."),
                new IntParam(true, 1, "", "the size in pixels of the filled segment of the path."),
                new IntParam(true, 1, "", "the size in pixels of the hollow segment of the path."),
                new IntParam(true, 1, "", "the color index of the filled segment of the path."),
                new IntParam(true, 1, "", "the color index of the outline of the filled segment of the path."),
                new IntParam(true, 1, "", "the start index (in percentage) where the drawing starts from out of the full available path."),
                new IntParam(true, 1, "", "the end index (in percentage) where the drawing ends on the full available path.")
            );

            AddFunction("Video", "Perlin Path",
                "Draws a deterministic Perlin-noise-based mountain-like polyline from (startX,startY) to (endX,endY) on the specified video page. "
                +"Supports two stacked noise patterns (Pattern1 mandatory, Pattern2 optional) each with its own seed, Y-range, zoom and horizontal shift. "
                +"No randomness is used; outputs are fully reproducible given the seeds. Pattern2 is disabled if its zoom is 0.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x26", "PerlinPath"),

                // Core drawing params
                new IntParam(true, 1, "", "the video page on which to draw (0 - 7)."),
                new IntParam(true, 2, "", "the starting x coordinate (signed 16-bit)."),
                new IntParam(true, 2, "", "the starting y coordinate (signed 16-bit)."),
                new IntParam(true, 2, "", "the ending x coordinate (signed 16-bit)."),
                new IntParam(true, 2, "", "the ending y coordinate (signed 16-bit)."),
                new IntParam(true, 1, "", "the color of the polyline."),

                // Pattern 1 (required)
                new IntParam(true, 2, "", "Pattern1 seed (unsigned 16-bit). Determines gradient hashing; deterministic."),
                new IntParam(true, 2, "", "Pattern1 MinY (signed 16-bit). Lower bound of vertical offset contributed by Pattern1."),
                new IntParam(true, 2, "", "Pattern1 MaxY (signed 16-bit). Upper bound of vertical offset contributed by Pattern1."),
                new IntParam(true, 1, "", "Pattern1 Zoom (8-bit). Higher = larger features (lower frequency)."),
                new IntParam(true, 2, "", "Pattern1 Shift (signed 16-bit). Horizontal shift in noise domain (left/right)."),

                // Pattern 2 (optional; set Zoom=0 to disable)
                new IntParam(true, 2, "", "Pattern2 seed (unsigned 16-bit)."),
                new IntParam(true, 2, "", "Pattern2 MinY (signed 16-bit)."),
                new IntParam(true, 2, "", "Pattern2 MaxY (signed 16-bit)."),
                new IntParam(true, 1, "", "Pattern2 Zoom (8-bit). Set to 0 to disable Pattern2."),
                new IntParam(true, 2, "", "Pattern2 Shift (signed 16-bit).")
            );


            AddFunction("Video", "Area or polygon fill", "",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x28", "AreaOrPolyFill"),
                new IntParam(true, 1, "", "the video page where we perform the fill (0 - 7)"),
                new IntParam(true, 2, "", "the x coordinate of the starting point of the fill."),
                new IntParam(true, 2, "", "the y coordinate of the starting point of the fill."),
                new IntParam(true, 1, "", "the color with which to fill."),
                new IntParam(true, 1, "", "the border color that marks the boundary of the fill area in case bordered fill is enabled."),
                new IntParam(true, 1, "", "flags. Bit 0 toggles between flood fill and bordered fill.")
            );

            AddFunction("Video", "Read Layers Visibility", "Returns an 8-bit value with the visible state of each video layer. Bit zero points to layer zero and so on. 1 visible, 0 not visible.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x30", "ReadLayersVisibility"),
                new IntParam(false, 1, "", "8 bits containing layer visibility, one bit per layer.")
            );

            AddFunction("Video", "Set Layers Visibility", "Sets the video layers visibility from a provided 8-bit value. Bit zero points to layer zero and so on. 1 visible, 0 not visible.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x31", "SetLayersVisibility"),
                new IntParam(true, 1, "", "8 bits to provide layer visibility, one bit per layer.")
            );

            AddFunction("Video", "Read Buffer Control Mode", "Returns an 8-bit value with the auto display state of the video buffers. Bit zero points to layer zero and so on. Any layer set on auto (1) will be drawn when the video processor decides, if it is set on manual (0) then the program is responsible to manually draw.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x32", "ReadBufferControlMode"),
                new IntParam(false, 1, "", "8 bits containing layer auto mode, one bit per layer.")
            );

            AddFunction("Video", "Set Buffer Control Mode", "Sets the layers auto draw mode from a provided 8-bit value. Bit zero points to layer zero and so on. Value 1 means auto enabled, 0 auto disabled (manual). Auto mode disabled requires the user to update the video buffer from the video RAM.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x33", "SetBufferControlMode"),
                new IntParam(true, 1, "", "8 bits to provide layer auto mode, one bit per layer.")
            );

            AddFunction("Video", "Hardware scroll/roll", "Scrolls a portion of the screen on the specified video page bound by a defined rectangle with specified speed/direction separate per horizontal/vertical. Flags control toggling between scroll and roll independently per horizontal/vertical.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x40", "HardwareScrollRoll"),
                new IntParam(true, 1, "", "the video page on which we perform the effect (0 - 7)"),
                new IntParam(true, 2, "", "the x coordinate of the top-left corner of the rectangle as an unsigned 16-bit number."),
                new IntParam(true, 2, "", "the y coordinate of the top-left corner of the rectangle as an unsigned 16-bit number."),
                new IntParam(true, 2, "", "the width of the rectangle perform the scroll/roll onto."),
                new IntParam(true, 2, "", "the height of the rectangle perform the scroll/roll onto."),
                new IntParam(true, 2, "", "the horizontal scroll speed as a signed 16-bit number. Sign determines direction."),
                new IntParam(true, 2, "", "the vertical scroll speed as a signed 16-bit number. Sign determines direction."),
                new IntParam(true, 1, "", "flags. When bit 0 is set, rolling is set for the horizontal instead of scroll. Bit 1 sets the same but for vertical.")
            );

            AddFunction("Video", "Copy rectangle", "Copies pixels from one rectangle of a given video page to another rectangle of a destination video page. Source and destination can be different or the same video page, source and destination rectangles can have different dimensions.",
                new IntParam(true, 1, "0x01", "Video"),
                new IntParam(true, 1, "0x41", "CopyRectangle"),
                new IntParam(true, 1, "", "the source video page from where we copy (0 - 7)"),
                new IntParam(true, 2, "", "the x coordinate of the top-left corner of the rectangle that will mark pixels to be copied, as an unsigned 16-bit number."),
                new IntParam(true, 2, "", "the y coordinate of the top-left corner of the rectangle that will mark pixels to be copied, as an unsigned 16-bit number."),
                new IntParam(true, 2, "", "the width of the rectangle to mark the pixels to be copied."),
                new IntParam(true, 2, "", "the height of the rectangle to mark the pixels to be copied."),
                new IntParam(true, 1, "", "the destination video page to where we copy (0 - 7)"),
                new IntParam(true, 2, "", "the x coordinate of the top-left corner of the rectangle that will mark the destination cfor the copied pixels, as an unsigned 16-bit number."),
                new IntParam(true, 2, "", "the y coordinate of the top-left corner of the rectangle that will mark the destination cfor the copied pixels, as an unsigned 16-bit number."),
                new IntParam(true, 2, "", "the width of the destination rectangle."),
                new IntParam(true, 2, "", "the height of the destination rectangle.")


            );

            // Input
            AddFunction("Input", "Read Keyboard State As Bits", "Dumps the state of all keyboard keys at the indicated RAM address. " +
                "The state of all 256 keys is dumped as 32 bytes for all 256 bits, 1 indicating pressed and 0 not pressed.",
                new IntParam(true, 1, "0x02", "Input"),
                new IntParam(true, 1, "0x00", "ReadKeyboardState"),
                new IntParam(true, 3, "", "the RAM address where to dump the keyboard state."),
                new IntParam(false, 1, "", "the keyboard state buffer length.")
            );

            AddFunction("Input", "Read Keyboard Buffer", "Reads the next typed key in the keyboard buffer.",
                new IntParam(true, 1, "0x02", "Input"),
                new IntParam(true, 1, "0x01", "ReadKeyboardBuffer"),
                new IntParam(false, 1, "", "the retrieved caracter."),
                new IntParam(false, 1, "", "the remaining characters count in the buffer.")
            );

            AddFunction("Input", "Read Mouse State", "Gets the X and Y of the mouse as well as if the left mouse is pressed",
                new IntParam(true, 1, "0x02", "Input"),
                new IntParam(true, 1, "0x03", "ReadMouseState"),
                new IntParam(false, 2, "", "mouse x position.", 1),
                new IntParam(false, 2, "", "mouse y position.", 1),
                new IntParam(false, 1, "", "mouse buttons state (left, right and middle as bits 0, 1 and 2 with '1' pressed, '0' not pressed).", 1)
            );

            AddFunction("Input", "Read Keyboard Pressed Keys As Codes", "Dumps only the codes of pressed keyboard keys at the indicated RAM address. " +
                "All pressed keys will have their codes in this response buffer.",
                new IntParam(true, 1, "0x02", "Input"),
                new IntParam(true, 1, "0x10", "ReadKeyboardPressedKeysAsCodes"),
                new IntParam(true, 3, "", "the RAM address where to dump the pressed keys codes."),
                new IntParam(false, 1, "", "the pressed keys codes buffer length.")
            );

            AddFunction("Input", "Read GamePads State", "Dumps the states of all gamepad controllers along with connectivity status at the indicated RAM address" +
                "There will be 41 bytes dumped, the first one containing connectivity and any button pressed while the others will have the full state, 10 bytes each.",
                new IntParam(true, 1, "0x02", "Input"),
                new IntParam(true, 1, "0x14", "ReadGamePadsState"),
                new IntParam(true, 3, "", "the RAM address where to dump the state.")
            );

            AddFunction("Input", "Read GamePads Capabilities", "Dumps the capabilities of all gamepad controllers along with connectivity status at the indicated RAM address" +
                "There will be 17 bytes dumped, the first one containing connectivity state for each controller in bits 0-3, the rest, capabilities, 4 bytes per controller." +
                "This feature might not always work on Linux.",
                new IntParam(true, 1, "0x02", "Input"),
                new IntParam(true, 1, "0x15", "ReadGamePadsCapabilities"),
                new IntParam(true, 3, "", "the RAM address where to dump the capabilities.")
            );

            AddFunction("Input", "Read GamePads Names", "Dumps the names of connected gamepads at the indicated RAM address as null terminated strings. The response is clamped " +
                "to 256 bytes, so if the names exceed that, they will be proportionally resized. If a name has an empty string, it means there is no controller connected in" +
                "the respective slot. There will be up to 256 bytes dumped. This feature might not always work on Linux.",
                new IntParam(true, 1, "0x02", "Input"),
                new IntParam(true, 1, "0x16", "ReadGamePadsNames"),
                new IntParam(true, 3, "", "the RAM address where to dump the names."),
                new IntParam(false, 1, "", "the length of the dumped string names including the null terminators.")
            );

            // Filesystem
            AddFunction("Filesystem", "File Exists", "Returns whether the file specified at null terminated string path exists.",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x02", "FileExists"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the file path."),
                new IntParam(false, 1, "", "true (0xFF) or false (0x00) depending on whether the file is found at the specified path.")
            );

            AddFunction("Filesystem", "Directory Exists", "Returns whether the directory specified at null terminated string path exists.",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x03", "DirectoryExists"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the directory path."),
                new IntParam(false, 1, "", "true (0xFF) or false (0x00) depending on whether the directory is found at the specified path.")
            );

            AddFunction("Filesystem", "List Files in Directory", "Dumps a file list of found files in a directory path specified by a null terminated string." +
                "Consistent order is not guaranteed and depends on the running platform (Linux/Windows).",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x04", "ListFilesInDirectory"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the directory path."),
                new IntParam(true, 3, "", "the destination address (in RAM) where the directory files listing will be dumped as null terminated strings."),
                new IntParam(false, 3, "", "the number of files found or " + Constants.MAX24BIT + " if directory was not found.", 1)
            );

            AddFunction("Filesystem", "List Directories in Directory", "Dumps a file list of found files in a directory path specified by a null terminated string." +
                "Consistent order is not guaranteed and depends on the running platform (Linux/Windows).",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x05", "ListDirectoriesInDirectory"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the directory path."),
                new IntParam(true, 3, "", "the destination address (in RAM) where the directory directories listing will be dumped as null terminated strings."),
                new IntParam(false, 3, "", "the number of directories found or " + Constants.MAX24BIT + " if directory was not found.", 1)
            );

            AddFunction("Filesystem", "Save File", "Saves a file specified by a null terminated string path to the filesystem. File is saved from RAM given the start address and length.",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x06", "SaveFile"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the desired save file path."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the file contents start."),
                new IntParam(true, 3, "", "the number of bytes to be saved to the file.")
            );

            AddFunction("Filesystem", "Load File", "Loads a file specified by a null terminated string path to the filesystem. File is loaded in RAM at the specified address.",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x07", "LoadFile"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the desired path to the file to be loaded."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the file will be loaded.")
            );

            AddFunction("Filesystem", "List Directories and Files in Directory", "Dumps a file list of found directories and files in a directory path specified by a null terminated string. After the directory list ends, another 0 is inserted to know the files are next." +
                "Consistent order is not guaranteed and depends on the running platform (Linux/Windows).",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x15", "ListDirectoriesAndFilesInDirectory"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the directory path."),
                new IntParam(true, 3, "", "the destination address (in RAM) where the directory directories listing will be dumped as null terminated strings."),
                new IntParam(false, 3, "", "the number of directories and files found or " + Constants.MAX24BIT + " if directory was not found.", 1),
                new IntParam(false, 3, "", "the number of directories found.", 1),
                new IntParam(false, 3, "", "the number of files found.", 1),
                new IntParam(false, 3, "", "the address where the file listing starts.", 1)
            );

            AddFunction("Filesystem", "Get file size", "Gets the size in bytes of the file pointed at by the input 24-bit register",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x20", "GetFileSize"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the file path."),
                new IntParam(false, 4, "", "The filesize, in bytes.", 1)
            );

            AddFunction("Filesystem", "Load image and palette", "Loads an image file into memory and also provides the color palette. The image is converted to a palette equivalent and is loaded in memory at the specified address. The palette is loaded just before this address.",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x30", "LoadImageAndPalette"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the desired path to the image file to be processed."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the image will be loaded. Image is loaded starting with this address and the palette is deposited just before this address at (address - palette size)."),
                new IntParam(false, 1, "", "the size of the loaded palette. It is per color, so multiply by 3 to get byte size.")
            );

            AddFunction("Filesystem", "Load image", "Loads an image file into memory without the color palette. The image is converted to a palette equivalent and is loaded in memory at the specified address.",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x31", "LoadImage"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the desired path to the image file to be processed."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the image will be loaded. Image is loaded starting with this address.")
            );

            AddFunction("Filesystem", "Load palette", "Loads the palette of a provided image file into memory.",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x32", "LoadImage"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the desired path to the image file to be processed."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the palette will be loaded.")
            );

            AddFunction("Filesystem", "Load 8bit PNG", "Loads an image file into memory and also provides the color palette. The image is converted to a palette equivalent and is loaded in memory at the specified address. The palette is loaded just before this address.",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x33", "Load8BitPng"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the desired path to the image file to be processed."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the PNG palette and transparency data will be loaded."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the PNG pixel data will be loaded."),
                new IntParam(false, 1, "", "the state of the operation. 0 means success. Other values mean specific errors detailed separately."),
                new IntParam(false, 2, "", "the width of the loaded PNG image file."),
                new IntParam(false, 2, "", "the height of the loaded PNG image file."),
                new IntParam(false, 1, "", "the number of colors present in the palette.")
            );

            AddFunction("Filesystem", "Load 8bit PNG with Custom Transparency", "Loads an image file into memory and also provides the color palette. The image is converted to a palette equivalent and is loaded in memory at the specified address. The palette is loaded just before this address.",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x34", "Load8BitPngWithCustomTransparency"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the desired path to the image file to be processed."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the PNG palette and transparency data will be loaded."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the PNG pixel data will be loaded."),
                new IntParam(true, 4, "", "the RGBA color that will be interpreted as the transparent color from the provided RGB."),
                new IntParam(false, 1, "", "the state of the operation. 0 means success. Other values mean specific errors detailed separately."),
                new IntParam(false, 2, "", "the width of the loaded PNG image file."),
                new IntParam(false, 2, "", "the height of the loaded PNG image file."),
                new IntParam(false, 1, "", "the number of colors present in the palette.")
            );

            AddFunction("Filesystem", "Merge 8bit PNG", "Merge an image file into memory and also provides the color palette. The image is converted to a palette equivalent and is loaded in memory at the specified address. The palette is loaded just before this address.",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x35", "Merge8BitPng"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the desired path to the image file to be processed."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the PNG palette and transparency data will be loaded."),
                new IntParam(true, 1, "", "the number of existing colors on the color palette that will be merged into."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the PNG pixel data will be loaded."),
                new IntParam(false, 1, "", "the state of the operation. 0 means success. Other values mean specific errors detailed separately."),
                new IntParam(false, 2, "", "the width of the loaded PNG image file."),
                new IntParam(false, 2, "", "the height of the loaded PNG image file."),
                new IntParam(false, 1, "", "the number of colors present in the resulting palette.")
            );

            AddFunction("Filesystem", "Merge 8bit PNG with Custom Transparency", "Merge an image file into memory and also provides the color palette. The image is converted to a palette equivalent and is loaded in memory at the specified address. The palette is loaded just before this address.",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x36", "Merge8BitPngWithCustomTransparency"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the desired path to the image file to be processed."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the PNG palette and transparency data will be loaded."),
                new IntParam(true, 1, "", "the number of existing colors on the color palette that will be merged into."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the PNG pixel data will be loaded."),
                new IntParam(true, 4, "", "the RGBA color that will be interpreted as the transparent color from the provided RGB."),
                new IntParam(false, 1, "", "the state of the operation. 0 means success. Other values mean specific errors detailed separately."),
                new IntParam(false, 2, "", "the width of the loaded PNG image file."),
                new IntParam(false, 2, "", "the height of the loaded PNG image file."),
                new IntParam(false, 1, "", "the number of colors present in the resulting palette.")
            );

            AddFunction("Filesystem", "LoadPNGFont", "Loads a font from a palette based PNG file that stores it in the form of a grid of 16 by 6 glyphs. The font is automatically converted to binary data and loaded in memory at the specified address.",
                new IntParam(true, 1, "0x04", "Filesystem"),
                new IntParam(true, 1, "0x40", "LoadPNGFont"),
                new IntParam(true, 3, "", "the source address (in RAM) of the null terminated string which represents the desired path to the font image file to be processed."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the PNG font data will be loaded."),
                new IntParam(false, 3, "", "the size in bytes of the font loaded.")
            );

            // Strings
            AddFunction("Strings", "FloatToString", "Converts a float register value to the string equivalent and deposits the resultins string at the specified memory location. It also takes a format string to fine tune how the result is produced.",
                new IntParam(true, 1, "0x05", "Strings"),
                new IntParam(true, 1, "0x01", "FloatToString"),
                new IntParam(true, 1, "", "The source register index that holds the value to be converted. For instance if register is F3, this needs to be 3."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the format string resides."),
                new IntParam(true, 3, "", "the pointer to the target address (in RAM) where the string result will be deposited.")
            );

            AddFunction("Strings", "UintToString", "Converts a 32 bit unsigned register value to the string equivalent and deposits the resultins string at the specified memory location. It also takes a format string to fine tune how the result is produced.",
                new IntParam(true, 1, "0x05", "Strings"),
                new IntParam(true, 1, "0x02", "UintToString"),
                new IntParam(true, 4, "", "The source register that holds the unsigned integer value to be converted."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the format string resides."),
                new IntParam(true, 3, "", "the pointer to the target address (in RAM) where the string result will be deposited.")
            );

            AddFunction("Strings", "IntToString", "Converts a 32 bit signed register value to the string equivalent and deposits the resultins string at the specified memory location. It also takes a format string to fine tune how the result is produced.",
                new IntParam(true, 1, "0x05", "Strings"),
                new IntParam(true, 1, "0x03", "IntToString"),
                new IntParam(true, 4, "", "The source register that holds the signed integer value to be converted."),
                new IntParam(true, 3, "", "the pointer to the address (in RAM) where the format string resides."),
                new IntParam(true, 3, "", "the pointer to the target address (in RAM) where the string result will be deposited.")
            );

            Log.WriteLine("Initialized documentation interrupt library");
        }

        public static void AddFunction(string interrupt, string name, string description, params IntParam[] parameters)
        {
            IntFunction func = new()
            {
                Interrupt = interrupt,
                Name = name,
                Description = description
            };

            foreach (IntParam param in parameters)
            {
                if (param.IsInput)
                    func.AddInput(param);
                else
                    func.AddOutput(param);
            }

            functions.Add(func);
        }

        public static string GenerateInterruptPages()
        {
            string response = "";
            string currentInterrupt = "";

            foreach (IntFunction iFunction in functions)
            {
                if (currentInterrupt != iFunction.Interrupt)
                {
                    response += HTML.H1(iFunction.Interrupt);
                    currentInterrupt = iFunction.Interrupt;
                }

                response += HTML.H2(iFunction.Name);
                response += HTML.P(iFunction.Description);

                response += HTML.P(HTML.Strong("Inputs"));
                response += iFunction.ExportInputs();
                response += HTML.P(HTML.Strong("Outputs"));
                response += iFunction.ExportOutputs();

                response += HTML.P(HTML.Strong("Example"));
                response += iFunction.ExportInputsExample();

                response += HTML.P(HTML.Strong("Response"));
                response += iFunction.ExportOutputsExample();
            }

            return HTML.HTMLRoot("Interrupt reference", response, "styles.css");
        }
    }

    public class IntFunction
    {
        public List<IntParam> Input = new();
        public List<IntParam> Output = new();
        public string Interrupt;
        public string Name;
        public string Description;

        public void AddInput(IntParam param)
        {
            Input.Add(param);
        }

        public void AddOutput(IntParam param)
        {
            Output.Add(param);
        }

        public string ExportInputs()
        {
            return ExportParams(Input, true);
        }

        public string ExportInputsExample()
        {
            byte regIndex = 0;
            bool start = true;
            string intLine = "";
            string response = "<pre>";

            foreach (IntParam param in Input)
            {
                if (regIndex == 0 && start)
                {
                    intLine = string.Format("INT {0}, A", param.Value) + "       ; Trigger interrupt " + param.Description + "<br />";
                    start = false;
                }
                else
                {
                    response += "LD " + GetExampleRegister(regIndex + param.Offset, param.Regs) + ", " +
                    ((param.Value.Length > 0) ? param.Value : "<i>value</i>") + "     ; " + param.Description + "<br />";

                    regIndex += param.Regs;
                }
            }

            response += intLine;
            response += "</pre>";

            return response;
        }

        public string ExportOutputsExample()
        {
            if (Output.Count == 0)
                return "none";

            string response = "";
            byte regIndex = 0;

            foreach (IntParam param in Output)
            {
                response += "Register <strong>" + GetExampleRegister(regIndex + param.Offset, param.Regs) + "</strong> will contain " + param.Description + "<br />";
                regIndex += param.Regs;
            };

            return response;
        }

        public string ExportOutputs()
        {
            return ExportParams(Output, false);
        }

        private string ExportParams(List<IntParam> parameters, bool ignoreFirst)
        {
            if (parameters.Count == 0)
                return "none";

            string response = "";
            byte regIndex = 0;

            response += HTML.InterruptRow(
                HTML.Strong("Register"),
                HTML.Strong("Example"),
                HTML.Strong("Description"),
                HTML.Strong("Value"),
                HTML.Strong("Capacity")
            );

            foreach (IntParam param in parameters)
            {
                response +=
                    HTML.InterruptRow(
                        ignoreFirst ? "N.A" :
                            new string('r', param.Regs) + "<span id='subscript'>" + (regIndex + param.Offset) + "</span>",
                        ignoreFirst ? "N.A" : GetExampleRegister(regIndex + param.Offset, param.Regs),
                        param.Description + (ignoreFirst ? " interrupt" : ""),
                        param.Value,
                        DocUtils.GetReadableBytesSize(param.Regs)
                    );

                if (ignoreFirst)
                {
                    ignoreFirst = false;
                }
                else
                {
                    regIndex += param.Regs;
                }
            }

            return HTML.IntTable(response);
        }

        private string GetExampleRegister(int index, byte length)
        {
            return Constants.ALPHABET.Substring(index, length);
        }
    }

    public class IntParam
    {
        public bool IsInput;            // Is input or output param
        public byte Regs;               // How many 8-bit registers does it take
        public string Value;            // If applicable
        public string Description;      // What is this parameter doing
        public byte Offset;             // Register offset, used for output that optionally offsets one register or more.

        public IntParam(bool isInput, byte regs, string value, string description, byte offset = 0)
        {
            IsInput = isInput;
            Regs = regs;
            Value = value;
            Description = description;
            Offset = offset;
        }
    }
}
