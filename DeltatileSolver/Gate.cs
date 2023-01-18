using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwapSolver
{
    /// <summary>
    /// Class that handles gates and their triggers
    /// </summary>
    internal class Gate
    {

        /// <summary>
        /// Get position of all gates present on the board
        /// </summary>
        /// <param name="Board">input board</param>
        /// <returns>A list of gate positions</returns>
        public static List<int> getGates(String Board)
        {
            List<int> gates = new List<int>();
            for (int i = 0; i < Board.Count(); i++)
            {
                if (Board[i] == '(')
                {
                    gates.Add(i);
                }
            }
            return gates;
        }

        /// <summary>
        /// Opens all gates on the board.
        /// </summary>
        /// <param name="Board">input board</param>
        /// <param name="gateCount">amount of moves the gate is open</param>
        /// <returns>a board with the gates open</returns>
        public static String setGates(String Board, int gateCount)
        {

            //fail fast if there are no gates
            if (gateCount == -1) return Board;

            
            char[] trial2 = Board.ToCharArray();
            //open all gates
            for (int i = 0; i < Board.Count(); i++)
            {
                if (trial2[i] == '(')
                {
                    trial2[i] = ')';
                }
                if (Char.IsDigit(trial2[i]))
                {
                    trial2[i] = Convert.ToChar(gateCount - 0 + 48);
                }
            }
            return new String(trial2);
        }

        /// <summary>
        /// Function that closes gates if time is up
        /// </summary>
        /// <param name="Board">input board</param>
        /// <param name="gateCount"> amount of time the gates are open</param>
        /// <returns></returns>
        public static String updateGates(String Board, int gateCount)
        {

            //do nothing if there are no gates
            if (gateCount == -1) return Board;

            //do nothing if the gates are already closed
            foreach (char c in Board)
            {
                if (c == '(')
                {
                    return Board;
                }
            }


            char[] trial2 = Board.ToCharArray();
            int gateValue = 0;

            //update gate timer
            foreach (char c in Board)
            {
                if (Char.IsDigit(c))
                {
                    gateValue = c - 48;
                }
            }

            //if there is still time, make sure all gates are open
            if (gateValue >= 1)
            {
                for (int i = 0; i < Board.Count(); i++)
                {
                    if (trial2[i] == '(')
                    {
                        trial2[i] = ')';
                    }
                    if (Char.IsDigit(trial2[i]))
                    {
                        trial2[i] = Convert.ToChar(trial2[i] - 1);
                    }
                }
            }
            else //otherwise close all gates
            {
                for (int i = 0; i < Board.Count(); i++)
                {
                    if (trial2[i] == ')')
                    {
                        trial2[i] = '(';
                    }
                    if (Char.IsDigit(trial2[i]))
                    {
                        trial2[i] = Convert.ToChar(gateCount + 48);
                    }
                }
            }

            return new String(trial2);
        }

    }
}
