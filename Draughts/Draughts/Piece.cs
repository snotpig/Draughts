using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snotsoft.Games
{
    partial class Game
    {
        public class Piece
        {
            protected readonly Game game;
            protected int row, col;
            public bool isBlack;
            protected Piece[,] board;

            public Piece() { }
            public Piece(Game game, int col, int row)
            {
                this.game = game;
                this.row = row;
                this.col = col;
                if(game != null) board = game.gameBoard;
            }
            public virtual bool PieceCanJump() { return false; }
            public virtual bool PieceCanSlide() { return false; }
            public virtual bool Jump(int toCol, int toRow) { return false; }
            public virtual bool Slide(int toCol, int toRow) { return false; }
            public virtual bool GetSlides(Turn turn, List<Turn> list) { return false; }
            protected virtual bool GetJump(Turn turn, List<Turn> list) { return false; }
            public void SetPos(int col, int row)
            {
                this.col = col;
                this.row = row;
            }

            public bool GetJumps(Turn turn, List<Turn> list)
            {
                bool canJump = false;
                List<Turn> _list = new List<Turn>();

                if (GetJump(turn, _list))
                {
                    canJump = true;
                    int count = _list.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (GetJump(_list[i], _list))
                        {
                            _list[i] = null;
                        }
                    }
                    int total = _list.Count;
                    for (int i = count; i < total; i++)
                    {
                        if (GetJump(_list[i], _list))
                        {
                            _list[i] = null;
                        }
                    }
                }
                foreach (Turn t in _list)
                {
                    if (t != null) list.Add(t);
                }
                return canJump;
            }

            public static Turn CopyTurn(Turn t)
            {
                Turn turn = new Turn(t.col, t.row, t.board, t.parent);
                for (int i = 0; i < t.moves.Count; i++)
                {
                    turn.moves.Add(t.moves[i]);
                }
                return turn;
            }

            public static bool JumpLeftFwd(Turn turn)
            {
                if ((turn.col < 2) || (turn.row > 5)) return false;                 // out of bounds
                if (turn.board[turn.col - 2, turn.row + 2] != null) return false;   // blocked
                if (turn.board[turn.col - 1, turn.row + 1] == null) return false;   // no piece in middle
                if(turn.board[turn.col, turn.row].isBlack ^ turn.board[turn.col - 1, turn.row + 1].isBlack) // right colour?
                {
                    turn.board[turn.col - 2, turn.row + 2] = turn.board[turn.col, turn.row];
                    turn.board[turn.col, turn.row] = null;
                    turn.board[turn.col - 1, turn.row + 1] = null;
                    turn.moves.Add(new Move(turn.col, turn.row, turn.col - 2, turn.row + 2));
                    turn.moves.Add(new Move(turn.col - 1, turn.row + 1, 8, 8));
                    turn.col -= 2;
                    turn.row += 2;
                    if ((turn.row == 7) && (turn.board[turn.col, turn.row] is BlackPiece))
                    {
                        turn.board[turn.col, turn.row] = new BlackKing(null, turn.col, turn.row);
                    }
                    return true;
                }
                return false;
            }

            public static bool JumpRightFwd(Turn turn)
            {
                if ((turn.col > 5) || (turn.row > 5)) return false;                 // out of bounds
                if (turn.board[turn.col + 2, turn.row + 2] != null) return false;   // blocked
                if (turn.board[turn.col + 1, turn.row + 1] == null) return false;   // no piece in middle
                if (turn.board[turn.col, turn.row].isBlack ^ turn.board[turn.col + 1, turn.row + 1].isBlack) // right colour?
                {
                    turn.board[turn.col + 2, turn.row + 2] = turn.board[turn.col, turn.row];
                    turn.board[turn.col, turn.row] = null;
                    turn.board[turn.col + 1, turn.row + 1] = null;
                    turn.moves.Add(new Move(turn.col, turn.row, turn.col + 2, turn.row + 2));
                    turn.moves.Add(new Move(turn.col + 1, turn.row + 1, 8, 8));
                    turn.col += 2;
                    turn.row += 2;
                    if ((turn.row == 7) && (turn.board[turn.col, turn.row] is BlackPiece))
                    {
                        turn.board[turn.col, turn.row] = new BlackKing(null, turn.col, turn.row);
                    }
                    return true;
                }
                return false;
            }

            public static bool JumpLeftBack(Turn turn)
            {
                if ((turn.col < 2) || (turn.row < 2)) return false;                                         // out of bounds
                if (turn.board[turn.col - 2, turn.row - 2] != null) return false;                           // blocked
                if (turn.board[turn.col - 1, turn.row - 1] == null) return false;                           // no piece in middle
                if(turn.board[turn.col, turn.row].isBlack ^ turn.board[turn.col - 1, turn.row - 1].isBlack) // right colour?
                {
                    turn.board[turn.col - 2, turn.row - 2] = turn.board[turn.col, turn.row];
                    turn.board[turn.col, turn.row] = null;
                    turn.board[turn.col - 1, turn.row - 1] = null;
                    turn.moves.Add(new Move(turn.col, turn.row, turn.col - 2, turn.row - 2));
                    turn.moves.Add(new Move(turn.col - 1, turn.row - 1, 8, 8));
                    turn.col -= 2;
                    turn.row -= 2;
                    if ((turn.row == 0) && (turn.board[turn.col, turn.row] is WhitePiece))
                    {
                        turn.board[turn.col, turn.row] = new WhiteKing(null, turn.col, turn.row);
                    }
                    return true;
                }
                return false;
            }

            public static bool JumpRightBack(Turn turn)
            {
                if ((turn.col > 5) || (turn.row < 2)) return false;                                         // out of bounds
                if (turn.board[turn.col + 2, turn.row - 2] != null) return false;                           // blocked
                if (turn.board[turn.col + 1, turn.row - 1] == null) return false;                           // no piece in middle
                if (turn.board[turn.col, turn.row].isBlack ^ turn.board[turn.col + 1, turn.row - 1].isBlack) // right colour?
                {
                    turn.board[turn.col + 2, turn.row - 2] = turn.board[turn.col, turn.row];
                    turn.board[turn.col, turn.row] = null;
                    turn.board[turn.col + 1, turn.row - 1] = null;
                    turn.moves.Add(new Move(turn.col, turn.row, turn.col + 2, turn.row - 2));
                    turn.moves.Add(new Move(turn.col + 1, turn.row - 1, 8, 8));
                    turn.col += 2;
                    turn.row -= 2;
                    if ((turn.row == 0) && (turn.board[turn.col, turn.row] is WhitePiece))
                    {
                        turn.board[turn.col, turn.row] = new WhiteKing(null, turn.col, turn.row);
                    }
                    return true;
                }
                return false;
            }

            public static bool SlideLeftFwd(Turn turn)
            {
                if ((turn.col < 1) || (turn.row > 6)) return false;
                if (turn.board[turn.col - 1, turn.row + 1] == null)
                {
                    turn.board[turn.col - 1, turn.row + 1] = turn.board[turn.col, turn.row];
                    turn.board[turn.col, turn.row] = null;
                    Move move = new Move(turn.col, turn.row, turn.col - 1, turn.row + 1);
                    turn.moves.Add(move);
                    turn.col--;
                    turn.row++;
                    if ((turn.row == 7) && (turn.board[turn.col, turn.row] is BlackPiece))
                    {
                        turn.board[turn.col, turn.row] = new BlackKing(null, turn.col, turn.row);
                    }
                    return true;
                }
                return false;
            }

            public static bool SlideRightFwd(Turn turn)
            {
                if ((turn.col > 6) || (turn.row > 6)) return false;
                if (turn.board[turn.col + 1, turn.row + 1] == null)
                {
                    turn.board[turn.col + 1, turn.row + 1] = turn.board[turn.col, turn.row];
                    turn.board[turn.col, turn.row] = null;
                    Move move = new Move(turn.col, turn.row, turn.col + 1, turn.row + 1);
                    turn.moves.Add(move);
                    turn.col++;
                    turn.row++;
                    if ((turn.row == 7) && (turn.board[turn.col, turn.row] is BlackPiece))
                    {
                        turn.board[turn.col, turn.row] = new BlackKing(null, turn.col, turn.row);
                    }
                    return true;
                }
                return false;
            }

            public static bool SlideLeftBack(Turn turn)
            {
                if ((turn.col < 1) || (turn.row < 1)) return false;
                if (turn.board[turn.col - 1, turn.row - 1] == null)
                {
                    turn.board[turn.col - 1, turn.row - 1] = turn.board[turn.col, turn.row];
                    turn.board[turn.col, turn.row] = null;
                    Move move = new Move(turn.col, turn.row, turn.col - 1, turn.row - 1);
                    turn.moves.Add(move);
                    turn.col--;
                    turn.row--;
                    if ((turn.row == 0) && (turn.board[turn.col, turn.row] is WhitePiece))
                    {
                        turn.board[turn.col, turn.row] = new WhiteKing(null, turn.col, turn.row);
                    }
                    return true;
                }
                return false;
            }

            public static bool SlideRightBack(Turn turn)
            {
                if ((turn.col > 6) || (turn.row < 1)) return false;
                if (turn.board[turn.col + 1, turn.row - 1] == null)
                {
                    turn.board[turn.col + 1, turn.row - 1] = turn.board[turn.col, turn.row];
                    turn.board[turn.col, turn.row] = null;
                    Move move = new Move(turn.col, turn.row, turn.col + 1, turn.row - 1);
                    turn.moves.Add(move);
                    turn.col++;
                    turn.row--;
                    if ((turn.row == 0) && (turn.board[turn.col, turn.row] is WhitePiece))
                    {
                        turn.board[turn.col, turn.row] = new WhiteKing(null, turn.col, turn.row);
                    }
                    return true;
                }
                return false;
            }
        }

// Black Piece
        class BlackPiece : Piece
        {
            public BlackPiece(Game game, int col, int row) : base(game, col, row) { isBlack = true; }

            public override bool PieceCanJump()
            {
                if(row > 5) return false;
                if (col > 1)                // jump left
                {
                    if (board[col - 2, row + 2] == null)
                    {
                        if (board[col - 1, row + 1] != null)
                        {
                            if (!board[col - 1, row + 1].isBlack) return true;
                        }
                    }
                }
                if (col < 6)                // jump right
                {
                    if (board[col + 2, row + 2] == null)
                    {
                        if (board[col + 1, row + 1] != null)
                        {
                            if (!board[col + 1, row + 1].isBlack) return true;
                        }
                    }
                }
                return false;
            }

            public override bool PieceCanSlide()
            {
                if (row > 6) return false;
                if (col > 0)                // slide left
                {
                    if (board[col - 1, row + 1] == null) return true;
                }
                if (col < 7)                // slide right
                {
                    if (board[col + 1, row + 1] == null) return true;
                }
                return false;
            }

            public override bool Jump(int toCol, int toRow)
            {
                if ((toCol < 0) || (toCol > 7) || (toRow < 0) || (toRow > 7)) return false; // out of bounds
                if ((Math.Abs(toCol - col) == 2) && (toRow - row == 2)) // 2 squares diagonal
                {
                    if (board[toCol, toRow] != null) return false; // space occupied
                    int tweenCol = (col + toCol) / 2;
                    int tweenRow = (row + toRow) / 2;

                    if (board[tweenCol, tweenRow] == null) return false;
                    if(!board[tweenCol, tweenRow].isBlack)
                    {
                        board[col, row] = null;
                        board[toCol, toRow] = this;
                        board[tweenCol, tweenRow] = null;
                        game.UI.MovePiece(col, row, toCol, toRow);
                        game.TakePiece(tweenCol, tweenRow);
                        col = toCol;
                        row = toRow;
                        game.isChained = PieceCanJump();
                        if (row == 7) game.isChained = false; // becomes King
                        return true;
                    }
                }
                return false;
            }

            public override bool Slide(int toCol, int toRow)
            {
                if ((toCol < 0) || (toCol > 7) || (toRow < 0) || (toRow > 7)) return false; // out of bounds
                if ((Math.Abs(toCol - col) == 1) && (toRow - row == 1)) // 1 square diagonal forward
                {
                    if (board[toCol, toRow] != null) return false; // space occupied
                    board[col, row] = null;
                    board[toCol, toRow] = this;
                    game.UI.MovePiece(col, row, toCol, toRow);
                    col = toCol;
                    row = toRow;
                    return true;
                }
                return false;
            }

            protected override bool GetJump(Turn turn, List<Turn> list)
            {
                Turn t;
                bool canJump = false;

                t = CopyTurn(turn);
                if (JumpLeftFwd(t))
                {
                    list.Add(t);
                    t = CopyTurn(turn);
                    canJump = true;
                }
                if (JumpRightFwd(t))
                {
                    list.Add(t);
                    canJump = true;
                }
                return canJump;
            }

            public override bool GetSlides(Turn turn, List<Turn> list) 
            {
                bool canSlide = false;
                Turn t = CopyTurn(turn);
                if(SlideLeftFwd(t))
                {
                    list.Add(t);
                    t = CopyTurn(turn);
                    canSlide = true;
                }
                if (SlideRightFwd(t))
                {
                    list.Add(t);
                    canSlide = true;
                }
                return canSlide;
            }
        }

// White Piece
        class WhitePiece : Piece
        {
            public WhitePiece(Game game, int col, int row) : base(game, col, row) { isBlack = false; }

            public override bool PieceCanJump()
            {
                if (row < 2) return false;
                if (col > 1)                // jump left
                {
                    if (board[col - 2, row - 2] == null)
                    {
                        if (board[col - 1, row - 1] != null)
                        {
                            if (board[col - 1, row - 1].isBlack) return true;
                        }
                    }
                }
                if (col < 6)                // jump right
                {
                    if (board[col + 2, row - 2] == null)
                    {
                        if (board[col + 1, row - 1] != null)
                        {
                            if (board[col + 1, row - 1].isBlack) return true;
                        }
                    }
                }
                return false;
            }

            public override bool PieceCanSlide()
            {
                if (row < 1) return false;
                if (col > 0)                // slide left
                {
                    if (board[col - 1, row - 1] == null) return true;
                }
                if (col < 7)                // slide right
                {
                    if (board[col + 1, row - 1] == null) return true;
                }
                return false;
            }

            public override bool Jump(int toCol, int toRow)
            {
                if ((toCol < 0) || (toCol > 7) || (toRow < 0) || (toRow > 7)) return false; // out of bounds
                if ((Math.Abs(toCol - col) == 2) && (toRow - row == -2)) // 2 squares diagonal
                {
                    if (board[toCol, toRow] != null) return false; // space occupied
                    int tweenCol = (col + toCol) / 2;
                    int tweenRow = (row + toRow) / 2;

                    if (board[tweenCol, tweenRow] == null) return false;
                    if (board[tweenCol, tweenRow].isBlack)
                    {
                        board[col, row] = null;
                        board[toCol, toRow] = this;
                        board[tweenCol, tweenRow] = null;
                        game.UI.MovePiece(col, row, toCol, toRow);
                        game.TakePiece(tweenCol, tweenRow);
                        col = toCol;
                        row = toRow;
                        game.isChained = PieceCanJump();
                        if (row == 0) game.isChained = false; // becomes King
                        return true;
                    }
                }
                return false;
            }

            public override bool Slide(int toCol, int toRow)
            {
                if ((toCol < 0) || (toCol > 7) || (toRow < 0) || (toRow > 7)) return false; // out of bounds
                if ((Math.Abs(toCol - col) == 1) && (toRow - row == -1)) // 1 square diagonal forward
                {
                    if (board[toCol, toRow] != null) return false; // space occupied
                    board[col, row] = null;
                    board[toCol, toRow] = this;
                    game.UI.MovePiece(col, row, toCol, toRow);
                    col = toCol;
                    row = toRow;
                    return true;
                }
                return false;
            }

            protected override bool GetJump(Turn turn, List<Turn> list)
            {
                Turn t;
                bool canJump = false;

                t = CopyTurn(turn);
                if (JumpLeftBack(t))
                {
                    list.Add(t);
                    t = CopyTurn(turn);
                    canJump = true;
                }
                if (JumpRightBack(t))
                {
                    list.Add(t);
                    canJump = true;
                }
                return canJump;
            }

            public override bool GetSlides(Turn turn, List<Turn> list) 
            {
                bool CanSlide = false;
                Turn t = CopyTurn(turn);
                if (SlideLeftBack(t))
                {
                    list.Add(t);
                    t = CopyTurn(turn);
                    CanSlide = true;
                }
                if (SlideRightBack(t))
                {
                    list.Add(t);
                    CanSlide = true;
                }
                return CanSlide;
            }

        }

// Black King
        class BlackKing : Piece
        {
            public BlackKing(Game game, int col, int row) : base(game, col, row) { isBlack = true; }

            public override bool PieceCanJump()
            {
                if (col > 1)                //--- jump left ---
                {
                    if (row > 2)            // jump back
                    {
                        if (board[col - 2, row - 2] == null)
                        {
                            if (board[col - 1, row - 1] != null)
                            {
                                if (!board[col - 1, row - 1].isBlack) return true;
                            }
                        }
                    }
                    if (row < 5)            // jump forward
                    {
                        if (board[col - 2, row + 2] == null)
                        {
                            if (board[col - 1, row + 1] != null)
                            {
                                if (!board[col - 1, row + 1].isBlack) return true;
                            }
                        }
                    }
                }
                if (col < 6)                // --- jump right ---
                {
                    if (row > 2)            // jump back
                    {
                        if (board[col + 2, row - 2] == null)
                        {
                            if (board[col + 1, row - 1] != null)
                            {
                                if (!board[col + 1, row - 1].isBlack) return true;
                            }
                        }
                    }
                    if (row < 5)            // jump forward
                    {
                        if (board[col + 2, row + 2] == null)
                        {
                            if (board[col + 1, row + 1] != null)
                            {
                                if (!board[col + 1, row + 1].isBlack) return true;
                            }
                        }
                    }
                }
                return false;
            }

            public override bool PieceCanSlide()
            {
                if (row < 7)
                {
                    if (col > 0)                // slide left fwd
                    {
                        if (board[col - 1, row + 1] == null) return true;
                    }
                    if (col < 7)                // slide right fwd
                    {
                        if (board[col + 1, row + 1] == null) return true;
                    }
                }
                if (row > 0)
                {
                    if (col > 0)                // slide left rev
                    {
                        if (board[col - 1, row - 1] == null) return true;
                    }
                    if (col < 7)                // slide right rev
                    {
                        if (board[col + 1, row - 1] == null) return true;
                    }
                }
                return false;
            }

            public override bool Jump(int toCol, int toRow)
            {
                if ((toCol < 0) || (toCol > 7) || (toRow < 0) || (toRow > 7)) return false; // out of bounds
                if ((Math.Abs(toCol - col) == 2) && (Math.Abs(toRow - row) == 2)) // 2 squares diagonal
                {
                    if (board[toCol, toRow] != null) return false; // space occupied
                    int tweenCol = (col + toCol) / 2;
                    int tweenRow = (row + toRow) / 2;

                    if (board[tweenCol, tweenRow] == null) return false;
                    if (!board[tweenCol, tweenRow].isBlack)
                    {
                        board[col, row] = null;
                        board[toCol, toRow] = this;
                        board[tweenCol, tweenRow] = null;
                        game.UI.MovePiece(col, row, toCol, toRow);
                        game.TakePiece(tweenCol, tweenRow);
                        col = toCol;
                        row = toRow;
                        game.isChained = PieceCanJump();
                        return true;
                    }
                }
                return false;
            }

            public override bool Slide(int toCol, int toRow)
            {
                if ((toCol < 0) || (toCol > 7) || (toRow < 0) || (toRow > 7)) return false; // out of bounds
                if ((Math.Abs(toCol - col) == 1) && (Math.Abs(toRow - row) == 1)) // 1 square diagonal forward or back
                {
                    if (board[toCol, toRow] != null) return false; // space occupied
                    board[col, row] = null;
                    board[toCol, toRow] = this;
                    game.UI.MovePiece(col, row, toCol, toRow);
                    col = toCol;
                    row = toRow;
                    return true;
                }
                return false;
            }

            protected override bool GetJump(Turn turn, List<Turn> list)
            {
                Turn t;
                bool canJump = false;

                t = CopyTurn(turn);
                if (JumpLeftFwd(t))
                {
                    list.Add(t);
                    t = CopyTurn(turn);
                    canJump = true;
                }
                if (JumpRightFwd(t))
                {
                    list.Add(t);
                    canJump = true;
                }
                if (JumpLeftBack(t))
                {
                    list.Add(t);
                    t = CopyTurn(turn);
                    canJump = true;
                }
                if (JumpRightBack(t))
                {
                    list.Add(t);
                    canJump = true;
                }
                return canJump;
            }

            public override bool GetSlides(Turn turn, List<Turn> list) // :-)
            {
                bool CanSlide = false;
                Turn t = CopyTurn(turn);
                if (SlideLeftFwd(t))
                {
                    list.Add(t);
                    t = CopyTurn(turn);
                    CanSlide = true;
                }
                if (SlideRightFwd(t))
                {
                    list.Add(t);
                    t = CopyTurn(turn);
                    CanSlide = true;
                }
                if (SlideLeftBack(t))
                {
                    list.Add(t);
                    t = CopyTurn(turn);
                    CanSlide = true;
                }
                if (SlideRightBack(t))
                {
                    list.Add(t);
                    CanSlide = true;
                }
                return CanSlide;
            }

        }

// White King
        class WhiteKing : Piece
        {
            public WhiteKing(Game game, int col, int row) : base(game, col, row) { isBlack = false; }

            public override bool PieceCanJump()
            {
                if (col > 1)                //--- jump left ---
                {
                    if (row > 2)            // jump back
                    {
                        if (board[col - 2, row - 2] == null)
                        {
                            if (board[col - 1, row - 1] != null)
                            {
                                if (board[col - 1, row - 1].isBlack) return true;
                            }
                        }
                    }
                    if (row < 5)            // jump forward
                    {
                        if (board[col - 2, row + 2] == null)
                        {
                            if (board[col - 1, row + 1] != null)
                            {
                                if (board[col - 1, row + 1].isBlack) return true;
                            }
                        }
                    }
                }
                if (col < 6)                // --- jump right ---
                {
                    if (row > 2)            // jump back
                    {
                        if (board[col + 2, row - 2] == null)
                        {
                            if (board[col + 1, row - 1] != null)
                            {
                                if (board[col + 1, row - 1].isBlack) return true;
                            }
                        }
                    }
                    if (row < 5)            // jump forward
                    {
                        if (board[col + 2, row + 2] == null)
                        {
                            if (board[col + 1, row + 1] != null)
                            {
                                if (board[col + 1, row + 1].isBlack) return true;
                            }
                        }
                    }
                }
                return false;
            }

            public override bool PieceCanSlide()
            {
                if (row < 7)
                {
                    if (col > 0)                // slide left fwd
                    {
                        if (board[col - 1, row + 1] == null) return true;
                    }
                    if (col < 7)                // slide right fwd
                    {
                        if (board[col + 1, row + 1] == null) return true;
                    }
                }
                if (row > 0)
                {
                    if (col > 0)                // slide left rev
                    {
                        if (board[col - 1, row - 1] == null) return true;
                    }
                    if (col < 7)                // slide right rev
                    {
                        if (board[col + 1, row - 1] == null) return true;
                    }
                }
                return false;
            }

            public override bool Jump(int toCol, int toRow)
            {
                if ((toCol < 0) || (toCol > 7) || (toRow < 0) || (toRow > 7)) return false; // out of bounds
                if ((Math.Abs(toCol - col) == 2) && (Math.Abs(toRow - row) == 2)) // 2 squares diagonal
                {
                    if (board[toCol, toRow] != null) return false; // space occupied
                    int tweenCol = (col + toCol) / 2;
                    int tweenRow = (row + toRow) / 2;

                    if (board[tweenCol, tweenRow] == null) return false;
                    if (board[tweenCol, tweenRow].isBlack)
                    {
                        board[col, row] = null;
                        board[toCol, toRow] = this;
                        board[tweenCol, tweenRow] = null;
                        game.UI.MovePiece(col, row, toCol, toRow);
                        game.TakePiece(tweenCol, tweenRow);
                        col = toCol;
                        row = toRow;
                        game.isChained = PieceCanJump();
                        return true;
                    }
                }
                return false;
            }

            public override bool Slide(int toCol, int toRow)
            {
                if ((toCol < 0) || (toCol > 7) || (toRow < 0) || (toRow > 7)) return false; // out of bounds
                if ((Math.Abs(toCol - col) == 1) && (Math.Abs(toRow - row) == 1)) // 1 square diagonal forward or back
                {
                    if (board[toCol, toRow] != null) return false; // space occupied
                    board[col, row] = null;
                    board[toCol, toRow] = this;
                    game.UI.MovePiece(col, row, toCol, toRow);
                    col = toCol;
                    row = toRow;
                    return true;
                }
                return false;
            }

            protected override bool GetJump(Turn turn, List<Turn> list)
            {
                Turn t;
                bool canJump = false;

                t = CopyTurn(turn);
                if (JumpLeftFwd(t))
                {
                    list.Add(t);
                    t = CopyTurn(turn);
                    canJump = true;
                }
                if (JumpRightFwd(t))
                {
                    list.Add(t);
                    canJump = true;
                }
                if (JumpLeftBack(t))
                {
                    list.Add(t);
                    t = CopyTurn(turn);
                    canJump = true;
                }
                if (JumpRightBack(t))
                {
                    list.Add(t);
                    canJump = true;
                }
                return canJump;
            }

            public override bool GetSlides(Turn turn, List<Turn> list) // :-)
            {
                bool CanSlide = false;

                Turn t = CopyTurn(turn);
                if (SlideLeftFwd(t))
                {
                    list.Add(t);
                    t = CopyTurn(turn);
                    CanSlide = true;
                }
                if (SlideRightFwd(t))
                {
                    list.Add(t);
                    t = CopyTurn(turn);
                    CanSlide = true;
                }
                if (SlideLeftBack(t))
                {
                    list.Add(t);
                    t = CopyTurn(turn);
                    CanSlide = true;
                }
                if (SlideRightBack(t))
                {
                    list.Add(t);
                    CanSlide = true;
                }
                return CanSlide;
            }

        }

    } // class Game
} // namespace Snotsoft.Games