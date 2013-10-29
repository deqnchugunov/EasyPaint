using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaint.Shapes
{
    [Serializable]
    class RectangleInfo : Shape
    {
        public RectangleInfo(Point startOrigin, int width, int height, Color selectedColor, int shapeSize, bool fillShape)
        {
            StartOrigin = startOrigin;
            Width = width;
            Height = height;
            SelectedColor = selectedColor;
            ShapeSize = shapeSize;
            FilledShape = fillShape;
        }

        public override void Draw(Graphics g)
        {
            Pen penColor = new Pen(SelectedColor, ShapeSize);
            int absWidth = Math.Abs(Width);
            int absHeight = Math.Abs(Height);

            if (FilledShape)
            {
                SolidBrush tempBrush = new SolidBrush(SelectedColor);
                g.FillRectangle(tempBrush, StartOrigin.X, StartOrigin.Y, Width, Height);
            }
            else
            {
                g.DrawRectangle(penColor, StartOrigin.X, StartOrigin.Y, Width, Height);
            }

        }

        public override bool ContainsPoint(Point p)
        {
            if (p.X > StartOrigin.X - 5 && p.X < StartOrigin.X + Width + 10 && p.Y > StartOrigin.Y - 5 && p.Y < StartOrigin.Y + Height + 10)
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
