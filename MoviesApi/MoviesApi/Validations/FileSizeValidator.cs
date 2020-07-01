using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Validations
{
    public class FileSizeValidator : ValidationAttribute
    {
        private readonly int _maxFileSizeInMbs;

        public FileSizeValidator(int MaxFileSizeInMbs)
        {
            _maxFileSizeInMbs = MaxFileSizeInMbs;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            IFormFile formFile = value as IFormFile;

            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            if (formFile.Length > _maxFileSizeInMbs * 1024 * 1024)
            {
                return new ValidationResult($"Max file size canoot be bigger than {_maxFileSizeInMbs} megabytes");
            }

            return ValidationResult.Success;
        }
    }
}
