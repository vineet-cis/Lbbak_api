namespace DataCommunication.CommonComponents
{
    public class Enums
    {
        public enum Status
        {
            Active = 0,
            Inactive = 1
        }

        public enum EventCategory
        {
            Invitation = 1,
            Greeting = 2,
            Documentry = 3
        }

        public enum Privacy
        {
            Private = 0,
            Public = 1,
            Local = 2
        }

        public enum InvitationStatus
        {
            NaN = 0,
            Invited = 1,
            Accepted = 2,
            Declined = 3
        }
    }
}
