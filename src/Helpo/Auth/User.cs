using Helpo.Common;

namespace Helpo.Auth
{
    public class User
    {
        public static User Create(string name, int appUserI3D, int? contactPersonI3D)
        {
            Guard.NotNull(name, nameof(name));
            Guard.NotZeroOrNegative(appUserI3D, nameof(appUserI3D));
            // contactPersonI3D

            return new User
            {
                Id = IdHelper.GenerateNewId(),
                Name = name,
                Kind = contactPersonI3D is null ? UserKind.User : UserKind.WebAccount,
                AppUserI3D = contactPersonI3D is null ? appUserI3D : default,
                ContactPersonI3D = contactPersonI3D
            };
        }
        
        // For serialization purposes only
        private User()
        {
            this.Id = default!;
            this.Name = default!;
        }
        
        public string Id { get; set; }
        public string Name { get; set; }
        
        public UserKind Kind { get; set; }
        public int? AppUserI3D { get; set; }
        public int? ContactPersonI3D { get; set; }
    }

    public enum UserKind
    {
        User,
        WebAccount
    }
}