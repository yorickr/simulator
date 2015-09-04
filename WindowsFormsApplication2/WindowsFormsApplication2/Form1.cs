using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        private SerialPort port;
        private delegate void SetTextDeleg(string data);
        private int afstand, tijd, vermogen;
        

        public Form1()
        {
            this.afstand = 0;
            this.tijd = 0;
            this.vermogen = 0;
            InitializeComponent();
            try
            {
                port = new SerialPort("COM10");

                port.BaudRate = 9600;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.DataBits = 8;
                port.Handshake = Handshake.None;
                port.ReadTimeout = 2000;
                port.WriteTimeout = 500;

                port.DtrEnable = true;

                port.RtsEnable = true;

                port.Open();

                port.DataReceived += DataReceivedHandler;

            }
            catch (Exception ex)
            {
               
            }

        }

        public void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            System.Threading.Thread.Sleep(500);
            string indata = sp.ReadExisting();
            // Console.WriteLine("Received");
            indata = indata.Replace("CM", "").Trim(); 
            string command = indata.Substring(0, 2);

            //is.BeginInvoke(new SetTextDeleg(Display), new object[] { command });
            afstand += 100;
            tijd += 30;

            switch (command)
            {
                case "PW":
                    
                    vermogen = int.Parse(indata.Replace("PW ", "").Trim());
                    String verm = "" + vermogen;
                    setTextLbl06(verm);
                    port.WriteLine($"{afstand} {vermogen} {tijd}");
                    break;
                case "ST":
                    port.WriteLine($"{afstand} {vermogen} {tijd}");
                    break;
                case "RS":
                    setTextLbl04("" + 0 + " s");
                    setTextLbl02("" + 0 + " m");
                    setTextLbl06("" + 0 + ", W");
                    vermogen = 0;
                    tijd = 0;
                    afstand = 0;
                    break;
                default:
                    port.WriteLine("ERROR");
                    break;
            }

            
            setTextLbl04("" + tijd + " s");
            setTextLbl02("" + afstand + " m");
            this.BeginInvoke(new SetTextDeleg(Display), new object[] { indata });
        }
        
        

        private void Display(string displayData)
        {
            richTextBox1.Text += displayData;

        }
    }
    
}
