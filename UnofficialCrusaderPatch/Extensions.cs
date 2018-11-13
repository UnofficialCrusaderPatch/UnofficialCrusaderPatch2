using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UnofficialCrusaderPatch
{
    static class Extensions
    {
        public static double MeasureHeight(this TextBlock block)
        {
            block.Arrange(new Rect());
            return block.ActualHeight;
        }
    }
}
