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
    class TriangleInfo : Shape
    {
        public TriangleInfo(Point startOrigin, int width, int height, Color selectedColor, int shapeSize, bool fillShape)
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
            Point[] trianglePoints = new Point[] {
                new Point(StartOrigin.X + Width, StartOrigin.Y + Height), 
                new Point(StartOrigin.X + Width / 2, StartOrigin.Y),
                new Point(StartOrigin.X, StartOrigin.Y + Height),
                new Point(StartOrigin.X + Width, StartOrigin.Y + Height)
            };

            Pen penColor = new Pen(SelectedColor, ShapeSize);
            if (FilledShape)
            {
                SolidBrush tempBrush = new SolidBrush(SelectedColor);
                g.FillPolygon(tempBrush, trianglePoints);
            }
            else
            {
                g.DrawLine(penColor, StartOrigin.X + Width, StartOrigin.Y + Height, StartOrigin.X + Width / 2, StartOrigin.Y);
                g.DrawLine(penColor, StartOrigin.X + Width / 2, StartOrigin.Y, StartOrigin.X, StartOrigin.Y + Height);
                g.DrawLine(penColor, StartOrigin.X, StartOrigin.Y + Height, StartOrigin.X + Width, StartOrigin.Y + Height);
            }
        }

        public override bool ContainsPoint(Point p)
        {
            Point[] trianglePoints = new Point[] {
                new Point(StartOrigin.X + Width + 7, StartOrigin.Y + Height + 7), 
                new Point(StartOrigin.X + Width / 2, StartOrigin.Y - 7),
                new Point(StartOrigin.X - 7, StartOrigin.Y + Height + 7),
                new Point(StartOrigin.X + Width + 7, StartOrigin.Y + Height + 7)
            };
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddLines(trianglePoints);
            bool pointWithinTriangle = myPath.IsVisible(p);

            if (pointWithinTriangle)
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
