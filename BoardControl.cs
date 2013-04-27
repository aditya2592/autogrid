using System;
using System.Drawing;
using System.Windows.Forms;

namespace MinotaurPathfinder
{
    /*
     * 
     * This class does the dirty work with drawing things on screen.
     * It has many optimizations that I developed in a previous program.
     * 
     * */
    public partial class BoardControl : UserControl
    {
        public event EventHandler MouseMoveSpecial;
        Color[,] _highlights = new Color[GRID, GRID];
        char[,] lines = new char[GRID, GRID];
        Point _pos = new Point(-1, -1);
        public Point Pos
        {
            get { return _pos; }
        }

        bool _mouseDown;
        Rectangle _thisRect;
        Rectangle _thisRectOnPaint = new Rectangle();
        Rectangle _borderRectOnPaint = new Rectangle();

        public BoardControl()
        {
            InitializeComponent();
            _thisRect = this.Bounds;
            _thisRectOnPaint.Height = _thisRectOnPaint.Width = 24;
            _borderRectOnPaint.Height = _borderRectOnPaint.Width = 23;
        }

        public void SetHighlight(Color highlightColor, int x, int y)
        {
            _highlights[x, y] = highlightColor;
           
        }
        public void Drawline(int x, int y, char direction)
        {

            lines[x, y] = direction;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _mouseDown = true;
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _mouseDown = false;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _pos.X = _pos.Y = -1;
            this.Invalidate();

            if (MouseMoveSpecial != null)
            {
                MouseMoveSpecial(null, e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_thisRect.Contains(e.Location) == false)
            {
                if (_pos.X == -1 || _pos.Y == -1)
                {
                    return;
                }
                _pos.X = _pos.Y = -1;
            }
            else
            {
                int posX = e.X / 24;
                int posY = e.Y / 24;

                if (posX == _pos.X && posY == _pos.Y)
                {
                    return;
                }
                _pos.X = posX;
                _pos.Y = posY;
            }
            this.Invalidate();

            if (MouseMoveSpecial != null)
            {
                MouseMoveSpecial(null, e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphicsContext = e.Graphics;

            for (int i = 0, startX = 0; i < GRID; i++, startX += 24)
            {
                for (int j = 0, startY = 0; j < GRID; j++, startY += 24)
                {
                    _thisRectOnPaint.X = startX;
                    _thisRectOnPaint.Y = startY;
                   
                    Color colorHere = _highlights[i, j];



                    if (_pos.X == i && _pos.Y == j)
                    {
                        if (_mouseDown)
                        {
                            graphicsContext.FillRectangle(Brushes.Black, _thisRectOnPaint);
                            
                        }
                        else
                        {

                            graphicsContext.FillRectangle(new SolidBrush(colorHere), _thisRectOnPaint);

                            _borderRectOnPaint.X = startX;
                            _borderRectOnPaint.Y = startY;

                            graphicsContext.DrawRectangle(Pens.Black, _borderRectOnPaint);

                        }
                    }
                    else
                    {
                        //if (colorHere != Color.White)
                       // {
                            Brush brush = new SolidBrush(colorHere);
                            graphicsContext.FillRectangle(brush, _thisRectOnPaint);
                            graphicsContext.DrawRectangle(Pens.Black, _thisRectOnPaint);
                        //}
                    }

                   
                    

                }
            }
            Pen skyBluePen = new Pen(Brushes.DarkViolet);
            skyBluePen.Width = 1;
            for (int i = 0, startX = 0; i < GRID; i++, startX += 24)
            {
                for (int j = 0, startY = 0; j < GRID; j++, startY += 24)
                {

                    if (lines[i, j] == 'N')
                    {
                        Point startline = new Point(), endline = new Point();
                        startline.X = startX + 12; startline.Y = startY - 12;
                        endline.X = startX + 12; endline.Y = startY + 12;
                        graphicsContext.DrawLine(skyBluePen, endline, startline);
                    }

                    else if (lines[i, j] == 'S')
                    {
                        Point startline = new Point(), endline = new Point();
                        startline.X = startX + 12; startline.Y = startY + 12;
                        endline.X = startX + 12; endline.Y = startY + 36;
                        graphicsContext.DrawLine(skyBluePen, startline, endline);
                    }
                    else if (lines[i, j] == 'W')
                    {
                        Point startline = new Point(), endline = new Point();
                        startline.X = startX + 12; startline.Y = startY + 12;
                        endline.X = startX - 12; endline.Y = startY + 12;
                        graphicsContext.DrawLine(skyBluePen, startline, endline);
                    }
                    else if (lines[i, j] == 'E')
                    {

                        Point startline = new Point(), endline = new Point();
                        startline.X = startX + 12; startline.Y = startY + 12;
                        endline.X = startX + 36; endline.Y = startY + 12;
                        graphicsContext.DrawLine(skyBluePen, endline, startline);


                    }
                }
            }
        }
    }
}
