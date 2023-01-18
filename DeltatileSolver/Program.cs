using System.Text;

namespace SwapSolver
{

    /*
     * @ = Player Avatar
     * # = Unpassable Empty Space
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
     * 
     */
    /// <summary>
    /// Main Class. Sets up a board and runs a solver on it.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Solving Board...");

            // Parse level, setup solver
            string level =

                                "#########," +
                                "#@ $   fw," +
                                "#########";

            Board myBoard = new Board(level.Split(','));
            Solver mySolver = new Solver(new Board(myBoard));
            myBoard.printBoard();

            /*Solve the board. Call different functions for different behaviour:
             *
             * single(myBoard,mySolver):        Looks for the solution with the least amount of moves
             * singleSwap(myBoard,mySolver):    Looks for the solution with the least amount of swaps
             * multi(myBoard,mySolver, n = 2):  Looks for several solutions, based on the optional parameter
             */
            singleSwap(myBoard, mySolver);

        }

        /// <summary>
        /// Looks for a single solution with minimal amount of moves.
        /// If a solution doesn't exist, it will print a message on the
        /// command line. The solution is then shown.
        /// </summary>
        /// <param name="myBoard"> the input Board </param>
        /// <param name="mySolver"> Solver Object, initialized using the Board. </param>
        private static void single(Board myBoard, Solver mySolver)
        {
            string sol = mySolver.Solve();
            Console.WriteLine("Solution found: " + sol);
            Console.WriteLine("Press any key to show solution...");
            Console.ReadKey();
            showSol(myBoard, sol);
        }

        /// <summary>
        /// Looks for several solutions with minimal amount of moves.
        /// If a solution doesn't exist, it will print a message on the
        /// command line. The solution is then shown.
        /// </summary>
        /// <param name="myBoard"> the input Board </param>
        /// <param name="mySolver"> Solver Object, initialized using the Board. </param>
        /// <param name="n"></param>
        private static void multi(Board myBoard, Solver mySolver, int n = 2)
        {
            List<string> sols = mySolver.MultiSolve(n);
            foreach (string sol in sols)
            {
                Console.WriteLine("Solution found: " + sol);
            }
        }

        /// <summary>
        /// Looks for a single solution with minimal amount of swaps.
        /// If a solution doesn't exist, it will print a message on the
        /// command line. The solution is then shown.
        /// </summary>
        /// <param name="myBoard"> the input Board </param>
        /// <param name="mySolver"> Solver Object, initialized using the Board. </param>
        private static void singleSwap(Board myBoard, Solver mySolver)
        {
            string sol = mySolver.SolveSwap(15);
            Console.WriteLine("Solution found: " + sol);
            Console.WriteLine("Press any key to show solution...");
            Console.ReadKey();
            showSol(myBoard, sol);
        }

        /// <summary>
        /// Shows the solution on the command line.
        /// 
        /// </summary>
        /// <param name="board"> Board on which to show the solution</param>
        /// <param name="sol"> Solution string.
        /// <list type="bullet">
        /// <item>
        /// <description>u/d/l/r: move up/down/left/right</description>
        /// </item>
        /// <item>
        /// <description>(n): Swap the nth block. Count starts from the top left corner, left to right</description>
        /// </item>
        /// </list>
        /// </param>
        static void showSol(Board board, string sol)
        {
            int move, i;
            move = 0;

            Console.WriteLine("Showing Solution for board:");
            int cursorpos = Console.CursorTop; // needed to smoothly render moves
            board.printBoard();
            Console.WriteLine(sol);
            System.Threading.Thread.Sleep(1000);
            for (i = 0; i < sol.Length; i++)
            {
                if (sol[i] == 'u') { move = 0; }
                if (sol[i] == 'r') { move = 1; }
                if (sol[i] == 'd') { move = 2; }
                if (sol[i] == 'l') { move = 3; }
                if (sol[i] == '(')
                {
                    // parse swap coordinates
                    StringBuilder destBuf = new StringBuilder();
                    char[] trial = sol.ToCharArray();
                    int i2 = i + 1;
                    while (trial[i2] != ')') // need to pay attention to multi digit numbers
                    {
                        destBuf.Append(trial[i2]);
                        i2++;
                        i++;
                    }
                    i++;
                    move = Int32.Parse(destBuf.ToString());
                }

                board.move(move, true);
                Console.CursorTop = cursorpos; // reset CLI pos so that we can keep overwriting the board.
                board.printBoard();
                System.Threading.Thread.Sleep(1000); //make a move every second
            }
        }

    }
}