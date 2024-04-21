using System;

namespace DaveTheMonitor.Core.Assets
{
    /// <summary>
    /// An asset for a core mod.
    /// </summary>
    public abstract class CoreModAsset : IDisposable
    {
        /// <summary>
        /// The full file path of this asset.
        /// </summary>
        public string FullPath { get; private set; }
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _disposedValue = true;
            }
        }

        /// <summary>
        /// Disposes this asset if it is disposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected CoreModAsset(string fullPath)
        {
            FullPath = fullPath;
        }
    }
}
