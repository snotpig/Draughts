using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;
using Draughts;

namespace Snotsoft.Games
{
    partial class Game
    {
        private readonly MainWindow UI;
        private Piece[,] gameBoard = new Piece[8, 8]; // contains references to pieces
        private bool isChained;
        private bool isPlayingBlack;
        public bool isBlacksTurn;
        private bool gameInProgress = false;
        private bool CanMove;
        private int level = 3;// 4;//
        private int strata;
        private List<Turn>[] grid;
        private bool enforceTurn = true;//false; // 
        private Turn computerTurn;
        private int markedCol, markedRow;
        private int countdown = 0;

// Constructor
        public Game(MainWindow ui)
        {
            UI = ui;
            SetLevel(level);
        }

        private void ClearBoard()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    gameBoard[col, row] = null;
                }
            }
            isChained = false;
        }

        private void AddBlackPiece(int col, int row, bool isKing=false)
        {
            if (isKing) gameBoard[col, row] = new BlackKing(this, col, row);
            else gameBoard[col, row] = new BlackPiece(this, col, row);
            UI.AddPiece(col, row, true, isKing);
        }

        private void AddWhitePiece(int col, int row, bool isKing=false)
        {
            if (isKing) gameBoard[col, row] = new WhiteKing(this, col, row);
            else gameBoard[col, row] = new WhitePiece(this, col, row);
            UI.AddPiece(col, row, false, isKing);
        }

// Setup pieces for start of game
        private void SetupPieces()
        {
            ClearBoard();
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (row < 3)
                    {
                        if ((row + col) % 2 == 0)
                        {
                            AddBlackPiece(col, row);
                        }
                    }
                    if (row > 4)
                    {
                        if ((col + row) % 2 == 0)
                        {
                            AddWhitePiece(col, row);
                        }
                    }
                }
            }
        }

// Tell UI to take a piece
        private void TakePiece(int col, int row)
        {
            UI.TakePiece(col, row);
        }

// Make piece up to King
        private bool MakeKing(int col, int row)
        {
            if ((row == 7) && (gameBoard[col, row] is BlackPiece))
            {
                gameBoard[col, row] = new BlackKing(this, col, row);
                UI.MakeKing(col, row);
                return true;
            }
            if ((row == 0) && (gameBoard[col, row] is WhitePiece))
            {
                gameBoard[col, row] = new WhiteKing(this, col, row);
                UI.MakeKing(col, row);
                return true;
            }
            return false;
        }

