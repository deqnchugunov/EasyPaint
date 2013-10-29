using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaint.Shapes
{
    [Serializable]
    public abstract class Shape
    {
        public Point StartOrigin { get; set; }
        public Point EndOrigin { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ShapeSize { get; protected set; }
        public Color SelectedColor { get; set; }
        public bool FilledShape { get; set; }

        public abstract void Draw(Graphics g);
        public abstract bool ContainsPoint(Point p);
    }
}
