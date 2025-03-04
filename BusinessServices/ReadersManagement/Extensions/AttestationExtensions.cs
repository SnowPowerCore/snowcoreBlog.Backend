﻿using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions;

[Mapper]
public static partial class AttestationExtensions
{
    public static partial ValidateAndCreateAttestation ToValidate(this RequestAttestationOptionsDto requestAttestationOptions);
}