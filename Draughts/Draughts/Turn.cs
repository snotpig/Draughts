using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snotsoft.Games
{
    partial class Game
    {
        public class Turn
        {
            public int col, row, parent, score;
            public Piece[,] board = new Piece[8,8];
            public List<Move> moves = new List<Move>();
            private static Random random = new Random();

            protected static int GetRandomNumber()
            {
                return random.Next(10) - 5;
            }

            public Turn()
            {
            }

            public Turn(int col, int row, Piece[,] board, int parent=0)
            {
                this.col = col;
                this.row = row;
                this.parent = parent;
                moves.Clear();
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        this.board[i, j] = board[i, j];
                    }
                }
            }

            public int GetScore()
            {
                int blackValue = 12;
                int whiteValue = 12;
                score = 0;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (board[i, j] is BlackPiece)
                        {
                            score += blackValue;
                            blackValue--;
                        }
                        if (board[i, j] is BlackKing)
                        {
                            score += blackValue;
                            blackValue--;
                        }
                        if (board[i, j] is WhitePiece)
                        {
                            score -= whiteValue;
                            whiteValue--;
                        }
                        if (board[i, j] is WhiteKing)
                        {
                            score -= whiteValue;
                            whiteValue--;
                        }
                    }
                }
                score = score * 100;
                score += random.Next(100) - 50;
                return score;
            }
        }
    }

// structure for move
    public struct Move
    {
        public int fromCol;
        public int fromRow;
        public int toCol;
        public int toRow;

        public Move(int fromCol, int fromRow, int toCol, int toRow)
        {
            this.fromCol = fromCol;
            this.fromRow = fromRow;
            this.toCol = toCol;
            this.toRow = toRow;
        }

        public void Assign(int fromCol, int fromRow, int toCol, int toRow)
        {
            this.fromCol = fromCol;
            this.fromRow = fromRow;
            this.toCol = toCol;
            this.toRow = toRow;
        }

        public static bool operator ==(Move a, Move b)
        {
            return ((a.fromCol == b.fromCol) && (a.fromRow == b.fromRow) && (a.toCol == b.toCol) && (a.toRow == b.toRow));
        }

        public static bool operator !=(Move a, Move b)
        {
            return ((a.fromCol != b.fromCol) || (a.fromRow != b.fromRow) || (a.toCol != b.toCol) || (a.toRow != b.toRow));
        }

        public override bool Equals(object ob)
        {
            if (!(ob is Move)) return false;
            Move sq = (Move)ob;
            return ((toCol == sq.toCol) && (toRow == sq.toRow) && (fromCol == sq.fromCol) && (fromRow == sq.fromRow));
        }

        public override int GetHashCode()
        {
            return fromRow * fromCol * toCol * toRow;
        }
    };

}
