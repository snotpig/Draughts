using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Snotsoft.Games;

namespace Draughts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game game;
        private bool gameInProgress = false;
        private bool playingBlack = true;
        private bool isBlacksTurn = true;
        private bool highlighted = false;
        private bool isMoving = false;
        private bool isKinging = false;
        private bool isChained = false;
        private bool isBlackWin;
        private bool isDraw;
        private bool firstMove;
        private RadialGradientBrush highlightBrush, redBrush, greenBrush, blueBrush;
        private LinearGradientBrush blackBrush, whiteBrush;
        private Rectangle[,] BoardSquares = new Rectangle[8, 8];
        private Rectangle[,] GameBoard = new Rectangle[8,8];
        private List<Stone> stones = new List<Stone>();
        private List<Move> moves = new List<Move>();
        private List<Move> kingings = new List<Move>();
        private int fromCol, fromRow;
        private int takenBlack = 0;
        private int takenWhite = 0;
        private DispatcherTimer timerMoves = new DispatcherTimer();     // Timer for Multiple moves
        private DispatcherTimer timerTakePiece = new DispatcherTimer(); // Timer for delayed piece removal
        private DispatcherTimer timerMakeKing = new DispatcherTimer();  // Timer for Making King
        private DispatcherTimer timerSetTurn = new DispatcherTimer();   // Timer for Set Turn
        private DispatcherTimer timerGameOver = new DispatcherTimer();  // Timer for Game Over
        private int takeCol, takeRow;                                   // temporary storage for take square
        private ContextMenu context1;
        ColorAnimation alpha0;                              // animation for
        ColorAnimation alpha1;                              // Game Over
        DoubleAnimation size;                               // animation for txtPlaying
        private bool ignoreWhiteSquares = true;
        private bool enforcePlayerOnly = true;
        private bool autoPlay = true;
        private bool testMode = false;
        private bool setupMode = false;
        private int primed = 0;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                Rect bounds = Properties.Settings.Default.WindowPosition;
                this.Top = bounds.Top;
                this.Left = bounds.Left;
                // Restore the size only for a manually sized window.
                if (this.SizeToContent == SizeToContent.Manual)
                {
                    this.Width = bounds.Width;
                    this.Height = bounds.Height;
                }
            }
            catch
            {
            } 
            CreateBoard();
            timerMoves.Tick += new EventHandler(timerMoves_Tick);           // Create Event Handler for Move timer
            timerMoves.Interval = new TimeSpan(4000000);                    // set interval for Move timer
            timerTakePiece.Tick += new EventHandler(timerTakePiece_Tick);   // Create Event Handler for TakePiece timer
            timerTakePiece.Interval = new TimeSpan(2500000);                // set interval for TakePiece timer
            timerMakeKing.Tick += new EventHandler(timerMakeKing_Tick);     // Create Event Handler for King timer
            timerMakeKing.Interval = new TimeSpan(6000000);                 // set interval for King timer
            timerSetTurn.Tick += new EventHandler(timerSetTurn_Tick);       // Create Event Handler of Set Turn
            timerSetTurn.Interval = new TimeSpan(3000000);                  // set interval for Set Turn
            timerGameOver.Tick += new EventHandler(timerGameOver_Tick);     // Create Event Handler for Game Over
            timerGameOver.Interval = new TimeSpan(15000000);                // set interval for GameOver
            size = new DoubleAnimation();
            size.Duration = TimeSpan.FromMilliseconds(900);                 // set duration for txtPlaying animation
            SetPlayingBlack(true); //false);// 
            game = new Game(this);
