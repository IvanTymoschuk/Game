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
        public MainWindow()
        {
            InitializeComponent();
            
            CreateTable();
            client = new TcpClient();
            client.Connect("127.0.0.1", 1488);


            Task.Run(() =>
            {
                NetworkStream networkStream = client.GetStream();
                while (true)
                {
                    byte[] arr = new byte[256];
                    int bytes = networkStream.Read(arr, 0, 256);
                    string msg1 = Encoding.Unicode.GetString(arr, 0, bytes);
                    Dispatcher.Invoke(() =>
                    {
                        tb1.Text = msg1;
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

        }
    }
}
