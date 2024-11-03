using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SpotifyAPI.Web;

namespace SpotifyWPF.Model
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AutoMapperConfiguration
    {
        public static MapperConfiguration Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PlaylistTrack<IPlayableItem>, Track>()
                    .ForMember(dest => dest.TrackName,
                        act => act.MapFrom((src, dest) => (src.Track as FullTrack)?.Name))
                    .ForMember(dest => dest.Artists, act => act.MapFrom((src, dest) =>
                    {
                        var fullTrack = src.Track as FullTrack;

                        var artists = string.Join(", ",
                            (fullTrack?.Artists ?? new List<SimpleArtist>()).Select(sa => sa.Name));

                        return $"{artists}";
                    }));
            });

            config.AssertConfigurationIsValid();

            return config;
        }
    }
}