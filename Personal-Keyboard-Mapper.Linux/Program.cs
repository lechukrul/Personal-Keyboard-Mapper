using System;
using System.Threading;
using Personal_Keyboard_Mapper.Core.Enums;
using Personal_Keyboard_Mapper.Linux;
using Personal_Keyboard_Mapper.Linux.Native;

// Demo of the full Linux pipeline: grab the real keyboard, match a two-key
// sequence, emit synthetic output, pass every other key through.
//
//   numpad "1" then numpad "2"  ->  types "ok"
//   ESC                         ->  releases the grab and exits (safety hatch!)
//   anything else               ->  passed through unchanged
//
// Usage: sudo dotnet run [/dev/input/eventX]

string device = args.Length > 0 ? args[0] : "/dev/input/event0";

using var uinput = new UinputDevice();
using var hook = new EvdevKeyboardHook(device, uinput);
var simulator = new UinputInputSimulator(uinput);
var exitRequested = new ManualResetEventSlim();

VirtualKeyCode? firstKey = null;

hook.KeyHandler = ev =>
{
    Console.WriteLine($"[debug] {ev}");
    if (ev.Key == VirtualKeyCode.ESCAPE)
    {
        exitRequested.Set();
        return true;
    }
    if (ev.Key == VirtualKeyCode.NUMLOCK)
    {
        // VirtualBox emits synthetic NumLock presses around every numpad key
        // to sync host/guest lock state — they must not reset the combination
        return false;
    }
    if (ev.State != KeyState.Down)
    {
        // suppress up/repeat of combination keys, pass through the rest
        return ev.Key == VirtualKeyCode.NUMPAD1 || ev.Key == VirtualKeyCode.NUMPAD2;
    }

    if (ev.Key == VirtualKeyCode.NUMPAD1)
    {
        firstKey = ev.Key;
        Console.WriteLine("first key of combination");
        return true;
    }
    if (ev.Key == VirtualKeyCode.NUMPAD2)
    {
        if (firstKey == VirtualKeyCode.NUMPAD1)
        {
            Console.WriteLine("combination 1+2 matched -> typing 'ok'");
            simulator.TextEntry("ok");
        }
        firstKey = null;
        return true;
    }

    firstKey = null;
    return false; // not a combination key: pass through
};

Console.WriteLine($"Grabbing {device} — press numpad '1' then numpad '2' to trigger, ESC to exit.");
hook.StartHook();
exitRequested.Wait();
hook.StopHook();
Console.WriteLine("Grab released, bye.");
