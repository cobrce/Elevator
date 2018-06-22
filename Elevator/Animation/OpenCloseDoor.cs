using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Elevator.Animation
{
    class OpenCloseDoor : AbstractOpenCloseDoorAnimation
    {
        public OpenCloseDoor(double min, double max, double step) : base(min, max, step)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="args">1 open, 0 close</param>
        public override void Animation(Shape shape, object args)
        {
            if (args is int direction)
            {
                double value = (2 * direction - 1) * _step;
                if (direction == 1 && shape.Width < _max || direction == 0 && shape.Width > _min)
                {
                    shape.Width += value;
                    double left = Canvas.GetLeft(shape);
                    left -= value / 2.0;
                    Canvas.SetLeft(shape,left);
                }
            }
        }
        
    }
}
