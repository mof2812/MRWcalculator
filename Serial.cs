using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace MRWCalculator
{
    enum SERIAL_DEVICE_T
    {
        LOADCELL = 0,
        CONTROLLER = 1,
    }
    public struct SERIAL_PARAMETER_T
    {
        public string PortName;
        public int BaudRate;
        public byte DataBits;
        public StopBits StopBits;
    }
    public partial class TempCharacteristics : Form
    {
        public bool CommunicationOn = false;
        private RETURN_T Serial_ClosePort(SERIAL_DEVICE_T Device)
        {
            RETURN_T RetVal;

            RetVal = RETURN_T.OKAY;

            try
            {
               sptLoadCell.Close();
               CommunicationOn = false; 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                RetVal = RETURN_T.SERIAL_PORT_CLOSE;
            }
            return (RetVal);
        }
        private RETURN_T Serial_Init()
        {
            RETURN_T RetVal;
            SerialPort port = new SerialPort();

            RetVal = RETURN_T.OKAY;

            port = sptLoadCell;
            
            port.PortName = Properties.Settings.Default.SerialPort;
            port.ReadTimeout = (int)Properties.Settings.Default.TimeoutGetData + 50;
            port.BaudRate = Properties.Settings.Default.Baudrate;
            port.DataBits = Properties.Settings.Default.DataBits;
            port.StopBits = (StopBits)Properties.Settings.Default.StopBits;

            if (port.PortName.Length > 3)
            {
                try
                {
                    port.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    RetVal = RETURN_T.SERIAL_PORT_UNDEF;
                }
            }
            

            return (RetVal);
        }
        private RETURN_T Serial_OpenPort(SERIAL_DEVICE_T Device)
        {
            RETURN_T RetVal;

            RetVal = RETURN_T.OKAY;

            try
            {
                sptLoadCell.Open();
                CommunicationOn = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                CommunicationOn = false;
                RetVal = RETURN_T.SERIAL_PORT_OPEN;
            }

            return(RetVal);
        }
        private RETURN_T Serial_SetBaudrate(SERIAL_DEVICE_T Device, int Baudrate)
        {
            RETURN_T RetVal;
            SerialPort port = new SerialPort();
            bool IsOpen;

            RetVal = RETURN_T.OKAY;

            if ((Baudrate >= 300) && (Baudrate <= 115200))
            {
                port = sptLoadCell;

                if (RetVal == RETURN_T.OKAY)
                {
                    IsOpen = port.IsOpen;

                    if (IsOpen == true)
                    {
                        RetVal = Serial_ClosePort(Device);
                    }

                    if (RetVal == RETURN_T.OKAY)
                    {
                        port.BaudRate = Baudrate;

                        if (Baudrate != Properties.Settings.Default.Baudrate)
                        {
                            Properties.Settings.Default.Baudrate = Baudrate;
                            Properties.Settings.Default.Save();
                        }

                        if (IsOpen == true)
                        {
                            RetVal = Serial_OpenPort(Device);
                        }
                    }
                }
            }
            return (RetVal);
        }
        private RETURN_T Serial_SetDatabits(SERIAL_DEVICE_T Device, byte Databits)
        {
            RETURN_T RetVal;
            bool IsOpen;
            SerialPort port = new SerialPort();

            RetVal = RETURN_T.OKAY;

            if ((Databits >= 5) && (Databits <= 8))
            {
                port = sptLoadCell;

                if (RetVal == RETURN_T.OKAY)
                {
                    IsOpen = port.IsOpen;

                    if (IsOpen == true)
                    {
                        RetVal = Serial_ClosePort(Device);
                    }

                    if (RetVal == RETURN_T.OKAY)
                    {
                        port.DataBits = Databits;

                        if (Databits != Properties.Settings.Default.DataBits)
                        {
                            Properties.Settings.Default.DataBits = Databits;
                            Properties.Settings.Default.Save();
                        }

                        if (IsOpen == true)
                        {
                            RetVal = Serial_OpenPort(Device);
                        }
                    }
                }
            }
            return (RetVal);
        }
        private RETURN_T Serial_SetPort(SERIAL_DEVICE_T Device, String Port)
        {
            RETURN_T RetVal;
            SerialPort port = new SerialPort();

            RetVal = RETURN_T.OKAY;

            if (Port.Length < 4)
            {
                RetVal = RETURN_T.SERIAL_PORT_UNDEF;
            }
            else
            {
                port = sptLoadCell;

                if (RetVal == RETURN_T.OKAY)
                {
                    if (sptLoadCell.IsOpen == true)
                    {
                        RetVal = Serial_ClosePort(Device);
                    }

                    if (RetVal == RETURN_T.OKAY)
                    {
                        port.PortName = Port;

                        if (Port != Properties.Settings.Default.SerialPort)
                        {
                            Properties.Settings.Default.SerialPort = Port;
                            Properties.Settings.Default.Save();
                        }

                        if (port.IsOpen == false)
                        {
                            RetVal = Serial_OpenPort(Device);
                        }
                    }
                }
                
            }
            return (RetVal);
        }
        private RETURN_T Serial_SetStopbits(SERIAL_DEVICE_T Device, StopBits Stopbits)
        {
            RETURN_T RetVal;
            SerialPort port = new SerialPort();
            bool IsOpen;

            RetVal = RETURN_T.OKAY;

            port = sptLoadCell;

            if (RetVal == RETURN_T.OKAY)
            {
                IsOpen = port.IsOpen;

                if (IsOpen == true)
                {
                    RetVal = Serial_ClosePort(Device);
                }

                if (RetVal == RETURN_T.OKAY)
                {
                    port.StopBits = Stopbits;

                    if (Stopbits != Properties.Settings.Default.StopBits)
                    {
                        Properties.Settings.Default.StopBits = Stopbits;
                        Properties.Settings.Default.Save();
                    }

                    if (IsOpen == true)
                    {
                        RetVal = Serial_OpenPort(Device);
                    }
                }
            }
            return (RetVal);
        }
        private RETURN_T Serial_Write(SERIAL_DEVICE_T Device, String Command)
        {
            RETURN_T RetVal;
            SerialPort port = new SerialPort();
            String Frame2Send = "";

            RetVal = RETURN_T.OKAY;
            Frame2Send = (char)0x02 + Command + (char)0x03;

            port = sptLoadCell;

            try
            {
                port.Write(Frame2Send);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                RetVal = RETURN_T.SERIAL_PORT_WRITE;
            }

//            Debug_AddSendFrame(Command, RetVal);

            return (RetVal);
        }
    }
}
