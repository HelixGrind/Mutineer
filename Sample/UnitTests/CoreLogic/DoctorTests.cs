using CoreLogic;
using Xunit;

namespace UnitTests.CoreLogic
{
    public sealed class DoctorTests
    {
        [Theory]
        [InlineData(14, Doctor.Type.Pediatrician)]
        [InlineData(15, Doctor.Type.GeneralPhysician)]
        [InlineData(0, Doctor.Type.Pediatrician)]
        [InlineData(13, Doctor.Type.Pediatrician)]
        public void GetDoctorType(int patientAge, Doctor.Type expectedType)
        {
            Assert.Equal(expectedType, Doctor.GetDoctorType(patientAge));            
        }
    }
}
