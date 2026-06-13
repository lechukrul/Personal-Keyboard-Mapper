using System;
using System.Threading;
using Personal_Keyboard_Mapper.Core.Config;
using Personal_Keyboard_Mapper.Core.Enums;
using Personal_Keyboard_Mapper.Linux;
using Personal_Keyboard_Mapper.Linux.Native;

string device     = args.Length > 0 ? args[0] : "/dev/input/event0";
string configPath = args.Length > 1 ? args[1] : "default.keysconfig";

var config      = MappingConfigLoader.Load(configPath);
using var uinput    = new UinputDevice();
using var hook      = new EvdevKeyboardHook(device, uinput);
var simulator       = new UinputInputSimulator(uinput);
var engine          = new MappingEngine(config, simulator);
var exitRequested   = new ManualResetEventSlim();

engine.OnExitRequested = () => exitRequested.Set();

hook.KeyHandler = ev =>
{
    // VirtualBox emituje syntetyczne NUMLOCK wokół każdego klawisza numpada —
    // ignoruj je żeby nie resetowały stanu engine.
    if (ev.Key == VirtualKeyCode.NUMLOCK)
        return false;

    return engine.HandleKey(ev);
};

Console.WriteLine($"Loaded {config.Combinations.Count} combinations from '{configPath}'.");
Console.WriteLine($"Grabbing {device} — press ESC to exit.");

hook.StartHook();
exitRequested.Wait();
hook.StopHook();
Console.WriteLine("Grab released, bye.");
