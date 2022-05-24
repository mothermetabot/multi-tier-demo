using Common.DataTransferObject;
using Application2.Models;

namespace Application2.Extensions
{
    public static class UserExtensions
    {
        public static User FromDto(this UserDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            if (dto.Id == null) throw new ArgumentNullException(nameof(dto.Id));
            if (dto.Name == null) throw new ArgumentNullException(nameof(dto.Name));

            return new User(dto.Id, dto.Name);
        }

        public static UserDto ToDto(this User user)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));
            if (user.Id == null) throw new ArgumentNullException(nameof(user.Id));
            if (user.Name == null) throw new ArgumentNullException(nameof(user.Name));

            return new UserDto {
                Id = user.Id,
                Name = user.Name,
            };
        }
    }
}
