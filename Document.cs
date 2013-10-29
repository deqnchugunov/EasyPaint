using System;
using EasyPaint.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaint
{
    [Serializable]
    public class Document
    {
        public List<Shape> allShapes = new List<Shape>();
    }
}
