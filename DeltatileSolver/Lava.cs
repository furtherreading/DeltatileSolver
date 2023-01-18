using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwapSolver
{

    /// <summary>
    /// Class that handles interaction with lava.
    /// </summary>
    internal class Lava
    {

        private int width;
        private int height;
        private String board;
        private int lavaCount;

        public Lava(int width, int height, string board, int lavaCount)
        {
            this.width = width;
            this.height = height;
            this.board = board;
            this.lavaCount = lavaCount;
        }

        /// <summary>
        /// Checks wheter the ball stepped on lava.
        /// If the ball has stepped on lava, it is on
        /// fire and has 3 moves to step into water, which gets consumed.
        /// </summary>
        /// <param name="newPlayerPos">new player position</param>
        /// <returns>a tuple consisting of the lavacount and the updated board.</returns>
        public (int, String) checkLava(int newPlayerPos)
        {
            //if the ball steps into water
            if (board[newPlayerPos] == 'W')
            {
                //if the ball is on fire
                if (lavaCount > 0)
                {
                    //consume water
                    char[] trial2 = board.ToCharArray();
                    trial2[newPlayerPos] = ' ';
                    board = new string(trial2);
                }
                //ball is no longer on fire
                lavaCount = 0;
                return (lavaCount, board);
            }

            //if the ball steps into lava
            if (board[newPlayerPos] == 'l')
            {
                //ball is on fire
                lavaCount++;
                return (lavaCount, board);
            }

            //update lavacount if ball is on fire and hasn't stepped into water
            if (lavaCount > 0)
            {
                lavaCount++;
            }

            return (lavaCount, board);
        }

    }
}
