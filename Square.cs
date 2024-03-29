﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinotaurPathfinder
{
    enum SquareContent
    {
        Empty,
        Monster,
        Hero,
        Wall
    };

    class CompleteSquare
    {
        SquareContent _contentCode = SquareContent.Empty;
        public SquareContent ContentCode
        {
            get { return _contentCode; }
            set { _contentCode = value; }
        }

        int _distanceSteps = 10000;
        public int DistanceSteps
        {
            get { return _distanceSteps; }
            set { _distanceSteps = value; }
        }

        bool _isPath = false;
        public bool IsPath
        {
            get { return _isPath; }
            set { _isPath = value; }
        }

        public void FromChar(char charIn)
        {
            switch (charIn)
            {
                case 'W':
                    _contentCode = SquareContent.Wall;
                    break;
                case 'H':
                    _contentCode = SquareContent.Hero;
                    break;
                case 'M':
                    _contentCode = SquareContent.Monster;
                    break;
                case ' ':
                default:
                    _contentCode = SquareContent.Empty;
                    break;
            }
        }
    }
}
