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
    class EllipseInfo : Shape
    {
        Point moveOriginStart;

        public EllipseInfo(Point startOrigin, int width, int height, Color selectedColor, int shapeSize, bool fillShape)

        {
            StartOrigin = startOrigin;
            Width = width;
            Height = height;
            SelectedColor = selectedColor;
            ShapeSize = shapeSize;
            moveOriginStart = startOrigin;
            FilledShape = fillShape;
        }

        public override void Draw(Graphics g)
        {
            SolidBrush tempBrush = new SolidBrush(SelectedColor);
            if (FilledShape)
            {
                g.FillEllipse(tempBrush, StartOrigin.X, StartOrigin.Y, Width, Height);
            }
            else
            {
                g.DrawEllipse(new Pen(SelectedColor, ShapeSize), StartOrigin.X, StartOrigin.Y, Width, Height);
            }
        }

        public override bool ContainsPoint(Point p)
        {
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddEllipse(StartOrigin.X - 6, StartOrigin.Y - 6, Width + 15, Height + 15);
            bool pointWithinEllipse = myPath.IsVisible(p);

            if (pointWithinEllipse)
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
