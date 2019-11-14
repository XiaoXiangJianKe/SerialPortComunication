using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialportComunication
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort = new SerialPort();
        private bool isSend = false;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] portNames = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            foreach (string name in portNames)
            {
                comboBox1.Items.Add(name);
            }
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;

            comboBox2.Text = "115200";
            serialPort.DataReceived += textBoxReceive_TextChanged;
        }

        private void buttonOpenSerial_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.PortName = comboBox1.SelectedItem.ToString();
                    serialPort.BaudRate = Convert.ToInt32(comboBox2.Text, 10);
                    serialPort.Open();

                    buttonOpenSerial.Text = "关闭串口";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                serialPort.Close();
                buttonOpenSerial.Text = "打开串口";
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                if (textBoxSend.Text.Trim() != null)
                {
                    //serialPort.Write(textBoxSend.Text);

                    byte[] buffer = Encoding.UTF8.GetBytes(textBoxSend.Text);
                    serialPort.Write(buffer, 0, buffer.Length);

                    //MessageBox.Show("发送数据" + textBoxSend.Text);
                    isSend = true;
                    Console.WriteLine("isSend=" + isSend);
                }
                else
                {
                    MessageBox.Show("发送数据为空！");
                }
            }
        }

        private void textBoxReceive_TextChanged(object sender, EventArgs e)
        {
            //if(isSend)
            //{
            //Thread.Sleep(1000);
                try 
                {
                    Console.WriteLine(isSend);
                    //string data = serialPort.ReadExisting();

                    byte[] buffer = new byte[serialPort.BytesToRead];
                    int length = serialPort.Read(buffer, 0, buffer.Length);
                    string data = Encoding.UTF8.GetString(buffer, 0, length);

                    //MessageBox.Show("接收数据" + data);
                    if (data.Trim() != null)
                    {
                        textBoxReceive.AppendText(data);
                        textBoxReceive.AppendText("\r\n");
                    }
                    isSend = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            //}

        }


    }
}
