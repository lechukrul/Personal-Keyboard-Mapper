using Personal_Keyboard_Mapper.Core.Enums;

namespace Personal_Keyboard_Mapper.Core.Model
{
    /// <summary>
    /// A single key press/release seen by the global hook,
    /// expressed in platform-neutral codes.
    /// </summary>
    public readonly struct KeyEvent
    {
        public KeyEvent(VirtualKeyCode key, KeyState state)
        {
            Key = key;
            State = state;
        }

        public VirtualKeyCode Key { get; }
        public KeyState State { get; }

        public override string ToString() => $"{Key} {State}";
    }
}
