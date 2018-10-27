using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI.Projects.Shared.Models;

namespace AI.Projects.Shared.Events
{
    public class TripAddedEventArgs
    {
        public Trip NewTrip { get; set; }

        public TripAddedEventArgs(Trip newTrip)
        {
            NewTrip = newTrip;
        }
    }
}
