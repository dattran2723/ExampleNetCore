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

    public class CheckEmail : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return "Email đã tồn tại";
        }
        protected override ValidationResult IsValid(object objValue, ValidationContext validationContext)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44395/api/");
            //HTTP GET
            var responseTask = client.GetAsync("users/checkexistingemail?email=" + objValue.ToString());
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }
    }

    public class CheckPhone : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return "Số điện thoại đã tồn tại";
        }
        protected override ValidationResult IsValid(object objValue, ValidationContext validationContext)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44395/api/");
            //HTTP GET
            var responseTask = client.GetAsync("users/checkexistingphone?phone=" + objValue.ToString());
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }
    }
}
