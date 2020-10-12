using Chess.Api.Models;
using FluentValidation;

namespace Chess.Api.Validators
{
    public class PostUserModelValidator : AbstractValidator<PostUserModel>
    {
        public PostUserModelValidator()
        {
            RuleFor(model => model.Username).NotNull().NotEmpty().MustNotContainUnicode();
            RuleFor(model => model.Password).NotNull().NotEmpty().MustNotContainUnicode();
        }
    }
}
