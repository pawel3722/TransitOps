using System.ComponentModel.DataAnnotations;
using TransitOps.Api.Models;

namespace TransitOps.Api.DTOs;

public class CreateDisruptionDto : IValidatableObject
{
    [StringLength(20)]
    public string Line { get; set; } = string.Empty;

    [EnumDataType(typeof(DisruptionType))]
    public DisruptionType Type { get; set; }

    [Required, StringLength(1000, MinimumLength = 1)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 1440)]
    public int? DelayMinutes { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Description))
            yield return new ValidationResult("Description cannot be blank.", [nameof(Description)]);
        if (Type == DisruptionType.Delay && (DelayMinutes is null || DelayMinutes <= 0))
            yield return new ValidationResult("A delay must have a positive DelayMinutes value.", [nameof(DelayMinutes)]);
        if (Type != DisruptionType.Delay && DelayMinutes is not null)
            yield return new ValidationResult("DelayMinutes is only allowed for delays.", [nameof(DelayMinutes)]);
    }
}