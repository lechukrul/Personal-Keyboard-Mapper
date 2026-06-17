using System;
using System.IO;
using Personal_Keyboard_Mapper.Core.Enums;
using Personal_Keyboard_Mapper.Core.Interfaces;
using Silk.NET.OpenAL;

namespace Personal_Keyboard_Mapper.Linux
{
    public sealed unsafe class OpenAlSoundPlayer : ISoundPlayer, IDisposable
    {
        private static readonly string[] FileNames =
            { "key1.wav", "key2.wav", "shift.wav", "ctrl.wav", "alt.wav", "win.wav" };

        private readonly AL _al;
        private readonly ALContext _alc;
        private readonly uint[] _buffers;
        private readonly uint[] _sources;
        private Device* _device;
        private Context* _context;
        private volatile bool _disposed;

        public OpenAlSoundPlayer(string resourceDir)
        {
            _al  = AL.GetApi();
            _alc = ALContext.GetApi();

            _device = _alc.OpenDevice(null);
            if (_device == null)
                throw new InvalidOperationException(
                    "OpenAL: could not open audio device. Run: sudo apt install libopenal1");

            _context = _alc.CreateContext(_device, null);
            _alc.MakeContextCurrent(_context);

            _buffers = _al.GenBuffers(FileNames.Length);
            _sources = _al.GenSources(FileNames.Length);

            try
            {
                for (int i = 0; i < FileNames.Length; i++)
                {
                    var path = Path.Combine(resourceDir, FileNames[i]);
                    var (pcm, channels, sampleRate, bits) = ParseWav(path);
                    var format = GetAlFormat(channels, bits);
                    _al.BufferData(_buffers[i], format, pcm, sampleRate);
                    _al.SetSourceProperty(_sources[i], SourceInteger.Buffer, _buffers[i]);
                }
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void PlaySound(SoundEvent sound)
        {
            if (_disposed) return;
            int i = (int)sound;
            if (i < 0 || i >= _sources.Length) return;
            _al.GetSourceProperty(_sources[i], GetSourceInteger.SourceState, out int state);
            if ((SourceState)state == SourceState.Playing)
                _al.SourceStop(_sources[i]);
            _al.SourcePlay(_sources[i]);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _al.DeleteSources(_sources);
            _al.DeleteBuffers(_buffers);
            _alc.DestroyContext(_context);
            _alc.CloseDevice(_device);
            _al.Dispose();
            _alc.Dispose();
        }

        private static (byte[] pcmData, int channels, int sampleRate, int bitsPerSample) ParseWav(string path)
        {
            using var fs = File.OpenRead(path);
            using var br = new BinaryReader(fs);

            if (new string(br.ReadChars(4)) != "RIFF")
                throw new InvalidDataException($"Not a RIFF file: {path}");
            br.ReadInt32();
            if (new string(br.ReadChars(4)) != "WAVE")
                throw new InvalidDataException($"Not a WAVE file: {path}");

            int channels = 0, sampleRate = 0, bitsPerSample = 0;
            byte[] pcmData = null;

            while (fs.Position <= fs.Length - 8)
            {
                var id   = new string(br.ReadChars(4));
                var size = (int)br.ReadUInt32();
                switch (id)
                {
                    case "fmt ":
                        br.ReadInt16();                         // audio format (PCM = 1)
                        channels      = br.ReadInt16();
                        sampleRate    = br.ReadInt32();
                        br.ReadInt32();                         // byte rate
                        br.ReadInt16();                         // block align
                        bitsPerSample = br.ReadInt16();
                        if (size > 16) br.ReadBytes(size - 16); // skip extension bytes
                        break;
                    case "data":
                        pcmData = br.ReadBytes(size);
                        break;
                    default:
                        br.ReadBytes(size);
                        break;
                }
            }

            if (pcmData == null)
                throw new InvalidDataException($"Missing data chunk: {path}");
            return (pcmData, channels, sampleRate, bitsPerSample);
        }

        private static BufferFormat GetAlFormat(int channels, int bitsPerSample) =>
            (channels, bitsPerSample) switch
            {
                (1, 8)  => BufferFormat.Mono8,
                (1, 16) => BufferFormat.Mono16,
                (2, 8)  => BufferFormat.Stereo8,
                (2, 16) => BufferFormat.Stereo16,
                _       => throw new InvalidDataException(
                               $"Unsupported WAV format: {channels} ch {bitsPerSample}-bit")
            };
    }
}
