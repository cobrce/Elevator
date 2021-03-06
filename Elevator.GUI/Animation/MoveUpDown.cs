﻿using System.Windows.Controls;
using System.Windows.Shapes;

namespace Elevator.Animation
{
    class MoveUpDown : AbstractAnimation
    {
        double _step;
        public MoveUpDown(double step)
        {
            _step = step;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape">the shape to move</param>
        /// <param name="args">1 for up, 0 for down</param>
        public override void Animation(Shape shape, object args)
        {
            if (args is int direction)
            {
                double top = Canvas.GetTop(shape);
                top -= (2 * direction - 1) * _step; // equivalant of if (direction == 1) top+= step; else top-= step;
                Canvas.SetTop(shape, top);
            }
        }
    }
}
