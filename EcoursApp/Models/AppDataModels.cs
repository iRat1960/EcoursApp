using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace EcoursApp.Models
{
    public enum MenuType
    {
        Модули = 0,
        Закладки = 1,
        Методы = 2,
        Страницы = 3,
        Формы = 4,
        Обработчик = 5
    }
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSystem { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Pwd { get; set; }
        public int HR_id { get; set; }
    }
    public class UserRole
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
    }
    public class MenuRole
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int MainMenuId { get; set; }
    }
    public class MainMenu
    {
        public int Id { get; set; }
        public int Pid { get; set; }
        public int Num { get; set; }

        public SubMenu SubMenu { get; set; }
    }
    public class SubMenu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MenuText { get; set; }
        public string ToolTip { get; set; }
        public string Template { get; set; }
        public string MenuAssembly { get; set; }
        public string MenuEvent { get; set; }
        public MenuType Type { get; set; }
    }

    public class ViewMenu
    {
        public int Id { get; set; }
        public int Sid { get; set; }
        public string Name { get; set; }
        public string MenuText { get; set; }
        public string ToolTip { get; set; }
        public string Template { get; set; }
        public string MenuAssembly { get; set; }
        public string MenuEvent { get; set; }
        public MenuType Type { get; set; }
    }

    public class ViewSMenu
    {
        public int Id { get; set; }
        public int Sid { get; set; }
        public string Name { get; set; }
    }

}
