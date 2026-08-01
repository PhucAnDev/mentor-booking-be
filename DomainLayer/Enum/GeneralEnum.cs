using System;

namespace DomainLayer.Enum
{
    public class GeneralEnum
    {
        public enum RoleStatusEnum
        {
            Active,
            Disable,
            Pending
        }

        public enum UserStatusEnum
        {
            Active,
            Suspended,
            Banned
        }

        public enum BookingStatusEnum
        {
            Pending,
            Accepted,
            Declined,
            Canceled
        }

        public enum CompanyStatusEnum
        {
            Active,
            Inactive
        }
    }
}
