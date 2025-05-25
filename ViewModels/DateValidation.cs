using System;
using System.ComponentModel.DataAnnotations;

namespace Hirfa.Web.ViewModels
{
    public static class DateValidation
    {
        public static ValidationResult? ValidateNotPastDate(DateTime date, ValidationContext context)
        {
            if (date < DateTime.Today)
            {
                return new ValidationResult("The date cannot be in the past.");
            }

            return ValidationResult.Success;
        }
    }
}
