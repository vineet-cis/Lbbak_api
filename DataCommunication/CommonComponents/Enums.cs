namespace DataCommunication.CommonComponents
{
    public class Enums
    {
        public enum Status
        {
            Active = 0,
            Inactive = 1
        }

        public enum CardType
        {
            Both = 0,
            Invitation = 1,
            Greeting = 2
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

        public enum PromotionType { Discount = 1, Promotion = 2, Advertisement = 3 }
        public enum DiscountValueType { Percentage = 1, FixedAmount = 2 }
        public enum PromotionScope { Local = 1, Public = 2 }
        public enum PromotionStatus { Draft = 0, PendingPayment = 1, Scheduled = 2, Live = 3, Ended = 4, StoppedByAdmin = 5 }
    }
}
