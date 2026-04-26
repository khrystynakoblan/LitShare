using LitShare.DAL.Models;

namespace LitShare.BLL.Common
{
    public static class RequestStatusExtensions
    {
        public static string ToDisplay(this RequestStatus status)
        {
            return status switch
            {
                RequestStatus.Pending => "Очікує",
                RequestStatus.Accepted => "Прийнято",
                RequestStatus.Rejected => "Відхилено",
                RequestStatus.Completed => "Завершено",
                _ => "Невідомо"
            };
        }
    }
}