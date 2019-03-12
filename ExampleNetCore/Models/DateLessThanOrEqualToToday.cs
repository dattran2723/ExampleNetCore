using ExampleNetCore.Controllers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExampleNetCore.Models
{
    public class DateLessThanOrEqualToToday : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return "Ngày sinh phải nhỏ hơn ngày hiện tại";
        }
        protected override ValidationResult IsValid(object objValue,
                                                   ValidationContext validationContext)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var dateValue = epoch.AddSeconds((long)objValue);

            if (dateValue.Date > DateTime.Now.Date)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }
    }

}
