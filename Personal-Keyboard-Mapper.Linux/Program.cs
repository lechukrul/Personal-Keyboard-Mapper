using System;
using System.Threading;
using Personal_Keyboard_Mapper.Core.Config;
using Personal_Keyboard_Mapper.Core.Enums;
using Personal_Keyboard_Mapper.Linux;
using Personal_Keyboard_Mapper.Linux.Native;

string device     = args.Length > 0 ? args[0] : "/dev/input/event0";
string configPath = args.Length > 1 ? args[1] : "default.keysconfig";
bool soundEnabled = Array.IndexOf(args, "--no-sound") < 0;

var config   = MappingConfigLoader.Load(configPath);
using var uinput = new UinputDevice();
using var hook   = new EvdevKeyboardHook(device, uinput);
var simulator    = new UinputInputSimulator(uinput);

using var sound = soundEnabled
    ? new OpenAlSoundPlayer(AppContext.BaseDirectory)
    : (OpenAlSoundPlayer)null;

var engine        = new MappingEngine(config, simulator, sound);
var exitRequested = new ManualResetEventSlim();

engine.OnExitRequested = () => exitRequested.Set();

hook.KeyHandler = ev =>
{
    if (ev.Key == VirtualKeyCode.NUMLOCK)
        return false;
    return engine.HandleKey(ev);
};

Console.WriteLine($"Loaded {config.Combinations.Count} combinations from '{configPath}'.");
Console.WriteLine($"Grabbing {device} — press ESC to exit. Sound: {(soundEnabled ? "on" : "off")}");

hook.StartHook();
exitRequested.Wait();
hook.StopHook();
Console.WriteLine("Grab released, bye.");
