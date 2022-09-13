using Idb;

namespace Microsoft.Maui.Automation.Driver;

public static class iOSGrpcTextExtensions
{
    public static IEnumerable<HIDEvent> AsHidEvents(this string text)
        => text.SelectMany(c => c.AsHidEvents());

    static IEnumerable<HIDEvent> AsHidEvents(this HIDEvent.Types.HIDPressAction pressAction)
        => new[]
        {
            new HIDEvent.Types.HIDPress
            {
                Action = pressAction,
                Direction = HIDEvent.Types.HIDDirection.Down
            }.AsHidEvent(),
            new HIDEvent.Types.HIDPress
            {
                Action = pressAction,
                Direction = HIDEvent.Types.HIDDirection.Up
            }.AsHidEvent(),
        };

    public static IEnumerable<HIDEvent> AsHidEvents(this ulong keyCode)
        => new HIDEvent.Types.HIDPressAction
        {
            Key = new HIDEvent.Types.HIDKey { Keycode = keyCode }
        }.AsHidEvents();

    static HIDEvent AsHidKeyDownEvent(this ulong keyCode)
            => new HIDEvent.Types.HIDPress
            {
                Action = new HIDEvent.Types.HIDPressAction
                {
                    Key = new HIDEvent.Types.HIDKey
                    {
                        Keycode = keyCode
                    },

                },
                Direction = HIDEvent.Types.HIDDirection.Down
            }.AsHidEvent();

    static HIDEvent AsHidKeyUpEvent(this ulong keyCode)
        => new HIDEvent.Types.HIDPress
        {
            Action = new HIDEvent.Types.HIDPressAction
            {
                Key = new HIDEvent.Types.HIDKey
                {
                    Keycode = keyCode
                },

            },
            Direction = HIDEvent.Types.HIDDirection.Up
        }.AsHidEvent();

    static IEnumerable<HIDEvent> AsShiftEvents(this ulong keyCode)
        => new[]
        {
            ((ulong)225).AsHidKeyDownEvent(),
            keyCode.AsHidKeyDownEvent(),
            keyCode.AsHidKeyUpEvent(),
            ((ulong)225).AsHidKeyUpEvent()
        };

    static HIDEvent AsHidEvent(this HIDEvent.Types.HIDPress press)
        => new HIDEvent
        {
            Press = press
        };

    public static IEnumerable<HIDEvent> AsHidEvents(this char c)
        => c switch
        {
            'a' => 4UL.AsHidEvents(),
            'b' => 5UL.AsHidEvents(),
            'c' => 6UL.AsHidEvents(),
            'd' => 7UL.AsHidEvents(),
            'e' => 8UL.AsHidEvents(),
            'f' => 9UL.AsHidEvents(),
            'g' => 10UL.AsHidEvents(),
            'h' => 11UL.AsHidEvents(),
            'i' => 12UL.AsHidEvents(),
            'j' => 13UL.AsHidEvents(),
            'k' => 14UL.AsHidEvents(),
            'l' => 15UL.AsHidEvents(),
            'm' => 16UL.AsHidEvents(),
            'n' => 17UL.AsHidEvents(),
            'o' => 18UL.AsHidEvents(),
            'p' => 19UL.AsHidEvents(),
            'q' => 20UL.AsHidEvents(),
            'r' => 21UL.AsHidEvents(),
            's' => 22UL.AsHidEvents(),
            't' => 23UL.AsHidEvents(),
            'u' => 24UL.AsHidEvents(),
            'v' => 25UL.AsHidEvents(),
            'w' => 26UL.AsHidEvents(),
            'x' => 27UL.AsHidEvents(),
            'y' => 28UL.AsHidEvents(),
            'z' => 29UL.AsHidEvents(),
            'A' => 4UL.AsShiftEvents(),
            'B' => 5UL.AsShiftEvents(),
            'C' => 6UL.AsShiftEvents(),
            'D' => 7UL.AsShiftEvents(),
            'E' => 8UL.AsShiftEvents(),
            'F' => 9UL.AsShiftEvents(),
            'G' => 10UL.AsShiftEvents(),
            'H' => 11UL.AsShiftEvents(),
            'I' => 12UL.AsShiftEvents(),
            'J' => 13UL.AsShiftEvents(),
            'K' => 14UL.AsShiftEvents(),
            'L' => 15UL.AsShiftEvents(),
            'M' => 16UL.AsShiftEvents(),
            'N' => 17UL.AsShiftEvents(),
            'O' => 18UL.AsShiftEvents(),
            'P' => 19UL.AsShiftEvents(),
            'Q' => 20UL.AsShiftEvents(),
            'R' => 21UL.AsShiftEvents(),
            'S' => 22UL.AsShiftEvents(),
            'T' => 23UL.AsShiftEvents(),
            'U' => 24UL.AsShiftEvents(),
            'V' => 25UL.AsShiftEvents(),
            'W' => 26UL.AsShiftEvents(),
            'X' => 27UL.AsShiftEvents(),
            'Y' => 28UL.AsShiftEvents(),
            'Z' => 29UL.AsShiftEvents(),

            '1' => 30UL.AsHidEvents(),
            '2' => 31UL.AsHidEvents(),
            '3' => 32UL.AsHidEvents(),
            '4' => 33UL.AsHidEvents(),
            '5' => 34UL.AsHidEvents(),
            '6' => 35UL.AsHidEvents(),
            '7' => 36UL.AsHidEvents(),
            '8' => 37UL.AsHidEvents(),
            '9' => 38UL.AsHidEvents(),
            '0' => 39UL.AsHidEvents(),

            '\n' => 40UL.AsHidEvents(),
            ' ' => 44UL.AsHidEvents(),
            '-' => 45UL.AsHidEvents(),
            '=' => 46UL.AsHidEvents(),
            '[' => 47UL.AsHidEvents(),
            ']' => 48UL.AsHidEvents(),
            '\\' => 49UL.AsHidEvents(),
            ';' => 51UL.AsHidEvents(),
            '\'' => 52UL.AsHidEvents(),
            '`' => 53UL.AsHidEvents(),
            ',' => 54UL.AsHidEvents(),
            '.' => 55UL.AsHidEvents(),
            '/' => 56UL.AsHidEvents(),
        
            '!' => 30UL.AsShiftEvents(),
            '@' => 31UL.AsShiftEvents(),
            '#' => 32UL.AsShiftEvents(),
            '$' => 33UL.AsShiftEvents(),
            '%' => 34UL.AsShiftEvents(),
            '^' => 35UL.AsShiftEvents(),
            '&' => 36UL.AsShiftEvents(),
            '*' => 37UL.AsShiftEvents(),
            '(' => 38UL.AsShiftEvents(),
            ')' => 39UL.AsShiftEvents(),
            '"' => 45UL.AsShiftEvents(),
            '+' => 46UL.AsShiftEvents(),
            '{' => 47UL.AsShiftEvents(),
            '}' => 48UL.AsShiftEvents(),
            '|' => 49UL.AsShiftEvents(),
            ':' => 51UL.AsShiftEvents(),
            '~' => 53UL.AsShiftEvents(),
            '<' => 54UL.AsShiftEvents(),
            '>' => 55UL.AsShiftEvents(),
            '?' => 56UL.AsShiftEvents(),

            _ => throw new Exception($"Unknown Character Code: {c}")
        };
}