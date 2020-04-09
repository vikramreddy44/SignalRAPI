using SignalRAPI.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SignalRAPI.Controllers
{
    public class DoctorController : ApiController
    {
        private DocTalkDBContext _docTalkDDContext;

        public DoctorController()
        {
            _docTalkDDContext = new DocTalkDBContext();
        }
        [Route("api/doctor/createEmployee")]
        [HttpPost]
        public Response CreateDcotor(Doctor doctor)
        {
            try
            {
                
                Doctor newDoctor = new Doctor();
                if (newDoctor.UserId == 0)
                {
                    newDoctor.UserName = doctor.UserName;
                    newDoctor.LoginName = doctor.LoginName;
                    newDoctor.Password = doctor.Password;
                    newDoctor.Email = doctor.Email;
                    newDoctor.ContactNo = doctor.ContactNo;
                    newDoctor.Address = doctor.Address;
                    newDoctor.IsActive = false;
                    newDoctor.IsLocked = true;
                    _docTalkDDContext.Doctors.Add(newDoctor);
                    _docTalkDDContext.SaveChanges();
                    return new Response
                    { Status = "Success", Message = "SuccessFully Saved." };
                }
            }
            catch (Exception)
            {

                throw;
            }
            return new Response
            { Status = "Error", Message = "Invalid Data." };
        }

        [Route("api/doctor/getActiveDoctors")]
        [HttpGet]
        public List<Doctor> GetAvailableDoctors()
        {
            try
            {
                
                List<Doctor> Obj = _docTalkDDContext.Doctors.Where(x => x.IsActive == true && x.IsLocked == false).ToList();
                return Obj;
            }
            catch (Exception)
            {

                throw;
            }

        }

        [Route("api/doctor/lockDoctor")]
        [HttpPost]
        public void LockDoctor(int id)
        {
            try
            {
                
                Doctor doct = _docTalkDDContext.Doctors.FirstOrDefault(x => x.UserId == id);
                if (doct != null)
                {
                    doct.IsLocked = true;
                    _docTalkDDContext.SaveChanges();
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
        [Route("api/doctor/unLockDoctor")]
        [HttpPost]
        public void UnLockDoctor(int id)
        {
            try
            {
                
                Doctor doct = _docTalkDDContext.Doctors.FirstOrDefault(x => x.UserId == id);
                if (doct != null)
                {
                    doct.IsLocked = false;
                    _docTalkDDContext.SaveChanges();
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        [Route("api/doctor/inActive")]
        [HttpPost]
        public void InActiveDoctor(int id)
        {
            try
            {
                
                Doctor doct = _docTalkDDContext.Doctors.FirstOrDefault(x => x.UserId == id);
                if (doct != null)
                {
                    doct.IsActive = false;
                    _docTalkDDContext.SaveChanges();
                }

            }
            catch (Exception)
            {

                throw;
            }

        }



        public void MakeDoctorAvailable(int userId)
        {
            try
            {
                
                Doctor doct = _docTalkDDContext.Doctors.FirstOrDefault(x => x.UserId == userId);
                if (doct != null)
                {
                    doct.IsActive = true;
                    doct.IsLocked = false;
                    _docTalkDDContext.SaveChanges();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        [Route("api/Doctor/makeDoctorUnAvailable")]
        [HttpPost]
        public void MakeDoctorUnAvailable(int userId)
        {
            try
            {
                
                Doctor doct = _docTalkDDContext.Doctors.FirstOrDefault(x => x.UserId == userId);
                if (doct != null)
                {
                    doct.IsActive = true;
                    doct.IsLocked = false;
                    _docTalkDDContext.SaveChanges();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


        [Route("api/Doctor/MakeUserUnavailable")]
        [HttpPost]
        public Response MakeUserUnavailable(string userId)
        {
            try
            {
                
                Doctor doct = _docTalkDDContext.Doctors.FirstOrDefault(x => x.UserId == long.Parse(userId));
                if (doct != null)
                {
                    doct.IsActive = false;
                    doct.IsLocked = false;
                    _docTalkDDContext.SaveChanges();
                    return new Response { Status = "Success" };
                }
                else
                {
                    return new Response { Status = "Failure", Message = "Not able to make use inactive" };
                }


            }
            catch (Exception)
            {

                throw;
            }
        }
    }

}