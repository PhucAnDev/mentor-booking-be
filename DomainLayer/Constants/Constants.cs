using System;

namespace DomainLayer.Constants
{
    public class Constants
    {
        public static class Errors
        {
            public const string NOT_EXIST_ERROR = "not exist";
            public const string ALREADY_EXIST_ERROR = "already exist";
        }
        public static class Http
        {
            public const string API_VERSION = "v1";
            public const string CORS = "CORS";
            public const string JSON_CONTENT_TYPE = "application/json";
        }
        public static class Entities
        {
            public const string USER = "User ";
            public const string ROLE = "Role ";
            public const string BOOKING = "Booking ";
            public const string MENTOR = "Mentor ";
            public const string STUDENT = "Student ";
            public const string ENTERPRISE = "Enterprise ";
            public const string SESSION = "Session ";
            public const string MINUTES = "Minutes ";
            public const string SLOT = "Slot ";
        }
    }
}
