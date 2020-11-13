using System;
using System.Configuration;
using System.IO;
using System.Media;
using WindowsInput;
using log4net;
using Personal_Keyboard_Mapper.Lib.Enums;

namespace Personal_Keyboard_Mapper.Lib
{
    /// <summary>
    /// Holds the streams to sounds files used as a notifications sounds
    /// </summary>
    public class KeysSoundEffects
    {
        private ILog logger;
        public KeysSoundEffects(ILog log, UnmanagedMemoryStream key1Sound, UnmanagedMemoryStream key2Sound, UnmanagedMemoryStream crtlSound, UnmanagedMemoryStream shiftSound, UnmanagedMemoryStream winSound, 
                                UnmanagedMemoryStream rightAltSound, UnmanagedMemoryStream leftAltSound = null)
        {
            logger = log;
            FirstKeySound = key1Sound;
            SecondKeySound = key2Sound;
            CrtlActionSound = crtlSound;
            ShiftActionSound = shiftSound;
            WinActionSound = winSound;
            AltActionSound = rightAltSound;
        }

        /// <summary>
        /// Gets or sets the first key sound.
        /// </summary>
        /// <value>
        /// The first key sound.
        /// </value>
        public UnmanagedMemoryStream FirstKeySound { get; set; } 

        /// <summary>
        /// Gets or sets the second key sound.
        /// </summary>
        /// <value>
        /// The second key sound.
        /// </value>
        public UnmanagedMemoryStream SecondKeySound { get; set; }

        /// <summary>
        /// Gets or sets the third key sound.
        /// </summary>
        /// <value>
        /// The third key sound.
        /// </value>
        public UnmanagedMemoryStream ThirdKeySound { get; set; }

        /// <summary>
        /// Gets or sets the CRTL action sound.
        /// </summary>
        /// <value>
        /// The CRTL action sound.
        /// </value>
        public UnmanagedMemoryStream CrtlActionSound { get; set; }

        /// <summary>
        /// Gets or sets the SHIFT action sound.
        /// </summary>
        /// <value>
        /// The SHIFT action sound.
        /// </value>
        public UnmanagedMemoryStream ShiftActionSound { get; set; }

        /// <summary>
        /// Gets or sets the ALT action sound.
        /// </summary>
        /// <value>
        /// The ALT action sound.
        /// </value>
        public UnmanagedMemoryStream AltActionSound { get; set; }

        /// <summary>
        /// Gets or sets the WINDOWS action sound.
        /// </summary>
        /// <value>
        /// The WINDOWS action sound.
        /// </value>
        public UnmanagedMemoryStream WinActionSound { get; set; }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <param name="sound">The sound.</param>
        public void PlaySound(SoundAction sound)
        {
            using (var soundPlayer = new SoundPlayer())
            {
                switch (sound)
                {
                    case SoundAction.FirstKey:
                        if (FirstKeySound == null)
                        {
                            logger.Warn($"{nameof(FirstKeySound)} is null");
                            break;
                        }
                        FirstKeySound.Position = 0;
                        soundPlayer.Stream = FirstKeySound;
                        soundPlayer.Play();
                        break;

                    case SoundAction.SecondKey:
                        if (SecondKeySound == null)
                        {
                            logger.Warn($"{nameof(SecondKeySound)} is null");
                            break;
                        }
                        SecondKeySound.Position = 0;
                        soundPlayer.Stream = SecondKeySound;
                        soundPlayer.Play();
                        break;

                    case SoundAction.ThirdKey:
                        if (ThirdKeySound == null)
                        {
                            logger.Warn($"{nameof(ThirdKeySound)} is null");
                            break;
                        }
                        ThirdKeySound.Position = 0;
                        soundPlayer.Stream = ThirdKeySound;
                        soundPlayer.Play();
                        break;

                    case SoundAction.Shift:
                        if (ShiftActionSound == null)
                        {
                            logger.Warn($"{nameof(ShiftActionSound)} is null");
                            break;
                        }
                        ShiftActionSound.Position = 0;
                        soundPlayer.Stream = ShiftActionSound;
                        soundPlayer.Play();
                        break;

                    case SoundAction.Crtl:
                        if (CrtlActionSound == null)
                        {
                            logger.Warn($"{nameof(CrtlActionSound)} is null");
                            break;
                        }
                        CrtlActionSound.Position = 0;
                        soundPlayer.Stream = CrtlActionSound;
                        soundPlayer.Play();
                        break;

                    case SoundAction.Alt:
                        if (AltActionSound == null)
                        {
                            logger.Warn($"{nameof(AltActionSound)} is null");
                            break;
                        }
                        AltActionSound.Position = 0;
                        soundPlayer.Stream = AltActionSound;
                        soundPlayer.Play();
                        break;

                    case SoundAction.Win:
                        if (WinActionSound == null)
                        {
                            logger.Warn($"{nameof(WinActionSound)} is null");
                            break;
                        }
                        WinActionSound.Position = 0;
                        soundPlayer.Stream = WinActionSound;
                        soundPlayer.Play();
                        break;
                }
            }
        }
    }
}