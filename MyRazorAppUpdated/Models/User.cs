using Microsoft.AspNetCore.Identity; // PasswordHasher'ı kullanabilmek için bu namespace'i ekleyin
using System;

namespace RazorApp.Models
{
    public static class UserData
    {
        // This static list will hold all user information
        public static List<User> Users { get; set; } = new List<User>();

        // You can also add methods to add
        public static void AddUser(User newUser)
        {
            // Hash the password
            var passwordHasher = new PasswordHasher<User>();
            if (!string.IsNullOrEmpty(newUser.Password))
            {
                newUser.Password = passwordHasher.HashPassword(newUser, newUser.Password);
            }
            else
            {
                throw new ArgumentException("Password cannot be null or empty.");
            }
            // Add user to list
            Users.Add(newUser);
        }
    }
    public class User : IdentityUser
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public bool isActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    
}