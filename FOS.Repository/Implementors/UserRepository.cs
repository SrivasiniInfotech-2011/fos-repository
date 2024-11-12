using Dapper;
using FOS.Models;
using FOS.Models.Constants;
using FOS.Models.Entities;
using FOS.Repository.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Intrinsics.Arm;
using static FOS.Models.Constants.Constants;

namespace FOS.Repository.Implementors
{
    public class UserRepository : IUserRepository
    {
        #region [Private Fields]
        public readonly string _connectionString = string.Empty;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connectionString"></param>
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        #endregion

        #region [Interface Methods]
        /// <summary>
        /// Validate User Credentials.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Returns Validation Result</returns>
        public async Task<bool> ValidateCredentials(string username, string password)
        {
            var user = await FindByUsername(username);
            if (user != null)
            {
                return user.Passsword!.Equals(password);
            }

            return false;
        }

        /// <summary>
        /// Find User By Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns User By Id</returns>
        public async Task<User> FindById(string id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryFirstAsync<User>(SqlCommandConstants.GetUserById, new { UserLoginID = id });
            }
        }


        /// <summary>
        /// Find By User Name.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Returns User By Username</returns>
        public async Task<User?> FindByUsername(string username)
        {
            using (var connection = new SqlConnection(_connectionString))
            {

                var parameters = new DynamicParameters();
                parameters.Add(SqlParameterConstants.USER_LOGIN_ID, username, DbType.String, ParameterDirection.Input, 20);
                parameters.Add(SqlParameterConstants.PASSWORD, direction: ParameterDirection.Output, size: 100);
                parameters.Add(SqlParameterConstants.ERRORCODE, direction: ParameterDirection.Output, dbType: DbType.Int32);
                parameters.Add(SqlParameterConstants.COMPANYID, direction: ParameterDirection.Output, dbType: DbType.Int32);
                parameters.Add(SqlParameterConstants.USER_ID, direction: ParameterDirection.Output, size: 100, dbType: DbType.Int32);
                parameters.Add(SqlParameterConstants.USER_LEVEL_ID, direction: ParameterDirection.Output, size: 100, dbType: DbType.Int32);
                parameters.Add(SqlParameterConstants.LAST_LOGIN_DATE, direction: ParameterDirection.Output, size: 100, dbType: DbType.DateTime);
                parameters.Add(SqlParameterConstants.USER_THEME, direction: ParameterDirection.Output, size: 50, dbType: DbType.String);
                parameters.Add(SqlParameterConstants.USER_NAME, direction: ParameterDirection.Output, size: 50, dbType: DbType.String);
                parameters.Add(SqlParameterConstants.COMPANY_NAME, direction: ParameterDirection.Output, size: 80, dbType: DbType.String);
                parameters.Add(SqlParameterConstants.COMPANY_CODE, direction: ParameterDirection.Output, size: 3, dbType: DbType.String);
                parameters.Add(SqlParameterConstants.LEVEL_ACCESS, direction: ParameterDirection.Output, size: 50, dbType: DbType.String);
                parameters.Add(SqlParameterConstants.COUNTRY_NAME, direction: ParameterDirection.Output, size: 60, dbType: DbType.String);
                parameters.Add(SqlParameterConstants.USER_TYPE, direction: ParameterDirection.Output, size: 60, dbType: DbType.String);
                parameters.Add(SqlParameterConstants.MARGQUEE_TEXT, direction: ParameterDirection.Output, size: 1000, dbType: DbType.String);
                parameters.Add(SqlParameterConstants.ADDRESS1, direction: ParameterDirection.Output, size: 60, dbType: DbType.String);
                parameters.Add(SqlParameterConstants.ADDRESS2, direction: ParameterDirection.Output, size: 60, dbType: DbType.String);
                parameters.Add(SqlParameterConstants.CITY, direction: ParameterDirection.Output, size: 30, dbType: DbType.String);
                parameters.Add(SqlParameterConstants.STATE, direction: ParameterDirection.Output, size: 60, dbType: DbType.String);
                parameters.Add(SqlParameterConstants.ZIP_CODE, direction: ParameterDirection.Output, size: 10, dbType: DbType.String);
                parameters.Add(SqlParameterConstants.LNC_SERIAL_DATE, direction: ParameterDirection.Output, size: 15, dbType: DbType.String);
                parameters.Add(SqlParameterConstants.LNC_DATE, direction: ParameterDirection.Output, size: 100, dbType: DbType.String);
                parameters.Add(SqlParameterConstants.SESSION_ID, direction: ParameterDirection.Output, size: 60, dbType: DbType.String);
                connection.Open();
                var data = await connection.ExecuteAsync(SqlCommandConstants.GetUserByUsername, parameters, commandType: CommandType.StoredProcedure);
                if (data > 0)
                {
                    return new User
                    {
                        Address1 = parameters.Get<string?>(SqlParameterConstants.ADDRESS1),
                        Address2 = parameters.Get<string?>(SqlParameterConstants.ADDRESS2),
                        City = parameters.Get<string?>(SqlParameterConstants.CITY),
                        State = parameters.Get<string?>(SqlParameterConstants.STATE),
                        CompanyId = parameters.Get<int?>(SqlParameterConstants.COMPANYID),
                        Passsword = parameters.Get<string?>(SqlParameterConstants.PASSWORD),
                        CompanyCode = parameters.Get<string?>(SqlParameterConstants.COMPANY_CODE),
                        CompanyName = parameters.Get<string?>(SqlParameterConstants.COMPANY_NAME),
                        CountryName = parameters.Get<string?>(SqlParameterConstants.COUNTRY_NAME),
                        LastLoginDate = parameters.Get<DateTime?>(SqlParameterConstants.LAST_LOGIN_DATE),
                        LevelAccess = parameters.Get<string?>(SqlParameterConstants.LEVEL_ACCESS),
                        LncDate = parameters.Get<DateTime?>(SqlParameterConstants.LNC_DATE),
                        LncSerialDate = parameters.Get<DateTime?>(SqlParameterConstants.LNC_SERIAL_DATE),
                        MarqueeText = parameters.Get<string?>(SqlParameterConstants.MARGQUEE_TEXT),
                        SessionId = parameters.Get<string?>(SqlParameterConstants.SESSION_ID),
                        UserId = parameters.Get<int?>(SqlParameterConstants.USER_ID),
                        UserLevelId = parameters.Get<int?>(SqlParameterConstants.USER_LEVEL_ID),
                        UserLoginId = parameters.Get<string?>(SqlParameterConstants.USER_LOGIN_ID),
                        UserName = parameters.Get<string?>(SqlParameterConstants.USER_NAME),
                        UserTheme = parameters.Get<string?>(SqlParameterConstants.USER_THEME),
                        UserType = parameters.Get<string?>(SqlParameterConstants.USER_TYPE),
                        ZipCode = parameters.Get<string?>(SqlParameterConstants.ZIP_CODE)
                    };
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the User Menus.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>Returns List of type <see cref="UserMenu"/></returns>
        public async Task<List<UserMenu>> GetUserMenus(int userId)
        {
            var userMenus = new List<UserMenu>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var userMenuItems = new List<UserMenuItem>();
                var parameters = new DynamicParameters();
                parameters.Add(SqlParameterConstants.USER_ID, userId, DbType.Int32, ParameterDirection.Input);
                connection.Open();
                var dataReader = await connection.ExecuteReaderAsync(SqlCommandConstants.GetUserProgramList, parameters, commandType: CommandType.StoredProcedure);
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        userMenuItems.Add(new UserMenuItem
                        {
                            ModuleName = dataReader.GetString(SqlColumnNames.ModuleName),
                            ProgramCode = dataReader.GetString(SqlColumnNames.ProgramCode)
                        });
                    }
                }
                var distinctModules = userMenuItems.Select(s => s.ModuleName).Distinct();
                foreach (var module in distinctModules)
                {
                    var moduleProgramList = userMenuItems.Where(s => s.ModuleName == module).Select(s => s.ProgramCode).ToList();
                    userMenus.Add(new UserMenu { ModuleName = module, Menus = moduleProgramList });
                }
            }

            return userMenus;
        }
        #endregion
    }
}
