namespace businessLogic.ModelViews
{
    public class ProfileViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string? Address { get; set; }

        public int ?Age { get; set; }

        public string Role { get; set; }
        public bool ?OldThan18 { get; set; }
    }
}
