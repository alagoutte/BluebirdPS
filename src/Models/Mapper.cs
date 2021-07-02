using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Tweetinvi.Models.V2;
using BluebirdPS.Models.APIV2;
using BluebirdPS.Models.APIV2.Metrics.User;

namespace BluebirdPS.Models
{
    internal class Mapper
    {
        internal static IMapper GetMapper()
        {
            MapperConfiguration mapperConfig = new MapperConfiguration(cfg => {
                cfg.CreateMap<UserV2, User>()
                    .ForMember("CreatedAt", opt => opt.Ignore())
                    .ForMember("Entities", opt => opt.Ignore());
                cfg.CreateMap<UserPublicMetricsV2, Public>();
            });
            return mapperConfig.CreateMapper();
        }
        
    }

}
