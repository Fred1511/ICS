using System.Windows.Media;

namespace Agenda_ICS.Views.Calendar
{
    class CHatchedBrushCreator
    {
        public static Brush Create()
        {
            Color FillColor;
            double HatchThickness;
            double HatchAngle;

            FillColor = Colors.SlateGray;
            HatchThickness = 5;
            HatchAngle = 45;

            DrawingBrush myBrush = new DrawingBrush();

            GeometryGroup myGeometryGroup = new GeometryGroup();

            // 
            // add a horizontal line to the geometry group
            // 
            myGeometryGroup.Children.Add(new LineGeometry(new System.Windows.Point(0, 0), new System.Windows.Point(10, 0)));

            // 
            // draw geometry with transparent brush and pen as defined
            // 
            System.Windows.Media.Pen p = new System.Windows.Media.Pen();
            p.Brush = new SolidColorBrush(FillColor);
            p.Thickness = HatchThickness;
            p.StartLineCap = PenLineCap.Square;
            p.EndLineCap = PenLineCap.Square;

            GeometryDrawing myDrawing = new GeometryDrawing(null/* TODO Change to default(_) if this is not a reference type */, p, myGeometryGroup);

            // 
            // apply the drawing to the brush
            // 
            myBrush.Drawing = myDrawing;

            // 
            // in case, there is more than one line use a Drawing Group
            // 

            // Dim myDrawingGroup As New DrawingGroup()
            // myDrawingGroup.Children.Add(checkers)
            // myBrush.Drawing = myDrawingGroup

            // set viewbox and viewport
            myBrush.Viewbox = new System.Windows.Rect(0, 0, 10, 10);
            myBrush.ViewboxUnits = BrushMappingMode.Absolute;
            myBrush.Viewport = new System.Windows.Rect(0, 0, 10, 10);
            myBrush.ViewportUnits = BrushMappingMode.Absolute;
            myBrush.TileMode = TileMode.Tile;
            myBrush.Stretch = Stretch.UniformToFill;
            // rotate
            myBrush.Transform = new RotateTransform(HatchAngle);

            return myBrush;
        }
    }
}
