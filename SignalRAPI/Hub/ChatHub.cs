using Microsoft.AspNet.SignalR;
using SignalRAPI.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRAPI
{
    public class ChatHub : Hub
    {
        private DocTalkDBContext _docTalkDBContext;
        private static List<Doctor> availableUsers = new List<Doctor>();
        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public ChatHub()
        {
            _docTalkDBContext = new DocTalkDBContext();
        }

        // Method used for doctors when he successfully logged in, store its connection id for usage 
        public void Connect(int doctorUserId)
        {
            try
            {
                DoctorEntity doctorEntity = _docTalkDBContext.DoctorEntities.FirstOrDefault(x => x.UserId == doctorUserId);
                if (doctorEntity != null)
                {
                    doctorEntity.DoctorConnectionId = Context.ConnectionId;
                }
                else
                {
                    DoctorEntity doctorEntities = new DoctorEntity()
                    {
                        DoctorConnectionId = Context.ConnectionId,
                        UserId = doctorUserId
                    };
                    _docTalkDBContext.DoctorEntities.Add(doctorEntities);
                }
                _docTalkDBContext.SaveChanges();
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }
        public async Task AddToGroup(string pContextId)
        {
            try
            {
                //availableUsers = GetAvailableDoctors();
                //if (availableUsers.Count > 0)
                //{
                    Guid guid = Guid.NewGuid();
                    string guid2 = Guid.NewGuid().ToString();
                    string gName = guid.ToString();
                    //LockDoctor(availableUsers[0]);
                    // for demo hardcoding
                    gName = "lobby";
                   // List<DoctorEntity> assignUserContext = availableUsers[0].DoctorEntities.ToList();
                    //if (assignUserContext != null)
                    //{
                    //    AddRoom(gName, assignUserContext[0].DoctorConnectionId, Context.ConnectionId);
                        await Groups.Add(Context.ConnectionId, gName);
                await Groups.Add(pContextId, gName);
                //await Groups.Add(assignUserContext[0].DoctorConnectionId, gName);
                // await Groups.Add(assignUserContext[0].ContextId, gName);
                Message message = new Message()
                        {
                            groupName = gName,
                            message = "joined to group"
                        };
                        await Clients.Group(gName).addChatMessage(message, "GroupCreated");
                //    }
                //}
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task JoinRoom()
        {
            try
            {
                string roomName = Guid.NewGuid().ToString();
                Doctor avilableDoctor = GetAvailableDoctor();
                if (avilableDoctor != null)
                {
                    LockDoctor(avilableDoctor);
                    DoctorEntity assignUserContext = avilableDoctor.DoctorEntities.FirstOrDefault();
                    if (assignUserContext != null)
                    {
                        AddRoom(roomName, assignUserContext.DoctorConnectionId, Context.ConnectionId);
                        await Groups.Add(Context.ConnectionId, roomName);
                        await Groups.Add(assignUserContext.DoctorConnectionId, roomName);
                        Message message = new Message()
                        {
                            groupName = roomName,
                            message = "joined to Room"
                        };
                        await Clients.Group(roomName).addChatMessage(message, "GroupCreated");
                    }
                }
                else
                {
                    Message message = new Message()
                    {
                        groupName = roomName,
                        message = "No Doctor is available to create room"
                    };
                    await Clients.Group(roomName).SendAsync("No Doctor is available to Join room");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        // PatientLeaveRoom
        // DoctorLeaveRoom
        public async Task LeaveRoom(string roomName)
        {
            try
            {
                Room details = GetRoom(roomName);
                DeleteRoom(roomName);
                ReleaseDoctor(details.DoctorConnectionId);
                if (details != null)
                {
                    await Groups.Remove(details.PatientConnectionId, roomName);

                    await Groups.Remove(details.DoctorConnectionId, roomName);

                    await Clients.Group(roomName).SendAsync("Send", $"{details.PatientConnectionId} has left the room {roomName}.");



                    await Clients.Group(roomName).SendAsync("Send", $"{details.DoctorConnectionId} has left the room {roomName}.");
                }
                else
                {
                    await Groups.Remove(Context.ConnectionId, roomName);

                    await Clients.Group(roomName).SendAsync("Send", $"{Context.ConnectionId} has left the room {roomName}.");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Task SendMessageToRoom(string roomName, Message message)
        {
            return Clients.Group(roomName).addChatMessage(message, "MessageReceived");
        }

        public void AddRoom(string roomname, string doctorConnectionId, string userConnectionId)
        {
            try
            {
                Room room = new Room()
                {
                    Name = roomname,
                    PatientConnectionId = userConnectionId,
                    DoctorConnectionId = doctorConnectionId,
                    // add doctor Id or use connectionid
                };
                _docTalkDBContext.Rooms.Add(room);
                _docTalkDBContext.SaveChanges();
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public Room GetRoom(string roomname)
        {
            try
            {
                Room room = _docTalkDBContext.Rooms.FirstOrDefault(x => x.Name == roomname);
                return room;
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public void DeleteRoom(string roomName)
        {
            try
            {
                Room room = _docTalkDBContext.Rooms.FirstOrDefault(x => x.Name == roomName);
                if (room != null)
                {
                    _docTalkDBContext.Rooms.Remove(room);
                    _docTalkDBContext.SaveChanges();

                }

            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        public List<Doctor> GetAvailableDoctors()
        {
            try
            {
                // get first doctor and lock
                // 

                availableUsers = _docTalkDBContext.Doctors.Where(x => x.IsActive == true && x.IsLocked == false).ToList();
                return availableUsers;

            }
            catch (Exception)
            {

                throw;
            }

        }

        public Doctor GetAvailableDoctor()
        {
            try
            {
                Doctor availableDoctor = _docTalkDBContext.Doctors.FirstOrDefault(x => x.IsActive == true && x.IsLocked == false);
                lock (availableDoctor)
                {
                    return availableDoctor;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void LockDoctor(Doctor doctor)
        {
            try
            {
                doctor.IsLocked = true;
                _docTalkDBContext.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void ReleaseDoctor(string contextId)
        {
            try
            {
                DoctorEntity doctorContext = _docTalkDBContext.DoctorEntities.FirstOrDefault(x => x.DoctorConnectionId == contextId);
                if (doctorContext != null)
                {
                    doctorContext.DoctorConnectionId = "";
                    Doctor doctor = doctorContext.Doctor;
                    doctor.IsLocked = false;
                    _docTalkDBContext.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}