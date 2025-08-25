using AutoMapper;
using DataCommunication;
using DataCommunication.DataLibraries;
using Handlers.Helpers;
using MediatR;
using MongoDB.Driver;

namespace Handlers
{
    public class GetOfferTypes
    {
        public class Query : IRequest<CommonResponseTemplateWithDataArrayList<OfferCategoryDTO>>
        {
        }

        public class Handler : IRequestHandler<Query, CommonResponseTemplateWithDataArrayList<OfferCategoryDTO>>
        {
            private readonly OfferDataLibrary OfferDL;
            private readonly IMapper _mapper;

            public Handler(OfferDataLibrary offerDataLibrary, IMapper mapper)
            {
                OfferDL = offerDataLibrary;
                _mapper = mapper;
            }

            public async Task<CommonResponseTemplateWithDataArrayList<OfferCategoryDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var types = await OfferDL.GetAllCategories();

                    if (types.Count > 0)
                    {
                        var typesDtoList = types.Select(c =>
                        {
                            return new OfferCategoryDTO
                            {
                                Id = c.Id,
                                Name = c.Name,
                            };
                        }).ToList();


                        return new CommonResponseTemplateWithDataArrayList<OfferCategoryDTO>
                        {
                            responseCode = ResponseCode.Success.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "Types Fetched Successfully!",
                            data = _mapper.Map<List<OfferCategoryDTO>, List<OfferCategoryDTO>>(typesDtoList.ToList())
                        };
                    }
                    else
                    {
                        return new CommonResponseTemplateWithDataArrayList<OfferCategoryDTO>
                        {
                            responseCode = ResponseCode.Empty.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "No types Found!",
                            data = null
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new CommonResponseTemplateWithDataArrayList<OfferCategoryDTO>
                    {
                        responseCode = ResponseCode.InternalServerError.ToString(),
                        statusCode = HttpStatusCodes.InternalServerError,
                        msg = ex.Message.ToString(),
                        data = null
                    };
                }
            }
        }
    }
}
