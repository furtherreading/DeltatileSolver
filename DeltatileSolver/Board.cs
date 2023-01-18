using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwapSolver
{
    /// <summary>
    /// Class that handles board logic. Implements all the moves.
    /// </summary>
    internal class Board : IEquatable<Board?>
    {

        String sol;
        String board;
        public int width, height;
        public int playerPos;
        int[] swapOffset = new int[] { 0, 0 };
        int gateCount = -1;
        private int lavaCount = 0;

        public Board(Board board)
        {
            this.board = board.board;
            this.width = board.width;
            this.height = board.height;
            this.sol = board.sol;
            this.swapOffset[0] = board.swapOffset[0];
            this.swapOffset[1] = board.swapOffset[1]; //needs to be done like this because of referencing issues
            this.playerPos = board.playerPos;
            this.gateCount = board.gateCount;
            this.lavaCount = board.lavaCount;
        }

        /// <summary>
        /// Parse board from input string.
        /// </summary>
        /// <param name="board"> Board as string.</param>
        public Board(string[] board)
        {
            width = board[0].Length;
            height = board.Length;

            StringBuilder destBuf = new StringBuilder();

            for (int r = 0; r < board.Length; r++) //iterate through string, rows then columns
            {
                for (int c = 0; c < width; c++)
                {

                    char ch = board[r][c];

                    destBuf.Append(ch != '@' ? ch : ' '); //remove player from board, we store the position as variable

                    if (ch == '@')
                    {
                        playerPos = xy2abs(c, r);
                    }
                    if (Char.IsDigit(ch)) //parse block that lowers gate
                    {
                        gateCount = ch - '0'; //char to int
                    }
                }
            }
            this.board = destBuf.ToString();
            sol = "";

        }

        /// <summary>
        /// Append move to solution string.
        /// </summary>
        /// <param name="move"> The move to add. Must be between 0 and (width * length + 4)</param>
        public void addMove(int move)
        {
            if (move < 4) //if we are doing a normal move
            {
                char[] dirLabels = { 'u', 'r', 'd', 'l' };
                sol = sol + dirLabels[move];
            }
            else //if we are doing a swap
            {
                sol = sol + "(" + move.ToString() + ")";

            }

        }

        /// <summary>
        /// Clears the solution string.
        /// </summary>
        public void ClearMoves()
        {
            sol = "";
        }

        public String getMoves()
        {
            return sol;
        }

        public String getBoard()
        {
            return board;
        }

        /// <summary>
        /// Checks if a move is legal. Optionally updates the board
        /// with the move.
        /// </summary>
        /// <param name="move">the move to check</param>
        /// <param name="doMove">Wheter the move should actually be done and the board updated.</param>
        /// <returns>Returns true if the move is legal, false if it is illegal.</returns>
        public bool move(int move, bool doMove = false)
        {
            int x, y, dx, dy;
            dx = 0;
            dy = 0;

            //get x and y pos of player
            x = playerPos % width;
            y = playerPos / width;

            int newPlayerPos = playerPos;
            int swapPos = 0;

            bool doSwap = false;


            if (move < 4) //if we are doing a normal move
            {
                //set newPlayerPos,deltax and deltay accordingly
                if (move == 0)
                {
                    if (y == 0) return false;
                    newPlayerPos -= width;
                    dy = -1;
                }
                if (move == 1)
                {
                    if (x + 1 == width) return false;
                    newPlayerPos += 1;
                    dx = 1;
                }
                if (move == 2)
                {
                    if (y + 1 == height) return false;
                    newPlayerPos += width;
                    dy = 1;
                }
                if (move == 3)
                {
                    if (x == 0) return false;
                    newPlayerPos -= 1;
                    dx = -1;
                }
            }
            else // we are doing a swap
            {
                swapPos = move - 4;
                doSwap = true;
            }

            if (!doSwap)
            {
                String tempBoard = new String(board);

                //gates
                tempBoard = Gate.updateGates(tempBoard, gateCount);

                //walls
                if (tempBoard[newPlayerPos] == '#' || tempBoard[newPlayerPos] == '$')
                {
                    return false;
                }

                //wind
                if (tempBoard[newPlayerPos] == 'n' || tempBoard[newPlayerPos] == 'e' || tempBoard[newPlayerPos] == 's' || tempBoard[newPlayerPos] == 'w')
                {
                    return false;
                }

                //gate
                if (tempBoard[newPlayerPos] == '(')
                {
                    return false;
                }

                //gate button
                if (Char.IsDigit(tempBoard[newPlayerPos]))
                {
                    tempBoard = Gate.setGates(tempBoard, gateCount);
                }

                //wind
                if (tempBoard.IndexOfAny("nwse".ToCharArray()) != -1)
                {
                    Wind wind = new Wind(width, height, tempBoard);
                    if (!wind.moveWind(ref dx, ref dy, ref newPlayerPos))
                    {
                        return false;
                    }
                }

                //ice slide
                if (tempBoard.IndexOfAny("i".ToCharArray()) != -1)
                {
                    Ice ice = new Ice(width, height, tempBoard);
                    if (!ice.moveSlide(x, y, ref dx, ref dy, ref newPlayerPos))
                    {
                        return false;
                    }
                }

                //ice break
                if (tempBoard.IndexOfAny("bB".ToCharArray()) != -1)
                {
                    Ice ice = new Ice(width, height, tempBoard);
                    tempBoard = ice.checkIce(newPlayerPos);
                }

                //lava
                int templava = 0;
                if (tempBoard.IndexOfAny("l".ToCharArray()) != -1)
                {
                    Lava lava = new Lava(width, height, tempBoard, lavaCount);
                    (templava, tempBoard) = lava.checkLava(newPlayerPos);
                    // if ball is on fire for more than three moves
                    if (templava > 3)
                    {
                        return false;
                    }
                }

                //if we change the board as well we need to update variables
                if (doMove)
                {
                    board = tempBoard;
                    playerPos = newPlayerPos;
                    swapOffset[0] += dx;
                    swapOffset[1] += dy;
                    lavaCount = templava;
                }

                return true;

            }
            else //swap logic
            {

                int swapTarget = swapPos;

                //no swaps outside the board allowed
                if (swapOffset[0] + swapPos % width >= width || swapOffset[0] + swapPos % width < 0) return false;
                if (swapOffset[1] + swapPos / width >= height || swapOffset[1] + swapPos / width < 0) return false;

                //correctly set the target of the swap
                swapTarget += swapOffset[0];
                swapTarget += swapOffset[1] * width;

                char placeholder = ' ';
                int finishPos = board.IndexOf('f');

                //swapping the player or the finish is not allowed
                if (swapPos == playerPos || swapTarget == playerPos || swapPos == finishPos || swapTarget == finishPos) return false;

                //swap the 2 blocks
                char[] trial2 = board.ToCharArray();
                placeholder = trial2[swapPos];
                trial2[swapPos] = trial2[swapTarget];
                trial2[swapTarget] = placeholder;

                //the swap must change something
                if (board.Equals(new String(trial2))) return false;

                //if the board cointains wind
                if (board.IndexOfAny("nwse".ToCharArray()) != -1)
                {
                    //check if wind moves the player
                    Wind wind = new Wind(width, height, new String(trial2));
                    if (!wind.moveWind(ref dx, ref dy, ref newPlayerPos))
                    {
                        return false;
                    }
                }

                //update board variables
                if (doMove)
                {
                    swapOffset[0] = 0;
                    swapOffset[1] = 0;
                    board = new String(trial2);

                    //if wind swap changes player pos
                    if (newPlayerPos != playerPos)
                    {
                        playerPos = newPlayerPos;
                        swapOffset[0] += dx;
                        swapOffset[1] += dy;
                    }
                }
                return true;

            }
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
        /// Check if a board is solved.
        /// A board is solved when the player reaches the finish block.
        /// </summary>
        /// <returns>Is the board solved?</returns>
        public bool isSolved()
        {

            if (board[playerPos] == 'f')
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Helper function to print the board to the console
        /// </summary>
        public void printBoard()
        {
            Console.WriteLine("\nBoard:");
            for (int i = 0; i < board.Length; i++)
            {
                if (i == playerPos)
                {
                    Console.Write('@');
                }
                else
                {
                    Console.Write(board[i]);
                }
                if (i % width == width - 1)
                {
                    Console.Write("\n");
                }
            }
            Console.WriteLine("\n");
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Board);
        }

        public bool Equals(Board? other)
        {
            return other != null &&
                   board == other.board &&
                   playerPos == other.playerPos &&
                   swapOffset[0] == other.swapOffset[0] &&
                   swapOffset[1] == other.swapOffset[1] &&
                   lavaCount == other.lavaCount;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(board, playerPos, swapOffset[0], swapOffset[1], lavaCount);
        }

        /// <summary>
        /// Custom hashing to save memory.
        /// This hashing function generates unique hashes for every possible unique position.
        /// </summary>
        /// <returns> The board, hashed and compressed.</returns>
        public string customHash()
        {
            string hash1 = Util.compress(board);
            string hash2 = playerPos.ToString();
            string hash3 = swapOffset[0].ToString() + "," + swapOffset[1].ToString();
            string hash4 = lavaCount.ToString();//lava

            return hash1 + "!" + hash2 + "!" + hash3 + "!" + hash4;
        }



    }
}
