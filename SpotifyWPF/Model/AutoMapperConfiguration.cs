using AutoMapper;
using SpotifyAPI.Web;

namespace SpotifyWPF.Model
{
    public class AutoMapperConfiguration
    {
        public static MapperConfiguration Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PlaylistTrack<IPlayableItem>, Track>()
                    .ForMember(dest => dest.TrackName, act => act.MapFrom(src => src.Track.ToString()));
            });

            config.AssertConfigurationIsValid();

            return config;
        }
    }
}
