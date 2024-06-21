using DaveTheMonitor.Core.API;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.IO;
using DaveTheMonitor.Core.Plugin;


#if SHARPDX
using SharpDX;
using SharpDX.MediaFoundation;
#endif

namespace DaveTheMonitor.Core.Assets.Loaders
{
    internal sealed class CoreSoundAssetLoader : ICoreAssetLoader
    {
        public CoreModAsset Load(string path, string name, ICoreMod mod)
        {
#if SHARPDX
            // SoundEffect.FromFile only supports .wav, so we use
            // the AudioDecoder for other file types (eg. mp3)
            // Unfortunately, the AudioDecoder doesn't support ogg
            // Look into a way to load OGG files in the future.
            if (Path.GetExtension(path) == ".wav")
            {
                return new CoreSoundAsset(path, name, SoundEffect.FromFile(path));
            }

            using AudioDecoder decoder = new AudioDecoder(File.OpenRead(path));
            List<byte[]> samples = new List<byte[]>();
            int totalBytes = 0;
            foreach (DataPointer sample in decoder.GetSamples())
            {
                samples.Add(sample.ToArray());
                totalBytes += sample.Size;
            }

            byte[] buffer = new byte[totalBytes];
            int i = 0;
            foreach (byte[] sample in samples)
            {
                sample.CopyTo(buffer, i);
                i += sample.Length;
            }

            SoundEffect sound = new SoundEffect(buffer, decoder.WaveFormat.SampleRate, (AudioChannels)decoder.WaveFormat.Channels);
            return new CoreSoundAsset(path, name, sound);
#endif
        }
    }
}
