namespace UserService.src.Constants
{
    public static class MessagesConstant
    {
        public const string Created = "The record has been successfully created.";
        public const string Updated = "The record has been successfully updated.";
        public const string Deleted = "The record has been successfully deleted.";


        public const string NoChangesUpdate = "No changes were made. The record is already up to date.";
        public const string NotFound = "The requested record was not found.";
        public const string AlreadyExists = "Registo já existe.";
        public const string InvalidData = "A record with the same value already exists.";
        public const string OperationFailed = "The operation could not be completed. Please try again or contact support if the issue persists.";

        public const string DatabaseErrorGeneric = "A database error occurred...";
        public const string UnexpectedError = "An unexpected error occurred...";
    }
}