using MaybeResults;
using snowcoreBlog.Backend.Stripe.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Stripe.Delegates;

public delegate Task<IMaybe<GoodsCheckoutResultDto>> CreateGoodsCheckoutDelegate(CreateGoodsCheckoutContext context, CancellationToken token = default);
