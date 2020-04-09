using SignalRAPI.DB;
using System;
using System.Linq;
using System.Web.Http;


namespace SignalRAPI.Controllers
{
    public class LoginController : ApiController
    {
        private DocTalkDBContext _docTalkDBContext;
        public LoginController()
        {
            _docTalkDBContext = new DocTalkDBContext();
        }

        [Route("api/login/UserLogin")]
        [HttpPost]
        public Response Login(Login Lg)
        {
            try
            {

                Doctor docObj = _docTalkDBContext.Doctors.FirstOrDefault(x => x.UserName == Lg.UserName && x.Password == Lg.Password);

                if (docObj != null)
                {
                    DoctorController doctorController = new DoctorController();
                    doctorController.MakeDoctorAvailable(docObj.UserId);
                    return new Response { Status = "Success", Message = Lg.UserName };
                }
                else
                {
                    return new Response { Status = "Invalid", Message = "Invalid User." };
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [Route("api/login/logOut")]
        [HttpPost]
        public void LogOut(int userId)
        {
            try
            {
                var docObj = _docTalkDBContext.Doctors.FirstOrDefault(x => x.UserId == userId);
                if (docObj != null)
                {
                    docObj.IsActive = false;
                    docObj.IsLocked = false;
                    var context = docObj.DoctorEntities.FirstOrDefault();
                    context.DoctorConnectionId = "";
                    _docTalkDBContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void RemoveUserFromRoom(string contextId)
        {
            try
            {
                var room = _docTalkDBContext.Rooms.FirstOrDefault(x => x.DoctorConnectionId == contextId);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}