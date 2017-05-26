using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluedo
{
    interface ICollectible
    {
        /// <summary>
        /// Collection
        /// </summary>
        void Collect();

        /// <summary>
        /// Is the object collectible ?
        /// </summary>
        /// <returns>The boolean associated</returns>
        bool IsCollectible();
    }
}
