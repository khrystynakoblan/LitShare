using LitShare.DAL;
using LitShare.DAL.Models;
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
        // У файлі LitShare.BLL.Services/UserService.cs

        public void AddUser(string name, string email, string password, string region, string district, string city)
        {
            var newUser = new Users
            {
                name = name,
                email = email,
                password = password,
                region = region,
                district = district,
                city = city
            };

            using (LitShareDbContext context = new LitShareDbContext())
            {
                context.Users.Add(newUser);
                context.SaveChanges();
            }
        }
    }
    public class PostService
    {
        public Posts? GetPostById(int id)
        {
            using (var context = new LitShareDbContext())
            {
                return context.posts.FirstOrDefault(p => p.id == id);
            }
        }
    }
    public class ComplaintService
    {
        public void AddComplaint(int postId, int complainantId, string text)
        {
            var complaint = new Complaints
            {
                post_id = postId,
                complainant_id = complainantId,
                text = text
            };

            using (var context = new LitShareDbContext())
            {
                context.complaints.Add(complaint);
                context.SaveChanges();
            }
        }
    }
}