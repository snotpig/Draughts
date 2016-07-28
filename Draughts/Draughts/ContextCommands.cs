using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Draughts
{
    public class DataCommands
    {
        private static RoutedUICommand delete;
        private static RoutedUICommand addBlack;
        private static RoutedUICommand addWhite;
        private static RoutedUICommand addBlackKing;
        private static RoutedUICommand addWhiteKing;

        static DataCommands()
        {
            var inputs_ = new InputGestureCollection();
            inputs_.Add(new KeyGesture(Key.Space, ModifierKeys.None, "Space"));
            delete = new RoutedUICommand("Remove Piece", "RemovePiece", typeof(DataCommands), inputs_);

            var inputsB = new InputGestureCollection();
            inputsB.Add(new KeyGesture(Key.B, ModifierKeys.Alt, "Alt+B"));
            addBlack = new RoutedUICommand(
            "Add Black Piece", "AddBlackPiece", typeof(DataCommands), inputsB);

            var inputsW = new InputGestureCollection();
            inputsW.Add(new KeyGesture(Key.W, ModifierKeys.Alt, "Alt+W"));
            addWhite = new RoutedUICommand("Add White Piece", "AddWhitePiece", typeof(DataCommands), inputsW);

            var inputsBK = new InputGestureCollection();
            inputsBK.Add(new KeyGesture(Key.B, ModifierKeys.Control, "Ctrl+B"));
            addBlackKing = new RoutedUICommand("Add Black King", "AddBlackKing", typeof(DataCommands), inputsBK);

            var inputsWK = new InputGestureCollection();
            inputsWK.Add(new KeyGesture(Key.W, ModifierKeys.Control, "Ctrl+W"));
            addWhiteKing = new RoutedUICommand("Add White King", "AddWhiteKing", typeof(DataCommands), inputsWK);

        }

        public static RoutedUICommand Delete
        {
            get { return delete; }
        }

        public static RoutedUICommand AddBlack
        {
            get { return addBlack; }
        }

        public static RoutedUICommand AddWhite
        {
            get { return addWhite; }
        }

        public static RoutedUICommand AddBlackKing
        {
            get { return addBlackKing; }
        }

        public static RoutedUICommand AddWhiteKing
        {
            get { return addWhiteKing; }
        }
    }
}
