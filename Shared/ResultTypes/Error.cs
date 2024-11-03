using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResultTypes;
public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");

    public static readonly Error ConditionNotMet = new("Error.ConditionNotMet", "The specified condition was not met.");
    public static Error NotFound(string code) => new(code, "The specified entity was not found.");
    public static Error Validation(string validationDetails) => new("Validation error.", validationDetails);
    public static Error Forbidden() => new("No access.", "You have no access to this resource.");


}
