using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluedo
{
    interface IObservable
    {
        /// <summary>
        /// Description
        /// </summary>
        List<ConsoleString> Description { get; set; }

        /// <summary>
        /// Has the object already been observed ?
        /// </summary>
        bool Observed { get; set; }

        /// <summary>
        /// The extra-time of the observation
        /// </summary>
        int ExtraObserveTime { get; set; }

        /// <summary>
        /// Observation
        /// </summary>
        void Observe();
    }
}
