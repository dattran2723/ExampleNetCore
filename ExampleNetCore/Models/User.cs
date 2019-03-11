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

        [Required(ErrorMessage = "Vui lòng nhập Email!")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DateLessThanOrEqualToToday]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại liên hệ!")]
        [RegularExpression("^0[0-9]+$", ErrorMessage = "Vui lòng nhập đúng định dạng!")]
        [StringLength(13, ErrorMessage = "Số điên thoại gồm 6-13 ký tự số!", MinimumLength = 6)]
        public string Phone { get; set; }

        public Gender Gerder { get; set; }

        public string Address { get; set; }
    }
}
