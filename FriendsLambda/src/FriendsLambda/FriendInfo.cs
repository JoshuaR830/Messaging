namespace FriendsLambda
{
    public class FriendInfo
    {
        public string UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }

        public FriendInfo(string userId, string firstName, string lastName)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}