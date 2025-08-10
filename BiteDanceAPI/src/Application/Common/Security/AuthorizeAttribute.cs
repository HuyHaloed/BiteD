namespace BiteDanceAPI.Application.Common.Security;

/// <summary>
/// Specifies the class this attribute is applied to requires authorization.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class AuthorizeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeAttribute"/> class.
    /// </summary>
    public AuthorizeAttribute() { }
    public bool RequireSupplier { get; set; } = false;
    public bool RequireAdmin { get; set; } = false;
    public bool RequireSuperAdmin { get; set; } = false;
    public bool DenySupplier { get; set; } = false;

    /// <summary>
    /// Gets or sets the policy name that determines access to the resource.
    /// </summary>
    public string Policy { get; set; } = string.Empty;
}
