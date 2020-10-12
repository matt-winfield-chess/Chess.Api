using FluentValidation;
using System.Linq;

namespace Chess.Api.Validators
{
    public static class CustomValidators
    {
        public static IRuleBuilderOptions<T, string> MustNotContainUnicode<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(username => username.All(c => c <= 255)).WithMessage("'{PropertyName}' cannot contain unicode (non-ascii) characters");
        }
    }
}
