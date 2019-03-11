using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
            var dateValue = objValue as DateTime? ?? new DateTime();
            
            if (dateValue.Date > DateTime.Now.Date)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }
    }
}
