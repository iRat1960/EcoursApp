using EcoursApp.Forms;
using EcoursCCont;
using EcoursCCont.Controls;
using EcoursCCont.Pages;
using EcoursXLib;
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
using EcoursCLib.Controls;
using System.IO;
using System.Windows.Media;
using EcoursCLib.Forms;
using EcoursCCont.Forms;
using EcoursCLib.Models;

namespace EcoursApp
{
    public partial class MainWindow : Window
    {
        bool IsOpenLeftPanel = false, IsOpenTopPanel = false;
        int HomePageId = 1;
        DispatcherTimer timer;
        ChatAndTask gridChat;
        static readonly string runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();

        public MainWindow()
        {
            InitializeComponent();
            InputLanguageManager.Current.InputLanguageChanged += new InputLanguageEventHandler(Current_InputLanguageChanged);
            X.DataReceived += ReceivedMessage;
        }

        #region --- Обработчики событий формы ---

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            G.nTypeLogin = Properties.Settings.Default.nTypeLogin;
            G.nSetupLogin = Properties.Settings.Default.nSetupLogin;
            G.flConfidential = Properties.Settings.Default.flConfidential;
            G.flEnabledSinchr = Properties.Settings.Default.flEnabledSinchr;
            G.flStyleSinchr = Properties.Settings.Default.flStyleSinchr;
            G.flModulsSinchr = Properties.Settings.Default.flModelsSinchr;
            G.flOtherSinchr = Properties.Settings.Default.flOtherSinchr;
            System.Drawing.Color color = Properties.Settings.Default.ChatWallPaper;
            G.ChatWallPaper = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            G.ChatPathImg = Properties.Settings.Default.ChatPathImg;
            G.ChatPathDoc = Properties.Settings.Default.ChatPathDoc;
            G.HumanFlags = Properties.Settings.Default.Flags;
            G.flEnabledStyle = Properties.Settings.Default.flEnabledStyle;
            G.nStyleKod = Properties.Settings.Default.nStyleKod;
            G.flStyleInTop = Properties.Settings.Default.flStyleInTop;
            G.flDateInBottom = Properties.Settings.Default.flDateInBottom;
            G.flKeyInBottom = Properties.Settings.Default.flKeyInBottom;
            G.UUID = Properties.Settings.Default.UUID;
            G.cAppName = Properties.Settings.Default.cAppName;
            G.cVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            G.cExtension = Properties.Settings.Default.cExtension;
            G.cDataRootDir = Properties.Settings.Default.DataRootDir;
            G.cDefaultAssemblyName = "EcoursСCont.FX";
            G.WinOwner = this;
            G.cAppRootDir = Environment.CurrentDirectory;
            G.cTemp = G.cAppRootDir + @"\Temp";
            if (!Directory.Exists(G.cTemp))
            {
                Directory.CreateDirectory(G.cTemp);
            }

            // только для Windows
            G.cDownloads = X.KnownFolders.GetPath(X.KnownFolder.Downloads);
            G.cDocuments = X.KnownFolders.GetPath(X.KnownFolder.Documents);
            G.cPictures = X.KnownFolders.GetPath(X.KnownFolder.Pictures);

            if (G.ChatPathDoc == string.Empty) G.ChatPathDoc = G.cDocuments;
            if (G.ChatPathImg == string.Empty) G.ChatPathImg = G.cPictures;
            