// Check if player can Jump
        private bool PlayerCanJump()
        {
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    Piece pc = gameBoard[col, row];
                    if (pc == null) continue;
                    if (isBlacksTurn ^ !pc.isBlack)
                    {
                        if (pc.PieceCanJump()) return true;
                    }
                }
            }
            return false;
        }

        private bool PlayerCanMove()
        {
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    Piece pc = gameBoard[col, row];
                    if (pc == null) continue;
                    if (isBlacksTurn ^ !pc.isBlack)
                    {
                        if (pc.PieceCanSlide()) return true;
                        if (pc.PieceCanJump()) return true;
                    }
                }
            }
            return false;
        }

        private void EndTurn()
        {
            if (!CanMove)   // no legal moves
            {
                if (isPlayingBlack) UI.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)BlackWins);
                else UI.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)WhiteWins);
            }
            if (GetNumWhitePieces() == 0) UI.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)BlackWins);
            if (GetNumBlackPieces() == 0) UI.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)WhiteWins);
            if (GetNumPieces() == 2) countdown++;
            if (countdown == 30) UI.AllowDraw();
            isBlacksTurn = !isBlacksTurn;
            if (PlayerCanMove())
            {
                UI.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)UI.SetTurn);
            }
            else
            {
                if (isBlacksTurn) UI.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)WhiteWins);
                else UI.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)BlackWins);
            }
        }

        private void FillGrid()
        {
            bool canMove = false;

            for (int col = 0; col < 8; col++)   // layer 0 jumps
            {
                for (int row = 0; row < 8; row++)
                {
                    if (gameBoard[col, row] == null) continue;
                    if (!(gameBoard[col, row].isBlack ^ isBlacksTurn))
                    {
                        if (gameBoard[col, row].GetJumps(new Turn(col, row, gameBoard, -1), grid[0]))// fill layer 0 with jumps
                        {
                            canMove = true;
                        }
                    }
                }
            }

            if (!canMove)
            {
                for (int col = 0; col < 8; col++)   // layer 0 slides
                {
                    for (int row = 0; row < 8; row++)
                    {
                        if (gameBoard[col, row] == null) continue;
                        if (!(gameBoard[col, row].isBlack ^ isBlacksTurn))
                        {
                            gameBoard[col, row].GetSlides(new Turn(col, row, gameBoard, -1), grid[0]);// fill layer 0 with slides
                        }
                    }
                }
            }

            for (int s = 1; s < strata; s++) // ----------- layer 1 & up ------------
            {
                for (int i = 0; i < grid[s-1].Count; i++) // parent
                {
                    canMove = false;
                    for (int col = 0; col < 8; col++)       //
                    {                                       // piece
                        for (int row = 0; row < 8; row++)   // 
                        {
                            if (grid[s-1][i].board[col, row] == null) continue;                         // ignore empty squares
                            if ((grid[s-1][i].board[col, row].isBlack ^ isBlacksTurn) == (s % 2 == 1))  // alternate colours on consecutive stratas
                            {
                                if (grid[s - 1][i].board[col, row].GetJumps(new Turn(col, row, grid[s - 1][i].board, i), grid[s]))
                                {
                                    canMove = true;
                                }
                            }
                        }
                    }
                    if (!canMove)
                    {
                        for (int col = 0; col < 8; col++)       //
                        {                                       // piece
                            for (int row = 0; row < 8; row++)   // 
                            {
                                if (grid[s - 1][i].board[col, row] == null) continue;                         // ignore empty squares
                                if ((grid[s - 1][i].board[col, row].isBlack ^ isBlacksTurn) == (s % 2 == 1))  // alternate colours on consecutive stratas
                                {
                                    if (grid[s - 1][i].board[col, row].GetSlides(new Turn(col, row, grid[s - 1][i].board, i), grid[s]))
                                    {
                                        canMove = true;
                                    }
                                }
                            }
                        }
                    }
                    if (!canMove)
                    {
                        Turn t = Piece.CopyTurn(grid[s - 1][i]);
                        t.parent = i;
                        grid[s].Add(t); // if no moves copy parent
                    }
                }// parent
            }// layer
        }

        private void SelectBestWhiteMoves(List<Turn> listSelected, List<Turn> listMoves)
        {
            int parent = -1;
            int floor = 1000000;
            Turn t = new Turn();

            for (int i = 0; i < listMoves.Count; i++) // get lowest score from each parent into listSelected
            {
                if (listMoves[i].parent == parent)
                {
                    if (listMoves[i].score < floor)
                    {
                        floor = listMoves[i].score;
                        t = listMoves[i];
                    }
                }
                else
                {
                    if (i > 0) listSelected.Add(t);
                    t = listMoves[i];
                    parent = listMoves[i].parent;
                    floor = listMoves[i].score;
                }
            }
            listSelected.Add(t);
        }

        private void SelectBestBlackMoves(List<Turn> listSelected, List<Turn> listMoves)
        {
            int parent = -1;
            int ceiling = -1000000;
            Turn t = new Turn();

            for (int i = 0; i < listMoves.Count; i++) // get highest score from each parent into listSelected
            {
                if (listMoves[i].parent == parent)
                {
                    if (listMoves[i].score > ceiling)
                    {
                        ceiling = listMoves[i].score;
                        t = listMoves[i];
                    }
                }
                else
                {
                    if (i > 0) listSelected.Add(t);
                    t = listMoves[i];
                    parent = listMoves[i].parent;
                    ceiling = listMoves[i].score;
                }
            }
            listSelected.Add(t);
        }

        private void ComputeMove()
        {
            strata = 2 * level;// 24-6 5-8 
            if (GetNumPieces() < 4) strata += 2;
            grid = new List<Turn>[strata];              // grid contains tree of possible moves

            for (int i = 0; i < strata; i++)
            {
                grid[i] = new List<Turn>();
            }

            FillGrid();                                 // fill grid with possible moves
            CanMove = (grid[0].Count > 0);              // check if move is possible
            if (!CanMove)
            {
                UI.Dispatcher.BeginInvoke((ThreadStart)EndTurn, null);
                return;
            }

            if (grid[0].Count == 1)     // only one move!!!
            {
                computerTurn = grid[0][0];
                UI.Dispatcher.BeginInvoke((ThreadStart)Execute, null);
                return;
            }
            foreach (Turn t in grid[strata - 1]) t.GetScore(); // - generate scores for each Turn in top layer -

            List<Turn> list = new List<Turn>();         // temporary storage list
            for (int s = strata - 1; s >= 0; s--)
            {
                list.Clear();
                if ((s % 2 == 1) ^ isBlacksTurn)        // (odd && white) || (Even && black)
                {
                    SelectBestBlackMoves(list, grid[s]);
                }
                else SelectBestWhiteMoves(list, grid[s]);

                if (s > 0)                                  // not lowest layer
                {
                    for (int i = 0; i < list.Count; i++)    //
                    {                                       //
                        grid[s-1][i].score = list[i].score;   // pass scores down to lower layer
                    }
                }
                grid[s].Clear();
            }
            computerTurn = list[0];
            UI.Dispatcher.BeginInvoke((ThreadStart)Execute, null);
        }

        private int GetNumPieces()
        {
            int numPieces = 0;
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    if (gameBoard[col, row] != null) numPieces++;
                }
            }
            return numPieces;
        }

        private int GetNumBlackPieces()
        {
            int numPieces = 0;
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    if (gameBoard[col, row] != null)
                    {
                        if (gameBoard[col, row].isBlack) numPieces++;
                    }
                }
            }
            return numPieces;
        }

        private int GetNumWhitePieces()
        {
            int numPieces = 0;
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    if (gameBoard[col, row] != null)
                    {
                        if (!gameBoard[col, row].isBlack) numPieces++;
                    }
                }
            }
            return numPieces;
        }

        private void Execute()
        {
            Turn t = computerTurn;
            for (int i = 0; i < t.moves.Count; i++)
            {
                Piece pc = gameBoard[t.moves[i].fromCol, t.moves[i].fromRow];
                gameBoard[t.moves[i].fromCol, t.moves[i].fromRow] = null;
                if (t.moves[i].toCol < 8)
                {
                    gameBoard[t.moves[i].toCol, t.moves[i].toRow] = pc;
                    pc.SetPos(t.moves[i].toCol, t.moves[i].toRow);
                }
                UI.MovePiece(t.moves[i].fromCol, t.moves[i].fromRow, t.moves[i].toCol, t.moves[i].toRow);

                if ((t.moves[i].toRow == 7) && (pc is BlackPiece))
                {
                    gameBoard[t.moves[i].toCol, 7] = new BlackKing(this, t.moves[i].toCol, 7);
                    UI.MakeKing(t.moves[i].toCol, 7);
                }
                if ((t.moves[i].toRow == 0) && (pc is WhitePiece))
                {
                    gameBoard[t.moves[i].toCol, 0] = new WhiteKing(this, t.moves[i].toCol, 0);
                    UI.MakeKing(t.moves[i].toCol, 0);
                }
            }
            EndTurn();
        }

        private void BlackWins()
        { 
            gameInProgress = false;
            UI.GameOver(true);
        }

        private void WhiteWins()
        { 
            gameInProgress = false;
            UI.GameOver(false);
        }

