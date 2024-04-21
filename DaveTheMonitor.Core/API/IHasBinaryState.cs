using System.IO;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Represents an item with binary save state.
    /// </summary>
    public interface IHasBinaryState
    {
        /// <summary>
        /// Reads this item's state from binary data.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read from.</param>
        /// <param name="tmVersion">The version of Total Miner the data was saved in.</param>
        /// <param name="coreVersion">The versino of the Core Mod the data was saved in.</param>
        void ReadState(BinaryReader reader, int tmVersion, int coreVersion);

        /// <summary>
        /// Writes this item's state as binary data.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write to.</param>
        void WriteState(BinaryWriter writer);
    }
}
