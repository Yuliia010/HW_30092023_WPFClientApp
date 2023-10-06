using HW_30092023_WPFClientApp.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
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
using System.IO;

namespace HW_30092023_WPFClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NetworkConnection connect;
        public MainWindow()
        {
            InitializeComponent();
            connect = new NetworkConnection();
        }
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await CheckConnection();
        }
        private async Task CheckConnection()
        {
            await Task.Run(async () =>
            {
                while (true)
                {

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (connect.IsConnected())
                        {
                            btn_Connect.Background = new SolidColorBrush(Colors.LightGreen);
                            btn_Connect.Content = "Connected";
                            
                        }
                        else
                        {
                            btn_Connect.Background = new SolidColorBrush(Colors.IndianRed);
                            btn_Connect.Content = "Disconnected";
                        }
                    });


                    await Task.Delay(1000);
                }
            });
        }



        private void btn_GetQuotation_Click(object sender, RoutedEventArgs e)
        {
            if(connect.IsConnected())
            {
                connect.SendARequest();
                connect.GetAnswer();
                tb_Answer.Text = connect.ReturnAnswer();
            }
            else
            {
                MessageBox.Show("Connect to the server!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           

        }


        private async void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!connect.IsConnected())
            {
                if (tb_IP.Text.Length == 0 || tb_Port.Text.Length == 0)
                {
                    MessageBox.Show("Input data to connect!");
                }
                else
                {
                    await ConnectAction();
                }
            }
            else if (connect.IsConnected())
            {
                await connect.Disconnect();
            }
        }

        private async Task ConnectAction()
        {
            string strport = tb_Port.Text;
            int port;
            if (int.TryParse(strport, out port))
            {
                string host = tb_IP.Text;
                try
                {
                    await connect.Connect(host, port);
                    MessageBox.Show("Connected to the server");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
