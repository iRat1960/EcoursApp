using EcoursCLib.Forms;
using EcoursXLib;
using System.Windows;
using System.Windows.Input;

namespace EcoursApp.Forms
{
    public partial class NewPassword : Window
    {
        public string OldPassword = "";

        public NewPassword()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (pwd1.Password.Length > 0 && pwd1.Password != OldPassword && pwd1.Password == pwd2.Password)
            {
                string cmnd = string.Format("exec up_setnewpwd {0},'{1}'", G.nUserId, pwd1.Password.Trim());
                int result = X.SQLE(G.nHnd, cmnd);
                if (result > 0)
                    MessageXBox.Show("Новый пароль успешно установлен!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
            }
            else
            {
                string mess = (pwd1.Password == OldPassword ? "Новый пароль не должен совпадать со старым" : "Введённые пароли не совпадают") + "! \nПовторить ввод?";
                if (MessageXBox.Show(mess, "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    DialogResult = false;
                }
                var element = e.OriginalSource as UIElement;
                e.Handled = true;
                element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            }
        }
    }
}
