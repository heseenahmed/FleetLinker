using Humanizer;

namespace FleetLinker.Application.Common.Localization
{
    /// <summary>
    /// Contains message keys for localization. Use with IStringLocalizer.
    /// </summary>
    public static class LocalizationMessages
    {
        // General
        public const string Success = "Success";
        public const string Failed = "Failed";
        public const string Error = "Error";


        //Email
        public const string InvalidEmailAddress = "InvalidEmailAddress";

        // Validation
        public const string ValidationError = "ValidationError";
        public const string ModelValidationFailed = "ModelValidationFailed";
        public const string InvalidId = "InvalidId";
        public const string BodyRequired = "BodyRequired";
        public const string InvalidArgument = "InvalidArgument";
        
        // Authentication
        public const string Unauthorized = "Unauthorized";
        public const string UserNotAuthenticated = "UserNotAuthenticated";
        public const string InvalidCredentials = "InvalidCredentials";
        public const string InvalidAccessToken = "InvalidAccessToken";
        public const string InvalidOrExpiredToken = "InvalidOrExpiredToken";
        public const string InvalidOrExpiredRefreshToken = "InvalidOrExpiredRefreshToken";
        public const string InvalidTokenAlgorithm = "InvalidTokenAlgorithm";

        
        // Users
        public const string UserNotFound = "UserNotFound";
        public const string UserNotFoundOrInactive = "UserNotFoundOrInactive";
        public const string UserNotFoundOrDeleted = "UserNotFoundOrDeleted";
        public const string TargetUserNotFound = "TargetUserNotFound";
        public const string PerformerNotFound = "PerformerNotFound";
        public const string UserRegisteredSuccessfully = "UserRegisteredSuccessfully";
        public const string UserUpdatedSuccessfully = "UserUpdatedSuccessfully";
        public const string UserDeletedSuccessfully = "UserDeletedSuccessfully";
        public const string UserActivatedSuccessfully = "UserActivatedSuccessfully";
        public const string UserDeactivated = "UserDeactivated";
        public const string UserInfoRetrieved = "UserInfoRetrieved";
        public const string UsersInfoRetrieved = "UsersInfoRetrieved";
        public const string UserRolesUpdated = "UserRolesUpdated";
        public const string InvalidUserId = "InvalidUserId";
        public const string UserIdRequired = "UserIdRequired";
        public const string UserDataMissingOrInconsistent = "UserDataMissingOrInconsistent";
        public const string UserUpdateDataRequired = "UserUpdateDataRequired";
        public const string FailedToUpdateUser = "FailedToUpdateUser";
        public const string ErrorRetrievingUserInfo = "ErrorRetrievingUserIfo";
        public const string EmailAlreadyInUse = "EmailAlreadyInUseByAnotherUser";
        public const string MobileAlreadyInUse = "MobileNumberAlreadyInUseByAnotherUser";
        public const string FailedToResetPassword = "FailedToResetPassword";
        public const string FullNameRequired = "FullNameRequired";
        public const string EmailRequired = "EmailRequired";
        public const string MobileRequired = "MobileRequired";
        public const string DuplicateEmail = "DuplicateEmail";
        public const string FailedToCreateUser = "FailedToCreateUser";
        public const string UserAlreadyDeleted = "UserAlreadyDeleted";
        public const string FailedToDeleteUser = "FailedToDeleteUser";
        public const string SomeRolesNotExist = "SomeRolesDoNotExist";
        
        // Registration
        public const string RegistrationPayloadRequired = "RegistrationPayloadRequired";
        public const string RegistrationFailedInternally = "RegistrationFailedInternally";
        
        // Login
        public const string LoginPayloadRequired = "LoginPayloadRequired";
        public const string UsernameAndPasswordRequired = "UsernameAndPasswordRequired";
        public const string InvalidPassword = "InvalidPassword";
        public const string LoginSuccessful = "LoginSuccessful";
        public const string LogoutSuccessful = "LogoutSuccessful";
        public const string LogoutSuccessfulAllDevices = "LogoutSuccessfulAllDevices";
        
        // Password
        public const string OldAndNewPasswordsRequired = "OldAndNewPasswordsRequired";
        public const string MobileAndPasswordRequired = "MobileAndPasswordRequired";
        public const string PasswordChangedSuccessfully = "PasswordChangedSuccessfully";
        public const string TokenRequired = "TokenRequired";
        public const string TokenRefreshedSuccessfully = "TokenRefreshedSuccessfully";
        
