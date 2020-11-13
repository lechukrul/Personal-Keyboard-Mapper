using System;
using Personal_Keyboard_Mapper.Lib.Structures;

namespace Personal_Keyboard_Mapper.Lib.Interfaces
{
    public interface IHook
    { 

        /// <summary>
        /// Starts the hook.
        /// </summary>
        void StartHook();

        /// <summary>
        /// Stops the hook.
        /// </summary>
        void StopHook();
    }
}