using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace test.Models
{
    public class Device
    {
        public int Id { get; set; }
        public int UserId {  get; set; }
        public Guid DeviceGuid { get; set; }
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public int DevNum { get; set; }
    }

}