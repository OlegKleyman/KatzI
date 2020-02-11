namespace Katz.Web.Website.Tests.Integration.Support
{
    public enum FeatureOrder
    {
        First = int.MinValue,
        Database,
        Web,
        Last = int.MaxValue
    }
}