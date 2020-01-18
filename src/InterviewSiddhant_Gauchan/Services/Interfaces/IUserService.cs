using InterviewSiddhant_Gauchan.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InterviewSiddhant_Gauchan.Services
{
    public interface IUserService
    {
       Task<List<UserModel>> GetInfo();
    }
}
