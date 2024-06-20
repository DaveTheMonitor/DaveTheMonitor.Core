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

        /// <summary>
        /// The name of this asset.
        /// </summary>
        public string Name { get; private set; }
        private bool _disposedValue;

        /// <inheritdoc/>
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

        /// <summary>
        /// Creates a new <see cref="CoreModAsset"/>.
        /// </summary>
        /// <param name="fullPath">The full path of this asset.</param>
        /// <param name="name">The name of this asset. This is different per asset, not per type.</param>
        protected CoreModAsset(string fullPath, string name)
        {
            FullPath = fullPath;
            Name = name;
        }
    }
}
