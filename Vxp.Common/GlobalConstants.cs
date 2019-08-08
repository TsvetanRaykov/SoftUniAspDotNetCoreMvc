namespace Vxp.Common
{
    public static class GlobalConstants
    {
        public static class Roles
        {
            public const string AdministratorRoleName = "Administrator";
            public const string VendorRoleName = "Vendor";
            public const string DistributorRoleName = "Distributor";
            public const string CustomerRoleName = "Customer";
        }

        public static class SystemEmail
        {
            public const string SendFromEmail = "noreply@vertexdesign.bg";
            public const string SendFromName = "VERTEX DESIGN LTD.";
        }

        public static class ErrorMessages
        {
            public const string RequiredField = "The {0} is required.";
            public const string EmailInvalid = "The {0} field is not a valid e-mail address.";
            public const string PhoneInvalid = "The {0} field is not a valid phone.";
        }
    }
}
