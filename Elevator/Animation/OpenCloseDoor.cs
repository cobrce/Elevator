using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Elevator.Animation
{
    class OpenCloseDoor : AbstractAnimation
    {
        double _min, _max, _step;

        public OpenCloseDoor(double min, double max, double step)
        {
            _min = min;
            _max = max;
            _step = step;
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
                    left -= value / 2;
                }
            }
        }
        public bool isOpen(Shape shape)
        {
            return (bool)Invoke(shape, isOpen_p);
        }
        private object isOpen_p(Shape shape)
        {
            return shape.Width == _max;
        }

        public bool isClosed(Shape shape)
        {
            return (bool)Invoke(shape, isClosed_p);
        }

        private object isClosed_p(Shape shape)
        {
            return shape.Width == _min;
        }
    }
}
