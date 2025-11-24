using System;
using System.Collections.Generic;
using Bogus;

namespace LitShare.ConsoleApp
{
    public interface IBookRepository
    {
        void ClearAllData();
        int AddUser(User user);
        int AddPost(Post post);
        int AddGenre(Genre genre);
        void AddBookGenre(int postId, int genreId);
        int AddComplaint(Complaint complaint);
        List<Post> GetAllPosts();
        List<User> GetAllUsers();
        List<Genre> GetAllGenres();
        List<Complaint> GetAllComplaints();
    }

    public class InMemoryBookRepository : IBookRepository
    {
        private readonly List<User> _users = new();
        private readonly List<Post> _posts = new();
        private readonly List<Genre> _genres = new();
        private readonly List<(int postId, int genreId)> _booksGenres = new();
        private readonly List<Complaint> _complaints = new();

        public void ClearAllData()
        {
            _users.Clear();
            _posts.Clear();
            _genres.Clear();
            _booksGenres.Clear();
            _complaints.Clear();
        }

        public int AddUser(User user)
        {
            user.Id = _users.Count + 1;
            _users.Add(user);
            return user.Id;
        }

        public int AddPost(Post post)
        {
            post.Id = _posts.Count + 1;
            _posts.Add(post);
            return post.Id;
        }

        public int AddGenre(Genre genre)
        {
            genre.Id = _genres.Count + 1;
            _genres.Add(genre);
            return genre.Id;
        }

        public void AddBookGenre(int postId, int genreId)
        {
            _booksGenres.Add((postId, genreId));
        }

        public int AddComplaint(Complaint complaint)
        {
            complaint.Id = _complaints.Count + 1;
            _complaints.Add(complaint);
            return complaint.Id;
        }

        public List<Post> GetAllPosts() => new(_posts);
        public List<User> GetAllUsers() => new(_users);
        public List<Genre> GetAllGenres() => new(_genres);
        public List<Complaint> GetAllComplaints() => new(_complaints);
    }

    public class DataSeeder
    {
        private readonly IBookRepository _repo;

        public DataSeeder(IBookRepository repo)
        {
            _repo = repo;
        }

        public void SeedUsersAndPosts(int usersCount, int postsPerUser)
        {
            var userFaker = new Faker<User>()
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Password, f => f.Internet.Password())
                .RuleFor(u => u.About, f => f.Lorem.Sentence())
                .RuleFor(u => u.Role, f => f.PickRandom(new[] { "user", "admin" }))
                .RuleFor(u => u.Region, f => f.Address.State())
                .RuleFor(u => u.District, f => f.Address.County())
                .RuleFor(u => u.City, f => f.Address.City());

            var postFaker = new Faker<Post>()
                .RuleFor(p => p.Title, f => f.Commerce.ProductName())
                .RuleFor(p => p.Author, f => f.Name.FullName())
                .RuleFor(p => p.DealType, f => f.PickRandom(new[] { "exchange", "donation" }))
                .RuleFor(p => p.Description, f => f.Lorem.Sentence(10))
                .RuleFor(p => p.PhotoUrl, f => f.Image.PicsumUrl());

            for (int i = 0; i < usersCount; i++)
            {
                var user = userFaker.Generate();
                int userId = _repo.AddUser(user);

                var posts = postFaker.Clone()
                    .RuleFor(p => p.UserId, f => userId)
                    .Generate(postsPerUser);

                foreach (var post in posts)
                    _repo.AddPost(post);

                Console.Write(".");
            }

            Console.WriteLine();
        }

        public void SeedGenres(int genresCount)
        {
            var genreFaker = new Faker<Genre>()
                .RuleFor(g => g.Name, f => f.Commerce.Categories(1)[0]);

            for (int i = 0; i < genresCount; i++)
            {
                _repo.AddGenre(genreFaker.Generate());
                Console.Write(".");
            }

            Console.WriteLine();
        }

        public void SeedComplaints(int complaintsCount)
        {
            var complaintFaker = new Faker<Complaint>()
                .RuleFor(c => c.Text, f => f.Lorem.Sentence())
                .RuleFor(c => c.PostId, f => f.Random.Int(1, Math.Max(1, _repo.GetAllPosts().Count)))
                .RuleFor(c => c.ComplainantId, f => f.Random.Int(1, Math.Max(1, _repo.GetAllUsers().Count)));

            for (int i = 0; i < complaintsCount; i++)
            {
                _repo.AddComplaint(complaintFaker.Generate());
                Console.Write(".");
            }

            Console.WriteLine();
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string About { get; set; }
        public string Role { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string City { get; set; }
    }

    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string DealType { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
    }

    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Complaint
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int PostId { get; set; }
        public int ComplainantId { get; set; }
    }

    public class ConsoleApp
    {
        static void Main()
        {
            RunMenu(Console.In, Console.Out);
        }

        public static void RunMenu(System.IO.TextReader input, System.IO.TextWriter output)
        {
            IBookRepository repo = new InMemoryBookRepository();
            var seeder = new DataSeeder(repo);

            output.WriteLine("=== LitShare In-Memory Seeder ===");
            repo.ClearAllData();

            seeder.SeedUsersAndPosts(5, 2);
            seeder.SeedGenres(3);
            seeder.SeedComplaints(2);

            output.WriteLine("Дані згенеровані.\n");

            while (true)
            {
                output.WriteLine("Що показати?");
                output.WriteLine("1 - Користувачі");
                output.WriteLine("2 - Пости");
                output.WriteLine("3 - Жанри");
                output.WriteLine("4 - Скарги");
                output.WriteLine("0 - Вихід");

                var choice = input.ReadLine();
                if (choice == "0") break;

                switch (choice)
                {
                    case "1":
                        foreach (var u in repo.GetAllUsers())
                            output.WriteLine($"UserID: {u.Id}, Name: {u.Name}, Role: {u.Role}");
                        break;
                    case "2":
                        foreach (var p in repo.GetAllPosts())
                            output.WriteLine($"PostID: {p.Id}, UserID: {p.UserId}, Title: {p.Title}, DealType: {p.DealType}");
                        break;
                    case "3":
                        foreach (var g in repo.GetAllGenres())
                            output.WriteLine($"GenreID: {g.Id}, Name: {g.Name}");
                        break;
                    case "4":
                        foreach (var c in repo.GetAllComplaints())
                            output.WriteLine($"ComplaintID: {c.Id}, PostID: {c.PostId}, ComplainantID: {c.ComplainantId}, Text: {c.Text}");
                        break;
                    default:
                        output.WriteLine("Невірний вибір");
                        break;
                }

                output.WriteLine();
            }
        }
    }
}
