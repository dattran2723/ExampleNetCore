using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExampleNetCore.Models
{
    public enum Gender { Female, Male }
    public class User
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Phone { get; set; }

        public Gender Gerder { get; set; }

        public string Address { get; set; }
    }
}
