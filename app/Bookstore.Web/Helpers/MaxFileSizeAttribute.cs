using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Bookstore.Web.Helpers
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize)
        {
            this.maxFileSize = maxFileSize;
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;

            if (!(value is HttpPostedFileBase file)) return base.IsValid(value);

            return file.ContentLength <= maxFileSize;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} cannot exceed {maxFileSize.ToStorageSize()}";
        }
    }
}