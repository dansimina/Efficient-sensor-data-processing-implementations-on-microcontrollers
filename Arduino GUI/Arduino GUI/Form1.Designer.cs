﻿namespace Arduino_GUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title3 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.ConnectionPanel = new System.Windows.Forms.Panel();
            this.labelBaudRate = new System.Windows.Forms.Label();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxPort = new System.Windows.Forms.ComboBox();
            this.buttonScanPort = new System.Windows.Forms.Button();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.horizonPanel = new System.Windows.Forms.Panel();
            this.chartZScoreX = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartZScoreY = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartZScoreD = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.distancePanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.ConnectionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartZScoreX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartZScoreY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartZScoreD)).BeginInit();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // ConnectionPanel
            // 
            this.ConnectionPanel.BackColor = System.Drawing.Color.White;
            this.ConnectionPanel.Controls.Add(this.labelBaudRate);
            this.ConnectionPanel.Controls.Add(this.buttonDisconnect);
            this.ConnectionPanel.Controls.Add(this.buttonConnect);
            this.ConnectionPanel.Controls.Add(this.label1);
            this.ConnectionPanel.Controls.Add(this.comboBoxPort);
            this.ConnectionPanel.Controls.Add(this.buttonScanPort);
            this.ConnectionPanel.Location = new System.Drawing.Point(25, 12);
            this.ConnectionPanel.Name = "ConnectionPanel";
            this.ConnectionPanel.Size = new System.Drawing.Size(297, 765);
            this.ConnectionPanel.TabIndex = 0;
            // 
            // labelBaudRate
            // 
            this.labelBaudRate.AutoSize = true;
            this.labelBaudRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBaudRate.Location = new System.Drawing.Point(109, 184);
            this.labelBaudRate.Name = "labelBaudRate";
            this.labelBaudRate.Size = new System.Drawing.Size(85, 29);
            this.labelBaudRate.TabIndex = 6;
            this.labelBaudRate.Text = "label2";
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDisconnect.Location = new System.Drawing.Point(56, 328);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(200, 50);
            this.buttonDisconnect.TabIndex = 5;
            this.buttonDisconnect.Text = "DISCONNECT";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonConnect.Location = new System.Drawing.Point(56, 263);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(200, 50);
            this.buttonConnect.TabIndex = 4;
            this.buttonConnect.Text = "CONNECT";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(92, 144);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 29);
            this.label1.TabIndex = 2;
            this.label1.Text = "Baud Rate";
            // 
            // comboBoxPort
            // 
            this.comboBoxPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxPort.FormattingEnabled = true;
            this.comboBoxPort.ItemHeight = 29;
            this.comboBoxPort.Location = new System.Drawing.Point(56, 64);
            this.comboBoxPort.Name = "comboBoxPort";
            this.comboBoxPort.Size = new System.Drawing.Size(200, 37);
            this.comboBoxPort.TabIndex = 1;
            // 
            // buttonScanPort
            // 
            this.buttonScanPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonScanPort.Location = new System.Drawing.Point(56, 13);
            this.buttonScanPort.Name = "buttonScanPort";
            this.buttonScanPort.Size = new System.Drawing.Size(200, 45);
            this.buttonScanPort.TabIndex = 0;
            this.buttonScanPort.Text = "Scan Port";
            this.buttonScanPort.UseVisualStyleBackColor = true;
            this.buttonScanPort.Click += new System.EventHandler(this.buttonScanPort_Click);
            // 
            // richTextBox
            // 
            this.richTextBox.Location = new System.Drawing.Point(361, 424);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(407, 350);
            this.richTextBox.TabIndex = 2;
            this.richTextBox.Text = "";
            this.richTextBox.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // horizonPanel
            // 
            this.horizonPanel.Location = new System.Drawing.Point(361, 12);
            this.horizonPanel.Name = "horizonPanel";
            this.horizonPanel.Size = new System.Drawing.Size(407, 274);
            this.horizonPanel.TabIndex = 3;
            // 
            // chartZScoreX
            // 
            chartArea1.Name = "ChartArea1";
            this.chartZScoreX.ChartAreas.Add(chartArea1);
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.chartZScoreX.Legends.Add(legend1);
            this.chartZScoreX.Location = new System.Drawing.Point(808, 12);
            this.chartZScoreX.Name = "chartZScoreX";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartZScoreX.Series.Add(series1);
            this.chartZScoreX.Size = new System.Drawing.Size(875, 234);
            this.chartZScoreX.TabIndex = 4;
            title1.Name = "Title1";
            title1.Text = "Z-Score X";
            this.chartZScoreX.Titles.Add(title1);
            // 
            // chart2
            // 
            chartArea2.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart2.Legends.Add(legend2);
            this.chart2.Location = new System.Drawing.Point(1176, 238);
            this.chart2.Name = "chart2";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart2.Series.Add(series2);
            this.chart2.Size = new System.Drawing.Size(8, 8);
            this.chart2.TabIndex = 5;
            this.chart2.Text = "chart2";
            // 
            // chartZScoreY
            // 
            chartArea3.Name = "ChartArea1";
            this.chartZScoreY.ChartAreas.Add(chartArea3);
            legend3.Enabled = false;
            legend3.Name = "Legend1";
            this.chartZScoreY.Legends.Add(legend3);
            this.chartZScoreY.Location = new System.Drawing.Point(808, 275);
            this.chartZScoreY.Name = "chartZScoreY";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chartZScoreY.Series.Add(series3);
            this.chartZScoreY.Size = new System.Drawing.Size(875, 234);
            this.chartZScoreY.TabIndex = 10;
            title2.Name = "Title1";
            title2.Text = "Z-Score Y";
            this.chartZScoreY.Titles.Add(title2);
            // 
            // chartZScoreD
            // 
            chartArea4.Name = "ChartArea1";
            this.chartZScoreD.ChartAreas.Add(chartArea4);
            legend4.Enabled = false;
            legend4.Name = "Legend1";
            this.chartZScoreD.Legends.Add(legend4);
            this.chartZScoreD.Location = new System.Drawing.Point(808, 541);
            this.chartZScoreD.Name = "chartZScoreD";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chartZScoreD.Series.Add(series4);
            this.chartZScoreD.Size = new System.Drawing.Size(875, 234);
            this.chartZScoreD.TabIndex = 11;
            title3.Name = "Title1";
            title3.Text = "DISTANCE";
            this.chartZScoreD.Titles.Add(title3);
            // 
            // distancePanel
            // 
            this.distancePanel.Location = new System.Drawing.Point(361, 302);
            this.distancePanel.Name = "distancePanel";
            this.distancePanel.Size = new System.Drawing.Size(407, 48);
            this.distancePanel.TabIndex = 13;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(361, 373);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(407, 45);
            this.button1.TabIndex = 7;
            this.button1.Text = "Clear";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1698, 824);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.distancePanel);
            this.Controls.Add(this.chartZScoreD);
            this.Controls.Add(this.chartZScoreY);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.chartZScoreX);
            this.Controls.Add(this.horizonPanel);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.ConnectionPanel);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1720, 880);
            this.MinimumSize = new System.Drawing.Size(1720, 880);
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ConnectionPanel.ResumeLayout(false);
            this.ConnectionPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartZScoreX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartZScoreY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartZScoreD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Panel ConnectionPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxPort;
        private System.Windows.Forms.Button buttonScanPort;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.Panel horizonPanel;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartZScoreX;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartZScoreY;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartZScoreD;
        private System.Windows.Forms.Panel distancePanel;
        private System.Windows.Forms.Label labelBaudRate;
        private System.Windows.Forms.Button button1;
    }
}

