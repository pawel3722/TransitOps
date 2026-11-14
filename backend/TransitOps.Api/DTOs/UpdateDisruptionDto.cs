using System.ComponentModel.DataAnnotations;
using TransitOps.Api.Models;

namespace TransitOps.Api.DTOs;

public sealed class UpdateDisruptionDto : CreateDisruptionDto
{
    [EnumDataType(typeof(DisruptionStatus))]
    public DisruptionStatus Status { get; set; }
}