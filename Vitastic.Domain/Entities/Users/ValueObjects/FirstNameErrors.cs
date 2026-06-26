using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Users.ValueObjects
{
    public static class FirstNameErrors
    {
        public static Error EmptyFirstName()=>
             Error.Validation("FirstName.Empty","نام نمیتواند خالی باشد.");
        public static Error FirstNameContainsDigits()=>
             Error.Validation("FirstName.ContainsDigits","نام نمیتواند شامل اعداد باشد.");
        public static Error FirstNameContainsPunctuation()=>
             Error.Validation("FirstName.ContainsPunctuation","نام نمیتواند شامل علائم نگارشی باشد.");
        public static Error FirstNameContainsSymbols()=>
             Error.Validation("FirstName.ContainsSymbols","نام نمیتواند شامل نمادها باشد.");
        public static Error FirstNameTooShort (int minLength)=>
             Error.Validation("FirstName.TooShort",$"نام نمیتواند کمتر از {minLength} کاراکتر باشد.");
        public static Error FirstNameTooLong (int maxLength)=>
             Error.Validation("FirstName.TooLong",$"نام نمیتواند بیشتر از {maxLength} کاراکتر باشد.");

    }
}
