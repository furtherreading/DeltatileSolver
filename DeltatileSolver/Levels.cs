using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltatileSolver
{
    /// <summary>
    /// Class that contains some levels from the game.
    /// </summary>
    internal class Levels
    {

        /*
         * @ = Player Avatar
         * # = Unpassable Empty Space
         * (whitespace) = walkable space
         * $ = Unpassable Wall
         * nswe = winds, blowing north/south/west/east
         * ( = closed gate
         * ) = opened gate
         * b = breakable ice
         * B = breakable ice that has been stepped on
         * i = slidy ice
         * 1-9 : gate counter, opens gate for 1-9 moves
         * l = lava
         * f = finish
         * W = Water
         */

        public static List<String> levels = new List<String> { 

            "######," +
            "####f#," +
            "####$#," +
            "# $$$#," +
            "#   @#," +
            "######",

            "#########," +
            "#@ $   fw," +
            "#########",

            "#########," +
            "#### ii #," +
            "####i##i#," +
            "####i##i#," +
            "f### iii#," +
            "####i##i#," +
            "####i##i#," +
            "####@ii #," +
            "#########",

            "######," +
            "f  lW#," +
            "####l#," +
            "#### #," +
            "####@#," +
            "######",



        };

    }
}
