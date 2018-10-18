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
using System.Net.Sockets;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClient client;
        public string X { get; set; }
        public string Y { get; set; }
        public bool Step = false;
        public bool isGameStarted = false;
        public MainWindow()
        {
            InitializeComponent();
            SEND_BTN.IsEnabled = false;
            CreateTable();
            client = new TcpClient();
            client.Connect("127.0.0.1", 1488);
            tb3.Text = "Waiting for users";
            Task.Run(() =>
            {
                NetworkStream networkStream = client.GetStream();
                while (true)
                {
                    byte[] arr = new byte[256];
                    int bytes = networkStream.Read(arr, 0, 256);
                    string msg1 = Encoding.Unicode.GetString(arr, 0, bytes);
                    //MessageBox.Show(msg1);
                    if (msg1 == "true")
                    {
                        Step = true;
                        if (isGameStarted == false)
                        {
                            isGameStarted = true;
                            MessageBox.Show("GAME STARTED!!!!");
                        }
                    }
                    else
                    if (msg1 == "false")
                    {
                        Step = false;
                        if (isGameStarted == false)
                        {
                            isGameStarted = true;
                            MessageBox.Show("GAME STARTED!!!!");
                        }
                    }
                    else
                    {
                        string[] strs = msg1.Split(' ');
                        if (strs[2] == "true")
                            Step = true;
                        else
                            Step = false;
                        
                        Y = strs[0];
                        X = strs[1];
                    }


                    if (Step == true)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            SEND_BTN.IsEnabled = true;
                        });
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            SEND_BTN.IsEnabled = false;
                        });
                    }
                    Dispatcher.Invoke(() =>
                    {
                        tb3.Text = Y+X;
                    });
                }
            });
        }
    
        
        void CreateTable()
        {
            for (int i = 0; i < 11; i++)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition());
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            }
            for (int i = 0; i < 10; i++)
            {
                TextBlock tb = new TextBlock();
                tb.Text = (i + 1).ToString();
                Grid.SetRow(tb, 10);
                Grid.SetColumn(tb, i);
                MainGrid.Children.Add(tb);

            }
            char s = 'A'; ;
            for (int i = 0; i < 10; i++)
            {
                TextBlock tb = new TextBlock();
                tb.Text = s.ToString();
               
                s++;
                Grid.SetRow(tb, i);
                Grid.SetColumn(tb, 10);
                MainGrid.Children.Add(tb);

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(tb1.Text)==false&& String.IsNullOrEmpty(tb.Text)==false)
            {
                
                NetworkStream networkStream = client.GetStream();
                byte[] arr = Encoding.Unicode.GetBytes(tb1.Text+" "+tb.Text);
                networkStream.Write(arr,0,arr.Length);
             //  MessageBox.Show(tb1.Text + " " + tb.Text);
            }
        }
    }
}
