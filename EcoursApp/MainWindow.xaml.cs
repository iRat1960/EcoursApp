﻿using EcoursApp.Forms;
using EcoursCCont;
using EcoursCCont.Controls;
using EcoursCCont.Forms;
using EcoursCCont.Pages;
using EcoursCLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace EcoursApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataView Accounts;
        bool IsOpenLeftPanel = false, IsOpenTopPanel = false;
        int HomePageId = 1;
        DispatcherTimer timer;
        static readonly string runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();

        public MainWindow()
        {
            InitializeComponent();
            InputLanguageManager.Current.InputLanguageChanged += new InputLanguageEventHandler(Current_InputLanguageChanged);
        }

        #region --- Обработчики событий формы ---

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            G.nTypeConnect = Properties.Settings.Default.nTypeConnect;
            G.cAppName = Properties.Settings.Default.cAppName;
            G.cVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            G.cExtension = Properties.Settings.Default.cExtension;
            G.nStyleKod = Properties.Settings.Default.nStyleKod;
            G.cUser = Properties.Settings.Default.cUser;
            G.nUserId = Properties.Settings.Default.nUserId;
            G.nAccountId = Properties.Settings.Default.nAccountId;
            G.cRole = Properties.Settings.Default.cRole;
            G.nRoleId = Properties.Settings.Default.nRoleId;
            G.nTypeLogin = Properties.Settings.Default.nTypeLogin;
            G.nSetupLogin = Properties.Settings.Default.nSetupLogin;
            G.flConfidential = Properties.Settings.Default.flConfidential;
            G.flEnabledSinchr = Properties.Settings.Default.flEnabledSinchr;
            G.flStyleSinchr = Properties.Settings.Default.flStyleSinchr;
            G.flModulsSinchr = Properties.Settings.Default.flModelsSinchr;
            G.flConfidSinchr = Properties.Settings.Default.flConfidSinchr;
            G.flOtherSinchr = Properties.Settings.Default.flOtherSinchr;
            G.UUID = Properties.Settings.Default.UUID;
            G.flDateInBottom = Properties.Settings.Default.flDateInBottom;
            G.flEnabledStyle = Properties.Settings.Default.flEnabledStyle;
            G.flKeyInBottom = Properties.Settings.Default.flKeyInBottom;
            G.flStyleInTop = Properties.Settings.Default.flStyleInTop;
            G.cAppRootDir = Environment.CurrentDirectory;
            G.cDefaultAssemblyName = "EcoursCCont.FX";
            G.SqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string[] array = G.SqlConnectionString.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            G.cDataSource = array.FirstOrDefault(o => o.Contains("Data Source=")).Replace("Data Source=", "");
            G.cBaseSQL = array.FirstOrDefault(o => o.Contains("Initial Catalog=")).Replace("Initial Catalog=", "");
            //G.HumanFlags = Properties.Settings.Default.Flags;
            ThemeChange();
            //List<string> list = new List<string> { "UUID", "ADID", "PCID" };
            //string uuid = X.GetUUID();
            //if (G.UUID.Length == 0 || G.UUID.Length > 0 && !list.Contains(G.UUID.Substring(0, 4)) || G.UUID != uuid)
            //    G.UUID = uuid;
            G.nHnd = X.SQLC(G.SqlConnectionString);

            //X.SQLX(new Action<dynamic, Exception>(
            //    delegate (dynamic views, Exception exception)
            //    {
            //        if (exception != null)
            //        {
            //            throw new InvalidOperationException("Проверка", exception);
            //        }
            //        if (views != null && views is DataView)
            //        {
            //            DataView qTemp = (DataView)views;
            //            G.nAccountId = (int)qTemp[0].Row["Id"];
            //            comboBox.ItemsSource = qTemp.Table.AsEnumerable().Select(o => o.Field<string>("Name"));
            //        }
            //    }), "exec up_getaccounts 2", "qTemp");

            timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, (object s, EventArgs ev) =>
            {
                timeText.Text = DateTime.Now.ToString("HH:mm");
                dateText.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }, Dispatcher);
            if (G.flDateInBottom) timer.Start();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ShowPwdWidnow("Авторизация");
        }

        private void Current_InputLanguageChanged(object sender, InputLanguageEventArgs e)
        {
            buttonLang.Content = InputLanguageManager.Current.CurrentInputLanguage.Name == "ru-RU" ? "РУС" : "ENG";
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int height = (int)Height;
            int width = (int)Width;
            status.Text = "H:" + height.ToString() + " W:" + width.ToString();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.nTypeConnect = G.nTypeConnect;
            Properties.Settings.Default.cAppName = G.cAppName;
            Properties.Settings.Default.cExtension = G.cExtension;
            Properties.Settings.Default.nStyleKod = G.nStyleKod;
            Properties.Settings.Default.cUser = G.cUser;
            Properties.Settings.Default.nUserId = G.nUserId;
            Properties.Settings.Default.nAccountId = G.nAccountId;
            Properties.Settings.Default.cRole = G.cRole;
            Properties.Settings.Default.nRoleId = G.nRoleId;
            Properties.Settings.Default.nTypeLogin = G.nTypeLogin;
            Properties.Settings.Default.nSetupLogin = G.nSetupLogin;
            Properties.Settings.Default.flConfidential = G.flConfidential;
            Properties.Settings.Default.flEnabledSinchr = G.flEnabledSinchr;
            Properties.Settings.Default.flStyleSinchr = G.flStyleSinchr;
            Properties.Settings.Default.flModelsSinchr = G.flModulsSinchr;
            Properties.Settings.Default.flConfidSinchr = G.flConfidSinchr;
            Properties.Settings.Default.flOtherSinchr = G.flOtherSinchr;
            Properties.Settings.Default.flDateInBottom = G.flDateInBottom;
            Properties.Settings.Default.flEnabledStyle = G.flEnabledStyle;
            Properties.Settings.Default.flKeyInBottom = G.flKeyInBottom;
            Properties.Settings.Default.flStyleInTop = G.flStyleInTop;
            //Properties.Settings.Default.Flags = G.HumanFlags;
            Properties.Settings.Default.Save();

            if (G.nHnd != null && (G.nHnd is SqlConnection || (G.nHnd is int && G.nHnd > 0)))
            {
                X.SQLD();
            }
        }

        #endregion

        #region --- Обработчики событий UserControls ---

        /// <summary>
        /// Раскрытие/скрытие боковой панели
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button0_Click(object sender, RoutedEventArgs e)
        {
            IsOpenLeftPanel = !IsOpenLeftPanel;
            double w = IsOpenLeftPanel ? 200 : 40;
            button0.SetWidth(w);
            sp2.Width = w;
            for (int i = 0; i < sp2.Children.Count; i++)
            {
                if (sp2.Children[i].GetType().Equals(typeof(InfoButton)))
                {
                    InfoButton ib = (InfoButton)sp2.Children[i];
                    ib.SetWidth(w);
                }
            }
            sp3.Width = w;
            for (int i = 0; i < sp3.Children.Count; i++)
            {
                if (sp3.Children[i].GetType().Equals(typeof(InfoButton)))
                {
                    InfoButton ib = (InfoButton)sp3.Children[i];
                    ib.SetWidth(w);
                }
            }
        }
        /// <summary>
        /// Отработка события нажатия статических кнопки боковой панели
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoButtonStatic_Click(object sender, RoutedEventArgs e)
        {
            if (sender.GetType().Equals(typeof(InfoButton)))
            {
                InfoButton ib = (InfoButton)sender;
                string s = ib.ControlText;
                tb1.Text = s;
                switch (s)
                {
                    case "Юридическая помощь":
                        object[] oPrm = new object[] { "Справочник ОКВЭД", "EcoursCCont.CheckedTreeViewControl", "select * from OKVED", new List<string>()};
                        ReferenceBook form = new ReferenceBook(8, oPrm);
                        if (form.ShowDialog() == true)
                        {

                        }
                        break;
                    case "Техподдержка":
                        Uri iconUri = new Uri("pack://application:,,,/flay.ico", UriKind.RelativeOrAbsolute);
                        Window win = new Window()
                        {
                            Title = "Учетные записи",
                            Width = 1020,
                            Height = 720,
                            Icon = BitmapFrame.Create(iconUri),
                            Style = Application.Current.Resources["WindowStyle"] as Style,
                        };
                        win.Content = new HomePage();
                        win.Show();
                        break;
                    case "Авторизация":
                    case "Сменить пользователя":
                        ShowPwdWidnow(s);
                        break;
                    case "Параметры":
                        ShowParamForm();
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// Отработка события нажатия кнопки на закладке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender.GetType().Equals(typeof(TabItemButton)))
            {
                TabItemButton tib = (TabItemButton)sender;
                TabItemTag tag = (TabItemTag)tib.Tag;
                if (tag.nType > 0) ExecMethod(tag);
            }
        }
        /// <summary>
        /// Отработка события нажатия кнопки боковой панели
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender.GetType().Equals(typeof(InfoButton)))
            {
                InfoButton ib = (InfoButton)sender;
                InfoButtonTag tag = (InfoButtonTag)ib.Tag;
                string s = ib.ControlText;
                tb1.Text = s;
                SetTabControl(tag.Id);
                if (tag.cNavigatePage.Length == 0)
                    tag.cNavigatePage = "/EcoursCCont;component/Pages/HomePage.xaml";
                grid2.Children.Remove(frame);
                frame = new Frame
                {
                    NavigationUIVisibility = NavigationUIVisibility.Hidden
                };
                grid2.Children.Add(frame);
                Grid.SetColumn(frame, 1);
                Uri uri = new Uri(tag.cNavigatePage, UriKind.RelativeOrAbsolute);
                frame.Navigate(uri);
                if (tag.nType > 0) ExecMethod(tag);
            }
        }
        /// <summary>
        ///  Отработка события нажатия кнопки отображения текста кнопок закладки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tib_Click(object sender, RoutedEventArgs e)
        {
            if (sender.GetType().Equals(typeof(UpDown)))
            {
                IsOpenTopPanel = !IsOpenTopPanel;
                for (int j = 0; j < tabControl.Items.Count; j++)
                {
                    TabItem ti = (TabItem)tabControl.Items[j];
                    StackPanel panel = (StackPanel)ti.Content;
                    for (int i = 0; i < panel.Children.Count; i++)
                    {
                        if (panel.Children[i].GetType().Equals(typeof(TabItemButton)))
                        {
                            TabItemButton tib = (TabItemButton)panel.Children[i];
                            tib.SetSize(IsOpenTopPanel);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Отработка события ComboBox выбора цветовой схемы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combobox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (G.nStyleKod != combobox1.SelectedIndex)
            {
                G.nStyleKod = combobox1.SelectedIndex;
                ThemeChange();
            }
        }
        /// <summary>
        /// Обработка события смены учетной записи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            DataRowView item = (DataRowView)comboBox.SelectedItem;
            if (item != null)
            {
                int id = (int)item.Row["Id"];
                if (G.nAccountId != id)
                {
                    G.nAccountId = id;
                    GenTopMenu();
                }
            }
        }
        /// <summary>
        /// Возврат на предыдущую страницу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPrev_Click(object sender, RoutedEventArgs e)
        {
            if (frame.CanGoBack)
            {
                frame.GoBack();
            }
        }
        /// <summary>
        /// Переход к следующей странице
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            if (frame.CanGoForward)
            {
                frame.GoForward();
            }
        }

        #endregion

        #region --- Методы ---

        /// <summary>
        /// Выполнение метода заданного в строке меню
        /// </summary>
        /// <param name="tag"></param>
        private void ExecMethod(TabItemTag tag)
        {
            if (tag.cMethod.Length > 0)
            {
                object[] obj = null;
                if (tag.IsParams) obj = FX.GetMenuParams(tag.Id);
                switch (tag.nType)
                {
                    case 1: // запуск метода данной формы
                        {
                            Type type = typeof(MainWindow);
                            type.InvokeMember(tag.cMethod, BindingFlags.InvokeMethod, null, this, obj);
                            break;
                        }
                    case 2: // запуск метода из библиотеки EcoursCCont.FX
                        {
                            Type type = typeof(FX);
                            object ci = Activator.CreateInstance(type);
                            if (obj != null)
                            {
                                type.InvokeMember(tag.cMethod, BindingFlags.InvokeMethod, null, ci, obj);
                            }
                            break;
                        }
                    case 3: // запуск метода из произвольной библиотеки
                        {
                            if (tag.cAssemblyName.Length > 0)
                            {
                                Type type = X.GetTypeFromAssembly(tag.cAssemblyName);
                                if (type != null)
                                {
                                    object ci = Activator.CreateInstance(type);
                                    if (ci != null)
                                        type.InvokeMember(tag.cMethod, BindingFlags.InvokeMethod, null, ci, obj);
                                }
                            }
                            break;
                        }
                    case 4: // запуск формы из библиотеки, прописанной в строке меню
                        {
                            string[] str = tag.cAssemblyName.Split(new string[1] { "." }, StringSplitOptions.RemoveEmptyEntries);
                            Assembly asm = Assembly.Load(str[0]);
                            var cs = asm.GetTypes().FirstOrDefault(o => o.Name == str[1]);
                            if (cs != null)
                            {
                                Window win = (Window)Activator.CreateInstance(cs, obj);
                                win.Show();
                            }
                            break;
                        }
                    case 5: // ??? генератор диалогового окна
                        {

                            break;
                        }
                }
            }
        }
        /// <summary>
        /// Активация окна авторизации
        /// </summary>
        /// <param name="title"></param>
        private void ShowPwdWidnow(string title)
        {
            Window pwd;
            if (G.nTypeLogin == 0)
                pwd = new PwdWindow(title);
            else
                pwd = new TopWindow();
            if (pwd.ShowDialog() == true)
            {
                string cmnd = "exec up_getaccounts " + G.nUserId;
                Accounts = X.SQLE(G.nHnd, cmnd, "qTemp");
                if (Accounts != null && Accounts.Count > 0)
                {
                    G.nAccountId = (int)Accounts[0].Row["Id"];
                    comboBox.ItemsSource = Accounts;
                    comboBox.SelectedIndex = 0;
                }
                GenTopMenu();
            }
            else
            {
                Close();
            }
        }
        /// <summary>
        /// Формирование основного меню (боковой панели)
        /// </summary>
        private void GenTopMenu()
        {
            if (IsConnected)
            {
                button3.ToolTip = "Сменить пользователя";
                button3.ControlText = "Сменить пользователя";
                ClearMenu();
                string cmnd = "exec up_genmenu " + G.nRoleId + ",0";
                DataView dv = X.SQLE(G.nHnd, cmnd, "qmenu");
                if (dv != null && dv.Count > 0)
                {
                    foreach (DataRow row in dv.Table.Rows)
                    {
                        string tool = (bool)row["IsToolTip"] ? "true" : "false";
                        InfoButton ib = new InfoButton(new string[] { row["Template"].ToString(), row["MenuText"].ToString(), tool });
                        InfoButtonTag tag = new InfoButtonTag
                        {
                            Id = Convert.ToInt32(row["Sid"]),
                            nType = Convert.ToInt32(row["EventType"]),
                            cNavigatePage = row["NavigatePage"] == DBNull.Value ? "" : row["NavigatePage"].ToString(),
                            cAssemblyName = row["MenuAssembly"] == DBNull.Value ? "" : row["MenuAssembly"].ToString(),
                            cMethod = row["MenuEvent"] == DBNull.Value ? "" : row["MenuEvent"].ToString(),
                            IsParams = Convert.ToBoolean(row["IsParams"])
                        };
                        ib.Tag = tag;
                        ib.SetWidth(IsOpenLeftPanel ? 180 : 40);
                        ib.AddHandler(System.Windows.Controls.Primitives.ButtonBase.ClickEvent, new RoutedEventHandler(InfoButton_Click));
                        sp2.Children.Add(ib);
                    }
                    SetTabControl(HomePageId);
                    Uri uri = new Uri("/EcoursCCont;component/Pages/HomePage.xaml", UriKind.RelativeOrAbsolute);
                    frame.Navigate(uri);
                }
                dv?.Dispose();
            }
            WindowState = WindowState.Maximized;
        }
        /// <summary>
        /// Формирование закладок основного экрана
        /// </summary>
        /// <param name="key"></param>
        private void SetTabControl(int key)
        {
            TabItem tabItem;
            StackPanel sp;
            if (tabControl.Items.Count > 0) tabControl.Items.Clear();
            int idx = 0;
            // Выбор закладок
            string cmnd = "exec up_genmenu " + G.nRoleId + "," + key.ToString();
            DataView dv = X.SQLE(G.nHnd, cmnd, "qmenu");
            if (dv != null && dv.Count > 0)
            {
                foreach (DataRow row in dv.Table.Rows)
                {
                    idx++;
                    sp = new StackPanel()
                    {
                        Name = "spItem" + idx.ToString().Trim(),
                        Orientation = Orientation.Horizontal
                    };
                    UpDown ud = new UpDown();
                    ud.AddHandler(System.Windows.Controls.Primitives.ButtonBase.ClickEvent, new RoutedEventHandler(Tib_Click));
                    sp.Children.Add(ud);
                    // Выбор кнопок на закладке
                    cmnd = "exec up_genmenu " + G.nRoleId + "," + row["Sid"].ToString();
                    DataView dvb = X.SQLE(G.nHnd, cmnd, "qmenu");
                    if (dvb != null && dvb.Count > 0)
                    {
                        foreach (DataRow dr in dvb.Table.Rows)
                        {
                            string tool = (bool)dr["IsToolTip"] ? "true" : "false";
                            TabItemButton tib = new TabItemButton(new string[] { dr["Template"].ToString(), dr["MenuText"].ToString(), tool });
                            TabItemTag tag = new TabItemTag
                            {
                                Id = Convert.ToInt32(dr["Sid"]),
                                nType = Convert.ToInt32(dr["EventType"]),
                                cMethod = dr["MenuEvent"] == DBNull.Value ? "" : dr["MenuEvent"].ToString(),
                                cAssemblyName = dr["MenuAssembly"] == DBNull.Value ? G.cDefaultAssemblyName : dr["MenuAssembly"].ToString(),
                                IsParams = Convert.ToBoolean(dr["IsParams"])
                            };
                            tib.Tag = tag;
                            tib.AddHandler(System.Windows.Controls.Primitives.ButtonBase.ClickEvent, new RoutedEventHandler(TabItemButton_Click));
                            tib.SetSize(IsOpenTopPanel);
                            sp.Children.Add(tib);
                        }
                    }
                    tabItem = new TabItem()
                    {
                        Style = Application.Current.Resources["TabItemStyle1"] as Style,
                        Header = " " + row["MenuText"].ToString() + " ",
                        Content = sp
                    };
                    tabControl.Items.Add(tabItem);
                }
            }
        }
        /// <summary>
        /// Очистка основного меню и закладок
        /// </summary>
        private void ClearMenu()
        {
            button3.ToolTip = " Авторизация ";
            button3.ControlText = "Авторизация";
            if (sp2.Children.Count > 0)
                sp2.Children.Clear();
            if (tabControl.Items.Count > 0)
                tabControl.Items.Clear();
        }
        /// <summary>
        /// Открытие формы настройки параметров
        /// </summary>
        private void ShowParamForm()
        {
            Uri iconUri = new Uri("pack://application:,,,/flay.ico", UriKind.RelativeOrAbsolute);
            NavigationWindow win = new NavigationWindow()
            {
                Title = "Параметры",
                MinWidth = 780,
                MinHeight = 540,
                Width = 870,
                Height = 640,
                ShowsNavigationUI = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Name = "ParamWin",
                Owner = this,
                Icon = BitmapFrame.Create(iconUri)
            };
            win.Content = new SetupPageMain();
            win.Show();
        }
        /// <summary>
        /// Установка цветовой схемы
        /// </summary>
        private void ThemeChange()
        {
            string[] style = new string[] { "Dark", "LightBlue", "LightGreen", "LightOrange", "LightYellow" };
            var uri = new Uri("/EcoursApp;component/Themes/" + style[G.nStyleKod] + ".xaml", UriKind.Relative);
            ResourceDictionary dict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
            buttonNext.Content = FindResource(G.nStyleKod == 0 ? "Nextw" : "Next");
            buttonPrev.Content = FindResource(G.nStyleKod == 0 ? "Prevw" : "Prev");
            sbItem2.Visibility = G.flStyleInTop ? Visibility.Visible : Visibility.Collapsed;
            combobox1.SelectedIndex = G.nStyleKod;
            sbItem3.Visibility = G.flKeyInBottom ? Visibility.Visible : Visibility.Collapsed;
            sbItem4.Visibility = G.flDateInBottom ? Visibility.Visible : Visibility.Collapsed;
            if (timer != null)
            {
                if (G.flDateInBottom)
                    timer.Start();
                else
                    timer.Stop();
            }
        }

        #endregion

        private bool IsConnected => G.nHnd != null && ((G.nHnd is SqlConnection) || (G.nHnd is int && G.nHnd > 0));
    }
}