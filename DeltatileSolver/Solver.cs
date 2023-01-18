using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SwapSolver
{
    /// <summary>
    /// The solver class. Uses a board as input and tries to find a solution.
    /// </summary>
    internal class Solver
    {

        Board board;

        public Solver(Board board)
        {
            this.board = board;
        }

        /// <summary>
        /// The basic solver. Finds the solution with the least moves.
        /// Uses Breadth first search and a hashmap for quick lookup.
        /// Prints some usefull information on the console while solving.
        /// </summary>
        /// <returns>Either "no solution" or the solution string.</returns>
        public String Solve()
        {
            Queue<Board> queue = new Queue<Board>();
            HashSet<string> history = new HashSet<string>();
            history.Add(board.customHash());

            queue.Enqueue(new Board(board)); // init queue with starting board

            int counter = 0;

            while (queue.Count > 0) // while there are still unprocesed boards keep searching
            {
                Board currBoard = queue.Dequeue();

                for (int i = 0; i < board.width * board.height + 4; i++) //go through all moves and swaps
                {

                    if (Move(currBoard, i)) //legal move found
                    {
                        Board currBoard2 = new Board(currBoard); // create a copy, otherwise we get in trouble with referencing
                        currBoard2.addMove(i);
                        Move(currBoard2, i, true);
                        counter++;

                        if (counter % 10000 == 0)
                        {
                            //some information for the command line
                            int queuecount = Regex.Replace(queue.First().getMoves(), @"[\d(]", string.Empty).Count();
                            Console.WriteLine("n = " + counter + ". queue = " + queue.Count + " depth: " + queuecount);
                        }
                        if (!history.Contains(currBoard2.customHash())) // have we never found this board at some point?
                        {
                            history.Add(currBoard2.customHash()); // add it to the history hashmap

                            if (isSolved(currBoard2))
                            {
                                Console.WriteLine("Size of hashset: " + history.Count());
                                Console.WriteLine("Legal Moves tried: " + counter);
                                Console.WriteLine("Length: " + currBoard2.getMoves().Length);
                                return currBoard2.getMoves();
                            }
                            else
                            {
                                queue.Enqueue(new Board(currBoard2)); // add copied board with the new move to the queue
                            }
                        }
                    }
                }
            }
            return "No solution";
        }

        /// <summary>
        /// A Solver that finds multiple solutions, starting from solutions with least amount of moves.
        /// Prints some usefull information on the console while solving.
        /// </summary>
        /// <returns>A list of solutions, that could be empty if no solution is found.</returns>
        public List<String> MultiSolve(int n)
        {
            //similar to Solve(), see comments there for explanation
            List<String> sols = new List<string>();

            Queue<Board> queue = new Queue<Board>();
            HashSet<Board> history = new HashSet<Board>();
            history.Add(new Board(board));

            queue.Enqueue(new Board(board));

            int counter = 0;

            while (queue.Count > 0)
            {
                Board currBoard = queue.Dequeue();

                for (int i = 0; i < board.width * board.height + 4; i++)
                {
                    if (Move(currBoard, i))
                    {
                        Board currBoard2 = new Board(currBoard);
                        currBoard2.addMove(i);
                        Move(currBoard2, i, true);
                        counter++;

                        if (counter % 10000 == 0)
                        {
                            int queuecount = Regex.Replace(queue.First().getMoves(), @"[\d(]", string.Empty).Count();
                            Console.WriteLine("n = " + counter + ". queue = " + queue.Count + " depth: " + queuecount);
                        }
                        if (!history.Contains(currBoard2))
                        {
                            if (isSolved(currBoard2))
                            {
                                // do not return here, add the solution to the list and keep looking
                                sols.Add(currBoard2.getMoves());
                                if (n == sols.Count) return sols;
                            }
                            else
                            {
                                history.Add(currBoard2);
                                queue.Enqueue(new Board(currBoard2));
                            }
                        }
                    }
                }
            }
            return sols;
        }

        /// <summary>
        /// A Solver that finds a solution with minimal amount of swaps.
        /// Uses DFS instead of BFS, but is usually significantly faster
        /// than the normal solver, since this solver reduces the search
        /// tree more aggresively.
        /// </summary>
        /// <param name="n"> max amount of swaps to look for.</param>
        /// <returns>Either "no solution" or the solution string.</returns>
        public String SolveSwap(int n = 10)
        {
            //similar to Solve(), see comments there for explanation

            for (int i2 = 0; i2 < n; i2++) // do a DFS search up to the specified amount of swaps
            {
                Queue<Board> queue = new Queue<Board>();
                HashSet<string> history = new HashSet<string>();
                history.Add(board.customHash());

                queue.Enqueue(new Board(board));

                int counter = 0;

                while (queue.Count > 0)
                {
                    Board currBoard = queue.Dequeue();

                    //get amount of swaps already done and modify for loop index accordingly.
                    int forIndex = board.width * board.height + 4;
                    int count = currBoard.getMoves().Count(f => f == '(');
                    // if we are at the limit, only search moves, not swaps
                    if (count >= i2)
                    {
                        forIndex = 4;
                    }

                    for (int i = 0; i < forIndex; i++)
                    {

                        if (Move(currBoard, i))
                        {
                            Board currBoard2 = new Board(currBoard);
                            currBoard2.addMove(i);
                            Move(currBoard2, i, true);
                            counter++;

                            if (counter % 10000 == 0)
                            {
                                int queuecount = Regex.Replace(queue.First().getMoves(), @"[\d(]", string.Empty).Count();
                                Console.WriteLine("n = " + counter + ". queue = " + queue.Count + " depth: " + queuecount);
                            }
                            if (!history.Contains(currBoard2.customHash()))
                            {
                                history.Add(currBoard2.customHash());

                                if (isSolved(currBoard2))
                                {
                                    Console.WriteLine("Size of hashset: " + history.Count());
                                    Console.WriteLine("Legal Moves tried: " + counter);
                                    Console.WriteLine("Length: " + currBoard2.getMoves().Length);
                                    return currBoard2.getMoves();
                                }
                                else
                                {
                                    queue.Enqueue(new Board(currBoard2));
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("No solution found for " + i2 + " swaps.");
            }
            return "no solution. Searched up to " + n + " swaps.";
        }
        /// <summary>
        /// Checks if a move is legal. Optionally updates the board
        /// with the move.
        /// </summary>
        /// <param name="tempBoard"> The input board</param>
        /// <param name="move">the move to check</param>
        /// <param name="doMove">Wheter the move should actually be done and the board updated.</param>
        /// <returns>Is the move legal?</returns>
        public bool Move(Board tempBoard, int move, bool doMove = false)
        {
            return tempBoard.move(move, doMove);
        }

        /// <summary>
        /// Check if the given board is solved.
        /// </summary>
        /// <param name="tempBoard">the input board</param>
        /// <returns> Is the board solved?</returns>
        public bool isSolved(Board tempBoard)
        {
            return tempBoard.isSolved();
        }



    }
}
