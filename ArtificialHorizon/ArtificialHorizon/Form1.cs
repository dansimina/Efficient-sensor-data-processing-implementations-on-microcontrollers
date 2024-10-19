using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArtificialHorizon
{
    public partial class Form1 : Form
    {
        private HorizonControl horizonControl;

        public Form1()
        {
            InitializeComponent();
            InitializeHorizonControl();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void InitializeHorizonControl()
        {
            horizonControl = new HorizonControl
            {
                Dock = DockStyle.Fill,
                Pitch = 0 // Set initial pitch
            };
            Controls.Add(horizonControl);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            horizonControl.Pitch = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            horizonControl.Tilt = trackBar2.Value;
        }
    }
}
