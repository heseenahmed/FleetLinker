using FleetLinker.Application.Common.Localization;
using Microsoft.AspNetCore.Identity;

namespace FleetLinker.API.Localization
{
    public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
    {
        private readonly IAppLocalizer _localizer;

        public LocalizedIdentityErrorDescriber(IAppLocalizer localizer)
        {
            _localizer = localizer;
        }

        public override IdentityError DefaultError()
        {
            return new IdentityError
            {
                Code = nameof(DefaultError),
                Description = _localizer[LocalizationMessages.IdentityDefaultError]
            };
        }

        public override IdentityError ConcurrencyFailure()
        {
            return new IdentityError
            {
                Code = nameof(ConcurrencyFailure),
                Description = _localizer[LocalizationMessages.IdentityConcurrencyFailure]
            };
        }

        public override IdentityError PasswordMismatch()
        {
            return new IdentityError
            {
                Code = nameof(PasswordMismatch),
                Description = _localizer[LocalizationMessages.IdentityPasswordMismatch]
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = string.Format(_localizer[LocalizationMessages.IdentityPasswordTooShort], length)
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = _localizer[LocalizationMessages.IdentityPasswordRequiresNonAlphanumeric]
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = _localizer[LocalizationMessages.IdentityPasswordRequiresDigit]
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = _localizer[LocalizationMessages.IdentityPasswordRequiresLower]
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = _localizer[LocalizationMessages.IdentityPasswordRequiresUpper]
            };
        }

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUniqueChars),
                Description = string.Format(_localizer[LocalizationMessages.IdentityPasswordRequiresUniqueChars], uniqueChars)
            };
        }

        public override IdentityError InvalidUserName(string? userName)
        {
            return new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = _localizer[LocalizationMessages.IdentityInvalidUserName]
            };
        }

        public override IdentityError InvalidEmail(string? email)
        {
            return new IdentityError
            {
                Code = nameof(InvalidEmail),
                Description = _localizer[LocalizationMessages.IdentityInvalidEmail]
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = string.Format(_localizer[LocalizationMessages.IdentityDuplicateUserName], userName)
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = string.Format(_localizer[LocalizationMessages.IdentityDuplicateEmail], email)
            };
        }
    }
}
