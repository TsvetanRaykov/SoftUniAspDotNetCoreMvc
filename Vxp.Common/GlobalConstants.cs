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

        public static class Email
        {
            public const string SystemEmailSendFromEmail = "noreply@vertexdesign.bg";
            public const string SystemEmailSendFromName = "VERTEX DESIGN LTD.";
        }

        public static class ErrorMessages
        {
            public const string RequiredField = "{0} is required.";
            public const string EmailInvalid = "The {0} field is not a valid e-mail address.";
        }
    }
}
