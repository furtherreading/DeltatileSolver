using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwapSolver
{

    /// <summary>
    /// Class that handles breaking ice and sliding ice.
    /// </summary>
    internal class Ice
    {

        private int width;
        private int height;
        private String board;

        public Ice(int width, int height, string board)
        {
            this.width = width;
            this.height = height;
            this.board = board;
        }

        /// <summary>
        /// Function that checks wheter the player is sliding on ice.
        /// Adjusts position accordingly.
        /// </summary>
        /// <param name="x">x position of player, as reference</param>
        /// <param name="y">y position of player, as reference</param>
        /// <param name="dx">delta x of player, as reference</param>
        /// <param name="dy">delta y of player, as reference</param>
        /// <param name="newPlayerPos">absolute new player position, as reference</param>
        /// <returns>Returns false if the ice causes the player to go out of bounds.</returns>
        public bool moveSlide(int x, int y, ref int dx, ref int dy, ref int newPlayerPos)
        {

            int newdx = dx;
            int newdy = dy;
            //while the next block is an ice block and not out of bounds
            while (isInBound(x + newdx, y + newdy) && board[xy2abs(x + newdx, y + newdy)] == 'i')
            {
                //update deltas
                newdx += dx;
                newdy += dy;

                //if the new deltas cause the player to slide out of bounds return false
                if (!isInBound(x + newdx, y + newdy)) return false;

                //if the next block is a wall or gate, stop
                if ("($".IndexOf(board[xy2abs(x + newdx, y + newdy)]) != -1)
                {
                    newdx -= dx;
                    newdy -= dy;
                    break;
                }
            }

            //final position must not be wind or empty slot
            if ("#nswe".IndexOf(board[xy2abs(x + newdx, y + newdy)]) != -1)
            {
                return false;
            }

            //update pointers
            dx = newdx;
            dy = newdy;
            newPlayerPos = x + newdx + (y + newdy) * width;

            return true;

        }

        /// <summary>
        /// Function that handles breakable ice.
        /// Once the player steps on breakable ice,
        /// it will break on the next move.
        /// </summary>
        /// <param name="newPlayerPos">new position of player</param>
        /// <returns>Updated board</returns>
        public string checkIce(int newPlayerPos)
        {
            //if there exists breakable ice
            if (board.Contains('B'))
            {
                if (board[newPlayerPos] == 'B')
                {
                    //edge case for swapping
                    return board;
                }
                else
                {
                    char[] trial2 = board.ToCharArray();
                    //check if there is any ice to break
                    for (int i = 0; i < trial2.Length; i++)
                    {
                        if (trial2[i] == 'B')
                        {
                            //replace the ice with empty space
                            trial2[i] = '#';
                        }
                    }
                    board = new string(trial2);
                }
            }
            //if player steps on breakable ice
            if (board[newPlayerPos] == 'b')
            {
                //ensure it breaks on the next move
                char[] trial2 = board.ToCharArray();
                trial2[newPlayerPos] = 'B';
                board = new string(trial2);
                return board;
            }
            return board;
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

        /// <summary>
        /// Helper function to check if a given xy coordinate is
        /// inside the bounds of the board.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns>Returns true if the coordinates are inside the bounds, false otherwise.</returns>
        private bool isInBound(int x, int y)
        {
            if (x < 0 || x > width - 1) return false;
            if (y < 0 || y > height - 1) return false;

            return true;
        }

    }
}
