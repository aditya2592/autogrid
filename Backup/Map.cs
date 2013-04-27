using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MinotaurPathfinder
{
    public partial class Map : Form
    {
        Pathfinder _pathfinder = new Pathfinder();

        public Map()
        {
            /*
             * 
             * Do initializations and events.
             * 
             * */
            this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();

            boardControl1.MouseClick += new MouseEventHandler(boardControl1_MouseClick);
            boardControl1.MouseMoveSpecial += new EventHandler(boardControl1_MouseMoveSpecial);
            toolStripStatusLabel1.Text = "";
        }

        void boardControl1_MouseMoveSpecial(object sender, EventArgs e)
        {
            /*
             * 
             * Called when the mouse moves to a different square.
             * 
             * */
            Point point = boardControl1.Pos;
            if (point.X == -1 || point.Y == -1)
            {
                return;
            }
            int count = _pathfinder.Squares[point.X, point.Y].DistanceSteps;
            if (count == 10000)
            {
                toolStripStatusLabel1.Text = "Impossible";
            }
            else if (count == 1)
            {
                toolStripStatusLabel1.Text = "1 step";
            }
            else if (count == 0)
            {
                toolStripStatusLabel1.Text = "Current";
            }
            else
            {
                toolStripStatusLabel1.Text = count.ToString() + " steps";
            }
        }

        void boardControl1_MouseClick(object sender, MouseEventArgs e)
        {
            Point point = boardControl1.Pos;
            if (point.X == -1 || point.Y == -1)
            {
                return;
            }
            if (e.Button == MouseButtons.Right)
            {
                /*
                 * 
                 * Right click isn't implemented!
                 * 
                 * */
                return;
            }
            if (_pathfinder.Squares[point.X, point.Y].ContentCode == SquareContent.Empty)
            {
                /*
                 * 
                 * Turn an empty square into a wall on click.
                 * 
                 * */
                _pathfinder.Squares[point.X, point.Y].ContentCode = SquareContent.Wall;
                Recalculate();
            }
            else if (_pathfinder.Squares[point.X, point.Y].ContentCode == SquareContent.Wall)
            {
                _pathfinder.Squares[point.X, point.Y].ContentCode = SquareContent.Empty;
                Recalculate();
            }
        }

        private void Recalculate()
        {
            _pathfinder.ClearLogic();
            _pathfinder.Pathfind();
            _pathfinder.HighlightPath();
            _pathfinder.DrawBoard(boardControl1);
        }

        private void ReadMap(string fileName)
        {
            /*
             * 
             * Read in a map file.
             * 
             * */
            using (StreamReader reader = new StreamReader(fileName))
            {
                int lineNum = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    char[] parts = line.ToCharArray();
                    for (int i = 0; i < parts.Length && i < 15; i++)
                    {
                        _pathfinder.Squares[i, lineNum].FromChar(parts[i]);
                    }
                    lineNum++;
                }
            }
        }


        private void mapButton_Click(object sender, EventArgs e)
        {
            /*
             * 
             * Just deals with the UI.
             * 
             * */
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                _pathfinder.ClearSquares();
                _pathfinder.ClearLogic();
                try
                {
                    ReadMap(openFileDialog1.FileName);
                    _pathfinder.Pathfind();
                    _pathfinder.HighlightPath();
                }
                catch
                {
                    toolStripStatusLabel1.Text = "IO Error";
                }
                finally
                {
                    _pathfinder.DrawBoard(boardControl1);
                }
            }
        }
    }
}
