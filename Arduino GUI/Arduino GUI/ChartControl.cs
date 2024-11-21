using System;
using System.Drawing;
using System.Windows.Forms;

namespace Arduino_GUI
{
    public class ChartControl : Control
    {
        private float[] _xData;
        private float[] _yData;
        private const int Padding = 20;

        public ChartControl(int height, int width)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);
            Size = new Size(width, height);
            BackColor = Color.White;
        }

        public void SetData(float[] x, float[] y)
        {
            _xData = x ?? throw new ArgumentNullException(nameof(x));
            _yData = y ?? throw new ArgumentNullException(nameof(y));

            if (x.Length != y.Length)
                throw new ArgumentException("Arrays must have equal length");

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_xData == null || _yData == null || _xData.Length < 2)
                return;

            var g = e.Graphics;
            var rect = ClientRectangle;

            const float xMin = 1f, xMax = 10f;
            const float yMin = -4f, yMax = 4f;

            float chartWidth = rect.Width - 2 * Padding;
            float chartHeight = rect.Height - 2 * Padding;

            using (var linePen = new Pen(Color.Blue, 2))
            using (var pointBrush = new SolidBrush(Color.Red))
            {
                for (int i = 0; i < _xData.Length - 1; i++)
                {
                    float x1 = Padding + (_xData[i] - xMin) / (xMax - xMin) * chartWidth;
                    float y1 = rect.Height - (Padding + (_yData[i] - yMin) / (yMax - yMin) * chartHeight);
                    float x2 = Padding + (_xData[i + 1] - xMin) / (xMax - xMin) * chartWidth;
                    float y2 = rect.Height - (Padding + (_yData[i + 1] - yMin) / (yMax - yMin) * chartHeight);

                    g.DrawLine(linePen, x1, y1, x2, y2);
                }

                for (int i = 0; i < _xData.Length; i++)
                {
                    float xPos = Padding + (_xData[i] - xMin) / (xMax - xMin) * chartWidth;
                    float yPos = rect.Height - (Padding + (_yData[i] - yMin) / (yMax - yMin) * chartHeight);

                    g.FillEllipse(pointBrush, xPos - 3, yPos - 3, 6, 6);
                }
            }
        }
    }
}