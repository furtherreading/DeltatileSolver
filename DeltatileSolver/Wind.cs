using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwapSolver
{

    /// <summary>
    /// class that handles the logic for wind blocks.
    /// </summary>
    internal class Wind
    {

        private int width;
        private int height;
        private String board;

        public Wind(int width, int height, string board)
        {
            this.width = width;
            this.height = height;
            this.board = board;
        }

        /// <summary>
        /// Top level function that handles player offset caused by wind
        /// </summary>
        /// <param name="dx"> delta x of the player, as reference</param>
        /// <param name="dy"> delta y of the player, as reference</param>
        /// <param name="newPlayerPos"> absolute new position of the player, as reference</param>
        /// <returns>Returns false if wind would blow the player of the map</returns>
        public bool moveWind(ref int dx, ref int dy, ref int newPlayerPos)
        {
            //generate all winds
            List<WindStruct> windlist = makeWindList();

            if (windlist == null) return false;

            //if the player is inside a wind channel
            if (isinWind(windlist, newPlayerPos))
            {
                WindStruct windStruct = new WindStruct();
                int x = newPlayerPos % width;
                int y = newPlayerPos / width;
                foreach (WindStruct current in windlist)
                {

                    //find the wind channel that the player is inside of
                    if (current.dx != 0)
                    {
                        if (y == current.y)
                        {
                            if (x >= Math.Min(current.x, (current.x + current.dx)) && x <= Math.Max(current.x, (current.x + current.dx))) { windStruct = current; break; }
                        }
                    }
                    if (current.dy != 0)
                    {
                        if (x == current.x)
                        {
                            if (y >= Math.Min(current.y, (current.y + current.dy)) && y <= Math.Max(current.y, (current.y + current.dy))) { windStruct = current; break; }
                        }
                    }
                }



                //update player position
                newPlayerPos = windStruct.x + windStruct.dx + (windStruct.y + windStruct.dy) * width;
                int dx1 = (windStruct.x + windStruct.dx) - x;
                int dy1 = (windStruct.y + windStruct.dy) - y;

                //step through the wind and check each block
                if (dx1 != 0)
                {
                    for (int i = 0; dx1 < 0 ? i >= dx1 : i <= dx1; i += Math.Sign(dx1))
                    {
                        if (x + i >= 0 && x + i <= width - 1)
                        {
                            //return false if we are blown off the map
                            if (board[xy2abs(x + i, y)] == '#')
                            {
                                return false;
                            }

                            //stop the player at the finish pos so that the board is recognized as solved
                            if (board[xy2abs(x + i, y)] == 'f')
                            {
                                dx1 = i;
                                dx += dx1;
                                dy += dy1;
                                newPlayerPos = (x + dx1) + (y + dy1) * width;
                                return true;
                            }
                        }

                    }
                }

                //same as above, but in the y axis
                if (dy1 != 0)
                {
                    for (int i = 0; dy1 < 0 ? i >= dy1 : i <= dy1; i += Math.Sign(dy1))
                    {
                        if (y + i >= 0 && y + i <= height - 1)
                        {

                            if (board[xy2abs(x, y + i)] == '#')
                            {
                                return false;
                            }

                            if (board[xy2abs(x, y + i)] == 'f')
                            {
                                dy1 = i;
                                dx += dx1;
                                dy += dy1;
                                newPlayerPos = (x + dx1) + (y + dy1) * width;
                                return true;
                            }
                        }

                    }
                }

                //update deltas
                dx += dx1;
                dy += dy1;

                //edge case where the wind is always on solid ground, but reaches the end of the map
                if (windStruct.x + windStruct.dx < 0 || windStruct.x + windStruct.dx > width - 1) return false;
                if (windStruct.y + windStruct.dy < 0 || windStruct.y + windStruct.dy > height - 1) return false;

                return true;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Helper function that recursively generates all wind channels.
        /// Used for moveWind().
        /// </summary>
        /// <returns>A list of wind channels</returns>
        private List<WindStruct> makeWindList()
        {
            int windPos = -1;
            int winddx, winddy;
            winddx = 0;
            winddy = 0;

            // find absolute position of wind start point
            if (board.IndexOf('n') != -1)
            {
                windPos = board.IndexOf('n');
                winddy = -1;
            }
            if (board.IndexOf('e') != -1)
            {
                windPos = board.IndexOf('e');
                winddx = 1;
            }
            if (board.IndexOf('s') != -1)
            {
                windPos = board.IndexOf('s');
                winddy = 1;
            }
            if (board.IndexOf('w') != -1)
            {
                windPos = board.IndexOf('w');
                winddx = -1;
            }

            // give up if there is no wind
            if (windPos == -1) 
            {
                return null;
            }

            //setup start of the recursion
            WindStruct startWind = new WindStruct();

            startWind.x = windPos % width;
            startWind.y = windPos / width;
            startWind.dx = winddx;
            startWind.dy = winddy;

            Queue<WindStruct> windQueue = new Queue<WindStruct>();
            windQueue.Enqueue(startWind);
            List<WindStruct> windlist = new List<WindStruct>();

            //recursively go through the wind channels
            while (windQueue.Count > 0) 
            {
                WindStruct wind = windQueue.Dequeue();

                //get the winds that split off of this wind
                WindStruct returnWind = Windworker(wind.x, wind.y, wind.dx, wind.dy, windlist);
                windlist.Add(returnWind);
                //go through new winds
                foreach (int current in returnWind.newWinds)
                {
                    //set parameters of new wind
                    WindStruct toAdd = new WindStruct();
                    toAdd.x = returnWind.x + returnWind.dx;
                    toAdd.y = returnWind.y + returnWind.dy;

                    if (current == 0)
                    {
                        toAdd.dx = 0;
                        toAdd.dy = -1;
                    }
                    if (current == 1)
                    {
                        toAdd.dx = 1;
                        toAdd.dy = 0;
                    }
                    if (current == 2)
                    {
                        toAdd.dx = 0;
                        toAdd.dy = 1;
                    }
                    if (current == 3)
                    {
                        toAdd.dx = -1;
                        toAdd.dy = 0;
                    }
                    windQueue.Enqueue(toAdd);
                }
            }
            return windlist;
        }

        /// <summary>
        /// Checks if a given absolute position is inside wind.
        /// </summary>
        /// <param name="windlist">Generated list of Winds</param>
        /// <param name="toCheck"> the absolute position to check</param>
        /// <returns>Returns true if the position is inside a wind</returns>
        private bool isinWind(List<WindStruct> windlist, int toCheck)
        {
            //convert absolute to xy
            int x = toCheck % width;
            int y = toCheck / width;

            //go through all winds
            foreach (WindStruct current in windlist)
            {
                if (current.dx != 0)
                {
                    if (y == current.y)
                    {
                        // check if the block is along the x start and end position of the wind
                        if (x >= Math.Min(current.x, (current.x + current.dx)) && x <= Math.Max(current.x, (current.x + current.dx))) return true;
                    }
                }
                if (current.dy != 0)
                {
                    if (x == current.x)
                    {
                        // check if the block is along the y start and end position of the wind
                        if (y >= Math.Min(current.y, (current.y + current.dy)) && y <= Math.Max(current.y, (current.y + current.dy))) return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Helper function for makeWindList(). Generates a single wind.
        /// Once it collides with something, it also checks wheter new winds
        /// should be generated.
        /// </summary>
        /// <param name="startx"> start position x of the wind</param>
        /// <param name="starty">start position y of the wind</param>
        /// <param name="startdx">start delta x of the wind</param>
        /// <param name="startdy">start delta y of the wind</param>
        /// <param name="windList">list of winds, by design incomplete when handed over to the algorithm</param>
        /// <returns></returns>
        private WindStruct Windworker(int startx, int starty, int startdx, int startdy, List<WindStruct> windList)
        {

            int x, y, dx, dy;

            x = startx;
            y = starty;
            dx = startdx;
            dy = startdy;

            WindStruct returnme = new WindStruct();
            returnme.x = x;
            returnme.y = y;
            returnme.dx = dx;
            returnme.dy = dy;
            returnme.newWinds = new List<int>();

            //main loop, stepping through each block and checking how to proceed.
            while (true)
            {
                //stop and return if we are out of bounds.
                if (x + returnme.dx < 0 || x + returnme.dx > width - 1) return returnme;
                if (y + returnme.dy < 0 || y + returnme.dy > height - 1) return returnme;

                //increase deltas if nothing is in our way
                if (board[xy2abs(x + returnme.dx, y + returnme.dy)] != '$' && board[xy2abs(x + returnme.dx, y + returnme.dy)] != '(')
                {
                    returnme.dx += dx;
                    returnme.dy += dy;
                    continue;
                }

                //we have hit a wall
                if (board[xy2abs(x + returnme.dx, y + returnme.dy)] == '$' || board[xy2abs(x + returnme.dx, y + returnme.dy)] == '(')
                {

                    //if end point is in wind
                    if (isinWind(windList, (xy2abs(x + returnme.dx - startdx, y + returnme.dy - startdy))))
                    {
                        returnme.dx -= dx;
                        returnme.dy -= dy;
                        return returnme;
                    }

                    if (dx != 0)
                    {
                        //up down check
                        returnme.dx -= dx;
                        returnme.dy = 1;

                        //if we are not out of bounds 
                        if (!(y + returnme.dy > height - 1))
                        {
                            if (board[xy2abs(x + returnme.dx, y + returnme.dy)] != '$' && board[xy2abs(x + returnme.dx, y + returnme.dy)] != '(')
                            {
                                //spawn a new wind northwards
                                returnme.newWinds.Add(2);
                            }
                        }
                        returnme.dy = -1;
                        if (!(y + returnme.dy < 0))
                        {
                            if (board[xy2abs(x + returnme.dx, y + returnme.dy)] != '$' && board[xy2abs(x + returnme.dx, y + returnme.dy)] != '(')
                            {
                                //spawn a new wind southwards
                                returnme.newWinds.Add(0);
                            }
                        }
                        returnme.dy = 0;
                        return returnme;
                    }

                    if (dy != 0)
                    {
                        //right left check
                        returnme.dy -= dy;
                        returnme.dx = 1;

                        //if we are not out of bounds 
                        if (!(x + returnme.dx > width - 1))
                        {
                            if (board[xy2abs(x + returnme.dx, y + returnme.dy)] != '$' && board[xy2abs(x + returnme.dx, y + returnme.dy)] != '(')
                            {
                                //spawn a new wind eastwards
                                returnme.newWinds.Add(1);
                            }
                        }
                        returnme.dx = -1;
                        if (!(x + returnme.dx < 0))
                        {
                            if (board[xy2abs(x + returnme.dx, y + returnme.dy)] != '$' && board[xy2abs(x + returnme.dx, y + returnme.dy)] != '(')
                            {
                                //spawn a new wind westwards
                                returnme.newWinds.Add(3);
                            }
                        }
                        returnme.dx = 0;
                        return returnme;
                    }

                }

                returnme.dy += dy;
                returnme.dx += dx;
            }



        }

        /// <summary>
        /// Struct that holds information about a wind channel
        /// </summary>
        private struct WindStruct
        {
            public int x { get; set; }
            public int y { get; set; }
            public int dx { get; set; }
            public int dy { get; set; }
            public List<int> newWinds { get; set; }
        }

        /// <summary>
        /// Helper function to convert xy position to absolute.
        /// 
        /// </summary>
        /// <param name="x">the x coordinate (0 is to the left)</param>
        /// <param name="y">the y coordinate (0 is at the top)</param>
        /// <returns> the absolute position of the xy position</returns>
        private int xy2abs(int x, int y)
        {
            return x + y * width;
        }

    }
}
