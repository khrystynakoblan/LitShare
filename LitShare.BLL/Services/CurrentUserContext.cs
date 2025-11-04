using LitShare.DAL.Models; // 1. Важливо: підключаємо ваші моделі

namespace LitShare.Presentation.Services
{
    public class CurrentUserContext
    {
        // Властивість, де будуть зберігатися дані
        public Users CurrentUser { get; private set; }

        // Метод для "входу"
        public void SetCurrentUser(Users user)
        {
            CurrentUser = user;
        }

        // Метод для "виходу"
        public void ClearCurrentUser()
        {
            CurrentUser = null;
        }

        // Перевірка, чи користувач увійшов
        public bool IsUserLoggedIn => CurrentUser != null;
    }
}