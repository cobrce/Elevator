using System.Windows.Shapes;

namespace Elevator.Animation
{
    public interface IAnimation
    {
        void Animate(Shape shape, object args);
    }
}