            List<string> list = new List<string> { "UUID", "ADID", "PCID" };
            string uuid = X.GetUUID();
            if (G.UUID.Length == 0 || G.UUID.Length > 0 && !list.Contains(G.UUID.Substring(0, 4)) || G.UUID != uuid)
                G.UUID = uuid;
            // *** тестовые данные ***
            G.Email = "xairat1960@gmail.com";
            G.EmailPwd = "abdoszsattvzhsba";
            G.flChatAndTasks = false;
            // *******
            G.cUser = Properties.Settings.Default.cUser;
            G.nUserId = Properties.Settings.Default.nUserId;
            G.nAccountId = Properties.Settings.Default.nAccountId;
            G.cRole = Properties.Settings.Default.cRole;
            G.nRoleId = Properties.Settings.Default.nRoleId;
            G.nTypeConnect = Properties.Settings.Default.nTypeConnect;
            G.SqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string[] array = G.SqlConnectionString.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            G.cDataSource = array.FirstOrDefault(o => o.Contains("Data Source=")).Replace("Data Source=", "");
            G.cBaseSQL = array.FirstOrDefault(o => o.Contains("Initial Catalog=")).Replace("Initial Catalog=", "");
            G.nHnd = X.SQLC(G.SqlConnectionString);
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
            gridChat?.Close();
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
            Properties.Settings.Default.flOtherSinchr = G.flOtherSinchr;
            Properties.Settings.Default.flDateInBottom = G.flDateInBottom;
            Properties.Settings.Default.flEnabledStyle = G.flEnabledStyle;
            Properties.Settings.Default.flKeyInBottom = G.flKeyInBottom;
            Properties.Settings.Default.flStyleInTop = G.flStyleInTop;
            Properties.Settings.Default.Flags = G.HumanFlags;
            Properties.Settings.Default.UUID = G.UUID;
            Properties.Settings.Default.DataRootDir = G.cDataRootDir;
            Properties.Settings.Default.flChatAndTasks = G.flChatAndTasks;
            Properties.Settings.Default.ChatPathImg = G.ChatPathImg;
            Properties.Settings.Default.ChatPathDoc = G.ChatPathDoc;
            Color color = ((SolidColorBrush)G.ChatWallPaper).Color;
            Properties.Settings.Default.ChatWallPaper = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)).Color;

            Properties.Settings.Default.Save();

            if (G.nHnd != null)
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

                        //string download = Environment.GetEnvironmentVariable("USERPROFILE") + @"\" + "Downloads";
                        //string down = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
                        //string path = X.KnownFolders.GetPath(X.KnownFolder.Downloads);


                        //EcoursWpfLibrary.Calendar.MyCalendar mc = new EcoursWpfLibrary.Calendar.MyCalendar(G.nHnd, "TestUser8");
                        //mc.Show();

                        //object[] oPrm = new object[] { "Редактор", "EcoursCCont.ExtRichTextBox" };
                        //DialogForm form = new DialogForm(oPrm);
                        //if (form.ShowDialog() == true)
                        //{

                        //}

                        //OpenFileDialog dialog = new OpenFileDialog();
                        //dialog.Title = "Открыть";
                        //if (dialog.ShowDialog() == true)
                        //{
                        //    string fileName = "1.rvf";
                        //    string fullName = dialog.FileName;
                        //    string path = Path.GetDirectoryName(dialog.FileName);
                        //    //string name = Path.GetFileName(dialog.FileName);

                        //    FilePosition fp = X.GetPosition(path + "kmf.kmf", fileName);

                        //    X.ReadFromFile(fullName, fileName, fp.OffSet, fp.Length);

                        //}

                        //List<string> list = new List<string> { "ОКВЭД-2", "ОКВЭД" };
                        //object[] oPrm = new object[] { "Справочник ОКВЭД", "EcoursCCont.CheckedTreeViewControl", "select * from OKVED", new List<string>() };
                        //ReferenceBook form = new ReferenceBook(10, oPrm, new object[] { list });

                        ////object[] oPrm = new object[] { "Справочник ОКТМО", "EcoursCCont.xTreeViewControl", "select * from OKTMO order by Kod" };
                        ////ReferenceBook form = new ReferenceBook(2, oPrm);
                        //if (form.ShowDialog() == true)
                        //{

                        //}
                        break;
                    case "Техподдержка":
                        //Uri iconUri = new Uri("pack://application:,,,/flay.ico", UriKind.RelativeOrAbsolute);
                        //Window win = new Window()
                        //{
                        //    Title = "Учетные записи",
                        //    Width = 1020,
                        //    Height = 720,
                        //    Icon = BitmapFrame.Create(iconUri),
                        //    Style = Application.Current.Resources["WindowStyle"] as Style,
                        //};
                        //win.Content = new HomePage();
                        //win.Show();
                        break;
                    case "Авторизация":
                    case "Сменить пользователя":
                        gridChat?.Close();
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
            if (comboBox.SelectedItem != null)
            {
                DataRowView item = (DataRowView)comboBox.SelectedItem;
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
        /// <summary>
        /// Открыть/Скрыть месенджер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chatOpen_Click(object sender, RoutedEventArgs e)
        {
            if (gridChat != null)
            {
                if (gridChat.Visibility == Visibility.Visible)
                {
                    gridChat.Visibility = Visibility.Collapsed;
                }
                else
                {
                    gridChat.Visibility = Visibility.Visible;
                    gridChat.ShowMessenger();
                }
            }
            else
            {
                OpenChatAndTask();
            }
        }
        /// <summary>
        /// Скрыть месенжер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void chatClose_Click(object sender, RoutedEventArgs e)
        {
            gridChat.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Выход из месенжера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void chatExit_Click(object sender, RoutedEventArgs e)
        {
            gridChat?.Close();
            gridChat.Visibility = Visibility.Collapsed;
            grid1.Children.Remove(gridChat);
            gridChat = null;
        }

        #endregion

        #region --- Методы ---

        /// <summary>
        /// Обратный вызов для сообщений из библиотеки
        /// </summary>
        /// <param name="e">Текст сообщения</param>
        private void ReceivedMessage(string e)
        {
            MessageBox.Show(e, "Внимание!");
        }
        /// <summary>
        /// Выполнение метода заданного в строке меню
        /// </summary>
        /// <param name="tag"></param>
        private void ExecMethod(TabItemTag tag)
        {
            if (tag.nType >= 4 || tag.nType < 4 && tag.cMethod.Length > 0)
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
                                bool fl = true;
                                foreach (Window w in Application.Current.Windows)
                                {
                                    if (w.ToString() == cs.FullName)
                                    {
                                        fl = false;
                                        w.Focus();
                                        break;
                                    }
                                }
                                if (fl)
                                {
                                    Window win = (Window)Activator.CreateInstance(cs, obj);
                                    win.Show();
                                }
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
            pwd.Owner = this;
            if (pwd.ShowDialog() == true)
            {
                tb2.Text = G.cUser;
                G.IsSinchron = false;
                G.nAccountId = 0;
                X.SQLEAsync(new Action<dynamic>(
                delegate (dynamic views)
                {
                    if (views != null && views is DataView)
                    {
                        DataView qAccount = (DataView)views;
                        comboBox.ItemsSource = qAccount;
                        comboBox.SelectedIndex = 0;
                    }
                }), "exec up_getaccounts " + G.nUserId, "qAccount");
                
                if (!G.flAdminRole)
                {
                    if (G.UUID != G.UserUUID)
                    {
                        int result = X.SQLE(G.nHnd, "exec up_checkuid " + G.nUserId.ToString() + ",'" + G.UUID + "'", "*");
                        if (result == 0)
                        {
                            if (MessageXBox.Show("Привязать этот компьютер в качестве основного устройства пользователя " + G.cUser, "Внимание!",
                                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                X.SQLE(G.nHnd, "update Users set UUID='" + G.UUID + "' where Id=" + G.nUserId.ToString());
                            }
                            else
                            {
                                if (MessageXBox.Show("Отметить этот компьютер как чужой?", "Внимание!", MessageBoxButton.YesNo,
                                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    X.SQLE(G.nHnd, "exec up_setuuid " + G.nUserId.ToString() + ",'" + G.UUID + "',1");
                                }
                                else
                                {
                                    X.SQLE(G.nHnd, "exec up_setuuid " + G.nUserId.ToString() + ",'" + G.UUID + "'");
                                }
                            }
                        }
                    }
                    DataView qParam = X.SQLE(G.nHnd, "exec up_getparams " + G.nUserId.ToString() + ",'" + G.UUID + "'", "qParam");
                    if (qParam != null && qParam.Count > 0)
                    {
                        foreach (DataRowView row in qParam)
                        {
                            string name = row["Name"].ToString();
                            object val = row["Value"];
                            G.IsSinchron |= G.SetValue(name, val);
                        }
                    }
                    qParam?.Dispose();
                }

                timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, (object s, EventArgs ev) =>
                {
                    timeText.Text = DateTime.Now.ToString("HH:mm");
                    dateText.Text = DateTime.Now.ToString("dd/MM/yyyy");
                }, Dispatcher);
                if (G.flDateInBottom) timer.Start();
                
                ThemeChange();

                // Заменить на автозагрузку
                if (G.flChatAndTasks)
                {
                    OpenChatAndTask();
                }
                else
                    sbItemChat.Visibility = Visibility.Collapsed;
            }
            else
            {
                Close();
            }
        }
        /// <summary>
        /// Авторизация на сервере
        /// </summary>
        /// <param name="login"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public int PwdAccept(string login, string pwd)
        {
            int result = 0;
            string title = "Внимание!";
            G.fdst = X.SQLC(login, pwd);
            if (G.fdst > 0)
            {
                G.cUser = login;
                G.cUserPwd = pwd;
                if (G.fdst > 8 && MessageXBox.Show("Необходимо сменить пароль входа в систему. \nХотите это сделать прямо сейчас?", 
                    title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    NewPassword pwdwin = new NewPassword();
                    pwdwin.OldPassword = pwd;
                    pwdwin.Owner = this;
                    pwdwin.ShowDialog();
                    G.fdst &= 7;
                }
                result = 1;
            }
            else
            {
                G.nHnd = X.SQLC(G.SqlConnectionString);
                string mess = "";
                bool flagError = false;
                switch (G.fdst)
                {
                    case -5:
                        mess = "Ошибка соединения с базой данных!";
                        flagError = true;
                        break;
                    case -4:
                        mess = "Ошибка настроек данных администратора!";
                        flagError = true;
                        break;
                    case -3:
                        mess = "Указанный пользователь не прописан на сервере!";
                        flagError = true;
                        break;
                    case -2:
                        mess = "Указан неверный пароль! \nПовторить ввод данных?";
                        break;
                    case -1:
                        mess = "Указанный пользователь заблокирован! \nВойти под другим именем?";
                        break;
                    default:
                        mess = "Указано неверно имя пользователя! \nПовторить ввод данных?";
                        break;
                }
                if (flagError)
                {
                    MessageXBox.Show(mess, title, MessageBoxButton.OK, MessageBoxImage.Error);
                    DialogResult = false;
                }
                else
                {
                    if (MessageXBox.Show(mess, title, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        result = -1;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Активировать задачу Чат
        /// </summary>
        private void OpenChatAndTask()
        {
            gridChat = new ChatAndTask(this)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 40, 20, 20)
            };
            Grid.SetRow(gridChat, 2);
            Grid.SetColumn(gridChat, 1);
            grid1.Children.Add(gridChat);
            sbItemChat.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Формирование основного меню (боковой панели)
        /// </summary>
        private void GenTopMenu()
        {
            if (G.nHnd != null && ((G.nHnd is SqlConnection) || (G.nHnd is int && G.nHnd > 0)))
            {
                button3.ToolTip = "Сменить пользователя";
                button3.ControlText = "Сменить пользователя";
                ClearMenu();
                string cmnd = "exec up_genmenu 0," + G.nUserId + "," + G.nRoleId;
                DataView dv = X.SQLE(G.nHnd, cmnd, "qmenu");
                if (dv != null && dv.Count > 0)
                {
                    foreach (DataRow row in dv.Table.Rows)
                    {
                        string tool = (bool)row["IsToolTip"] ? "true" : "false";
                        bool IsLocked = (bool)row["IsLocked"];
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
                        ib.IsEnabled = !IsLocked;
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
            bool IsLocked;
            TabItem tabItem;
            StackPanel sp;
            if (tabControl.Items.Count > 0) tabControl.Items.Clear();
            int idx = 0;
            // Выбор вкладок
            string cmnd = "exec up_genmenu " + key.ToString() + "," + G.nUserId + "," + G.nRoleId;
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
                    // Выбор кнопок на вкладке
                    cmnd = "exec up_genmenu " + row["Id"].ToString() + "," + G.nUserId + "," + G.nRoleId;
                    DataView dvb = X.SQLE(G.nHnd, cmnd, "qmenu");
                    if (dvb != null && dvb.Count > 0)
                    {
                        foreach (DataRow dr in dvb.Table.Rows)
                        {
                            string tool = (bool)dr["IsToolTip"] ? "true" : "false";
                            IsLocked = (bool)dr["IsLocked"];
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
                            tib.IsEnabled = !IsLocked;
                            tib.AddHandler(System.Windows.Controls.Primitives.ButtonBase.ClickEvent, new RoutedEventHandler(TabItemButton_Click));
                            tib.SetSize(IsOpenTopPanel);
                            sp.Children.Add(tib);
                        }
                    }
                    bool IsUnLocked = !(bool)row["IsLocked"];
                    tabItem = new TabItem()
                    {
                        Style = Application.Current.Resources["TabItemStyle1"] as Style,
                        Header = " " + row["MenuText"].ToString() + " ",
                        IsEnabled = IsUnLocked,
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
        /// <summary>
        /// Установить иконку кнопки вызова месенджера
        /// </summary>
        /// <param name="val"></param>
        public void SetImgChat(int val)
        {
            if (val == 0)
                imgChat.Source = new BitmapImage(new Uri("pack://application:,,,/EcoursCLib;component/Images/Black24x24/chats.png", UriKind.RelativeOrAbsolute));
            else
                imgChat.Source = new BitmapImage(new Uri("pack://application:,,,/EcoursCLib;component/Images/Black24x24/message.png", UriKind.RelativeOrAbsolute));
        }

        #endregion
    }
}
