using AutoMapper;
using SpotifyAPI.Web.Models;

namespace SpotifyWPF.Model
{
    public class AutoMapperConfiguration
    {
        public static MapperConfiguration Configure()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PlaylistTrack, Track>();            
            });

            config.AssertConfigurationIsValid();

            return config;
        }
    }
}
