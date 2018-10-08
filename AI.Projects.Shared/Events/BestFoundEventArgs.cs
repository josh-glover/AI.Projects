using System;
using AI.Projects.Shared.Models;

namespace AI.Projects.Shared.Events
{
    public class BestFoundEventArgs : EventArgs
    {
        /// <summary>
        /// A property that stores the generation this path belongs to
        /// </summary>
        public int Generation { get; set; }
        /// <summary>
        /// A property that stores the new best path
        /// </summary>
        public Trip NewBest { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="generation">The generation of the path</param>
        /// <param name="best">The new best trip</param>
        public BestFoundEventArgs(int generation, Trip best)
        {
            Generation = generation;
            NewBest = best;
        }
    }
}
