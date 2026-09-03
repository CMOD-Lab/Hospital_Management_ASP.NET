using Xunit;
using CareTrack.Domain.Enums;

namespace CareTrack.Domain.Enums.Tests
{
    public class EnumsTests
    {
        [Fact]
        public void UserType_Patient_HasValueOne()
        {
            Assert.Equal(1, (int)UserType.Patient);
        }

        [Fact]
        public void UserType_Doctor_HasValueTwo()
        {
            Assert.Equal(2, (int)UserType.Doctor);
        }

        [Fact]
        public void UserType_Admin_HasValueThree()
        {
            Assert.Equal(3, (int)UserType.Admin);
        }

        [Fact]
        public void AppointmentStatus_Approved_HasValueOne()
        {
            Assert.Equal(1, (int)AppointmentStatus.Approved);
        }

        [Fact]
        public void AppointmentStatus_Pending_HasValueTwo()
        {
            Assert.Equal(2, (int)AppointmentStatus.Pending);
        }

        [Fact]
        public void AppointmentStatus_Completed_HasValueThree()
        {
            Assert.Equal(3, (int)AppointmentStatus.Completed);
        }

        [Fact]
        public void AppointmentStatus_Rejected_HasValueFour()
        {
            Assert.Equal(4, (int)AppointmentStatus.Rejected);
        }

        [Fact]
        public void DoctorStatus_Left_HasValueZero()
        {
            Assert.Equal(0, (int)DoctorStatus.Left);
        }

        [Fact]
        public void DoctorStatus_Present_HasValueOne()
        {
            Assert.Equal(1, (int)DoctorStatus.Present);
        }

        [Fact]
        public void NotificationStatus_Seen_HasValueOne()
        {
            Assert.Equal(1, (int)NotificationStatus.Seen);
        }

        [Fact]
        public void NotificationStatus_Unseen_HasValueTwo()
        {
            Assert.Equal(2, (int)NotificationStatus.Unseen);
        }

        [Fact]
        public void FeedbackStatus_Given_HasValueOne()
        {
            Assert.Equal(1, (int)FeedbackStatus.Given);
        }

        [Fact]
        public void FeedbackStatus_Pending_HasValueTwo()
        {
            Assert.Equal(2, (int)FeedbackStatus.Pending);
        }

        [Fact]
        public void UserType_CanCastFromInt()
        {
            var userType = (UserType)1;
            Assert.Equal(UserType.Patient, userType);
        }

        [Fact]
        public void AppointmentStatus_CanCastFromInt()
        {
            var status = (AppointmentStatus)3;
            Assert.Equal(AppointmentStatus.Completed, status);
        }

        [Theory]
        [InlineData(1, UserType.Patient)]
        [InlineData(2, UserType.Doctor)]
        [InlineData(3, UserType.Admin)]
        public void UserType_AllValues_CorrectMapping(int value, UserType expected)
        {
            Assert.Equal(expected, (UserType)value);
        }

        [Theory]
        [InlineData(1, AppointmentStatus.Approved)]
        [InlineData(2, AppointmentStatus.Pending)]
        [InlineData(3, AppointmentStatus.Completed)]
        [InlineData(4, AppointmentStatus.Rejected)]
        public void AppointmentStatus_AllValues_CorrectMapping(int value, AppointmentStatus expected)
        {
            Assert.Equal(expected, (AppointmentStatus)value);
        }
    }
}
