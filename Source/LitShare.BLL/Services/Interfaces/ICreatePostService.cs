<<<<<<< HEAD
﻿using System.Threading.Tasks;
using LitShare.BLL.DTOs;
using Microsoft.AspNetCore.Http;
=======
﻿using LitShare.BLL.DTOs;
>>>>>>> ef4b67b (Add Create Post Window and tests)

namespace LitShare.BLL.Services.Interfaces
{
    public interface ICreatePostService
    {
<<<<<<< HEAD
        // Змінюємо Task на Task<int>
        Task<int> CreatePostAsync(CreatePostDto dto, IFormFile? imageFile, int userId);
=======
        Task<int> CreatePostAsync(CreatePostDto dto, int userId);
>>>>>>> ef4b67b (Add Create Post Window and tests)
    }
}