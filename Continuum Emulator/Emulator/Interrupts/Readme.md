Keyboard interrupts modes:

- KeyPressedCallback Triggers the stored callback address.
Needs to call from inside GetKeyPressedCode interrupt to get
the actual key code

- CheckKey gets the status of a specified key.
Input:
    - keycode
    - status type:
        - released - previous is pressed, current is not pressed
        - down - both previous and current are pressed
        - pressed - previous is not pressed, current is pressed


DrawText
    3 bytes: font source address;
    3 bytes: string source address;
    2 bytes: x
    2 bytes: y
    1 byte: color
    1 byte: video page
    2 bytes: max width
    1 byte: flags

    Draws text at the specified position
    flags:
    - monospace (disabled by default);
    - kerning (enabled by default);
    - center (center within a the provided maxWidth);
    - wrap (spawns on multiple rows along the maxWidth);

GetTextMetrics

    Simulates drawing with given parameters and returns the actual width and height of the rendered
    block of text