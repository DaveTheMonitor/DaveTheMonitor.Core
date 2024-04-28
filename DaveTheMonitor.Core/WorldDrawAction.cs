using DaveTheMonitor.Core.API;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.Core
{
    /// <summary>
    /// A delegate that is called before or after a specific stage of map drawing.
    /// </summary>
    /// <param name="map">The map being drawn.</param>
    /// <param name="player">The player.</param>
    /// <param name="virtualPlayer">The virtual player.</param>
    /// <param name="options">The current world draw options.</param>
    /// <param name="stage">The current stage.</param>
    public delegate void WorldDrawAction(ITMMap map, ICorePlayer player, ITMPlayer virtualPlayer, WorldDrawOptions options, WorldDrawStage stage);
}
