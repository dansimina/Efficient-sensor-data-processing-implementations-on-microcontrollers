using System;
using System.Drawing;
using System.Windows.Forms;

namespace ArtificialHorizon
{
    public class HorizonControl : Control
    {
        private float pitch; // Simulate pitch in degrees

        public float Pitch
        {
            get => pitch;
            set
            {
                pitch = value;
                Invalidate(); // Redraw the control when the pitch changes
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

            // Calculate horizon line position based on pitch
            float centerY = Height / 2;
            float horizonY = centerY - (pitch / 90) * (Height / 2);

            // Draw horizon line
            using (Pen pen = new Pen(Color.White, 3))
            {
                g.DrawLine(pen, 0, horizonY, Width, horizonY);
            }

            // Draw the ground (lower part)
            using (Brush brush = new SolidBrush(Color.Green))
            {
                g.FillRectangle(brush, 0, horizonY, Width, Height - horizonY);
            }

            // Draw the sky (upper part)
            using (Brush brush = new SolidBrush(Color.CornflowerBlue))
            {
                g.FillRectangle(brush, 0, 0, Width, horizonY);
            }
        }
    }
}
