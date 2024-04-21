using StudioForge.TotalMiner.API;
using System.IO;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Reads components as <see cref="ITMMap"/>s from streams.
    /// </summary>
    public interface IMapComponentLoader
    {
        /// <summary>
        /// Loads a TM component from a file.
        /// </summary>
        /// <param name="path">The full path of the file to load.</param>
        /// <returns>An <see cref="ITMMap"/> for the component.</returns>
        ITMMap LoadComponent(string path);

        /// <summary>
        /// Loads a TM component from a stream.
        /// </summary>
        /// <param name="stream">The stream to read the component from.</param>
        /// <returns>An <see cref="ITMMap"/> for the component.</returns>
        ITMMap LoadComponent(Stream stream);

        /// <summary>
        /// Loads a TM component from a <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read the component from.</param>
        /// <returns>An <see cref="ITMMap"/> for the component.</returns>
        ITMMap LoadComponent(BinaryReader reader);
    }
}
