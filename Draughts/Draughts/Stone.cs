using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Draughts
{
    class Stone
    {
        public int col, row;
        public Canvas disc = new Canvas();
        public Path stone = new Path();

        public Stone(int col, int row)
        {
            this.col = col;
            this.row = row;
            disc.Children.Add(stone);
            Canvas.SetLeft(stone, 0);
            Canvas.SetBottom(stone, 0);
        }
    }

// Black Stone Class
    class BlackStone : Stone
    {
        public BlackStone(int col, int row)
            : base(col, row)
        {
            stone.Data = (GeometryGroup)Application.Current.TryFindResource("stone");
            stone.Stroke = (Brush)Application.Current.FindResource("strokeB");
            stone.Fill = (Brush)Application.Current.FindResource("fillB"); //rgb;
        }
    }

// White Stone Class
    class WhiteStone : Stone
    {
        public WhiteStone(int col, int row)
            : base(col, row)
        {
            stone.Data = (GeometryGroup)Application.Current.TryFindResource("stone");
            stone.Stroke = (Brush)Application.Current.FindResource("strokeW");
            stone.Fill = (Brush)Application.Current.FindResource("fillW");
        }
    }

    // Black King Class
    class BlackKingStone : Stone
    {
        public Path crownB = new Path();
        public Path crownG = new Path();

        public BlackKingStone(int col, int row)
            : base(col, row)
        {
            stone.Data = (GeometryGroup)Application.Current.TryFindResource("king");
            stone.Stroke = (Brush)Application.Current.FindResource("strokeB");
            stone.Fill = (Brush)Application.Current.FindResource("fillB");
            disc.Children.Add(crownB);
            disc.Children.Add(crownG);
            Canvas.SetLeft(crownB, 1);
            Canvas.SetBottom(crownB, 8);
            Canvas.SetBottom(crownG, 9);
            crownB.Data = (GeometryGroup)Application.Current.TryFindResource("crown");
            crownG.Data = (GeometryGroup)Application.Current.TryFindResource("crown");
            crownB.Stroke = (Brush)Application.Current.FindResource("strokeBS");
            crownG.Stroke = (Brush)Application.Current.FindResource("strokeBG");
        }
    }

    // White King Class
    class WhiteKingStone : Stone
    {
        public Path crownW = new Path();
        public Path crownG = new Path();

        public WhiteKingStone(int col, int row)
            : base(col, row)
        {
            stone.Data = (GeometryGroup)Application.Current.TryFindResource("king");
            stone.Stroke = (Brush)Application.Current.FindResource("strokeW");
            stone.Fill = (Brush)Application.Current.FindResource("fillW");
            disc.Children.Add(crownW);
            disc.Children.Add(crownG);
            Canvas.SetLeft(crownW, 1);
            Canvas.SetBottom(crownW, 8);
            Canvas.SetBottom(crownG, 9);
            crownW.Data = (GeometryGroup)Application.Current.TryFindResource("crown");
            crownG.Data = (GeometryGroup)Application.Current.TryFindResource("crown");
            crownW.Stroke = (Brush)Application.Current.FindResource("strokeWS");
            crownG.Stroke = (Brush)Application.Current.FindResource("strokeWG");
        }
    }


}
