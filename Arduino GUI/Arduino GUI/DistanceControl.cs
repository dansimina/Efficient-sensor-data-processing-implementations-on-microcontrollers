using System;
using System.Drawing;
using System.Windows.Forms;

namespace Arduino_GUI
{
    internal class DistanceControl : Control
    {
        private float distance;

        public double Distance
        {
            get => distance;
            set
            {
                if (Width > 0)
                {
                    distance = Math.Min((float)(value * (Width / 100.0)), Width);
                }
                else
                {
                    distance = 0;
                }

                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawRec(e.Graphics);
        }

        private void DrawRec(Graphics g)
        {
            g.Clear(Color.White);

            using (Pen selPen = new Pen(Color.Magenta))
            {
                g.FillRectangle(Brushes.DarkBlue, 0, 0, distance, Height);
            }
        }
    }
}
