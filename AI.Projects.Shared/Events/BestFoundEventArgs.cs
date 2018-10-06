using System;
using AI.Projects.Shared.Models;

namespace AI.Projects.Shared.Events
{
    public class BestFoundEventArgs : EventArgs
    {
        public Trip NewBest { get; set; }

        public BestFoundEventArgs(Trip best)
        {
            NewBest = best;
        }
    }
}
