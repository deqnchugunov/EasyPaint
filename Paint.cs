using EasyPaint.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyPaint
{
    public partial class frmPaint : Form
    {
        bool paint = false;
        private int shapeWidth, shapeHeight;
        int shapeSize = 1;
        private bool hasShapes = false;
        private bool fillShape = false;
        ShapeKind shapeKind;
        SelectedColor color = SelectedColor.Black;
        Color selectedColor;
        private int sizeNodeRect = 7;
        private bool mIsClick = false;
        private bool selectionMode = false;
        private bool isShapeWasSelected = false;
        private bool mMove = false;
        private ResizePosition nodeSelected = ResizePosition.None;
        private int? indexOfSelectedShape;
        Point startPoint;
        Point endPoint;
        Document doc;
        Shape figure;
        Panel pnlGraphic;
        private bool loadedFile = false;
        private bool buttonDeletePressed = false;

        public frmPaint()
        {
            InitializeComponent();
            doc = new Document();
            pnlGraphic = new MyPanel();
            Controls.Add(pnlGraphic);
            btnEllipse.BackColor = Color.White;
            this.pnlGraphic.BackColor = System.Drawing.Color.White;
            this.pnlGraphic.Location = new System.Drawing.Point(150, 45);
            this.pnlGraphic.Name = "pnlGraphic";
            this.pnlGraphic.Size = new System.Drawing.Size(715, 520);
            this.pnlGraphic.TabIndex = 2;
            this.pnlGraphic.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlGraphic_Paint);
            this.pnlGraphic.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlGraphic_MouseClick);
            this.pnlGraphic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlGraphic_MouseDown);
            this.pnlGraphic.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlGraphic_MouseMove);
            this.pnlGraphic.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlGraphic_MouseUp);
        }

        private void pnlGraphic_Paint(object sender, PaintEventArgs e)
        {
            if (hasShapes && doc.allShapes != null)
            {
                foreach (Shape shape in doc.allShapes)
                {
                    shape.Draw(e.Graphics);
                }
            }
            
            if (loadedFile)
            {
                foreach (Shape shape in doc.allShapes)
                {
                    shape.Draw(e.Graphics);
                }
            }

            if (buttonDeletePressed)
            {
                foreach (Shape shape in doc.allShapes)
                {
                    shape.Draw(e.Graphics);
                }
            }

            if (selectionMode)
            {
                foreach (Shape shape in doc.allShapes)
                {
                    shape.Draw(e.Graphics);
                }
            }

            if (isShapeWasSelected)
            {
                if (shapeKind == ShapeKind.Line)
                {
                    MakeSelectionOfLine(doc.allShapes[indexOfSelectedShape.Value], e.Graphics);
                }
                else
                {
                    MakeSelectionOfShape(doc.allShapes[indexOfSelectedShape.Value], e.Graphics);
                }
            }
            if (paint && figure != null)
            {
                figure.Draw(e.Graphics);
            }
        }

        private void pnlGraphic_MouseDown(object sender, MouseEventArgs e)
        {
            mIsClick = true;

            if (selectionMode && doc.allShapes.Count == 0)
            {
                MessageBox.Show("Няма фигури за избиране !");
            }
            else if (selectionMode && doc.allShapes.Count != 0)
            {
                for (int i = doc.allShapes.Count - 1; i >= 0; i--)
                {
                    if (doc.allShapes[i].ContainsPoint(e.Location))
                    {
                        isShapeWasSelected = true;
                        Shape tempShape = doc.allShapes[i];
                        if (doc.allShapes.Count == 2 && tempShape != doc.allShapes[doc.allShapes.Count - 1])
                        {
                            doc.allShapes[i] = doc.allShapes[i + 1];
                            doc.allShapes[i + 1] = tempShape;
                        }
                        else if (doc.allShapes.Count > 2 && tempShape != doc.allShapes[doc.allShapes.Count - 1])
                        {
                            for (int j = i; j < doc.allShapes.Count - 1; j++)
                            {
                                doc.allShapes[j] = doc.allShapes[j + 1];
                            }
                            doc.allShapes[doc.allShapes.Count - 1] = tempShape;
                        }
                        indexOfSelectedShape = doc.allShapes.Count - 1;
                        mMove = true;
                        break;
                    }
                }
                if (isShapeWasSelected && indexOfSelectedShape != null)
                {
                    btnDefaultColor.BackColor = doc.allShapes[indexOfSelectedShape.Value].SelectedColor;
                    nodeSelected = ResizePosition.None;
                    nodeSelected = GetNodeSelectable(e.Location);
                }

            }
            else if (!selectionMode)
            {
                paint = true;
            }

            startPoint.X = e.X;
            startPoint.Y = e.Y;
            pnlGraphic.Invalidate();
        }

        private void ChangeColor(Color color)
        {
            Shape currentShape = null;

            if (isShapeWasSelected && indexOfSelectedShape != null)
            {
                doc.allShapes[indexOfSelectedShape.Value].SelectedColor = color;
                if (chBoxFill.Checked == true)
                {
                    currentShape = doc.allShapes[indexOfSelectedShape.Value];
                    doc.allShapes[indexOfSelectedShape.Value].FilledShape = true;
                }
                else
                {
                    doc.allShapes[indexOfSelectedShape.Value].FilledShape = false;
                }
            }
            pnlGraphic.Invalidate();
        }

        private void pnlGraphic_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false;
            if (!selectionMode && (e.X - startPoint.X) != 0)
            {
                doc.allShapes.Add(figure);
                hasShapes = true;
            }
            mIsClick = false;
            mMove = false;
            figure = null;
            pnlGraphic.Invalidate();
        }

        private void pnlGraphic_MouseMove(object sender, MouseEventArgs e)
        {
            if (paint)
            {
                endPoint.X = e.X;
                endPoint.Y = e.Y;
                shapeWidth = e.X - startPoint.X;
                shapeHeight = e.Y - startPoint.Y;
                shapeSize = (int)numSize.Value;
                int absShapeWidth = Math.Abs(shapeWidth);
                int absShapeHeight = Math.Abs(shapeHeight);
                Point tempStartPoint;
                Point tempEndPoint;

                if (chBoxFill.Checked == true)
                {
                    fillShape = true;
                }
                else
                {
                    fillShape = false;
                }

                // check selected color
                if (color == SelectedColor.Black)
                {
                    selectedColor = Color.Black;
                }
                else if (color == SelectedColor.Yellow)
                {
                    selectedColor = Color.Yellow;
                }
                else if (color == SelectedColor.Red)
                {
                    selectedColor = Color.Red;
                }
                else if (color == SelectedColor.Blue)
                {
                    selectedColor = Color.Blue;
                }
                else if (color == SelectedColor.White)
                {
                    selectedColor = Color.White;
                }
                else if (color == SelectedColor.Green)
                {
                    selectedColor = Color.Green;
                }
                else if (color == SelectedColor.Orange)
                {
                    selectedColor = Color.Orange;
                }
                else if (color == SelectedColor.Purple)
                {
                    selectedColor = Color.Violet;
                }

                // check shape kind
                if (shapeKind == ShapeKind.Ellipse)
                {
                    if (shapeWidth < 0 && shapeHeight < 0)
                    {
                        tempStartPoint = new Point(startPoint.X - absShapeWidth, startPoint.Y - absShapeHeight);
                    }
                    else if (shapeWidth < 0 && shapeHeight > 0)
                    {
                        tempStartPoint = new Point(startPoint.X - absShapeWidth, startPoint.Y);
                    }
                    else if (shapeWidth > 0 && shapeHeight < 0)
                    {
                        tempStartPoint = new Point(startPoint.X, startPoint.Y - absShapeHeight);
                    }
                    else
                    {
                        tempStartPoint = new Point(startPoint.X, startPoint.Y);
                    }
                    figure = new Shapes.EllipseInfo(tempStartPoint, absShapeWidth, absShapeHeight, selectedColor, shapeSize, fillShape);
                }
                else if (shapeKind == ShapeKind.Rectangle)
                {
                    if (shapeWidth < 0 && shapeHeight < 0)
                    {
                        tempStartPoint = new Point(startPoint.X - absShapeWidth, startPoint.Y - absShapeHeight);
                    }
                    else if (shapeWidth < 0 && shapeHeight > 0)
                    {
                        tempStartPoint = new Point(startPoint.X - absShapeWidth, startPoint.Y);
                    }
                    else if (shapeWidth > 0 && shapeHeight < 0)
                    {
                        tempStartPoint = new Point(startPoint.X, startPoint.Y - absShapeHeight);
                    }
                    else
                    {
                        tempStartPoint = new Point(startPoint.X, startPoint.Y);
                    }

                    figure = new Shapes.RectangleInfo(tempStartPoint, absShapeWidth, absShapeHeight, selectedColor, shapeSize, fillShape);
                }
                else if (shapeKind == ShapeKind.Line)
                {
                    tempStartPoint = startPoint;
                    tempEndPoint = endPoint;

                    figure = new Shapes.LineInfo(tempStartPoint, tempEndPoint, absShapeWidth, absShapeHeight, selectedColor, shapeSize);
                }
                else if (shapeKind == ShapeKind.Triangle)
                {
                    if (shapeWidth < 0 && shapeHeight < 0)
                    {
                        tempStartPoint = new Point(startPoint.X - absShapeWidth, startPoint.Y - absShapeHeight);
                    }
                    else if (shapeWidth < 0 && shapeHeight > 0)
                    {
                        tempStartPoint = new Point(startPoint.X - absShapeWidth, startPoint.Y);
                    }
                    else if (shapeWidth > 0 && shapeHeight < 0)
                    {
                        tempStartPoint = new Point(startPoint.X, startPoint.Y - absShapeHeight);
                    }
                    else
                    {
                        tempStartPoint = new Point(startPoint.X, startPoint.Y);
                    }

                    figure = new Shapes.TriangleInfo(tempStartPoint, absShapeWidth, absShapeHeight, selectedColor, shapeSize, fillShape);//figure = new Shapes.TriangleInfo(startPoint, endPoint, shapeWidth, shapeHeight, chosenColor, shapeSize);
                }

            }
            else if (selectionMode && isShapeWasSelected)
            {
                ChangeCursor(e.Location);
                if (mIsClick == false)
                {
                    return;
                }

                Shape tempShape = doc.allShapes[indexOfSelectedShape.Value];

                switch (nodeSelected)
                {
                    case ResizePosition.LeftUp:
                        tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - startPoint.X, tempShape.StartOrigin.Y);
                        tempShape.Width -= e.X - startPoint.X;
                        tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - startPoint.Y);
                        tempShape.Height -= e.Y - startPoint.Y;
                        break;
                    case ResizePosition.LeftMiddle:
                        tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - startPoint.X, tempShape.StartOrigin.Y);
                        tempShape.Width -= (e.X - startPoint.X);
                        break;
                    case ResizePosition.LeftBottom:
                        tempShape.Width -= e.X - startPoint.X;
                        tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - startPoint.X, tempShape.StartOrigin.Y);
                        tempShape.Height += e.Y - startPoint.Y;
                        break;
                    case ResizePosition.BottomMiddle:
                        tempShape.Height += e.Y - startPoint.Y;
                        break;
                    case ResizePosition.RightUp:
                        tempShape.Width += e.X - startPoint.X;
                        tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - startPoint.Y);
                        tempShape.Height -= e.Y - startPoint.Y;
                        break;
                    case ResizePosition.RightBottom:
                        tempShape.Width += e.X - startPoint.X;
                        tempShape.Height += e.Y - startPoint.Y;
                        if (shapeKind == ShapeKind.Line)
                        {
                            tempShape.EndOrigin = new Point(e.X - tempShape.EndOrigin.X + startPoint.X, e.Y - tempShape.EndOrigin.Y + endPoint.Y);
                        }
                        break;
                    case ResizePosition.RightMiddle:
                        tempShape.Width += e.X - startPoint.X;
                        break;
                    case ResizePosition.UpMiddle:
                        tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - startPoint.Y);
                        tempShape.Height -= e.Y - startPoint.Y;
                        break;
                    default:
                        if (mMove)
                        {
                            tempShape.StartOrigin = new Point(tempShape.StartOrigin.X + e.X - startPoint.X, tempShape.StartOrigin.Y);
                            tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y + e.Y - startPoint.Y);
                            pnlGraphic.Cursor = Cursors.SizeAll;
                        }
                        break;
                }
                startPoint.X = e.X;
                startPoint.Y = e.Y;
            }
            if (hasShapes && isShapeWasSelected)
            {
                TestIfRectInsideArea();
            }

            pnlGraphic.Invalidate();
        }

        private void MakeSelectionOfShape(Shape shape, Graphics g)
        {
            for (int i = 0; i < 8; i++)
            {
                g.DrawRectangle(new Pen(Color.Blue), GetRect(i, shape));
            }

            Pen tempPen = new Pen(Color.Blue);
            tempPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            g.DrawRectangle(tempPen, shape.StartOrigin.X - 3, shape.StartOrigin.Y - 3, shape.Width + 6, shape.Height + 6);
        }


        private Rectangle GetRect(int value, Shape shape)
        {
            int xValue = shape.StartOrigin.X;
            int yValue = shape.StartOrigin.Y;

            switch (value)
            {
                case 0:
                    return new Rectangle(xValue - 3 - sizeNodeRect / 2, yValue - 3 - sizeNodeRect / 2, sizeNodeRect, sizeNodeRect);

                case 1:
                    return new Rectangle(xValue - 4 - sizeNodeRect / 2, yValue + shape.Height / 2 - sizeNodeRect / 2, sizeNodeRect, sizeNodeRect);

                case 2:
                    return new Rectangle(xValue - 3 - sizeNodeRect / 2, yValue + 3 + shape.Height - sizeNodeRect / 2, sizeNodeRect, sizeNodeRect);

                case 3:
                    return new Rectangle(xValue + shape.Width / 2 - sizeNodeRect / 2, yValue + 3 + shape.Height - sizeNodeRect / 2, sizeNodeRect, sizeNodeRect);

                case 4:
                    return new Rectangle(xValue + 3 + shape.Width - sizeNodeRect / 2, yValue - 3 - sizeNodeRect / 2, sizeNodeRect, sizeNodeRect);

                case 5:
                    return new Rectangle(xValue + 3 + shape.Width - sizeNodeRect / 2, yValue + 3 + shape.Height - sizeNodeRect / 2, sizeNodeRect, sizeNodeRect);

                case 6:
                    return new Rectangle(xValue + 3 + shape.Width - sizeNodeRect / 2, yValue + shape.Height / 2 - sizeNodeRect / 2, sizeNodeRect, sizeNodeRect);

                case 7:
                    return new Rectangle(xValue + shape.Width / 2 - sizeNodeRect / 2, yValue - 4 - sizeNodeRect / 2, sizeNodeRect, sizeNodeRect);
                default:
                    return new Rectangle();
            }
        }

        private void MakeSelectionOfLine(Shape shape, Graphics g)
        {
            int xValue = shape.StartOrigin.X;
            int yValue = shape.StartOrigin.Y;

            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    g.DrawEllipse(new Pen(Color.Blue), xValue - sizeNodeRect / 2, yValue - sizeNodeRect / 2, sizeNodeRect, sizeNodeRect);
                }
                else
                {
                    g.DrawEllipse(new Pen(Color.Blue), shape.EndOrigin.X - 3, shape.EndOrigin.Y - 3, sizeNodeRect, sizeNodeRect);
                }
            }

            Pen tempPen = new Pen(Color.Blue);
            tempPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            g.DrawLine(tempPen, shape.StartOrigin.X, shape.StartOrigin.Y, shape.EndOrigin.X, shape.EndOrigin.Y);
            pnlGraphic.Invalidate();
        }

        private ResizePosition GetNodeSelectable(Point p)
        {
            foreach (ResizePosition r in Enum.GetValues(typeof(ResizePosition)))
            {
                if (GetRectangle(r).Contains(p))
                {
                    return r;
                }
            }
            return ResizePosition.None;
        }

        private void ChangeCursor(Point p)
        {
            pnlGraphic.Cursor = GetCursor(GetNodeSelectable(p));
        }

        private Cursor GetCursor(ResizePosition p)
        {
            switch (p)
            {
                case ResizePosition.LeftUp:
                    return Cursors.SizeNWSE;

                case ResizePosition.LeftMiddle:
                    return Cursors.SizeWE;

                case ResizePosition.LeftBottom:
                    return Cursors.SizeNESW;

                case ResizePosition.BottomMiddle:
                    return Cursors.SizeNS;

                case ResizePosition.RightUp:
                    return Cursors.SizeNESW;

                case ResizePosition.RightBottom:
                    return Cursors.SizeNWSE;

                case ResizePosition.RightMiddle:
                    return Cursors.SizeWE;

                case ResizePosition.UpMiddle:
                    return Cursors.SizeNS;
                default:
                    return Cursors.Default;
            }
        }
        private Rectangle GetRectangle(ResizePosition value)
        {
            Shape tempShape = doc.allShapes[indexOfSelectedShape.Value];

            switch (value)
            {
                case ResizePosition.LeftUp:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y - 7, sizeNodeRect, sizeNodeRect);

                case ResizePosition.LeftMiddle:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y + tempShape.Height / 2, sizeNodeRect, sizeNodeRect);

                case ResizePosition.LeftBottom:
                    return new Rectangle(tempShape.StartOrigin.X - 7, tempShape.StartOrigin.Y + 5 + tempShape.Height, sizeNodeRect, sizeNodeRect);

                case ResizePosition.BottomMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + tempShape.Width / 2, tempShape.StartOrigin.Y + 5 + tempShape.Height, sizeNodeRect, sizeNodeRect);

                case ResizePosition.RightUp:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width, tempShape.StartOrigin.Y - 7, sizeNodeRect, sizeNodeRect);

                case ResizePosition.RightBottom:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width, tempShape.StartOrigin.Y + 5 + tempShape.Height, sizeNodeRect, sizeNodeRect);

                case ResizePosition.RightMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + 5 + tempShape.Width, tempShape.StartOrigin.Y + tempShape.Height / 2, sizeNodeRect, sizeNodeRect);

                case ResizePosition.UpMiddle:
                    return new Rectangle(tempShape.StartOrigin.X + tempShape.Width / 2, tempShape.StartOrigin.Y - 6, sizeNodeRect, sizeNodeRect);
                default:
                    return new Rectangle();
            }
        }

        private enum ResizePosition
        {
            UpMiddle,
            LeftMiddle,
            LeftBottom,
            LeftUp,
            RightUp,
            RightMiddle,
            RightBottom,
            BottomMiddle,
            None
        };

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            string button = "rectangle";
            shapeKind = ShapeKind.Rectangle;
            ChangeButtonColor(button);
            selectionMode = true;
            btnSelection_Click(sender, e);
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            shapeKind = ShapeKind.Ellipse;
            string button = "ellipse";
            ChangeButtonColor(button);
            selectionMode = true;
            btnSelection_Click(sender, e);
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            shapeKind = ShapeKind.Line;
            string button = "line";
            ChangeButtonColor(button);
            selectionMode = true;
            btnSelection_Click(sender, e);
        }

        private void btnTriangle_Click(object sender, EventArgs e)
        {
            shapeKind = ShapeKind.Triangle;
            string button = "triangle";
            ChangeButtonColor(button);
            selectionMode = true;
            btnSelection_Click(sender, e);
        }

        private void menuOpen_Click(object sender, EventArgs e)
        {
            if (doc.allShapes.Count > 0)
            {
                const string message = "Do you want to save changes?";
                const string caption = "Easy Paint";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    menuSave_Click(sender, e);
                    OpenDialogForm();
                }
                else if (result == DialogResult.No)
                {
                    OpenDialogForm();
                }
            }
            else
            {
                OpenDialogForm();
            }
        }

        private void OpenDialogForm()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Easy Paint Files | *.epnt";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                Document newDoc = new Document();
                doc = newDoc;

                using (Stream file = openDialog.OpenFile())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    doc = (Document)formatter.Deserialize(file);
                }
                loadedFile = true;
                pnlGraphic.Invalidate();
            }
        }

        private void menuNew_Click(object sender, EventArgs e)
        {
            if (doc.allShapes.Count > 0)
            {
                const string message = "Do you want to save changes?";
                const string caption = "Save changes";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    menuSave_Click(sender, e);
                    NewBlankPage();
                }
                else if (result == DialogResult.No)
                {
                    NewBlankPage();
                }
            }
            else
            {
                NewBlankPage();
            }
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Easy Paint Files | *.epnt";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                using (Stream file = saveDialog.OpenFile())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(file, doc);
                }
            }
        }

        private void NewBlankPage()
        {
            Document newDoc = new Document();
            doc = newDoc;
            isShapeWasSelected = false;
            indexOfSelectedShape = null;
            pnlGraphic.Invalidate();
        }

        private void ChangeButtonColor(string button)
        {
            if (button == "rectangle")
            {
                btnRectangle.BackColor = Color.White;
                btnEllipse.BackColor = Color.Transparent;
                btnLine.BackColor = Color.Transparent;
                btnTriangle.BackColor = Color.Transparent;
            }
            else if (button == "line")
            {
                btnEllipse.BackColor = Color.Transparent;
                btnRectangle.BackColor = Color.Transparent;
                btnLine.BackColor = Color.White;
                btnTriangle.BackColor = Color.Transparent;
            }
            else if (button == "triangle")
            {
                btnEllipse.BackColor = Color.Transparent;
                btnRectangle.BackColor = Color.Transparent;
                btnLine.BackColor = Color.Transparent;
                btnTriangle.BackColor = Color.White;
            }
            else if (button == "ellipse")
            {
                btnEllipse.BackColor = Color.White;
                btnRectangle.BackColor = Color.Transparent;
                btnLine.BackColor = Color.Transparent;
                btnTriangle.BackColor = Color.Transparent;
            }
        }

        private void frmPaint_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && isShapeWasSelected)
            {
                doc.allShapes.Remove(doc.allShapes[indexOfSelectedShape.Value]);
                isShapeWasSelected = false;
                indexOfSelectedShape = null;
                buttonDeletePressed = true;
                pnlGraphic.Invalidate();
            }
        }

        private void btnSelection_Click(object sender, EventArgs e)
        {
            if (selectionMode)
            {
                selectionMode = false;
                isShapeWasSelected = false;
                btnSelection.Text = "OFF";
            }
            else
            {
                selectionMode = true;
                btnSelection.Text = "ON";
            }
            pnlGraphic.Invalidate();
        }

        private void TestIfRectInsideArea()
        {

            Shape tempShape = doc.allShapes[indexOfSelectedShape.Value];
            Point staticPoint;
            staticPoint = new Point();
            staticPoint.X = pnlGraphic.Width - tempShape.StartOrigin.X;
            staticPoint.Y = tempShape.StartOrigin.Y;

            if (tempShape.StartOrigin.X < 0) tempShape.StartOrigin = new Point(0, tempShape.StartOrigin.Y);
            if (tempShape.StartOrigin.Y < 0) tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, 0);
            if (tempShape.Width <= 1)
            {
                tempShape.Width = 1;
                tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y);
            }
            if (tempShape.Height <= 1)
            {
                tempShape.Height = 1;
                tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, tempShape.StartOrigin.Y);
            }

            if (tempShape.StartOrigin.X + tempShape.Width > pnlGraphic.Width)
            {
                tempShape.StartOrigin = new Point(pnlGraphic.Width - tempShape.Width, tempShape.StartOrigin.Y);
                tempShape.Width = pnlGraphic.Width - tempShape.StartOrigin.X;
            }
            if (tempShape.StartOrigin.Y + tempShape.Height > pnlGraphic.Height)
            {
                tempShape.StartOrigin = new Point(tempShape.StartOrigin.X, pnlGraphic.Height - tempShape.Height);
                tempShape.Height = pnlGraphic.Height - tempShape.StartOrigin.Y;
            }
            pnlGraphic.Invalidate();
        }

        // set current color
        private void btnRed_Click(object sender, EventArgs e)
        {
            color = SelectedColor.Red;
            btnDefaultColor.BackColor = Color.Red;
            ChangeColor(Color.Red);
        }

        private void btnYellow_Click(object sender, EventArgs e)
        {
            color = SelectedColor.Yellow;
            btnDefaultColor.BackColor = Color.Yellow;
            ChangeColor(Color.Yellow);
        }

        private void btnBlack_Click(object sender, EventArgs e)
        {
            color = SelectedColor.Black;
            btnDefaultColor.BackColor = Color.Black;
            ChangeColor(Color.Black);
        }

        private void btnBlue_Click(object sender, EventArgs e)
        {
            color = SelectedColor.Blue;
            btnDefaultColor.BackColor = Color.Blue;
            ChangeColor(Color.Blue);
        }

        private void btnWhite_Click(object sender, EventArgs e)
        {
            color = SelectedColor.White;
            btnDefaultColor.BackColor = Color.White;
            ChangeColor(Color.White);
        }

        private void btnGreen_Click(object sender, EventArgs e)
        {
            color = SelectedColor.Green;
            btnDefaultColor.BackColor = Color.Green;
            ChangeColor(Color.Green);
        }

        private void btnOrange_Click(object sender, EventArgs e)
        {
            color = SelectedColor.Orange;
            btnDefaultColor.BackColor = Color.Orange;
            ChangeColor(Color.Orange);
        }

        private void btnPurple_Click(object sender, EventArgs e)
        {
            color = SelectedColor.Purple;
            btnDefaultColor.BackColor = Color.Violet;
            ChangeColor(Color.Violet);
        }
    }
}
