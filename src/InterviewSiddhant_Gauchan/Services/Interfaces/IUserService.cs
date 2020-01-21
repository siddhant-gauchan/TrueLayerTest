using TrulayerApiTest.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrulayerApiTest.Services
{
    public interface IUserService
    {
       Task<List<UserModel>> GetInfo();
    }
}
