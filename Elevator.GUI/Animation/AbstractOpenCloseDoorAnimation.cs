using System.Windows.Shapes;

namespace Elevator.Animation
{
    abstract class AbstractOpenCloseDoorAnimation : AbstractAnimation
    {
        protected double _min, _max, _step;

        public AbstractOpenCloseDoorAnimation(double min, double max, double step)
        {
            _min = min;
            _max = max;
            _step = step;
        }
        public virtual bool isOpen(Shape shape)
        {
            return (bool)Invoke(shape, isOpen_p);
        }
        protected virtual object isOpen_p(Shape shape)
        {
            return shape.Width == _max;
        }

        public virtual bool isClosed(Shape shape)
        {
            return (bool)Invoke(shape, isClosed_p);
        }

        protected virtual object isClosed_p(Shape shape)
        {
            return shape.Width == _min;
        }
    }
}
