﻿using System;
using System.Windows.Forms;
using System.IO.Ports;
using ArtificialHorizon;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading.Tasks;
using System.Threading;

namespace Arduino_GUI
{
    public partial class Form1 : Form
    {
        private HorizonControl horizonControl;
        private DistanceControl distanceControl;
        private string serialDataIn;
        private string auxData;
        const int BAUD_RATE = 115200;
        private const int MAX_TEXTBOX_LENGTH = 10000;

        public Form1()
        {
            InitializeComponent();
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            InitializeHorizonControl();
            InitializeDistanceControl();
            InitializeChart(chartZScoreX);
            InitializeChart(chartZScoreY);
            InitializeChart(chartZScoreD);
        }



        private void InitializeHorizonControl()
        {
            horizonControl = new HorizonControl
            {
                Dock = DockStyle.Fill,
                Pitch = 0,
                Tilt = 0
            };
            horizonPanel.Controls.Add(horizonControl);
        }

        private void InitializeDistanceControl()
        {
            distanceControl = new DistanceControl
            {
                Dock = DockStyle.Fill,
                Distance = 0
            };
            distancePanel.Controls.Add(distanceControl);
        }

        private void InitializeChart(Chart chart)
        {
            try
            {
                int[] xAxis = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
                int[] yAxis = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                chart.Series.Clear();
                Series series = new Series();
                series.ChartType = SeriesChartType.Spline;
                series.Points.DataBindXY(xAxis, yAxis);
                chart.Series.Add(series);

                ChartArea chartArea = chart.ChartAreas[0];
                chartArea.AxisX.Minimum = 1;
                chartArea.AxisX.Maximum = 15;
                chartArea.AxisX.Interval = 1;
                chartArea.AxisY.Minimum = -4;
                chartArea.AxisY.Maximum = 4;
                chartArea.AxisY.Interval = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing chart: {ex.Message}", "Initialization Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            buttonConnect.Enabled = true;
            buttonDisconnect.Enabled = false;
            labelBaudRate.Text = BAUD_RATE.ToString();
        }

        private void buttonScanPort_Click(object sender, EventArgs e)
        {
            try
            {
                string[] portList = SerialPort.GetPortNames();
                comboBoxPort.Items.Clear();
                comboBoxPort.Items.AddRange(portList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error scanning ports: {ex.Message}", "Port Scan Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBoxPort.Text))
            {
                MessageBox.Show("Please select a port first.", "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                serialPort1.PortName = comboBoxPort.Text;
                serialPort1.BaudRate = BAUD_RATE;
                serialPort1.Open();

                buttonConnect.Enabled = false;
                buttonDisconnect.Enabled = true;
            }
            catch (Exception error)
            {
                MessageBox.Show($"Connection error: {error.Message}", "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonDisconnect_Click(object sender, EventArgs e)
        {
            buttonDisconnect.Enabled = false;
            try
            {
                await Task.Run(() =>
                {
                    using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3))) // 3 second timeout
                    {
                        try
                        {
                            if (serialPort1?.IsOpen == true)
                            {
                                // Clear any buffered data
                                serialPort1.DiscardInBuffer();
                                serialPort1.DiscardOutBuffer();

                                // Close the port with timeout
                                Task.Run(() => serialPort1.Close(), cts.Token).Wait(cts.Token);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // Handle timeout
                            MessageBox.Show("Port disconnection timed out. The application will force close the connection.",
                                "Disconnection Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            // Force close the port
                            if (serialPort1 != null)
                            {
                                serialPort1.Dispose();
                            }
                        }
                    }
                });

                buttonConnect.Enabled = true;
                buttonDisconnect.Enabled = false;
            }
            catch (Exception error)
            {
                MessageBox.Show($"Disconnection error: {error.Message}", "Disconnection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3)))
                    {
                        if (serialPort1?.IsOpen == true)
                        {
                            serialPort1.DiscardInBuffer();
                            serialPort1.DiscardOutBuffer();
                            Task.Run(() => serialPort1.Close(), cts.Token).Wait(cts.Token);
                        }
                    }
                });
            }
            catch (Exception)
            {
                // Force dispose on form close
                serialPort1?.Dispose();
            }
        }

        private void DisconnectSerialPort()
        {
            if (serialPort1?.IsOpen == true)
            {
                try
                {
                    serialPort1.Close();
                    buttonConnect.Enabled = true;
                    buttonDisconnect.Enabled = false;
                }
                catch (Exception error)
                {
                    MessageBox.Show($"Disconnection error: {error.Message}", "Disconnection Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private const string START_MARKER = "<";
        private const string END_MARKER = ">#";

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string newData = serialPort1.ReadExisting();
                if (string.IsNullOrEmpty(newData))
                    return;

                auxData += newData;

                int startIndex = auxData.IndexOf(START_MARKER);
                int endIndex = auxData.IndexOf(END_MARKER);

                while (startIndex != -1 && endIndex != -1 && startIndex < endIndex)
                {
                    string message = auxData.Substring(startIndex + 1, endIndex - startIndex - 1);
                    ProcessMessage(message);

                    auxData = auxData.Substring(endIndex + END_MARKER.Length);

                    startIndex = auxData.IndexOf(START_MARKER);
                    endIndex = auxData.IndexOf(END_MARKER);
                }

                if (auxData.Length > 1024)
                {
                    auxData = auxData.Substring(auxData.Length - 1024);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in data received: {ex.Message}");
            }
        }

        private void ProcessMessage(string message)
        {
            try
            {
                var parts = message.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) 
                    return;

                string command = parts[0];
                serialDataIn = message;

                if (ValidateMessageFormat(command, parts))
                {
                    ProcessCommand(command);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }

        private bool ValidateMessageFormat(string command, string[] parts)
        {
            switch (command)
            {
                case "angles":
                    return parts.Length == 3;
                case "distance":
                    return parts.Length == 2;
                case "zscorex":
                case "zscorey":
                case "zscored":
                    return parts.Length == 16; 
                case "running_info":
                    return parts.Length == 3;
                default:
                    return false;
            }
        }

        private void ProcessCommand(string command)
        {
            try
            {
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
                    case "running_info":
                        serialDataIn = serialDataIn.Remove(0, 13);
                        this.Invoke(new EventHandler(DisplayRunningInfo));
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing command: {ex.Message}");
            }
        }

        private void DisplayRunningInfo(object sender, EventArgs e)
        {
            try
            {
                string[] infoParts = serialDataIn.Split(' ');
                if (infoParts.Length > 0)
                {
                    string displayText = $"Average running time: {infoParts[0]} ms\nMax used RAM: {infoParts[1]}\n";

                    if (richTextBox.Text.Length > MAX_TEXTBOX_LENGTH)
                    {
                        richTextBox.Text = displayText;
                    }
                    else
                    {
                        richTextBox.AppendText(displayText);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying running time: {ex.Message}");
            }
        }

        private void UpdateZScore(Chart chart, string data)
        {
            try
            {
                if (string.IsNullOrEmpty(data))
                    return;

                float[] values = Array.ConvertAll(data.Split(' '), s => float.Parse(s));

                var points = chart.Series[0].Points;

                if (values.Length != points.Count)
                {
                    Console.WriteLine($"Mismatch between data length ({values.Length}) and chart points ({points.Count}).");
                    return;
                }

                for (var i = 0; i < points.Count; ++i)
                {
                    points[i].YValues[0] = values[i];
                }

                chart.Invalidate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Z-Score: {ex.Message}");
            }
        }

        private void UpdateZScoreX(object sender, EventArgs e)
        {
            UpdateZScore(chartZScoreX, serialDataIn);
        }

        private void UpdateZScoreY(object sender, EventArgs e)
        {
            UpdateZScore(chartZScoreY, serialDataIn);
        }

        private void UpdateZScoreD(object sender, EventArgs e)
        {
            UpdateZScore(chartZScoreD, serialDataIn);
        }

        private void UpdateDistance(object sender, EventArgs e)
        {
            try
            {
                string[] dist = serialDataIn.Split(' ');
                if (dist.Length == 1 && double.TryParse(dist[0], out double distance))
                {
                    distanceControl.Distance = 100 - (int)distance;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating distance: {ex.Message}");
            }
        }

        private void UpdateHorizon(object sender, EventArgs e)
        {
            try
            {
                string[] coord = serialDataIn.Split(' ');

                if (coord.Length == 2 &&
                    double.TryParse(coord[0], out double x) &&
                    double.TryParse(coord[1], out double y))
                {
                    horizonControl.Pitch = -y * 2;
                    horizonControl.Tilt = -x * 2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating horizon: {ex.Message}");
            }
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                richTextBox.SelectionStart = richTextBox.Text.Length;
                richTextBox.ScrollToCaret();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating text box: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox.Text = "";
        }
    }
}