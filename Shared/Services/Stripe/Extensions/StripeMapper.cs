using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Core.Entities.Stripe;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Stripe.Extensions;

[Mapper]
public static partial class StripeMapper
{
    public static partial DonationDto ToDto(this DonationEntity entity);

    public static partial PaymentDto ToDto(this PaymentEntity entity);

    public static partial StripePaymentStatusDto ToStatusDto(this StripePaymentStatus status);
}
