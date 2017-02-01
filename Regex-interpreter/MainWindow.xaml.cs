using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Regex_interpreter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Prop

        public static string _pattern;

        #endregion

        #region Methods
        private void RegexBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RegexBox.Text == "")
                Clearhighlight();
        }

        private async void RegexBox_KeyUp(object sender, KeyEventArgs e)
        {
            await Task.Delay(500);
            if (e.Key == Key.Enter)
            {
                _pattern = RegexBox.Text;
                Clearhighlight();
                await AddRange(RichBox);
                Highlight();
            }
        }

        private void Github_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Shadyzpop/Regex_interpreter");
        }
        #endregion

        #region Highlighter
        public void Highlight()
        {
            foreach (var a in synx.Ranges)
            {
                TextRange range = new TextRange(a.Startindex, a.Endindex);
                range.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Crimson);
            }
            synx.Ranges.Clear();
        }

        public void Clearhighlight()
        {
            TextRange range = new TextRange(RichBox.Document.ContentStart, RichBox.Document.ContentEnd);
            SolidColorBrush def = new SolidColorBrush(Color.FromRgb(37, 37, 37));
            range.ApplyPropertyValue(TextElement.BackgroundProperty, def);
        }
        #endregion

        #region Tasks
        public Task<int> AddRange(RichTextBox RichT)
        {
            return Task.Run(() =>
            {
                string pattern = @_pattern;
                TextPointer pointer = RichT.Document.ContentStart;
                try
                {
                    while (pointer != null)
                    {
                        if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                        {
                            string textRun = pointer.GetTextInRun(LogicalDirection.Forward);
                            MatchCollection matches = Regex.Matches(textRun, pattern);
                            foreach (Match match in matches)
                            {
                                synx.Tags tag = new synx.Tags();
                                int startIndex = match.Index;
                                int length = match.Length;
                                TextPointer start = pointer.GetPositionAtOffset(startIndex);
                                TextPointer end = start.GetPositionAtOffset(length);
                                tag.Startindex = start;
                                tag.Endindex = end;
                                synx.Ranges.Add(tag);
                            }
                        }

                        pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
                    }
                    return 1;
                }
                catch { return 0; }
            });
        }
        #endregion
    }
    #region synxClass
    public class synx
    {
        public static List<Tags> Ranges = new List<Tags>();
        public struct Tags
        {
            public TextPointer Startindex;
            public TextPointer Endindex;
        }
    }

    #endregion
}