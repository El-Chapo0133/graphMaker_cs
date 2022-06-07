using System.Drawing;

namespace GraphMaker;

internal class DrawTools
{
    private Pen Pen { get; set; } = Pens.Black;
    private Brush Brush { get; set; } = Brushes.Black;

    #region constructors
    public DrawTools(Pen pen, Brush brush)
    {
        Pen = pen;
        Brush = brush;
    }
    public DrawTools(Pen pen, byte red, byte green, byte blue)
    {
        Pen = pen;
        Brush = new SolidBrush(Color.FromArgb(red, green, blue));
    }
    public DrawTools(Pen pen)
    {
        Pen = pen;
    }
    public DrawTools(Brush brush)
    {
        Brush = brush;
    }
    public DrawTools(byte red, byte green, byte blue)
    {
        Brush = new SolidBrush(Color.FromArgb(red, green, blue));
    }
    #endregion

    #region getters
    public Pen GetPen() =>
        Pen;
    
    public Brush GetBrush() =>
        Brush;
    #endregion

    #region setters
    public void SetPen(Pen pen) =>
        Pen = pen;
    public void SetPenWidth(int width) =>
        Pen.Width = width;
    public void SetPenColor(Color color) =>
        Pen.Color = color;

    public void SetBrush(Brush brush) =>
        Brush = brush;
    public void SetBrush(byte red, byte green, byte blue) =>
        Brush = new SolidBrush(Color.FromArgb(red, green, blue));
    #endregion
}

internal class GraphData
{
    internal class GraphGroupData
    {
        public List<double> DataPoints = new();
        public string Name = "default name";
        public Color LinesColor = Color.Black;

        public GraphGroupData(string name, Color linesColor, List<double> dataPoint)
        {
            Name = name;
            LinesColor = linesColor;
            DataPoints = dataPoint;
        }
    }
    private readonly List<GraphGroupData> Data = new();
    public string GraphName { get; set; } = "default graph name";
    public (string, string) MetaDatas = ("MetaData of axis X", "MetaData of axis Y");

    public GraphData(string name, (string, string) metaDatas, List<GraphGroupData> data)
    {
        GraphName = name;
        MetaDatas = metaDatas;
        Data = data;
    }
    public GraphData(string name, (string, string) metaDatas, GraphGroupData data)
    {
        GraphName = name;
        MetaDatas = metaDatas;
        Data = new() { data };
    }
    public GraphData(string name, (string, string) metaDatas)
    {
        GraphName = name;
        MetaDatas = metaDatas;
    }
    public GraphData(string name)
    {
        GraphName = name;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="linesColor"></param>
    /// <param name="dataPoints"></param>
    /// <returns>If the GroupData has been added</returns>
    public bool AddGroupData(string name, Color linesColor, List<double> dataPoints)
    {
        if (DataContainsName(name) || DataContainsColor(linesColor))
            return false;

        Data.Add(new(name, linesColor, dataPoints));

        return true;
    }
    public bool AddGroupData(GraphGroupData groupData)
    {
        if (DataContainsName(groupData.Name) || DataContainsColor(groupData.LinesColor))
            return false;

        Data.Add(groupData);

        return true;
    }


    private bool DataContainsName(string name) =>
        Data.FirstOrDefault(x => x.Name.ToLower() == name.ToLower()) is not null;
    private bool DataContainsColor(Color color) =>
        Data.FirstOrDefault(x => x.LinesColor.R == color.R && x.LinesColor.G == color.G && x.LinesColor.B == color.B) is not null;
}

public class GraphToBitmap
{
    private enum GraphType
    {
        Classic,
        VerticalBars,
        HorizontalBars,
        Dots,
        VerticalStackedBar,
        HorizontalStackedBar
    };


    private const int DEFAULT_BITMAP_WIDTH = 100;
    private const int DEFAULT_BITMAP_HEIGHT = 100;


