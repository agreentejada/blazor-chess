using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blazor_chess.Data
{
    public class PawnBoard
    {
        public bool[][] Coordinates { get; set; } = new bool[8][];
        public Player A;
        public Player B;

        public PawnBoard()
        {
            A = new Player(this, null, true);
            B = new Player(this, "Bob", false);

            for (int i = 0; i < 8; i++)
            {
                Coordinates[i] = new bool[8];

                if (i == 0)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Coordinates[i][j] = true;
                        B.Pawns.Add((i, j));
                    }
                }

                if (i == 7)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Coordinates[i][j] = true;
                        A.Pawns.Add((i, j));
                    }
                }
            }
        }

        public PawnBoard(string playerName) : this()
        {
            var pawns = A.Pawns;
            A = new Player(this, playerName, true);
            A.Pawns = pawns;
        }

        public string Move((int, int) curr, (int, int) move, Player player)
        {
            string msg = $"{player.Name} moved [{curr.Item1},{curr.Item2}] to [{move.Item1},{move.Item2}].";

            //Moves the pawn on the board, then players.
            Coordinates[curr.Item1][curr.Item2] = false;
            Coordinates[move.Item1][move.Item2] = true;

            //Checks to see if the player has any pawns. player assumes A.
            if (A == player)
            {
                A.Pawns.Remove(curr);
                A.Pawns.Add(move);

                if (B.HasPawn(move.Item1, move.Item2))
                {
                    B.Pawns.Remove(move);
                    msg = $"{player.Name} took pawn at [{move.Item1},{move.Item2}].";
                }
            }
            else
            {
                B.Pawns.Remove(curr);
                B.Pawns.Add(move);

                if (A.HasPawn(move.Item1, move.Item2))
                {
                    A.Pawns.Remove(move);
                    msg = $"{player.Name} took pawn at [{move.Item1},{move.Item2}].";
                }
            }

            return msg;
        }
    }

    public class Player
    {
        PawnBoard Board;
        public string Name { get; set; }
        public bool Bottom { get; set; }
        public List<(int, int)> Pawns { get; set; } = new List<(int, int)>();

        public Player(PawnBoard board, string name, bool startBottom)
        {
            Board = board;
            Name = name;
            Bottom = startBottom;
        }

        public bool HasPawn(int i, int j)
        {
            return Pawns.Any(X => X == (i, j));
        }

        public bool CanMove()
        {
            foreach (var pawn in Pawns)
            {
                if (ValidMoves(pawn.Item1, pawn.Item2).Any())
                {
                    return true;
                }
            }

            return false;
        }

        public List<(int, int)> ValidMoves(int i, int j)
        {
            //Since it's a pawn, we can easily control it's logic.
            var list = new List<(int, int)>();

            //Check if pawns haven't already reached the end.
            if (Bottom && i > 0 || !Bottom && i < 7)
            {
                int dir = Bottom ? -1 : 1;
                //Check if a pawn is there.
                if (!Board.Coordinates[i + dir][j])
                {
                    list.Add((i + dir, j));
                }
                if (j != 0)
                {
                    if (Board.Coordinates[i + dir][j - 1] && !HasPawn(i + dir, j - 1))
                    {
                        list.Add((i + dir, j - 1));
                    }
                }
                if (j != 7)
                {
                    if (Board.Coordinates[i + dir][j + 1] && !HasPawn(i + dir, j - 1))
                    {
                        list.Add((i + dir, j + 1));
                    }
                }
            }

            return list;
        }
    }
}