        // Roles
        public const string RolesRetrievedSuccessfully = "RolesRetrievedSuccessfully";
        public const string RoleNameRequired = "RoleNameRequired";
        public const string RoleNameCannotBeNull = "RoleNameCannotBeNull";
        public const string RoleNotFound = "RoleNotFound";
        public const string RoleAddedSuccessfully = "RoleAddedSuccessfully";
        public const string RoleDeletedSuccessfully = "RoleDeletedSuccessfully";
        public const string ErrorAddingRole = "ErrorAddingRole";
        public const string ErrorUpdateRole = "ErrorUpdateRole";
        public const string ErrorDeleteRole = "ErrorDeleteRole";
        public const string RoleIdsCannotBeNull = "RoleIdsCannotBeNull";
        public const string FailedToAssignRoles = "FailedToAssignRoles";
        
        // Errors
        public const string NotFound = "NotFound";
        public const string OperationFailed = "OperationFailed";
        public const string DatabaseError = "DatabaseError";
        public const string DatabaseErrorOccurred = "DatabaseErrorOccurred";
        public const string SqlError = "SqlError";
        public const string ApplicationError = "ApplicationError";
        public const string ServerError = "ServerError";
        public const string UnexpectedErrorOccurred = "UnexpectedErrorOccurred";
        
        // Identity Errors
        public const string IdentityDefaultError = "IdentityDefaultError";
        public const string IdentityConcurrencyFailure = "IdentityConcurrencyFailure";
        public const string IdentityPasswordTooShort = "IdentityPasswordTooShort";
        public const string IdentityPasswordRequiresNonAlphanumeric = "IdentityPasswordRequiresNonAlphanumeric";
        public const string IdentityPasswordRequiresDigit = "IdentityPasswordRequiresDigit";
        public const string IdentityPasswordRequiresLower = "IdentityPasswordRequiresLower";
        public const string IdentityPasswordRequiresUpper = "IdentityPasswordRequiresUpper";
        public const string IdentityPasswordRequiresUniqueChars = "IdentityPasswordRequiresUniqueChars";
        public const string IdentityInvalidUserName = "IdentityInvalidUserName";
        public const string IdentityInvalidEmail = "IdentityInvalidEmail";
        public const string IdentityDuplicateUserName = "IdentityDuplicateUserName";
        public const string IdentityDuplicateEmail = "IdentityDuplicateEmail";
        public const string IdentityPasswordMismatch = "IdentityPasswordMismatch";

        // Equipment
        public const string EquipmentCreatedSuccessfully = "EquipmentCreatedSuccessfully";
        public const string EquipmentUpdatedSuccessfully = "EquipmentUpdatedSuccessfully";
        public const string EquipmentDeletedSuccessfully = "EquipmentDeletedSuccessfully";
        public const string EquipmentNotFound = "EquipmentNotFound";
        public const string EquipmentUnauthorized = "EquipmentUnauthorized";
        public const string BrandRequired = "BrandRequired";
        public const string YearRequired = "YearRequired";
        public const string ChassisNumberRequired = "ChassisNumberRequired";
        public const string ModelRequired = "ModelRequired";
        public const string EquipmentRetrievedSuccessfully = "EquipmentRetrievedSuccessfully";
        public const string EquipmentsRetrievedSuccessfully = "EquipmentsRetrievedSuccessfully";
        public const string EquipmentsAddedSuccessfully = "EquipmentsAddedSuccessfully";
        
        // Spare Parts
        public const string SparePartCreatedSuccessfully = "SparePartCreatedSuccessfully";
        public const string SparePartUpdatedSuccessfully = "SparePartUpdatedSuccessfully";
        public const string SparePartDeletedSuccessfully = "SparePartDeletedSuccessfully";
        public const string SparePartNotFound = "SparePartNotFound";
        public const string SparePartRetrievedSuccessfully = "SparePartRetrievedSuccessfully";
        public const string SparePartsRetrievedSuccessfully = "SparePartsRetrievedSuccessfully";

        // Roles
        public const string RoleAdmin = "RoleAdmin";
        public const string RoleVisitor = "RoleVisitor";
        public const string RoleEquipmentOwner = "RoleEquipmentOwner";
        public const string RoleSupplier = "RoleSupplier";
        public const string RoleMaintenanceWorkshopOwner = "RoleMaintenanceWorkshopOwner";
        public const string RoleDriver = "RoleDriver";
        public const string RoleMechanical = "RoleMechanical";
        public const string RoleClient = "RoleClient";
        public const string RoleWorkshop = "RoleWorkshop";

        public static string GetRoleKey(string roleName)
        {
            return roleName.Trim().ToLower() switch
            {
                "admin" => RoleAdmin,
                "visitor" => RoleVisitor,
                "equipment owner" => RoleEquipmentOwner,
                "supplier" => RoleSupplier,
                "maintenance workshop owner" => RoleMaintenanceWorkshopOwner,
                "driver" => RoleDriver,
                "mechanical" => RoleMechanical,
                "client" => RoleClient,
                "workshop" => RoleWorkshop,
                _ => roleName // Fallback to original if not found
            };
        }
    }
}
