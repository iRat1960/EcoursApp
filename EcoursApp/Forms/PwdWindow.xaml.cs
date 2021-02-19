using System.Windows;
using EcoursApp.Models;
using System.Windows.Input;
using EcoursXLib;
using System.Data.SqlClient;
using System.Windows.Media;
using System.Data;
using System;
using System.Globalization;

namespace EcoursApp
{
    public partial class PwdWindow : Window
    {
        bool flAlarm = false;

        public PwdWindow(string title)
        {
            InitializeComponent();

            
            InputLanguageManager.Current.InputLanguageChanged += new InputLanguageEventHandler(Current_InputLanguageChanged);
            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("en-US");

            flAlarm = !(G.nHnd != null && ((G.nHnd is SqlConnection) || (G.nHnd is int && G.nHnd > 0)));
            Title = title;
            if (flAlarm)
            {
                Background = new SolidColorBrush(Colors.Red);
                OK.IsEnabled = false;
                loginBox.IsEnabled = false;
                roleBox.IsEnabled = false;
                passwordBox.IsEnabled = false;
                Cancel.Focus();
            }
            else
            {
                Background = new SolidColorBrush(Colors.DimGray);
                LoadRoles();
            }
        }

        private void Current_InputLanguageChanged(object sender, InputLanguageEventArgs e)
        {
            buttonLang.Content = InputLanguageManager.Current.CurrentInputLanguage.Name == "ru-RU" ? "РУС" : "ENG";
        }

        private void LoadRoles()
        {
            string cmnd = "select a.Id,a.Name from Roles a inner join UserRoles b ON b.RoleId = a.Id " +
                          "where b.UserId in (select Id from Users where Name='" + G.cUser + "')";
            DataView qRoles = X.SQLE(G.nHnd, cmnd, "qRole");
            if (qRoles != null && qRoles.Count > 0)
            {
                qRoles.Sort = "Id";
                int index = qRoles.Find(G.nRoleId);
                roleBox.ItemsSource = qRoles;
                roleBox.SelectedIndex = index;
            }
            else
            {
                G.cRole = "";
                G.nRoleId = 0;
            }
        }
        
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            G.fdst = X.SQLC(Login, Password);
            if (G.fdst > 0)
            {
                G.cUser = Login;
                G.cRole = SelectedRole["Name"].ToString();
                G.nRoleId = Convert.ToInt32(SelectedRole["Id"]);
                DialogResult = true;
            }
            else
            {
                G.nHnd = X.SQLC(G.SqlConnectionString);
                if (G.fdst == -3)
                {
                    MessageBox.Show("Ошибка соединения с базой данных!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                    DialogResult = false;
                }
                else
                {
                    if (MessageBox.Show("Указано неверное имя пользователя или пароль! \n" +
                    "Повторить ввод данных?", "Внимание!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        DialogResult = false;
                    }
                    passwordBox.Password = "";
                    var element = e.OriginalSource as UIElement;
                    e.Handled = true;
                    element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                }
            }
        }

        public string Password
        {
            get { return passwordBox.Password; }
        }

        public string Login
        {
            get { return loginBox.Text; }
        }

        public Role dbSelectedRole
        {
            get { return (Role)roleBox.SelectedValue; }
        }

        public DataRowView SelectedRole
        {
            get { return (DataRowView)roleBox.SelectedValue; }
        }

        private void loginBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Login.Length > 0)
            {
                string cmnd = "select * from Users where Name='" + Login + "'";
                DataView dv = X.SQLE(G.nHnd, cmnd, "qUser");
                if (dv != null && dv.Count > 0)
                {
                    G.cUser = Login;
                    G.nUserId = Convert.ToInt32(dv[0].Row["Id"]);
                    LoadRoles();
                }
                else
                    MessageBox.Show("Пользователь с таким именем не найден...", "Внимание!");
                dv?.Dispose();
            }
        }

        private void control_KeyDown(object sender, KeyEventArgs e)
        {
            var element = e.OriginalSource as UIElement;
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            } 
        }

        private void roleBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (roleBox.SelectedItem != null)
            {
                G.cRole = SelectedRole["Name"].ToString();
                G.nRoleId = Convert.ToInt32(SelectedRole["Id"]);
            }
        }
    }
}
