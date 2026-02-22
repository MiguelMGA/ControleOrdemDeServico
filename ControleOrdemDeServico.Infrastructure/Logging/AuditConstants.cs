namespace OsService.Infrastructure.Logging
{
    public static class AuditEntities
    {
        public const string Customer = "Customer";
        public const string ServiceOrder = "ServiceOrder";
    }

    public static class AuditActions
    {
        public const string Created = "Created";
        public const string CreateFailedDuplicateDocument = "CreateFailed_DuplicateDocument";
        public const string CreateFailedDuplicateEmail = "CreateFailed_DuplicateEmail";
        public const string CreateFailedDuplicatePhone = "CreateFailed_DuplicatePhone";

        public const string Opened = "Opened";
        public const string OpenFailedCustomerNotFound = "OpenFailed_CustomerNotFound";
    }
}
