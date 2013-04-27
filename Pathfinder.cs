#define GRID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;


namespace MinotaurPathfinder
{
    class Pathfinder
    {
        /*
         * 
         * Movements is an array of various directions.
         * 
         * */
        
        Point[] _movements;
        Point[] directions = new Point[4];
        Point startingPoint = new Point();
        Point endingPoint = new Point();
        const int GRID = 8;
        /*
         * 
         * Squares is an array of square objects.
         * 
         * */
        CompleteSquare[,] _squares = new CompleteSquare[GRID, GRID];
        public CompleteSquare[,] Squares
        {
            get { return _squares; }
            set { _squares = value; }
        }

        public Pathfinder()
        {
            InitMovements(4);
            ClearSquares();
        }

        public void InitMovements(int movementCount)
        {
            /*
             * 
             * Just do some initializations.
             * 
             * */
            if (movementCount == 4)
            {
                _movements = new Point[]
                {
                    new Point(0, -1),
                    new Point(1, 0),
                    new Point(0, 1),
                    new Point(-1, 0)
                };
            }
            else
            {
                _movements = new Point[]
                {
                    new Point(-1, -1),
                    new Point(0, -1),
                    new Point(1, -1),
                    new Point(1, 0),
                    new Point(1, 1),
                    new Point(0, 1),
                    new Point(-1, 1),
                    new Point(-1, 0)
                };
            }
        }

        public void ClearSquares()
        {
            /*
             * 
             * Reset every square.
             * 
             * */
            foreach (Point point in AllSquares())
            {
                _squares[point.X, point.Y] = new CompleteSquare();
            }
        }

        public void ClearLogic()
        {
            /*
             * 
             * Reset some information about the squares.
             * 
             * */
            foreach (Point point in AllSquares())
            {
                int x = point.X;
                int y = point.Y;
                _squares[x, y].DistanceSteps = 10000;
                _squares[x, y].IsPath = false;
            }
        }

        public void DrawBoard(BoardControl boardControl1)
        {
            /*
             * Call to the path finding function is placed from within this function
             * so that the drawing elements can be utilised directly
             * 
             * */

            startingPoint = FindCode(SquareContent.Hero);  //Find the starting point as marked in the text file
            endingPoint = FindCode(SquareContent.Monster); //Find the ending point
          

            /*
             * Map is drawn. Path finder has not yet been called
             * */
            foreach (Point point in AllSquares())
            {
                int x = point.X;
                int y = point.Y;
                int num = _squares[x, y].DistanceSteps;
                Color setColor = Color.Gainsboro;
                SquareContent content = _squares[x, y].ContentCode;

                if (content == SquareContent.Empty)
                {
                    if (_squares[x, y].IsPath == true)
                    {
                        setColor = Color.LightBlue;
                    }
                    else
                    {
                        setColor = Color.White;
                    }
                }
                else
                {
                    if (content == SquareContent.Hero)
                    {
                        setColor = Color.Blue;
                    }
                    else if (content == SquareContent.Monster)
                    {
                        setColor = Color.Coral;
                    }
                    else
                    {
                        setColor = Color.Gray;
                    }
                }
                boardControl1.SetHighlight(setColor, x, y);
            
            }
            /*
             * Call to RecurPath our path finding function is placed
             * Drawing of the path is done within that function itself
             * */
            bool found = RecurPath(startingPoint.X, startingPoint.Y, boardControl1);
            boardControl1.SetHighlight(Color.Blue, startingPoint.X, startingPoint.Y); //Starting point is colored again
            boardControl1.Invalidate();
        }

        public void Pathfind()
        {
            /*
             * 
             * Find path from hero to monster. First, get coordinates
             * of hero.
             * 
             * */
            Point startingPoint = FindCode(SquareContent.Hero);
            int heroX = startingPoint.X;
            int heroY = startingPoint.Y;
            if (heroX == -1 || heroY == -1)
            {
                return;
            }
            /*
             * 
             * Hero starts at distance of 0.
             * 
             * */
            _squares[heroX, heroY].DistanceSteps = 0;

            while (true)
            {
                bool madeProgress = false;

                /*
                 * 
                 * Look at each square on the board.
                 * 
                 * */
                foreach (Point mainPoint in AllSquares())
                {
                    int x = mainPoint.X;
                    int y = mainPoint.Y;

                    /*
                     * 
                     * If the square is open, look through valid moves given
                     * the coordinates of that square.
                     * 
                     * */
                    if (SquareOpen(x, y))
                    {
                        int passHere = _squares[x, y].DistanceSteps;

                        foreach (Point movePoint in ValidMoves(x, y))
                        {
                            int newX = movePoint.X;
                            int newY = movePoint.Y;
                            int newPass = passHere + 1;

                            if (_squares[newX, newY].DistanceSteps > newPass)
                            {
                                _squares[newX, newY].DistanceSteps = newPass;
                                madeProgress = true;
                            }
                        }
                    }
                }
                if (!madeProgress)
                {
                    break;
                }
            }
        }

        static private bool ValidCoordinates(int x, int y)
        {
            /*
             * 
             * Our coordinates are constrained between 0 and 14.
             * 
             * */
            if (x < 0)
            {
                return false;
            }
            if (y < 0)
            {
                return false;
            }
            if (x > GRID-1)
            {
                return false;
            }
            if (y > GRID-1)
            {
                return false;
            }
            return true;
        }

        private bool SquareOpen(int x, int y)
        {
            /*
             * 
             * A square is open if it is not a wall.
             * 
             * */
            switch (_squares[x, y].ContentCode)
            {
                case SquareContent.Empty:
                    return true;
                case SquareContent.Hero:
                    return true;
                case SquareContent.Monster:
                    return true;
                case SquareContent.Wall: 
                default:
                    return false;
            }
        }

