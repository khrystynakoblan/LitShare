
using LitShare.DAL; // 1. Доступ до нашого DbContext
using LitShare.DAL.Models; // 2. Доступ до наших моделей (User, Post і т.д.)
using System.Collections.Generic;
using System.Linq;

namespace LitShare.BLL.Services
{
    public class UserService
    {
        // Він не приймає нічого, а повертає список (List) об'єктів User
        public List<Users> GetAllUsers()
        {
            //    'using' гарантує, що з'єднання з базою буде закрито 
            //    після виконання коду, навіть якщо станеться помилка.
            using (LitShareDbContext context = new LitShareDbContext())
            {
                List<Users> allUsers = context.Users.ToList();

                return allUsers;
            }
        }

        // ---
        // ЗАГОТОВКИ ДЛЯ МАЙБУТНЬОЇ ЛОГІКИ:
        // ---

        // Метод для отримання одного користувача за його ID
        public Users? GetUserById(int id)
        {
            using (LitShareDbContext context = new LitShareDbContext())
            {
                return context.Users.Find(id);
            }
        }

        // Метод для додавання нового користувача
        public void AddUser(string name, string email, string password)
        {
            // Тут може буде логіка (BLL):
            // - Перевірка, чи email вже не зайнятий
            // - Хешування парол

            var newUser = new Users
            {
                name = name,
                email = email,
                password = password, // (Тут має бути хешування!)
                role = "user", // Значення за замовчуванням
                region = "Default",
                district = "Default",
                city = "Default"
            };

            using (LitShareDbContext context = new LitShareDbContext())
            {
                context.Users.Add(newUser); // Повідомляємо EF, що хочемо додати об'єкт
                context.SaveChanges();      // Виконуємо команду INSERT в базу даних
            }
        }
    }
}