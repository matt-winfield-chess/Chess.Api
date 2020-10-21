using Chess.Api.Models;
using Chess.Api.Models.Post;
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
