﻿using EcoursXLib;
using EcoursApp.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using System.Reflection;

namespace EcoursApp.Forms
{
    public partial class TopWindow : Window
    {
        bool flAlarm = false;

        public TopWindow()
        {
            InitializeComponent();

            InputLanguageManager.Current.InputLanguageChanged += new InputLanguageEventHandler(Current_InputLanguageChanged);
            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("en-US");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            flAlarm = !(G.nHnd != null && ((G.nHnd is SqlConnection) || (G.nHnd is int && G.nHnd > 0)));
            string uri = "";
            if (flAlarm)
            {
                uri = "networkw_alarm.png";
                wp1.Margin = new Thickness(0, 10, 0, 40);
                button1.Visibility = Visibility.Collapsed;
                button2.Visibility = Visibility.Collapsed;
                text1.Text = "Администратор";
                loginBox.Text = "Admin";
                Title = "Аварийный вход (отсутствие связи)";
                var image = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/EcoursApp;component/Images/Admin.png", UriKind.RelativeOrAbsolute))
                };
                img1.ImageSource = image.Source;
            }
            else
            {
                uri = "networkw.png";
                LoadAvatars();
                LoadRoles();
                text1.Text = G.cUser;
                loginBox.Text = G.cUser;
                Title = G.cRole == string.Empty ? "Внимание! Роль не указана" : ("Вход с ролью " + G.cRole);
            }
            var uriImageSource = new Uri(@"/EcoursApp;component/Images/" + uri, UriKind.RelativeOrAbsolute);
            imgNetwork.Source = new BitmapImage(uriImageSource);
            passwordBox.Focus();
        }

        private void Current_InputLanguageChanged(object sender, InputLanguageEventArgs e)
        {
            buttonLang.Content = InputLanguageManager.Current.CurrentInputLanguage.Name == "ru-RU" ? "РУС" : "ENG";
        }

        private void LoadAvatars()
        {
            string cmnd = "select DataFile from UserAvatars where UserId in (select Id from Users where Name='" + G.cUser + "')";
            DataView dv = X.SQLE(G.nHnd, cmnd, "qbuf");
            if (dv != null && dv.Count > 0)
            {
                byte[] b = (byte[])dv[0].Row["DataFile"];
                MemoryStream ms = new MemoryStream(b);
                ms.Position = 0;
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = ms;
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                var image = new Image
                {
                    Source = bmp
                };
                img1.ImageSource = image.Source;
                ms.Close();
            }
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
                if (qRoles.Count < 2)
                {
                    button2.Visibility = Visibility.Collapsed;
                    button1.Margin = new Thickness(10, 0, 10, 60);
                }
            }
            else
            {
                G.cRole = "";
                G.nRoleId = 0;
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (flAlarm)
            {
                DialogResult = false;
            }
            else
            {
                Type type = Owner.GetType();
                object obj = type.InvokeMember("PwdAccept", BindingFlags.InvokeMethod, null, Owner, new object[] { Login, Password });
                int result = (int)obj;
                if (result == 1)
                    DialogResult = true;
                else
                {
                    if (result == -1)
                        DialogResult = false;
                    else
                    {
                        passwordBox.Password = "";
                        var element = e.OriginalSource as UIElement;
                        e.Handled = true;
                        element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    }
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

        private void buttonQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void loginBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Login.Length > 0)
            {
                if (!flAlarm)
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
                }
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Thickness margin = new Thickness(0, 0, 0, 40);
            text1.Text = "Другой пользователь";
            label1.Visibility = Visibility.Visible;
            loginBox.Visibility = Visibility.Visible;
            roleBox.Visibility = Visibility.Visible;
            wp1.Margin = margin;
            button1.Visibility = Visibility.Collapsed;
            button2.Visibility = Visibility.Collapsed;
            var image = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/EcoursApp;component/Images/userAdd.png", UriKind.RelativeOrAbsolute))
            };
            img1.ImageSource = image.Source;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Thickness margin = new Thickness(0, 0, 0, 40);
            label1.Content = "Выберите нужную роль";
            label1.Visibility = Visibility.Visible;
            roleBox.Visibility = Visibility.Visible;
            wp1.Margin = margin;
            button1.Visibility = Visibility.Collapsed;
            button2.Visibility = Visibility.Collapsed;
        }

        private void roleBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (roleBox.SelectedItem != null)
            {
                G.cRole = SelectedRole["Name"].ToString();
                G.nRoleId = Convert.ToInt32(SelectedRole["Id"]);
            }
        }
    }
}
