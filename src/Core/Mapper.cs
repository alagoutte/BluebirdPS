using System;
using AutoMapper;
using BluebirdPS.Core.MapperConverters;
using BluebirdPS.Models;
using BluebirdPS.Models.APIV2;
using BluebirdPS.Models.APIV2.Metrics.User;
using Tweetinvi.Events;
using Tweetinvi.Models.V2;

namespace BluebirdPS.Core
{
    internal class Mapper
    {
        private static IMapper mapper;

        private static IMapper Create()
        {
            MapperConfiguration mapperConfig = new(cfg =>
            {
                cfg.CreateMap<DateTimeOffset, DateTime>().ConvertUsing(new DateTimeConverter());
                cfg.CreateMap<UserV2, User>()
                    .ForMember("Entities", opt => opt.Ignore());
                cfg.CreateMap<UserPublicMetricsV2, Public>();
                cfg.CreateMap<AfterExecutingQueryEventArgs, ResponseData>().ConvertUsing(new ResponseDataConverter());
            });
            return mapperConfig.CreateMapper();
        }

        public static IMapper GetOrCreateInstance() => mapper ??= Create();
    }
}
