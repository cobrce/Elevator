using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace Elevator.Animation
{
    abstract class AbstractAnimation : IAnimation
    {
        public void Animate(Shape shape, object args)
        {
            Invoke(shape, args, (Action<Shape, object>)Animation);
        }
        public abstract void Animation(Shape shape, object args);


        // takes two params, return object
        protected object Invoke(Shape shape, object args, Func<Shape, object, object> function)
        {
            return shape.Dispatcher.Invoke(function, new object[] { shape, args });
        }
        // takes one param, returns object
        protected object Invoke(Shape shape, Func<Shape, object> function)
        {
            return shape.Dispatcher.Invoke(function, new object[] { shape });
        }
        // takes two params, returns void
        protected void Invoke(Shape shape, object args, Action<Shape, object> action)
        {
            Invoke(shape, args, ((s, a) => { action(s, a); return null; }));
        }
        // takes one param, returns void
        protected void Invoke(Shape shape, Action<Shape> action)
        {
            Invoke(shape, ((s) => { action(s); return null; }));
        }
    }
}
