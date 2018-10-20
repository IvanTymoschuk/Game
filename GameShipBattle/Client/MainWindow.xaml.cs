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
using System.Diagnostics;

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
            UserGrid.IsEnabled = true;

     
           
            //Fill tables
            CreateTable(UserGrid);
            CreateTable(OpponentGrid);

            UserGridBtn();

            lbShips.Items.Add(new Ship(){Name ="Large",Length=4});
            
            

            array2Da = new int[10, 10];
            try
            {
                client = new TcpClient();
                client.Connect("127.0.0.1", 1488);
                tb3.Text = "Waiting for users";
            }
            catch(Exception exp)
            {
                MessageBox.Show("Server is disabled!!!");
                this.Close();
            }

            Task.Run(() =>
            {
                NetworkStream networkStream = client.GetStream();
                while (true)
                {
                    byte[] arr = new byte[256];
                    int bytes = networkStream.Read(arr, 0, 256);
                    string msg1 = Encoding.Unicode.GetString(arr, 0, bytes);
                    //MessageBox.Show(msg1);
                    if (msg1 == "WIN")
                    {
                        MessageBox.Show("You win");
                        this.Dispatcher.Invoke(() =>
                        {
                            Process.Start("Client.exe");
                            this.Close();
                        });
                    }
                    if (msg1 == "LOSE")
                    {
                        MessageBox.Show("You LOSE");
                        this.Dispatcher.Invoke(() =>
                        {
                            Process.Start("Client.exe");
                            this.Close();
                        });
                    }
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
                    if (msg1 == "false false" || msg1 == "false true" || msg1=="false")
                    {
                        if (btn != null)
                        {
                            if (msg1 == "false true")
                            {
                                Hit(true);
                            }
                            else
                            if (msg1 == "false false")
                            {
                                Hit(false);
                            }
                        }
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
                        Hit(strs[3]);
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
    
        void Hit(bool is_hit)
        {
            if (btn == null)
                return;
            if(is_hit==true)
            {
                Dispatcher.Invoke(() =>
                {
                    btn.Content = "X";
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    btn.Content = "0";
                });
            }
        }
        void Hit(string is_hit)
        {
            IEnumerable<Button> collection = null;
            Dispatcher.Invoke(() =>
            {
                 collection = UserGrid.Children.OfType<Button>();
           
            foreach (var el in collection)
                if (el.Name == Y + X)
                {
                    //MessageBox.Show("EL");
                    Dispatcher.Invoke(() =>
                    {
                        el.IsEnabled = false;
                        el.FontSize = 30;
                        el.Content = ".";
                    });
                }
            });
        }
        //IEnumerable<Button> collection = null;
        void UserGridBtn()
        {
            IEnumerable<Button> collection = null;
            Dispatcher.Invoke(() =>
            {
                collection = UserGrid.Children.OfType<Button>();

                foreach (var el in collection)
                    el.Click += UserBtn;
            });
        }
        
        Button Yourbtn = null;
        private void UserBtn(object sender, RoutedEventArgs e)
        {
            Yourbtn= sender as Button;
            MessageBox.Show("You click on button, name: " + Yourbtn.Name);

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
            char btn_fname = 'A';
            for (int i = 0; i < 10; i++)
            {

                for (int j = 0; j < 10; j++)
                {
                    Button bt = new Button();


                    bt.Name = string.Format($"{btn_fname}{j}");
                    bt.Content = bt.Name.ToString();
                    Grid.SetRow(bt, i);
                    Grid.SetColumn(bt, j);
                    grid.Children.Add(bt);
                    bt.Click += Attack_click;
                   // bt.IsEnabled = false;
                }
                btn_fname++;
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

            //
            //
            // ALL BUTTONS FROM GRID
            //  var el = UserGrid.Children.OfType<Button>();
            //
            //

            try
            {

                if (btn != null)
                {
                    if(btn.IsEnabled==false)
                    {
                        MessageBox.Show("You have already gone here ");
                        return;
                    }
                    #region
                    //string btn_name = btn.Name.Remove(0, 1);
                    //int id = Convert.ToInt32(btn_name);
                    //#region
                    //if (id < 10)
                    //{
                    //    Y = "A";
                    //    X = id.ToString();
                    //}
                    //if (id > 9 && id < 21)
                    //{
                    //    Y = "B";
                    //    X = (id - 10).ToString();
                    //}
                    //if (id > 19 && id < 31)
                    //{
                    //    Y = "C";
                    //    X = (id - 20).ToString();
                    //}
                    //if (id > 29 && id < 41)
                    //{
                    //    Y = "D";
                    //    X = (id - 30).ToString();
                    //}
                    //if (id > 39 && id < 51)
                    //{
                    //    Y = "E";
                    //    X = (id - 40).ToString();
                    //}
                    //if (id > 49 && id < 61)
                    //{
                    //    Y = "F";
                    //    X = (id - 50).ToString();
                    //}
                    //if (id > 59 && id < 71)
                    //{
                    //    Y = "G";
                    //    X = (id - 60).ToString();
                    //}
                    //if (id > 69 && id < 81)
                    //{
                    //    Y = "H";
                    //    X = (id - 70).ToString();
                    //}
                    //if (id > 79 && id < 91)
                    //{
                    //    Y = "I";
                    //    X = (id - 80).ToString();
                    //}
                    //if (id > 89 && id < 101)
                    //{
                    //    Y = "J";
                    //    X = (id - 90).ToString();
                    //}
                    #endregion
                    // MessageBox.Show(Y + " " + X);
                    NetworkStream networkStream = client.GetStream();
                    byte[] arr = Encoding.Unicode.GetBytes(btn.Name[0] + " " + btn.Name[1]);//getByteFromMatrix(); //Encoding.Unicode.GetBytes(tb1.Text+" "+tb.Text);
                    networkStream.Write(arr, 0, arr.Length);
                    btn.IsEnabled = false;
                }
                //NetworkStream networkStream = client.GetStream();
                //byte[] arr = Encoding.Unicode.GetBytes(tb1.Text + " " + tb.Text);//getByteFromMatrix(); //Encoding.Unicode.GetBytes(tb1.Text+" "+tb.Text);
                //networkStream.Write(arr, 0, arr.Length);
                //MessageBox.Show(tb1.Text + " " + tb.Text);
            }
            catch(Exception)
            {
                MessageBox.Show("You lost connect to server!!");
            }
        }
        Button btn = null;
        private void Attack_click(object sender, RoutedEventArgs e)
        {
            if (btn != null)
            {
                Thickness th1 = new Thickness();
                th1.Right = 0;
                th1.Left = 0;
                th1.Top = 0;
                th1.Bottom = 0;
                btn.BorderThickness = th1;
            }
            Thickness th = new Thickness();
            th.Right = 6;
            th.Left = 6;
            th.Top = 6;
            th.Bottom = 6;
            (sender as Button).BorderThickness = th;
            btn= (sender as Button);

        }


        private void BtnPush_OnClick(object sender, RoutedEventArgs e)
        {
            if (lbShips.Items == null)
            {
                
            }
        }


      

        private void lbShips_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
