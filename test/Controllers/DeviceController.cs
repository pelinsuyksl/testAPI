using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Http;
using test.Models;

namespace test.Controllers
{
    public class DeviceController : ApiController
    {
        private readonly AppDbContext context;
        public DeviceController()
        {
            context = new AppDbContext();
        }
        public class AddDeviceDto
        {
            public int UserId { get; set; }
            public string DeviceName { get; set; }
            public string DeviceModel { get; set; }
        }
        public class DeleteDeviceDto
        {
            public int UserId { get; set; }
            public int DevNum { get; set; }
        }
        public class ListDevicesDto
        {
           public int UserId { get; set; }
        }

        [HttpPost]
        [Route("api/device/add")]
        public IHttpActionResult AddDevice(AddDeviceDto request)
        {
            if (request.UserId <= 0)
                return Ok(new
                {
                    Message = "UserId is required!!",
                    Status = false
                });

            if (string.IsNullOrEmpty(request.DeviceName))
                return Ok(new
                {
                    Message = "Device name is required!!",
                    Status = false
                });

            var user = context.Users.FirstOrDefault(u=>u.Id==request.UserId);
            if (user == null)
                return Ok(new
                {
                    Message = "User not found with this user id!!",
                    Status = false
                });
            if (!user.IsActive)
                return Ok(new
                {
                    Message = "User account is inactive!!",
                    Status = false
                });
            user.UserDevCount += 1;
            var device = new Device {
                UserId = user.Id,           
                DeviceGuid = Guid.NewGuid(), 
                DeviceName = request.DeviceName,
                DeviceModel = string.IsNullOrEmpty(request.DeviceModel)? "Unknown": request.DeviceModel,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                DevNum= user.UserDevCount
            };

            context.Devices.Add(device);
            context.SaveChanges();

            return Ok(
                new
                {
                    Message = "Device added successfully!",
                    DeviceNumber = device.DevNum,
                    Status =true
                }
            );
        }

        [HttpPost]
        [Route("api/device/delete")]
        public IHttpActionResult DeleteDevice(DeleteDeviceDto request)
        {
            if (request.UserId <= 0)
                return Ok(new
                {
                    Message = "Valid user id must be provided!",
                    Status = false
                });
            if (request.DevNum <=0)
                return Ok(new
                {
                    Message = "Valid Device Number (DevNum) must be provided!",
                    Status = false
                });
            var user =context.Users.FirstOrDefault(u=>u.Id == request.UserId);
            if (user == null)
                return Ok(new
                {
                    Message = "User not found with this Email!",
                    Status = false
                });
            if (!user.IsActive)
                return Ok(new
                {
                    Message = "User account is inactive!",
                    Status = false
                });

            var device = context.Devices.FirstOrDefault(d => d.DevNum == request.DevNum && d.UserId == request.UserId);
            if (device == null)
                return Ok(new
                {
                    Message = "DEVICE NOT FOUND! Valid Device number is required!",
                    Status = false
                });

            device.IsActive = false;
            device.UpdatedAt = DateTime.Now;
            context.SaveChanges();
            return Ok(new
                {
                    Message = "Device removed successfully!",
                    RemovedDevNum = request.DevNum,
                    Status=true
                });
        }
        [HttpPost]
        [Route("api/device/list")]
        public IHttpActionResult ListDevices(ListDevicesDto request)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == request.UserId);
            if (user == null)
                return Ok(new
                {
                    Message = "User not found with this User Id!",
                    Status = false
                });
            if (!user.IsActive)
                return Ok(new
                {
                    Message = "User account is inactive!",
                    Status = false
                });

            var userDevices = context.Devices.Where(d => d.UserId == user.Id && d.IsActive == true).Select(x => new
            {
                x.Id,
                x.DevNum,
                x.DeviceGuid,
                x.DeviceName,
                x.DeviceModel,
                x.CreatedAt
            }).ToList();
            if (!userDevices.Any())
                return Ok(new
                {
                    Message = "The user does not have any registered devices!",
                    Status = false
                });
          
            return Ok(new
            {   userDevices,
                Status = true
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }


    }

}