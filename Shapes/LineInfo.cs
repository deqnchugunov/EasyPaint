using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaint.Shapes
{
    [Serializable]
    class LineInfo : Shape
    {
        public LineInfo(Point startOrigin, Point endOrigin, int width, int height, Color selectedColor, int shapeSize)
        {
            StartOrigin = startOrigin;
            EndOrigin = endOrigin;
            Width = width;
            Height = height;
            SelectedColor = selectedColor;
            ShapeSize = shapeSize;
        }

        public override void Draw(Graphics g)
        {
            g.DrawLine(new Pen(SelectedColor, ShapeSize), StartOrigin, EndOrigin);
        }

        public override bool ContainsPoint(Point p) 
        {
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddLine(StartOrigin, EndOrigin);
            bool pointWithinLine = myPath.IsOutlineVisible(p, new Pen(SelectedColor, ShapeSize));

            if (pointWithinLine)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
