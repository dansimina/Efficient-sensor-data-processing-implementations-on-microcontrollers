using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using ArtificialHorizon;
using System.Windows.Forms.DataVisualization.Charting;

namespace Arduino_GUI
{
    public partial class Form1 : Form
    {
        private HorizonControl horizonControl;
        string serialDataIn;
        string auxData;
        public Form1()
        {
            InitializeComponent();

            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            Chart1Control();

            horizonControl = new HorizonControl
            {
                Dock = DockStyle.Fill,
                Pitch = 0 
            };
            horizonPanel.Controls.Add(horizonControl);

            //chart1.
        }

        private void Chart1Control()
        {
            Series series = new Series();
            int[] valoriX = { 0, 1, 2, 3, 4, 5 };
            int[] valoriY = { 0, 1, 3, -2, 2, -3 };

            series.ChartType = SeriesChartType.Spline;

            series.Points.DataBindXY(valoriX, valoriY);
            chart1.Series.Add(series);

            ChartArea chartArea = chart1.ChartAreas[0];

            chartArea.AxisX.Minimum = 0;          // Valoarea minimă de pe axa X
            chartArea.AxisX.Maximum = 10;          // Valoarea maximă de pe axa X
            chartArea.AxisX.Interval = 1;         // Intervalul dintre valorile de pe axa X

            chartArea.AxisY.Minimum = -3;          // Valoarea minimă de pe axa Y
            chartArea.AxisY.Maximum = 3;         // Valoarea maximă de pe axa Y
            chartArea.AxisY.Interval = 1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            buttonConnect.Enabled = true;
            buttonDisconnect.Enabled = false;

            comboBoxBaudRate.Text = "9600"; 
        }

        private void buttonScanPort_Click(object sender, EventArgs e)
        {
            string[] portList = SerialPort.GetPortNames();
            comboBoxPort.Items.Clear();
            comboBoxPort.Items.AddRange(portList);
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBoxPort.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBoxBaudRate.Text);
                serialPort1.Open();

                buttonConnect.Enabled = false;
                buttonDisconnect.Enabled = true;
            }
            catch(Exception error)
            {
                MessageBox.Show(error.Message);
            }

        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            if(serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Close();
                    buttonConnect.Enabled = true;
                    buttonDisconnect.Enabled = false;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Close();
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            auxData += serialPort1.ReadExisting();
            if (auxData[auxData.Length - 1] == '#')
            {
                serialDataIn = auxData;
                auxData = "";
                this.Invoke(new EventHandler(ShowData));
            }
        }

        private void ShowData(object sender, EventArgs e)
        {
            string[] coord = serialDataIn.Split(' ');

            if (coord.Length == 3)
            {
                double x = Convert.ToDouble(coord[0]) * 2;
                double y = Convert.ToDouble(coord[1]) * 2;

                horizonControl.Pitch = -x;
                horizonControl.Tilt = -y;

                richTextBox.Text += "x: " + coord[0] + " y: " + coord[1] + "\n";
            }
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            richTextBox.SelectionStart = richTextBox.Text.Length;
            richTextBox.ScrollToCaret();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
