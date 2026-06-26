using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Shared.Models
{
    public static class BaseIdErrors{
        public static Error Empty => Error.Validation(
            "Id.Empty",
            $"شناسه نمی‌تواند خالی باشد");

        public static Error Invalid=> Error.Validation(
            "Id.Invalid",
            $"شناسه نامعتبر است");

        public static Error InvalidFormat(string value)=> Error.Validation(
            "Id.InvalidFormat",
            $"فرمت شناسه '{value}' نامعتبر است");


    }
}