using FOS.Models.Entities;

namespace FOS.Repository.Interfaces
{
    public interface IUserRepository
    {

        /// <summary>
        /// Validate User Credentials.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Returns Validation Result</returns>
        Task<bool> ValidateCredentials(string username, string password);

        /// <summary>
        /// Find User By Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns User By Id</returns>
        Task<User> FindById(string id);

        /// <summary>
        /// Find By User Name.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Returns User By Username</returns>
        Task<User?> FindByUsername(string username);

        /// <summary>
        /// Gets the User Menus.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>Returns List of type <see cref="UserMenu"/></returns>
        Task<List<UserMenu>> GetUserMenus(int userId);
    }
}
