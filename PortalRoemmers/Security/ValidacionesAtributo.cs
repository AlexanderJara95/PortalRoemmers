using System.ComponentModel.DataAnnotations;


namespace PortalRoemmers.Security
{
    public class TipoCambioAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool valid = false;
            double sval = 0;
            try
            {
                sval = double.Parse(value.ToString());
            }
            catch
            {    }
            
            if (sval != 0)
            {
                valid = true;
            }

            return valid ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }

    }

}