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

            InitializeHorizonControl();
            InitializeProgressBar();
            InitializeChart(chartZScoreX);
            InitializeChart(chartZScoreY);
            InitializeChart(chartZScoreD);
        }

        private void InitializeHorizonControl()
        {
            horizonControl = new HorizonControl
            {
                Dock = DockStyle.Fill,
                Pitch = 0
            };
            horizonPanel.Controls.Add(horizonControl);
        }

        private void InitializeProgressBar()
        {
            progressBarDistance.Style = ProgressBarStyle.Continuous;
            progressBarDistance.Value = 0;
            progressBarDistance.Minimum = 0;
            progressBarDistance.Maximum = 100;
        }

        private void InitializeChart(Chart chart)
        {
            int[] xAxis = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15, 17, 18, 19, 20 };
            int[] yAxis = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            Series series = new Series();
            series.ChartType = SeriesChartType.Spline;
            series.Points.DataBindXY(xAxis, yAxis);
            chart.Series.Add(series);

            ChartArea chartArea = chart.ChartAreas[0];
            chartArea.AxisX.Minimum = 1;          // Valoarea minimă de pe axa X
            chartArea.AxisX.Maximum = 20;          // Valoarea maximă de pe axa X
            chartArea.AxisX.Interval = 1;         // Intervalul dintre valorile de pe axa X
            chartArea.AxisY.Minimum = -4;          // Valoarea minimă de pe axa Y
            chartArea.AxisY.Maximum = 4;         // Valoarea maximă de pe axa Y
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
                String command = serialDataIn.Split(' ')[0];

                switch (command)
                {
                    case "angles":
                        serialDataIn = serialDataIn.Remove(0, 7);
                        this.Invoke(new EventHandler(UpdateHorizon));
                        break;
                    case "zscorex":
                        serialDataIn = serialDataIn.Remove(0, 8);
                        this.Invoke(new EventHandler(UpdateZScoreX));
                        break;

                    case "zscorey":
                        serialDataIn = serialDataIn.Remove(0, 8);
                        this.Invoke(new EventHandler(UpdateZScoreY));
                        break;

                    case "distance":
                        serialDataIn = serialDataIn.Remove(0, 9);
                        this.Invoke(new EventHandler(UpdateDistance));
                        break;

                    case "zscored":
                        serialDataIn = serialDataIn.Remove(0, 8);
                        this.Invoke(new EventHandler(UpdateZScoreD));
                        break;
                }
            }
        }

        private void UpdateZScoreX(object sender, EventArgs e)
        {
            try
            {
                serialDataIn = serialDataIn.Remove(serialDataIn.Length - 2, 2);
                float[] values = Array.ConvertAll(serialDataIn.Split(' '), s => float.Parse(s));

                var points = chartZScoreX.Series[1].Points;

                if (values.Length != points.Count)
                {
                    Console.WriteLine("Mismatch between data length and chart points.");
                    return;
                }

                for (var i = 0; i < points.Count; ++i)
                {
                    points[i].YValues[0] = values[i];
                }

                chartZScoreX.Invalidate();
                
            }
            catch (Exception error)
            {
            }
        }

        private void UpdateZScoreY(object sender, EventArgs e)
        {
            try
            {
                serialDataIn = serialDataIn.Remove(serialDataIn.Length - 2, 2);
                float[] values = Array.ConvertAll(serialDataIn.Split(' '), s => float.Parse(s));

                var points = chartZScoreY.Series[1].Points;

                if (values.Length != points.Count)
                {
                    Console.WriteLine("Mismatch between data length and chart points.");
                    return;
                }

                for (var i = 0; i < points.Count; ++i)
                {
                    points[i].YValues[0] = values[i];
                }

                chartZScoreY.Invalidate();

            }
            catch (Exception error)
            {
            }
        }

        private void UpdateZScoreD(object sender, EventArgs e)
        {
            try
            {
                serialDataIn = serialDataIn.Remove(serialDataIn.Length - 2, 2);
                float[] values = Array.ConvertAll(serialDataIn.Split(' '), s => float.Parse(s));

                var points = chartZScoreD.Series[1].Points;

                if (values.Length != points.Count)
                {
                    Console.WriteLine("Mismatch between data length and chart points.");
                    return;
                }

                for (var i = 0; i < points.Count; ++i)
                {
                    points[i].YValues[0] = values[i];
                }

                chartZScoreD.Invalidate();

            }
            catch (Exception error)
            {
            }
        }

        private void UpdateDistance(object sender, EventArgs e)
        {
            string[] dist = serialDataIn.Split(' ');
            int distance = (int)Convert.ToDouble(dist[0]);
            progressBarDistance.Value = 100 - distance;
        }

        private void UpdateHorizon(object sender, EventArgs e)
        {
            string[] coord = serialDataIn.Split(' ');

            if (coord.Length == 3)
            {
                double x = Convert.ToDouble(coord[0]) * 2;
                double y = Convert.ToDouble(coord[1]) * 2;

                horizonControl.Pitch = -y;
                horizonControl.Tilt = -x;

                //richTextBox.Text += "x: " + coord[0] + " y: " + coord[1] + "\n";
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
