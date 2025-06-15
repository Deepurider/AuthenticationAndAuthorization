namespace AuthenticationAndAuthorization.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RequiredClaimAttribute : Attribute
    {
        public string Claim { get; }
        public RequiredClaimAttribute(string claim)
        {
            Claim = claim;
        }
    }
}