//            TestMode(true);// <------- TestMode!!!
            NewGame(true);
        }

        private void NewGame(bool playBlack)
        {
            SetPlayingBlack(playBlack);
            UnHighlightAllSquares();
            isBlacksTurn = true;
            isChained = false;
            highlighted = false;
            isMoving = false;
            isKinging = false;
            isDraw = false;
            gameInProgress = true;
            game.NewGame(playingBlack);
            if (!playingBlack)
            {
                firstMove = true;
                game.MakeNextMove();
            }
        }

        private void CreateBoard()
        {
            whiteBrush = new LinearGradientBrush(Color.FromRgb(245, 245, 225), Color.FromRgb(250, 245, 225), 45);
            blackBrush = new LinearGradientBrush(Color.FromRgb(100, 80, 60), Color.FromRgb(80, 60, 40), 45);
            highlightBrush = new RadialGradientBrush(Color.FromRgb(160, 120, 80), Color.FromRgb(255, 245, 100));
            redBrush = new RadialGradientBrush(Color.FromRgb(255, 70, 70), Color.FromRgb(100, 80, 80));
            greenBrush = new RadialGradientBrush(Color.FromRgb(70, 255, 70), Color.FromRgb(100, 80, 80));
            blueBrush = new RadialGradientBrush(Color.FromRgb(70, 70, 255), Color.FromRgb(100, 80, 80));
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    BoardSquares[col, row] = new Rectangle();
                    if ((col + row) % 2 == 0)
                    {
                        BoardSquares[col, row].Fill = blackBrush;
                    }
                    else
                    {
                        BoardSquares[col, row].Fill = whiteBrush;
                    }
                    GridBoard.Children.Add(BoardSquares[col, row]);
                    Grid.SetColumn(BoardSquares[col, row], col);
                    Grid.SetRow(BoardSquares[col, row], 7 - row);
                }
            }
        }

        private void SetPlayingBlack(bool playBlack)
        {
            playingBlack = playBlack;
            int row, col;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (playBlack)
                    {
                        row = i;
                        col = j;
                    }
                    else
                    {
                        row = 7 - i;
                        col = 7 - j;
                    }
                    GameBoard[row, col] = BoardSquares[i, j];
                }
            }
        }

        private void HighlightSquare(int col, int row)
        {
//            Orientate(ref col, ref row);
            GameBoard[col, row].Fill = highlightBrush;
            highlighted = true;
        }

        private void UnHighlightSquare(int col, int row)
        {
            GameBoard[col, row].Fill = blackBrush;
            highlighted = false;
        }

        public void UnHighlightAllSquares()
        {
            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < 8; row++)
                {
                    if ((col + row) % 2 == 0)
                    {
                        GameBoard[col, row].Fill = blackBrush;
                    }
                }
            }
            highlighted = false;
        }

        private void MovePieceAnimated(Move move)
        {
            if (move.toCol > 7)
            {
                TakePieceAnimated(move.fromCol, move.fromRow);
                return;
            }

            for (int i = 0; i < stones.Count; i++)
            {
                if ((stones[i].col == move.fromCol) && (stones[i].row == move.fromRow))
                {
                    stones[i].col = move.toCol;
                    stones[i].row = move.toRow;
                    Orientate(ref move.fromCol, ref move.fromRow);  // orientate for animation
                    Orientate(ref move.toCol, ref move.toRow);      //
                    double fromX = move.fromCol * 50 + 150;
                    double fromY = move.fromRow * 50 + 5;
                    double toX = move.toCol * 50 + 150;
                    double toY = move.toRow * 50 + 5;
                    DoubleAnimation daX = new DoubleAnimation();
                    DoubleAnimation daY = new DoubleAnimation();
                    daX.From = fromX;
                    daY.From = fromY;
                    daX.To = toX;
                    daY.To = toY;
                    daX.Duration = daY.Duration = TimeSpan.FromMilliseconds(300);
                    canvas.Children.Remove(stones[i].disc);
                    canvas.Children.Add(stones[i].disc);
                    stones[i].disc.BeginAnimation(Canvas.LeftProperty, daX);
                    stones[i].disc.BeginAnimation(Canvas.BottomProperty, daY);
                    break;
                }
            }
        }

        private void TakePieceAnimated(int col, int row)
        {
            for (int i = 0; i < stones.Count; i++)
            {
                if ((stones[i].col == col) && (stones[i].row == row))
                {
                    Orientate(ref col, ref row); // reverse if playing white
                    bool isBlack = ((stones[i] is BlackStone) || (stones[i] is BlackKingStone));
                    stones[i].col = 8;  // taken pieces
                    stones[i].row = 8;  // set to (8, 8)
                    _square sq = NextFreeSpace(isBlack);
                    if (isBlack) takenBlack++;
                    else takenWhite++;
                    DoubleAnimation daX = new DoubleAnimation();
                    DoubleAnimation daY = new DoubleAnimation();
                    double fromX = col * 50 + 150;
                    double fromY = row * 50;
                    double toX = sq.col;
                    double toY = sq.row;
                    daX.From = fromX;
                    daY.From = fromY;
                    daX.To = toX;
                    daY.To = toY;
                    daX.Duration = daY.Duration = TimeSpan.FromMilliseconds(350);
                    canvas.Children.Remove(stones[i].disc); // get disc to top
                    canvas.Children.Add(stones[i].disc);    // of Z-order
                    stones[i].disc.BeginAnimation(Canvas.LeftProperty, daX);
                    stones[i].disc.BeginAnimation(Canvas.BottomProperty, daY);
                    break;
                }
            }
        }

        private void MakeKingAnimated(int col, int row)
        {
            for (int i = 0; i < stones.Count; i++)
            {
                if ((stones[i].col == col) && (stones[i].row == row))
                {
                    canvas.Children.Remove(stones[i].disc);
                    if (stones[i] is BlackStone)
                    {
                        stones.RemoveAt(i);
                        stones.Insert(i, new BlackKingStone(col, row));
                    }
                    if (stones[i] is WhiteStone)
                    {
                        stones.RemoveAt(i);
                        stones.Insert(i, new WhiteKingStone(col, row));
                    }
                    canvas.Children.Add(stones[i].disc);
                    Orientate(ref col, ref row);
                    Canvas.SetLeft(stones[i].disc, 50 * col + 150);
                    Canvas.SetBottom(stones[i].disc, 50 * row + 5);
                }
            }
        }

        public void SetTurnAnimated()
        {
            if (isBlacksTurn) Message("Black's Turn");
            else Message("White's Turn");
            timerSetTurn.Stop();
        }

        private void GameOverAnimated()
        {
            canvas.Children.Remove(txtGameOver);
            canvas.Children.Add(txtGameOver);
            txtGameOver.Text = "GAME OVER";
            var lgb = new LinearGradientBrush(Color.FromArgb(0, 100, 100, 255), Color.FromArgb(200, 200, 200, 255), 90);
            txtGameOver.Foreground = lgb;

            alpha0 = new ColorAnimation();
            alpha1 = new ColorAnimation();
            size = new DoubleAnimation();
            size.Completed +=new EventHandler(size_Completed);
            alpha0.From = Color.FromArgb(0, 100, 100, 255);
            alpha0.To = Color.FromArgb(200, 100, 100, 255);
            alpha0.Duration = TimeSpan.FromMilliseconds(900);
            alpha1.From = Color.FromArgb(0, 200, 200, 255);
            alpha1.To = Color.FromArgb(200, 200, 200, 255);
            alpha1.Duration = TimeSpan.FromMilliseconds(500);
            size.From = 14;
            size.To = 22;
            lgb.GradientStops[0].BeginAnimation(GradientStop.ColorProperty, alpha0);
            lgb.GradientStops[1].BeginAnimation(GradientStop.ColorProperty, alpha1);
            txtPlaying.BeginAnimation(FontSizeProperty, size);
            if (isDraw) Message("Draw!");
            else
            {
                if (isBlackWin ^ !playingBlack) Message("You win!");
                else
                {
                    if (isBlackWin) Message("Black Wins!");
                    else Message("White Wins!");
                }
            }
            timerGameOver.Stop();
        }

        private void Message(string msg)
        {
            txtPlaying.Text = msg;
        }

        // ------------- Left-Click ---------------
        private void LeftClicked(Point pt)
        {
            primed = 0;
            if (!gameInProgress) return;
            int col = (int)(pt.X / 50 - 3);
            int row = 7 - (int)(pt.Y / 50);
            Orientate(ref col, ref row); // reverse if playing white
            if (ignoreWhiteSquares && ((col + row) % 2 == 1)) return; // ignore clicks on white squares
            if(enforcePlayerOnly) { if (playingBlack ^ isBlacksTurn) return; } // ignore out of turn move attempts

            if (!highlighted)
            {
                foreach (Stone stone in stones)
                {
                    if ((stone.col == col) && (stone.row == row)) // only highlight occupied squares
                    {
                        if (enforcePlayerOnly)
                        {
                            if (((stone is BlackStone) || (stone is BlackKingStone)) ^ (playingBlack)) break;
                        }
                        fromCol = col;
                        fromRow = row;
                        HighlightSquare(col, row);
                        break;
                    }
                }
            }
            else
            {
                UnHighlightSquare(fromCol, fromRow);
                if ((game.Move(fromCol, fromRow, col, row)) && (!isChained) && (autoPlay)) game.MakeNextMove(); // computer move
                if (isChained)
                {
                    fromCol = col;
                    fromRow = row;
                    HighlightSquare(col, row);
                }
            }
        }

        private void board_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point pt = Mouse.GetPosition(canvas);
            if(pt.X > 149) LeftClicked(pt);
        }

        private void cmdNewB_Click(object sender, RoutedEventArgs e)
        {
            NewGame(true);
        }

        private void cmdNewW_Click(object sender, RoutedEventArgs e)
        {
            NewGame(false);
        }

        private void timerMoves_Tick(object sender, EventArgs e)
        {
            if (moves.Count > 0)
            {
                MovePieceAnimated(moves[0]);
                moves.RemoveAt(0);
            }
            if(moves.Count == 0) 
            {
                timerMoves.Stop();
                isMoving = false;
            }
        }

        private void timerTakePiece_Tick(object sender, EventArgs e)
        {
            TakePieceAnimated(takeCol, takeRow);
            timerTakePiece.Stop();
        }

        private void timerMakeKing_Tick(object sender, EventArgs e)
        {
            if (isMoving) return;
            if (kingings.Count > 0)
            {
                MakeKingAnimated(kingings[0].toCol, kingings[0].toRow);
                kingings.RemoveAt(0);
            }
            else
            {
                isKinging = false;
                timerMakeKing.Stop();
            }
        }

        private void timerSetTurn_Tick(object sender, EventArgs e)
        {
            if (isMoving) return;
            SetTurnAnimated();
        }

        private void timerGameOver_Tick(object sender, EventArgs e)
        {
            GameOverAnimated();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            double _aspectRatio = 1.25;
            var percentWidthChange = Math.Abs(sizeInfo.NewSize.Width - sizeInfo.PreviousSize.Width) / sizeInfo.PreviousSize.Width;
            var percentHeightChange = Math.Abs(sizeInfo.NewSize.Height - sizeInfo.PreviousSize.Height) / sizeInfo.PreviousSize.Height;

            if (percentWidthChange > percentHeightChange)
                this.Height = sizeInfo.NewSize.Width / _aspectRatio;
            else
                this.Width = sizeInfo.NewSize.Height * _aspectRatio;

            base.OnRenderSizeChanged(sizeInfo);
        }

        private void cmdNewB_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation stretch = new DoubleAnimation();
            stretch.From = 10;
            if (testMode) stretch.To = 50;
            else stretch.To = 75;
            stretch.Duration = TimeSpan.FromMilliseconds(200);
            cmdNewB.BeginAnimation(HeightProperty, stretch);
        }

        private void cmdNewB_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation stretch = new DoubleAnimation();
            stretch.From = cmdNewB.ActualHeight;
            stretch.To = 10;
            stretch.Duration = TimeSpan.FromMilliseconds(300);
            cmdNewB.BeginAnimation(HeightProperty, stretch);
        }

        private void cmdNewW_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation stretch = new DoubleAnimation();
            stretch.From = 10;
            if (testMode) stretch.To = 50;
            else stretch.To = 75;
            stretch.Duration = TimeSpan.FromMilliseconds(200);
            cmdNewW.BeginAnimation(HeightProperty, stretch);
        }

        private void cmdNewW_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation stretch = new DoubleAnimation();
            stretch.From = cmdNewB.ActualHeight;
            stretch.To = 10;
            stretch.Duration = TimeSpan.FromMilliseconds(300);
            cmdNewW.BeginAnimation(HeightProperty, stretch);
        }

        private void cmdClear_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation stretch = new DoubleAnimation();
            stretch.From = 10;
            stretch.To = 50;
            stretch.Duration = TimeSpan.FromMilliseconds(200);
            cmdClear.BeginAnimation(HeightProperty, stretch);
        }

        private void cmdClear_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation stretch = new DoubleAnimation();
            stretch.From = cmdClear.ActualHeight;
            stretch.To = 10;
            stretch.Duration = TimeSpan.FromMilliseconds(300);
            cmdClear.BeginAnimation(HeightProperty, stretch);
        }

        private void draw_Click(object sender, RoutedEventArgs e)
        {
            isDraw = true;
            GameOverAnimated();
            DoubleAnimation width = new DoubleAnimation();
            DoubleAnimation height = new DoubleAnimation();
            width.From = cmdDraw.ActualWidth;
            width.To = 0;
            height.From = cmdDraw.ActualHeight;
            height.To = 0;
            width.Duration = TimeSpan.FromMilliseconds(500);
            height.Duration = TimeSpan.FromMilliseconds(500);
            cmdDraw.BeginAnimation(WidthProperty, width);
            cmdDraw.BeginAnimation(HeightProperty, height);
        }

        private void Orientate(ref int a, ref int b)
        {
            if (!playingBlack)
            {
                a = 7 - a;
                b = 7 - b;
            }
        }

        private _square NextFreeSpace(bool isBlack)
        {
            bool toTop = (!(isBlack ^ playingBlack));
            int taken;
            if(isBlack) taken = takenBlack;
            else taken = takenWhite;
            _square sq;
            sq.col = 40 * (taken % 3) + 5;
            sq.row = (toTop ? 355 : 5) + (toTop ? -40 : 40) * (int)(taken / 3);
            return sq;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.WindowPosition = RestoreBounds;
            Properties.Settings.Default.Save();
        }

        // ----------------- Public methods -------------------
        public void ClearBoard()
        {
            canvas.Children.Clear();
            txtGameOver.Text = "";
            canvas.Children.Add(txtGameOver);
            Canvas.SetLeft(txtGameOver, 155);
            Canvas.SetBottom(txtGameOver, 160);
            stones.Clear();
            takenBlack = takenWhite = 0;
            size.From = txtPlaying.FontSize;
            size.To = 14;
            txtPlaying.BeginAnimation(FontSizeProperty, size);
        }

        public void AddPiece(int col, int row, bool isBlack, bool isKing = false)
        {
            Stone stone;

            if(isBlack)
            {
                if(isKing)
                {
                    stone = new BlackKingStone(col, row);
                }
                else
                {
                    stone = new BlackStone(col, row);
                }
            }
            else
            {
                if(isKing)
                {
                    stone = new WhiteKingStone(col, row);
                }
                else
                {
                    stone = new WhiteStone(col, row);
                }
            }
            Orientate(ref col, ref row); // reverse if playing white
            canvas.Children.Add(stone.disc);
            Canvas.SetLeft(stone.disc, 50 * col + 150);
            Canvas.SetBottom(stone.disc, 50 * row + 5);
            stones.Add(stone);
        }

        public void MovePiece(int fromCol, int fromRow, int toCol, int toRow)
        {
            if (!(isMoving || firstMove))
            {
                MovePieceAnimated(new Move(fromCol, fromRow, toCol, toRow));
                isMoving = true;
            }
            else
            {
                moves.Add(new Move(fromCol, fromRow, toCol, toRow));
                firstMove = false;
            }
            timerMoves.Start();
            cmdMove.IsEnabled = true;
        }

        public void TakePiece(int col, int row)
        {
            takeCol = col;
            takeRow = row;
            timerTakePiece.Start();
        }

        public void MakeKing(int col, int row)
        {
            kingings.Add(new Move(8, 8, col, row));
            if (!isKinging)
            {
                timerMakeKing.Start();
            }
        }

        public void SetTurn()
        {
            isChained = false;
            this.isBlacksTurn = game.isBlacksTurn;
            if (isBlacksTurn ^ playingBlack)
            {
                SetTurnAnimated();
            }
            else
            {
                timerSetTurn.Start();
            }
        }

        public void SetChained(bool isChained)
        {
            this.isChained = isChained;
        }

        public void GameOver(bool isBlackWin)
        {
            this.isBlackWin = isBlackWin;
            gameInProgress = false;
            UnHighlightAllSquares();
            timerGameOver.Start();
        }

        public void AllowDraw()
        {
            DoubleAnimation width = new DoubleAnimation();
            DoubleAnimation height = new DoubleAnimation();
            width.From = 0;
            width.To = 90;
            height.From = 0;
            height.To = 20;
            width.Duration = TimeSpan.FromMilliseconds(500);
            height.Duration = TimeSpan.FromMilliseconds(500);
            cmdDraw.BeginAnimation(WidthProperty, width);
            cmdDraw.BeginAnimation(HeightProperty, height);
        }