// ------------ public methods -------------
        public void NewGame(bool playBlack)
        {
            UI.ClearBoard();
            SetupPieces();
            isPlayingBlack = playBlack;
            isBlacksTurn = true;
            UI.SetTurn();
            isChained = false;
            gameInProgress = true;
        }

        public void SetLevel(int level)
        {
            this.level = level;
        }

        public bool Move(int fromCol, int fromRow, int toCol, int toRow)
        {
            if(!gameInProgress) return false;
            CanMove = true;
            Piece pc = gameBoard[fromCol, fromRow];
            if (pc == null) return false;
            if(enforceTurn) { if (isBlacksTurn ^ pc.isBlack) return false; }// enforce right to move (black's/white's turn)

            if (pc.Jump(toCol, toRow))              // if player can jump
            {
                if (MakeKing(toCol, toRow)) isChained = false;
                UI.SetChained(isChained);     // chained?
                if(!isChained) EndTurn();
                return true;
            }

            if (!PlayerCanJump())     // if player can't jump
            {
                if (pc.Slide(toCol, toRow))
                {
                    MakeKing(toCol, toRow);
                    EndTurn();
                    return true;
                }
            }
            return false;// true;// 
        }

        public void MakeNextMove()
        {
            if (!gameInProgress) return;
            Thread thread = new Thread(ComputeMove);
            thread.Start();
        }

