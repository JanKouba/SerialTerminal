using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;

namespace SerialTerminal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        SerialPort serialPort = new SerialPort();

        #region Form events
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillComboBoxPort();
            FillComboBoxBaudRate();

            serialPort.DataReceived += SerialPort_DataReceived;
        }

        private void buttonOpen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenClosePort();
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            SerialPort_Write(textBoxCommand.Text);
        }

        private void buttonGetTemp_Click(object sender, RoutedEventArgs e)
        {
            SerialPort_Write("temp");
        }

        private void buttonGetPress_Click(object sender, RoutedEventArgs e)
        {
            SerialPort_Write("press");
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            textBoxLog.Clear();
        }

        #endregion

        #region Combo box filling

        private void FillComboBoxPort()
        {
            for (int i = 1; i < 10; i++)
            {
                comboBoxPort.Items.Add("COM" + i.ToString());
            }
        }

        private void FillComboBoxBaudRate()
        {
            int baudrate = 300;
            for (int i = 1; i < 8; i++)
                comboBoxBaudrate.Items.Add((baudrate *= 2).ToString());

            comboBoxBaudrate.Items.Add((115200).ToString());

        }

        #endregion

        #region Serial port handling

        private void OpenClosePort()
        {
            string status = string.Empty;

            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.PortName = comboBoxPort.Text;
                    serialPort.BaudRate = Convert.ToInt32(comboBoxBaudrate.Text);
                    serialPort.WriteTimeout = 50;
                    serialPort.ReadTimeout = 2000;

                    serialPort.Open();

                    status = "OK";

                    buttonOpen.Content = "Close";
                }
                else
                {
                    serialPort.Close();
                    buttonOpen.Content = "Open";

                    status = "Closed";
                }
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }
            finally
            {
                labelStatus.Content = status;
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    textBoxLog.AppendText(String.Format("{0}{1}"
                       , DateTime.Now.ToString("[HH:mm:ss.fff] ")
                       , serialPort.ReadLine()));

                    textBoxLog.ScrollToEnd();
                }

                catch (Exception ex)
                {
                    labelStatus.Content = ex.Message;
                }
            }
            );
          
        }

        private void SerialPort_Write(string command)
        {
            string status = string.Empty;

            try
            {
                serialPort.Write(command);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    textBoxLog.AppendText(String.Format("{0}{1}{2}{3}"
                        , DateTime.Now.ToString("[HH:mm:ss.fff] ")
                        , "[MyPC] [Sent] "
                        , command
                        , Environment.NewLine));

                    textBoxLog.ScrollToEnd();
                }
                );


            }
            catch (Exception ex)
            {
                labelStatus.Content = ex.Message;
            }
           

        }

        #endregion





    }
}
