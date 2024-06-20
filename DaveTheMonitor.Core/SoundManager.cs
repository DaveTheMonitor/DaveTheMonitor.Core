using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DaveTheMonitor.Core
{
    internal sealed class SoundManager : IDisposable
    {
        private List<SoundEffectInstance> _sounds;
        private bool _disposedValue;

        public SoundEffectInstance PlaySound(SoundEffect sound, AudioListener listener, AudioEmitter emitter, float volume, float pitch)
        {
            SoundEffectInstance instance = sound.CreateInstance();
            instance.Volume = volume;
            instance.Pitch = pitch;
            instance.Pan = 0;
            instance.Apply3D(listener, emitter);
            Play(instance);
            return instance;
        }

        public SoundEffectInstance PlaySound(SoundEffect sound, float volume, float pitch, float pan)
        {
            SoundEffectInstance instance = sound.CreateInstance();
            instance.Volume = volume;
            instance.Pitch = pitch;
            instance.Pan = pan;
            Play(instance);
            return instance;
        }

        private void Play(SoundEffectInstance sound)
        {
            sound.Play();
            lock (_sounds)
            {
                _sounds.Add(sound);
            }
        }

        public void Update()
        {
            lock (_sounds)
            {
                for (int i = _sounds.Count - 1; i >= 0; i--)
                {
                    SoundEffectInstance instance = _sounds[i];
                    if (instance.IsDisposed)
                    {
                        _sounds.RemoveAt(i);
                        continue;
                    }

                    if (!instance.IsLooped && instance.State == SoundState.Stopped)
                    {
                        Debug.WriteLine("disposed sound");
                        instance.Dispose();
                        _sounds.RemoveAt(i);
                        continue;
                    }
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    lock (_sounds)
                    {
                        foreach (SoundEffectInstance sound in _sounds)
                        {
                            sound.Dispose();
                        }
                    }
                }

                lock (_sounds)
                {
                    _sounds = null;
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public SoundManager()
        {
            _sounds = new List<SoundEffectInstance>();
        }
    }
}
