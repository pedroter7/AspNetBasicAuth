namespace PedroTer7.AspNetBasicAuth.Documentation
{
    internal record OperationFilterSettings(bool ShouldFilterAuthorizeAttributes, bool UseDefaultFilter,
        string[] PoliciesAuthorizeAttributeShouldContain, string[] RolesAuthorizeAttributeShouldContain);
}
