﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
        private TcpClient client;

        public bool Step = false;
        public bool isGameStarted = false;

        public bool isSendMatrix = false;

        private int count_xp = 0;

        private ObservableCollection<Ship> ships;

        private IEnumerable<Button> collectionYourButtons = null;

        private Button btn = null;


        private Button Yourbtn = null;
        private List<int> FallCels;

        public MainWindow()
        {
            InitializeComponent();
            SEND_BTN.IsEnabled = false;
            UserGrid.IsEnabled = true;
            btnPush.IsEnabled = false;

            //Fill tables
            CreateTable(UserGrid);
            CreateTable(OpponentGrid);
            UserGridBtn();

            ships = new ObservableCollection<Ship>();

            #region FillShipList

            ships.Add(new Ship() { Name = "ExtraLarge", Length = 4 });

            ships.Add(new Ship() { Name = "Large", Length = 3 });
            ships.Add(new Ship() { Name = "Large", Length = 3 });

            ships.Add(new Ship() { Name = "Middle", Length = 2 });
            ships.Add(new Ship() { Name = "Middle", Length = 2 });
            ships.Add(new Ship() { Name = "Middle", Length = 2 });

            ships.Add(new Ship() { Name = "Small", Length = 1 });
            ships.Add(new Ship() { Name = "Small", Length = 1 });
            ships.Add(new Ship() { Name = "Small", Length = 1 });
            ships.Add(new Ship() { Name = "Small", Length = 1 });

            #endregion FillShipList

            FallCels = new List<int>();
            lbShips.ItemsSource = ships;

            //array2Da = new int[10, 10];
            try
            {
                client = new TcpClient();
                client.Connect("127.0.0.1", 1488);
                tb3.Text = "Waiting for users";
            }
            catch (Exception exp)
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
                        if (isSendMatrix == false)
                        {
                            Dispatcher.Invoke(() => { btnPush.IsEnabled = true; });
                            isSendMatrix = true;
                        }
                        else
                        {
                            Step = true;
                        }

                        if (isGameStarted == false)
                        {
                            isGameStarted = true;
                            MessageBox.Show("GAME STARTED!!!!");
                        }
                    }
                    else if (msg1 == "false")
                    {
                        Dispatcher.Invoke(() => { btnPush.IsEnabled = false; });
                        Step = false;
                        if (isGameStarted == false)
                        {
                            isGameStarted = true;
                            MessageBox.Show("GAME STARTED!!!!");
                        }
                    }
                    else if (msg1 == "true true")
                    {
                        if (btn != null)
                        {
                            if (msg1 == "true true")
                            {
                                Hit(true);
                            }
                        }
                    }
                    else if (msg1 == "false false" || msg1 == "false true")
                    {
                        if (btn != null)
                        {
                            if (msg1 == "false true")
                            {
                                Hit(true);
                            }
                            else if (msg1 == "false false")
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
                        Dispatcher.Invoke(() => { SEND_BTN.IsEnabled = true; });
                    }
                    else
                    {
                        Dispatcher.Invoke(() => { SEND_BTN.IsEnabled = false; });
                    }

                    Dispatcher.Invoke(() => { tb3.Text = Y + X; });
                }
            });
        }

        private string getMatrix()
        {
            string strMatrix = "#matrix ";
            IEnumerable<Button> collection = null;
            Dispatcher.Invoke(() => { collection = UserGrid.Children.OfType<Button>(); });
            foreach (var el in collection)
            {
                if (el.Content == "☻")
                {
                    strMatrix += "1 ";
                    count_xp++;
                }
                else
                    strMatrix += "0 ";
            }

            strMatrix += count_xp;
            return strMatrix;
        }

        private void Hit(bool is_hit)
        {
            if (btn == null)
                return;
            if (is_hit == true)
            {
                Dispatcher.Invoke(() => { btn.Content = "X"; });
            }
            else
            {
                Dispatcher.Invoke(() => { btn.Content = "0"; });
            }
        }

        private void Hit(string is_hit)
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
        private void UserGridBtn()
        {
            Dispatcher.Invoke(() =>
            {
                collectionYourButtons = UserGrid.Children.OfType<Button>();

                foreach (var el in collectionYourButtons)
                    el.Click += UserBtn;
            });
        }


        private void UserBtn(object sender, RoutedEventArgs e)
        {
            Yourbtn = sender as Button;

            if (lbShips.SelectedItem == null)
                return;
            Ship currentShip = lbShips.SelectedItem as Ship;

            foreach (var el in FallCels)
            {
                for (int i = 0; i < currentShip.Length; i++)
                {
                    if (el == collectionYourButtons.ToList().IndexOf(Yourbtn) + i)
                    {
                        MessageBox.Show("You can`t pick ship here.Another ship fill this space");
                        return;
                    }
                }
            }

            int currentPos = collectionYourButtons.ToList().IndexOf(Yourbtn);

            if (int.Parse(Yourbtn.Name[1].ToString()) + currentShip.Length < 10)
            {
                //Yourbtn.Content = "☻";

                for (int j = 0; j < currentShip.Length; j++)
                {
                    if (j == 0)
                    {
                        FallCels.Add(collectionYourButtons.ToList().IndexOf(Yourbtn) - 1);
                        FallCels.Add(collectionYourButtons.ToList().IndexOf(Yourbtn) - 11);
                        FallCels.Add(collectionYourButtons.ToList().IndexOf(Yourbtn) + 11);
                    }

                    collectionYourButtons.ToList()[collectionYourButtons.ToList().IndexOf(Yourbtn) + j].Content = "☻";

                    FallCels.Add(collectionYourButtons.ToList().IndexOf(Yourbtn) + j);
                    FallCels.Add(collectionYourButtons.ToList().IndexOf(Yourbtn) + j + 10);
                    FallCels.Add(collectionYourButtons.ToList().IndexOf(Yourbtn) + j - 10);

                    if (j == currentShip.Length - 1)
                    {
                        FallCels.Add(collectionYourButtons.ToList().IndexOf(Yourbtn) + j - 1);
                        FallCels.Add(collectionYourButtons.ToList().IndexOf(Yourbtn) + j - 11);
                        FallCels.Add(collectionYourButtons.ToList().IndexOf(Yourbtn) + j + 11);
                    }
                }

                ships.Remove(lbShips.SelectedItem as Ship);
            }
            else

            {
                lblInfo.Content = "Ship is too big.Here not enough space";
            }
        }

        private void CreateTable(Grid grid)
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

        private byte[] getByteFromMatrix(string str_matrix)
        {
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
                    if (btn.IsEnabled == false)
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
            catch (Exception)
            {
                MessageBox.Show("You lost connect to server!!");
            }
        }

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
            btn = (sender as Button);
        }

        private void BtnPush_OnClick(object sender, RoutedEventArgs e)
        {
            if (lbShips.Items.Count == 0)
            {
                NetworkStream networkStream = client.GetStream();
                byte[] arr = Encoding.Unicode.GetBytes(getMatrix());//getByteFromMatrix(); //Encoding.Unicode.GetBytes(tb1.Text+" "+tb.Text);
                networkStream.Write(arr, 0, arr.Length);
                btnPush.IsEnabled = false;
                UserGrid.IsEnabled = false;
            }
        }

        private void lbShips_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}