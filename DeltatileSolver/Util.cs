using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwapSolver
{
    internal class Util
    {
        /// <summary>
        /// Compresses the board to save memory.
        /// </summary>
        /// <param name="trial">input board</param>
        /// <returns> a compressed version of the board</returns>
        public static string compress(string trial)
        {
            int count;
            StringBuilder output = new StringBuilder();
            count = 0;

            for (int i = 0; i < trial.Length; i++)
            {
                if (trial[i] == '#') // count # appearances
                {
                    count++;
                }
                else
                {
                    if (count != 0)
                    {
                        //append the number of # instead of # itself
                        output.Append(count.ToString());
                        output.Append(trial[i]);
                        count = 0;
                    }
                    else // otherwise append whatever else is there
                    {
                        output.Append(trial[i]);
                        count = 0;
                    }
                }
            }
            return output.ToString();
        }
    }
}
