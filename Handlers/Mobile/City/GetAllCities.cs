using AutoMapper;
using DataCommunication;
using DataCommunication.DataLibraries;
using DataCommunication.DTOs;
using Handlers.Helpers;
using MediatR;

namespace Handlers.Mobile.City
{
    public class GetAllCities
    {
        public class Query : IRequest<CommonResponseTemplateWithDataArrayList<CityDTO>>
        {
        }

        public class Handler : IRequestHandler<Query, CommonResponseTemplateWithDataArrayList<CityDTO>>
        {
            private readonly CityDataLibrary CityDL;
            private readonly IMapper _mapper;

            public Handler(CityDataLibrary cityDataLibrary, IMapper mapper)
            {
                CityDL = cityDataLibrary;
                _mapper = mapper;
            }

            public async Task<CommonResponseTemplateWithDataArrayList<CityDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var cities = await CityDL.GetAllCities();

                    if (cities.Count > 0)
                    {
                        var cityDtoList = cities.Select(c =>
                        {
                            return new CityDTO
                            {
                                Id = c.Id,
                                Name = c.Name,
                            };
                        }).ToList();


                        return new CommonResponseTemplateWithDataArrayList<CityDTO>
                        {
                            responseCode = ResponseCode.Success.ToString(),
                            statusCode = HttpStatusCodes.OK,
                            msg = "Cities Fetched Successfully!",
                            data = _mapper.Map<List<CityDTO>, List<CityDTO>>(cityDtoList.ToList())
                        };
                    }
                    else
                    {
                        return new CommonResponseTemplateWithDataArrayList<CityDTO>
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
                    return new CommonResponseTemplateWithDataArrayList<CityDTO>
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
