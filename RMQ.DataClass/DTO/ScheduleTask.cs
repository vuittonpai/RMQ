using System;
using System.ComponentModel.DataAnnotations;

namespace RMQ.DataClass.DTO
{
    [Serializable]
    //[DataContract]
    public class ScheduleTask
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public ScheduleStatus Status { get; set; }
        [Required]
        public ScheduleType ScheduleType { get; set; }
        [Required]
        public string Name { get; set; }
        public string ScheduleData { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime SubmittedDate { get; set; }
    }

    [Serializable]
    public enum ScheduleStatus
    {
        Initialized = 1,
        Started = 2,
        Completed = 3,
        Failed = 4,
        ReStarted = 5,
        Canceled = 6
    }
    [Serializable]
    public enum ScheduleType
    {
        PushMessage = 1
    }
}