    private Bitmap OutputBitmap { get; set; } = new(width: DEFAULT_BITMAP_WIDTH, height: DEFAULT_BITMAP_HEIGHT);
    private Graphics BitmapGraphics { get; set; }

    private GraphType GraphTape = GraphType.Classic;


    

    public GraphToBitmap()
    {
        BitmapGraphics = Graphics.FromImage(OutputBitmap);
    }


    private void SetGraphics() =>
        BitmapGraphics = Graphics.FromImage(OutputBitmap);

    public Bitmap GetGraph() =>
        OutputBitmap;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns>If the type has been setted</returns>
    public bool SetGraphType(string input)
    {
        if (GetAllGraphType().Contains(input))
        {
            GraphTape = (GraphType)Enum.Parse(typeof(GraphType), input);
            return true;
        }
        return false;
    }
    public string GetGraphType()
    {
        var result = Enum.GetName(typeof(GraphType), GraphTape);
        if (result is null)
            return "not retreivable";
        return result.ToString();
    }

    public void ResetGraph()
    {
        OutputBitmap = new(
            width: OutputBitmap.Width,
            height: OutputBitmap.Height
        );
        SetGraphics();
    }
    public void ResizeBitmap(int width, int height)
    {
        Bitmap bufferBitmap = (Bitmap)OutputBitmap.Clone();
        OutputBitmap = new(width: width, height: height);
        using Graphics graphics = Graphics.FromImage(OutputBitmap);
        graphics.DrawImage(bufferBitmap, 0, 0, bufferBitmap.Width, bufferBitmap.Height);
        SetGraphics();
    }
    public void ResizeBitmapWidth(int width)
    {
        Bitmap bufferBitmap = (Bitmap)OutputBitmap.Clone();
        OutputBitmap = new(width: width, height: OutputBitmap.Height);
        using Graphics graphics = Graphics.FromImage(OutputBitmap);
        graphics.DrawImage(bufferBitmap, 0, 0, bufferBitmap.Width, bufferBitmap.Height);
        SetGraphics();
    }
    public void ResizeBitmapHeight(int height)
    {
        Bitmap bufferBitmap = (Bitmap)OutputBitmap.Clone();
        OutputBitmap = new(width: OutputBitmap.Width, height: height);
        using Graphics graphics = Graphics.FromImage(OutputBitmap);
        graphics.DrawImage(bufferBitmap, 0, 0, bufferBitmap.Width, bufferBitmap.Height);
        SetGraphics();
    }

    public void DrawBaseGraph()
    {
        DrawLine(10, 10, 10, 100);
        DrawLine(10, 100, 100, 100);
    }
    public void DrawBaseGraph(bool includesCaption)
    {
        if (includesCaption)
            DrawBaseGraph();

        
    }

    public void DrawBackgroundColor()
    {
        Rectangle rectangle = new(0, 0, OutputBitmap.Width, OutputBitmap.Height);
        BitmapGraphics.FillRectangle(Brushes.White, rectangle);
    }
    public void DrawBackgroundColor(Brush brush)
    {
        Rectangle rectangle = new(0, 0, OutputBitmap.Width, OutputBitmap.Height);
        BitmapGraphics.FillRectangle(brush, rectangle);
    }
    private void DrawBackgroundColor(byte red, byte blue, byte green)
    {
        SolidBrush brush = new(Color.FromArgb(red, blue, green));
        Rectangle rectangle = new(0, 0, OutputBitmap.Width, OutputBitmap.Height);
        BitmapGraphics.FillRectangle(brush, rectangle);
    }

    private void DrawLine(int startX, int startY, int destX, int destY)
    {
        BitmapGraphics.DrawLine(new Pen(Color.Black, 1), new Point(startX, startY), new Point(destX, destY));
    }
    private void DrawLine(Point start, Point dest)
    {
        BitmapGraphics.DrawLine(new Pen(Color.Black, 1), start, dest);
    }

    private string[] GetAllGraphType() =>
        (string[])Enum.GetValues(typeof(GraphType));
}
