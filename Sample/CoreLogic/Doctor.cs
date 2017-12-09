namespace CoreLogic
{
    public static class Doctor
    {
        public static Type GetDoctorType(int patientAge)
        {
            var doctorType = Type.Pediatrician;
            if (patientAge > 14) doctorType = Type.GeneralPhysician;
            return doctorType;
        }

        public enum Type
        {
            Pediatrician,
            GeneralPhysician,
            Chiropractor
        }
    }
}
