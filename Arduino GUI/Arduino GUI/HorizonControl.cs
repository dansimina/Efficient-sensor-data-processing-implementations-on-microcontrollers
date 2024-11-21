using System;
using System.Drawing;
using System.Windows.Forms;

namespace ArtificialHorizon
{
    internal class HorizonControl : Control
    {
        private double pitch; // Simulate pitch in degrees
        private int tilt;  // Simulate roll in degrees

        public double Pitch
        {
            get => pitch;
            set
            {
                pitch = value / 90.0;
                Invalidate(); // Redraw the control when the pitch changes
            }
        }

        public double Tilt
        {
            get => tilt;
            set
            {
                tilt = (int)Math.Round(Math.Tan(value * Math.PI / 360) * (Width / 2.0));
                Invalidate(); // Redraw the control when the roll changes
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawHorizon(e.Graphics);
        }

        private void DrawHorizon(Graphics g)
        {
            // Draw background
            g.Clear(Color.CornflowerBlue);

            //// Calculate horizon line position based on pitch
            int centerY = Height / 2;
            int horizonY = (int)(centerY - pitch * (Height / 2));

            PointF startPoint = new PointF(0, horizonY + tilt);
            PointF endPoint = new PointF(Width, horizonY - tilt);

            // Create a path for filling the area under the line
            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddLine(startPoint, endPoint);
                path.AddLine(endPoint, new PointF(endPoint.X, this.ClientSize.Height));
                path.AddLine(new PointF(endPoint.X, this.ClientSize.Height), new PointF(startPoint.X, this.ClientSize.Height));
                path.CloseFigure();

                // Fill the area under the line
                using (var brush = new SolidBrush(Color.Green))
                {
                    g.FillPath(brush, path);
                }
            }

            // Draw horizon line
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawLine(pen, 0, centerY, Width, centerY);
            }
        }
    }
}
