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
        
        // Game vars
        public string X { get; set; } // X
        public string Y { get; set; } // Y
        public bool isHit; // is hit?

        //System vars
        TcpClient client;
        public bool Step = false; 
        public bool isGameStarted = false;
        public int[,] array2Da;
        public MainWindow()
        {
            InitializeComponent();
            SEND_BTN.IsEnabled = false;



            //Fill tables
            CreateTable(UserGrid);
            CreateTable(OpponentGrid);



            array2Da = new int[10, 10];
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
                        //strs[0]  -  Y;
                        //strs[1]  -  X;
                        //strs[2]  -  Step;
                        //strs[3]  -  isHit;
                        if (strs[2] == "true")
                            Step = true;
                        else
                            Step = false;
                        
                        Y = strs[0];
                        X = strs[1];
                        if (strs[3] == "true")
                            isHit = true;
                        else
                           if (strs[3] == "false")
                            isHit = false;
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
    
        
        void CreateTable(Grid grid)
        {
            //Definitions
            for (int i = 0; i < 11; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

            }

            //Fill Header Row
            for (int i = 0; i < 10; i++)
            {
                TextBlock tb = new TextBlock();
                tb.Text = (i + 1).ToString();
                Grid.SetRow(tb, 10);
                Grid.SetColumn(tb, i);
                grid.Children.Add(tb);

            }

            //Fill Header Column
            char s = 'A'; 
            for (int i = 0; i < 10; i++)
            {
                TextBlock tb = new TextBlock();
                tb.Text = s.ToString();
               
                s++;
                Grid.SetRow(tb, i);
                Grid.SetColumn(tb, 10);
                grid.Children.Add(tb);

            }


            //fill buttons
            for (int i = 0; i < 10; i++)
            {

                for (int j = 0; j < 10; j++)
                {
                    Button bt = new Button();


                    bt.Name = string.Format($"C{i}{j}");
                    bt.Content = bt.Name.ToString();
                    Grid.SetRow(bt, i);
                    Grid.SetColumn(bt, j);
                    grid.Children.Add(bt);
                }
            }
        


        }
        private byte [] getByteFromMatrix()
        {
           
            string str_matrix= "#matrix ";
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    str_matrix += array2Da[i, j].ToString() + " ";
            MessageBox.Show(str_matrix);
            return Encoding.Unicode.GetBytes(str_matrix);
            
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(tb1.Text)==false&& String.IsNullOrEmpty(tb.Text)==false)
            {
                
                NetworkStream networkStream = client.GetStream();
                byte[] arr = Encoding.Unicode.GetBytes(tb1.Text + " " + tb.Text);//getByteFromMatrix(); //Encoding.Unicode.GetBytes(tb1.Text+" "+tb.Text);
                networkStream.Write(arr,0,arr.Length);
                //MessageBox.Show(tb1.Text + " " + tb.Text);

            }
        }
    }
}
