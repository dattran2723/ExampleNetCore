using ExampleNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleNetCore.ViewModels
{
    public class UserViewModel
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        public string Email { get; set; }

        [DateLessThanOrEqualToToday]
        public long DateOfBirth { get; set; }
        public Gender Gender { get; set; }

        [RegularExpression("^0[0-9]+$", ErrorMessage = "Vui lòng nhập đúng định dạng!")]
        [StringLength(13, ErrorMessage = "Số điên thoại gồm 6-13 ký tự số!", MinimumLength = 6)]
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
