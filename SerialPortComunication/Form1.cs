using Microsoft.Win32;
using SerialportComunication.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Settings.Default.Reload();
            comboBox1.Text = Settings.Default.PortName;
            comboBox2.Text = Settings.Default.BaudRate;
            comboBoxParity.Text = Settings.Default.Parity;
            comboBoxDataBits.Text = Settings.Default.DataBits;
            comboBoxStopBits.Text = Settings.Default.StopBits;
            textBoxSend.Text = Settings.Default.Command;
            

            //string[] portNames = SerialPort.GetPortNames();
            //comboBox1.Items.Clear();
            //foreach (string name in portNames)
            //{
            //    comboBox1.Items.Add(name);
            //}
            //if (comboBox1.Items.Count > 0)
            //    comboBox1.SelectedIndex = 0;

            //comboBox2.Text = "115200";
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
                    serialPort.Parity = GetParity(comboBoxParity.Text);
                    serialPort.DataBits = Convert.ToInt32(comboBoxDataBits.Text);
                    serialPort.StopBits = GetStopBits(comboBoxStopBits.Text);
                    serialPort.Open(); 

                    buttonOpenSerial.Text = "关闭串口";

                    byte[] data = new byte[] { 0xFF, 0xFF };
                    string value1 = BitConverter.ToString(data).Replace("-", "");
                    Int16 value2 = Convert.ToInt16(value1, 16);
                    Console.WriteLine(value2);
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

                    byte[] data = new byte[8];
                    char[] sendChar = textBoxSend.Text.Replace(" ", "").ToCharArray();
                    string[] sendContent = new string[sendChar.Length / 2 + 1];
                    for (int i = 0, j = 0; i < sendContent.Length && j < sendChar.Length; i++)
                    {
                        sendContent[i] = string.Format("{0}{1}", sendChar[j].ToString(), sendChar[j + 1].ToString());
                        j += 2;
                        //receiveText.AppendText(string.Format("{0}\r\n", sendContent[i]));
                        data[i] = byte.Parse(sendContent[i], NumberStyles.HexNumber);
                    }
                    serialPort.Write(data, 0, data.Length);

                    //byte[] buffer = Encoding.UTF8.GetBytes(textBoxSend.Text);
                    //serialPort.Write(buffer, 0, buffer.Length);

                    //MessageBox.Show("发送数据" + textBoxSend.Text);
                }
                else
                {
                    MessageBox.Show("发送数据为空！");
                }
            }
        }

        private void textBoxReceive_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //string data = serialPort.ReadExisting();
                //if (data.Trim().Length <= 0)
                //{
                //    return;
                //}

                if (serialPort.BytesToRead <= 0)
                {
                    return;
                }

                byte[] buffer = new byte[serialPort.BytesToRead];
                //byte[] buffer = new byte[]{0x01, 0x04, 0xB4, 0x00, 0x9C, 0x01, 0x33, 0x01, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x91, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFE, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x2C, 0xA1, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1B, 0x00, 0x02, 0x00, 0x53, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x07, 0xD0, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA6, 0x00, 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0xDE, 0xD5};
                serialPort.Read(buffer, 0, buffer.Length);
                string data = BitConverter.ToString(buffer);
                //string data = Encoding.UTF8.GetString(buffer, 0, length);

                //MessageBox.Show("接收数据" + data);
                if (data.Trim() != null)
                {
                    textBoxReceive.AppendText(data);
                    textBoxReceive.AppendText("\r\n");
                }

                int length = 90;
                int[] values = new int[length];
                for (int i = 0; i < length; i++)
                {
                    if ((3 + i * 2) >= buffer.Length || (4 + i * 2) >= buffer.Length)
                    {
                        break;
                    }
                    string value1 = string.Format("{0}{1}", BitConverter.ToString(buffer, 3 + i * 2, 1), BitConverter.ToString(buffer, 4 + i * 2, 1));
                    values[i] = Convert.ToInt16(value1, 16);
                    textBox1.AppendText(values[i].ToString());
                    textBox1.AppendText("   ");
                }
                textBox1.AppendText("\r\n");
                textBox1.AppendText("\r\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.PortName = comboBox1.Text;
            Settings.Default.BaudRate = comboBox2.Text;
            Settings.Default.Parity = comboBoxParity.Text;
            Settings.Default.DataBits = comboBoxDataBits.Text;
            Settings.Default.StopBits = comboBoxStopBits.Text;
            Settings.Default.Command = textBoxSend.Text;
            Settings.Default.Save();
        }

        private Parity GetParity(string value)
        {
            Parity parity;
            switch (value)
            {
                case "None":
                    parity = Parity.None;
                    break;
                case "Odd":
                    parity = Parity.Odd;
                    break;
                case "Even":
                    parity = Parity.Even;
                    break;
                case "Mark":
                    parity = Parity.Mark;
                    break;
                case "Space":
                    parity = Parity.Space;
                    break;
                default:
                    parity = Parity.None;
                    break;
            }
            return parity;
        }

        private StopBits GetStopBits(string value)
        {
            StopBits stopBits;
            switch (value)
            {
                case "None":
                    stopBits = StopBits.None;
                    break;
                case "One":
                    stopBits = StopBits.One;
                    break;
                case "Two":
                    stopBits = StopBits.Two;
                    break;
                case "OnePointFive":
                    stopBits = StopBits.OnePointFive;
                    break;
                default:
                    stopBits = StopBits.None;
                    break;
            }
            return stopBits;
        }
    }
}