// --------------- test methods ------------------
        private void board_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = Mouse.GetPosition(canvas);
            int col = (int)(pt.X / 50 - 3);
            int row = 7 - (int)(pt.Y / 50);

            if (setupMode)
            {
                fromCol = col;
                fromRow = row;
            }
//            AllowDraw();
        }

        private void board_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point pt = Mouse.GetPosition(canvas);
            int col = (int)(pt.X / 50 - 3);
            int row = 7 - (int)(pt.Y / 50);
            if(primed == 3)
            {
                if ((col == 4) && (row == 4)) TestMode(true);
                else primed = 0;
            }
            else primed = 1;
            if (testMode)
            {
                if (!setupMode) game.ShowMoves(col, row);
            }
        }

        private void cmdMove_Click(object sender, RoutedEventArgs e)
        {
            if (!setupMode)
            {
                game.MakeNextMove();
                cmdMove.IsEnabled = false;
            }
            else
            {
                game.isBlacksTurn = !game.isBlacksTurn;
                SetTurn();
            }
         }

        private void cmdClear_Click(object sender, RoutedEventArgs e)
        {
            game.ClearAll();
//            game.TestSetup();
        }

        private void cmdSetup_Click(object sender, RoutedEventArgs e)
        {
            SetupMode(!setupMode);
        }

        private void cmdReturn_Click(object sender, RoutedEventArgs e)
        {
            TestMode(false);
        }

        public void MarkSquare(int col, int row, int colour)
        {
            if (col > 7) return;
            int sel = colour % 3;
            switch (sel)
            {
                case 0:
                    GameBoard[col, row].Fill = redBrush;
                    break;
                case 1:
                    GameBoard[col, row].Fill = greenBrush;
                    break;
                case 2:
                    GameBoard[col, row].Fill = blueBrush;
                    break;
            }           
        }

        public void Reset()
        {
            UnHighlightAllSquares();
            isBlacksTurn = true;
            isChained = false;
            highlighted = false;
            isMoving = false;
            isKinging = false;
            isDraw = false;
            gameInProgress = true;
            txtPlaying.FontSize = 14;
        }

        public void RemovePiece(int col, int row)
        {
            for (int i = 0; i < stones.Count; i++)
            {
                if ((stones[i].col == col) && (stones[i].row == row))
                {
                    canvas.Children.Remove(stones[i].disc);
                    stones[i].col = 8;
                    stones[i].row = 8;
                }
            }
        }

        private void TestMode(bool on)
        {
            testMode = on;
            autoPlay = !on;
            enforcePlayerOnly = !on;
            if (on)
            {
                Reset();
                txtGameOver.Text = "";  
                cmdNewB.Width = 50;
                cmdNewW.Width = 50;
                cmdMove.Visibility = System.Windows.Visibility.Visible;
                cmdSetup.Visibility = System.Windows.Visibility.Visible;
                cmdClear.Visibility = System.Windows.Visibility.Visible;
                cmdReturn.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                SetupMode(false);
                cmdNewB.Width = 75;
                cmdNewW.Width = 75;
                cmdMove.Visibility = System.Windows.Visibility.Hidden;
                cmdSetup.Visibility = System.Windows.Visibility.Hidden;
                cmdClear.Visibility = System.Windows.Visibility.Hidden;
                cmdReturn.Visibility = System.Windows.Visibility.Hidden;
                if (playingBlack ^ isBlacksTurn) game.MakeNextMove();
            }
        }

        private void SetupMode(bool on)
        {
            if (on)
            {
                txtGameOver.Text = "";
                SetTurn();
                UnHighlightAllSquares();
                setupMode = true;
                cmdSetup.Background = new SolidColorBrush(Colors.Red);
                context1 = (ContextMenu)Application.Current.TryFindResource("context1");
                context1.IsEnabled = true;
                context1.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                setupMode = false;
                cmdSetup.Background = new SolidColorBrush(Colors.ForestGreen);
                if (context1 != null)
                {
                    context1.IsEnabled = false;
                    context1.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.C) && (primed == 1))
            {
                primed = 2;
                return;
            }
            if ((e.Key == Key.J) && (primed == 2)) primed = 3;
            else primed = 0;
        }

        private void empty_Click(object sender, ExecutedRoutedEventArgs e)
        {
            game.RemovePiece(fromCol, fromRow);
        }

        private void black_Click(object sender, ExecutedRoutedEventArgs e)
        {
            game.AddPiece(fromCol, fromRow, true, false);
        }

        private void white_Click(object sender, ExecutedRoutedEventArgs e)
        {
            game.AddPiece(fromCol, fromRow, false, false);
        }

        private void kingB_Click(object sender, ExecutedRoutedEventArgs e)
        {
            game.AddPiece(fromCol, fromRow, true, true);
        }

        private void kingW_Click(object sender, ExecutedRoutedEventArgs e)
        {
            game.AddPiece(fromCol, fromRow, false, true);

        }

        private void size_Completed(object sender, EventArgs e)
        {
            double fs = txtPlaying.FontSize;
            txtPlaying.BeginAnimation(FontSizeProperty, null);
            txtPlaying.FontSize = fs;
        }

    } // class MainWindow

    // structure for square on board
    public struct _square
    {
        public int row;
        public int col;

        public static bool operator ==(_square a, _square b)
        {
            return ((a.row == b.row) && (a.col == b.col));
        }

        public static bool operator !=(_square a, _square b)
        {
            return ((a.row != b.row) || (a.col != b.col));
        }

        public override bool Equals(object ob)
        {
            if (!(ob is _square)) return false;
            _square sq = (_square)ob;
            return ((row == sq.row) && (col == sq.col));
        }

        public override int GetHashCode()
        {
            return row * col;
        }
    };

} // namespace Draughts