// ----------- Test methods ------------
        public void Test(int col, int row)
        {
//            CheckGrid();
//            MakeNextMove();
            ShowMoves(col, row);
        }

        public void AddPiece(int col, int row, bool isBlack, bool isKing)
        {
            if(isBlack)
            {
                if(isKing)
                {
                    gameBoard[col, row] = new BlackKing(this, col, row);
                    UI.RemovePiece(col, row);
                    UI.AddPiece(col, row, true, true);
                }
                else
                {
                    gameBoard[col, row] = new BlackPiece(this, col, row);
                    UI.RemovePiece(col, row);
                    UI.AddPiece(col, row, true, false);
                }
            }
            else
            {
                if (isKing)
                {
                    gameBoard[col, row] = new WhiteKing(this, col, row);
                    UI.RemovePiece(col, row);
                    UI.AddPiece(col, row, false, true);
                }
                else
                {
                    gameBoard[col, row] = new WhitePiece(this, col, row);
                    UI.RemovePiece(col, row);
                    UI.AddPiece(col, row, false, false);
                }
            }
        }

        public void RemovePiece(int col, int row)
        {
            gameBoard[col, row] = null;
            UI.RemovePiece(col, row);
        }

        public void ShowMoves(int col, int row) // test: 
        {
            if ((col < 0) || (col > 7)) return;
            if ((row < 0) || (row > 7)) return;
            UI.UnHighlightAllSquares();
            if ((col == markedCol) && (row == markedRow))
            {
                markedCol = -1;
                markedRow = -1;
                return;
            }
            markedCol = col;
            markedRow = row;
            List<Turn> list = new List<Turn>();
            Turn turn = new Turn(col, row, gameBoard);

            if (gameBoard[col, row] != null)
            {
                if (!gameBoard[col, row].GetJumps(turn, list))
                {
                    gameBoard[col, row].GetSlides(turn, list);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = 0; j < list[i].moves.Count; j++)
                    {
                        UI.MarkSquare(list[i].moves[j].toCol, list[i].moves[j].toRow, j);
                    }
                }
            }
        }

        public void ClearAll()
        {
            ClearBoard();
            UI.Reset();
            UI.ClearBoard();
        }

        public void TestSetup()
        {
            ClearBoard();
            UI.Reset();
            gameInProgress = true;
            isBlacksTurn = true;//false;// 
            isPlayingBlack = true;
            UI.SetTurn();

            AddBlackPiece(1, 1, true);//);//
//            AddBlackPiece(6, 6);//, true);//
//            AddBlackPiece(7, 1, true);//);//
//            AddBlackPiece(5, 1);//, true);//
            AddWhitePiece(2, 2);//, true);//
            AddWhitePiece(4, 4);//, true);//
            AddWhitePiece(0, 6, true);//);//
//            AddWhitePiece(7, 1);//, true);//
//            MakeNextMove();
        }


    } // Class Game
} // Namespace Snotsoft.Game