        private Point FindCode(SquareContent contentIn)
        {
            /*
             * 
             * Find the requested code and return the point.
             * 
             * */
            foreach (Point point in AllSquares())
            {
                if (_squares[point.X, point.Y].ContentCode == contentIn)
                {
                    return new Point(point.X, point.Y);
                }
            }
            return new Point(-1, -1);
        }
        public Point GoWest(int curX, int curY)
        {
            while (true)
            {
                _squares[curX, curY].IsPath = true;
                if (SquareOpen(curX, curY-1)) break;
                curX--;
            }
            Point point = new Point();
            point.X = curX;
            point.Y = curY;
            return point;
        }
        
        public bool RecurPath(int x, int y, BoardControl boardcontrol1)
        {
            
            if (ValidCoordinates(x, y) == false)
            {
                return false;
            }
            if (x == (endingPoint.X) && y == (endingPoint.Y))
            {
                
                return true;
            }
            if (_squares[x, y].ContentCode == SquareContent.Wall)
            {
                return false;
            }
            
           
            _squares[x, y].ContentCode = SquareContent.Wall;

            if(RecurPath(x, y-1, boardcontrol1) == true)
            {
                boardcontrol1.Drawline(x, y, 'N');
                boardcontrol1.SetHighlight(Color.LightBlue, x, y);
                
                return true;
            }
            if (RecurPath(x + 1, y, boardcontrol1) == true)
            {
                boardcontrol1.Drawline(x, y, 'E');
                boardcontrol1.SetHighlight(Color.LightBlue, x, y);
               
                return true;
            }
            if (RecurPath(x, y + 1, boardcontrol1) == true)
            {
                boardcontrol1.SetHighlight(Color.LightBlue, x, y); 
                boardcontrol1.Drawline(x, y, 'S');
              
                return true;
            }
            if (RecurPath(x - 1, y, boardcontrol1) == true)
            {
                boardcontrol1.Drawline(x, y, 'W'); 
                boardcontrol1.SetHighlight(Color.LightBlue, x, y);
                return true;
            }
            return false;

        }
        public void HighlightPath()
        { 
            //startingPoint = FindCode(SquareContent.Hero);
            //endingPoint = FindCode(SquareContent.Monster);
            //bool found = RecurPath(startingPoint.X, startingPoint.Y);
            //_squares[startingPoint.X, startingPoint.Y].ContentCode = SquareContent.Hero;
            /*
            int pointX = startingPoint.X;
            int pointY = startingPoint.Y;
            if (pointX == -1 && pointY == -1)
            {
                return;
            }
            int endX = endingPoint.X;
            int endY = endingPoint.Y;
            foreach (Point movePoint in ValidMoves(pointX, pointY))
            {
                int newX = movePoint.X;
                int newY = movePoint.Y;
                _squares[newX, newY].IsPath = true;
            }
            
            for (int x = 0; x < GRID; x++)
            {
                _squares[x, pointY].IsPath = true;
                if (x == endX) break;
            }

            for (int y = pointY; y > endY; y--)
            {
                if (SquareOpen(endX, y - 1))
                {
                    _squares[endX, y].IsPath = true;

                }
                else
                {
                    int newX = 0, newY = 0;
                    foreach (Point movePoint in ValidMoves(endX, y))
                    {
                         newX = movePoint.X;
                         newY = movePoint.Y;
                        
                        _squares[newX, newY].IsPath = true;
                    }
                    GoWest(newX,newY);

                    break;
                }
                foreach (Point movePoint in ValidMoves(endX, y))
                { 
                    int newX = movePoint.X;
                    int newY = movePoint.Y;
                    if (newY != y) break;
                   
                }
            }*/
            
            /*
            Point startingPoint = FindCode(SquareContent.Monster);
            int pointX = startingPoint.X;
            int pointY = startingPoint.Y;
            if (pointX == -1 && pointY == -1)
            {
                return;
            }

            while (true)
            {
               
                Point lowestPoint = Point.Empty;
                int lowest = 10000;

                foreach (Point movePoint in ValidMoves(pointX, pointY))
                {
                    int count = _squares[movePoint.X, movePoint.Y].DistanceSteps;
                    if (count < lowest)
                    {
                        lowest = count;
                        lowestPoint.X = movePoint.X;
                        lowestPoint.Y = movePoint.Y;
                    }
                }
                if (lowest != 10000)
                {
                   
                    _squares[lowestPoint.X, lowestPoint.Y].IsPath = true;
                    pointX = lowestPoint.X;
                    pointY = lowestPoint.Y;
                }
                else
                {
                    break;
                }

                if (_squares[pointX, pointY].ContentCode == SquareContent.Hero)
                {
                   
                    break;
                }
            } */
        }

        private static IEnumerable<Point> AllSquares()
        {
            /*
             * 
             * Return every point on the board in order.
             * 
             * */
            for (int x = 0; x < GRID; x++)
            {
                for (int y = 0; y < GRID; y++)
                {
                    yield return new Point(x, y);
                }
            }
        }

        private IEnumerable<Point> ValidMoves(int x, int y)
        {
            /*
             * 
             * Return each valid square we can move to.
             * 
             * */
            foreach (Point movePoint in _movements)
            {
                int newX = x + movePoint.X;
                int newY = y + movePoint.Y;

                if (ValidCoordinates(newX, newY) &&
                    SquareOpen(newX, newY))
                {
                    yield return new Point(newX, newY);
                }
            }
        }
    }
}
