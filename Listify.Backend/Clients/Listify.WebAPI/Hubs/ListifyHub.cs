﻿using Listify.Lib.VMs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using IdentityModel.Client;
using System.Net.Http;
using Listify.Paths;
using System.Linq;
using Listify.DAL;
using Listify.Lib.Requests;
using Listify.Domain.CodeFirst;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Listify.BLL.Polls;
using Listify.BLL.Events.Args;
using Listify.Lib.Responses;
using Listify.Domain.Lib.Enums;
using System.Text;
using Listify.Services;
using Listify.WebAPI.Models;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Listify.WebAPI.Hubs
{
    public class ListifyHub : Hub
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IHubContext<ListifyHub> _listifyHub;
        protected readonly IListifyDAL _dal;
        protected readonly IListifyService _service;
        protected readonly IMapper _mapper;

        private static IPingPoll _pingPoll;

        public ListifyHub(
            ApplicationDbContext context,
            IHubContext<ListifyHub> listifyHub,
            IListifyDAL dal,
            IListifyService service,
            IPingPoll pingPoll,
            IMapper mapper)
        {
            _context = context;
            _listifyHub = listifyHub;
            _dal = dal;
            _service = service;
            _mapper = mapper;

            if (_pingPoll == null)
            {
                _pingPoll = pingPoll;
                _pingPoll.PollingEvent += async (s, e) => await OnPingPollEvent(s, e);
            }
        }

        protected virtual async Task OnPingPollEvent(object sender, PingPollEventArgs args)
        {
            foreach (var item in args.ConnectionsPinged)
            {
                await _listifyHub.Clients.Client(item.ConnectionId).SendAsync("PingRequest", "Ping");
            }

            //foreach (var applicationUserRoomConnection in args.PingPoll.ApplicationUserRoomConnectionsRemoved)
            //{
            //    await ForceServerDisconnectAsync(applicationUserRoomConnection.ConnectionId);
            //}
        }

        //public async Task RequestApplicationUserInformation()
        //{
        //    try
        //    {
        //        var userId = await GetUserIdAsync();
        //        var applicationUser = await _dal.ReadApplicationUserAsync(userId);

        //        await Clients.Caller.SendAsync("ReceiveApplicationUser", applicationUser);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
        //public async Task UpdateApplicationUserInformation(ApplicationUserUpdateRequest request)
        //{
        //    try
        //    {
        //        var userId = await GetUserIdAsync();
        //        if (userId != Guid.Empty)
        //        {
        //            var applicationUser = await _dal.UpdateApplicationUserAsync(request, userId);
        //            await Clients.Caller.SendAsync("ReceiveApplicationUserInformation", applicationUser);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
        public async Task RequestApplicationUserUpdatedPlaylistsCount()
        {
            try
            {
                var userId = await GetUserIdAsync();
                var applicationUser = await _dal.ReadApplicationUserAsync(userId);

                if (applicationUser != null)
                {
                    await Clients.Caller.SendAsync("ReceiveUpdatedPlaylistsCount", applicationUser.PlaylistSongCount);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestValidatedText(ContentAvailabilityRequest request)
        {
            var userId = await GetUserIdAsync();

            ContentAvailability contentAvailability = null;

            switch (request.ValidatedTextType)
            {
                case ValidatedTextType.Username:
                    contentAvailability = new ContentAvailability
                    {
                        ValidatedTextType = request.ValidatedTextType,
                        IsAvailable = await _service.IsContentValid(request.Content) &&
                            await _dal.IsUsernameAvailableAsync(request.Content, userId)
                    };
                    break;
                case ValidatedTextType.RoomCode:
                    contentAvailability = new ContentAvailability
                    {
                        ValidatedTextType = request.ValidatedTextType,
                        IsAvailable = await _service.IsContentValid(request.Content) &&
                            await _dal.IsRoomCodeAvailableAsync(request.Content, userId)
                    };
                    break;
                case ValidatedTextType.RoomTitle:
                    contentAvailability = new ContentAvailability
                    {
                        ValidatedTextType = request.ValidatedTextType,
                        IsAvailable = await _service.IsContentValid(request.Content)
                    };
                    break;
                case ValidatedTextType.ProfileDescription:
                    contentAvailability = new ContentAvailability
                    {
                        ValidatedTextType = request.ValidatedTextType,
                        IsAvailable = await _service.IsContentValid(request.Content)
                    };
                    break;
            }
            if (contentAvailability != null)
            {
                await Clients.Caller.SendAsync("ReceiveValidatedText", contentAvailability);
            }
        }

        public async Task RequestRooms()
        {
            try
            {
                var rooms = await _dal.ReadRoomsAsync();
                await Clients.Caller.SendAsync("ReceiveRooms", rooms);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestRoomsFollowed()
        {
            try
            {
                var userId = await GetUserIdAsync();
                var rooms = await _dal.ReadRoomsFollowsAsync(userId);
                await Clients.Caller.SendAsync("ReceiveRoomsFollowed", rooms);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestRoomsFollowedProfile(Guid applicationUserId)
        {
            try
            {
                var rooms = await _dal.ReadRoomsFollowsAsync(applicationUserId);
                await Clients.Caller.SendAsync("ReceiveRoomsFollowed", rooms);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestRoom(string roomCode)
        {
            try
            {
                RoomVM room;
                if (!string.IsNullOrWhiteSpace(roomCode))
                {
                    room = await _dal.ReadRoomAsync(roomCode);
                    //room.RoomKey = string.Empty;
                }
                else
                {
                    // this is the default room
                    var userId = await GetUserIdAsync();
                    var user = await _dal.ReadApplicationUserAsync(userId);
                    room = await _dal.ReadRoomAsync(user.Room.Id);
                    
                    //var decodedRoomKey = Encoding.UTF8.GetString(Convert.FromBase64String(room.RoomKey));
                    //room.RoomKey = decodedRoomKey;
                }

                if (room != null)
                {
                    room.RoomKey = string.Empty;
                }
                await Clients.Caller.SendAsync("ReceiveRoom", room);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        // for the owner/current user
        public async Task RequestPlaylists()
        {
            try
            {
                var userId = await GetUserIdAsync();

                if (userId != Guid.Empty)
                {
                    var playlists = await _dal.ReadPlaylistsAsync(userId);
                    await Clients.Caller.SendAsync("ReceivePlaylists", playlists);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestPlaylist(string id)
        {
            try
            {
                if (Guid.TryParse(id, out var guid))
                {
                    var userId = await GetUserIdAsync();
                    if (userId != Guid.Empty)
                    {
                        var playlist = await _dal.ReadPlaylistAsync(guid, userId);

                        await Clients.Caller.SendAsync("ReceivePlaylist", playlist);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task CreatePlaylist(PlaylistCreateRequest request)
        {
            try
            {
                var userId = await GetUserIdAsync();
                if (userId != Guid.Empty)
                {
                    // Create or update the playlist
                    PlaylistVM playlist = request.Id == Guid.Empty
                    ? await _dal.CreatePlaylistAsync(request, userId)
                    : await _dal.UpdatePlaylistAsync(request, userId);

                    await Clients.Caller.SendAsync("ReceivePlaylist", playlist);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task DeletePlaylist(string id)
        {
            try
            {
                if (Guid.TryParse(id, out var guid))
                {
                    var userId = await GetUserIdAsync();
                    if (userId != Guid.Empty)
                    {
                        if (await _dal.DeletePlaylistAsync(guid, userId))
                        {
                            await Clients.Caller.SendAsync("ReceivePlaylists");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestPlaylistsCommunity()
        {
            try
            {
                var playlistsCommunity = await _dal.ReadPlaylistsCommunityAsync();
                await Clients.Caller.SendAsync("ReceivePlaylistsCommunity", playlistsCommunity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task RequestGenres()
        {
            try
            {
                var genres = await _dal.ReadGenresAsync();
                await Clients.Caller.SendAsync("ReceiveGenres", genres);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestGenresRoom()
        {
            try
            {
                var genres = await _dal.ReadGenresAsync();
                await Clients.Caller.SendAsync("ReceiveGenresRoom", genres);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task RequestSongsPlaylist(string id)
        {
            try
            {
                if (Guid.TryParse(id, out var guid))
                {
                    var SongsPlaylist = await _dal.ReadSongsPlaylistAsync(guid);
                    await Clients.Caller.SendAsync("ReceiveSongsPlaylist", SongsPlaylist);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestSongPlaylist(string id)
        {
            try
            {
                if (Guid.TryParse(id, out var guid))
                {
                    //var userId = await GetUserIdAsync();

                    var songPlaylist = await _dal.ReadSongPlaylistAsync(guid);
                    await Clients.Caller.SendAsync("ReceiveSongPlaylist", songPlaylist);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task CreateSongPlaylist(SongPlaylistCreateRequest request)
        {
            try
            {
                var userId = await GetUserIdAsync();
                if (userId != Guid.Empty)
                {
                    var songPlaylist = await _dal.CreateSongPlaylistAsync(request, userId);
                    await Clients.Caller.SendAsync("ReceiveSongPlaylist", songPlaylist);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task DeleteSongPlaylist(string id)
        {
            try
            {
                if (Guid.TryParse(id, out var guid))
                {
                    var userId = await GetUserIdAsync();
                    if (userId != Guid.Empty)
                    {
                        var songPlaylist = await _dal.ReadSongPlaylistAsync(guid);
                        if (songPlaylist != null && await _dal.DeleteSongPlaylistAsync(guid, userId))
                        {
                            var songsPlaylist = await _dal.ReadSongsPlaylistAsync(songPlaylist.Playlist.Id);
                            await Clients.Caller.SendAsync("ReceiveSongsPlaylist", songsPlaylist);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task RequestSearchYoutube(string searchSnippet)
        {
            try
            {
                var responses = await _dal.SearchYoutubeLightAsync(searchSnippet);
                await Clients.Caller.SendAsync("ReceiveSearchYoutube", responses);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestSearchYoutubePlaylist(string searchSnippet)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestAddYoutubePlaylistToPlaylist(string youtubePlaylistUrl, Guid playlistId)
        {
            try
            {
                var userId = await GetUserIdAsync();
                var songsPlaylist = await _dal.AddYoutubePlaylistToPlaylistAsync(youtubePlaylistUrl, playlistId, userId);
                await Clients.Caller.SendAsync("ReceiveAddYoutubePlaylistToPlaylist", songsPlaylist);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestAddSpotifyPlaylistToPlaylist(string spotifyPlaylistId, Guid playlistId)
        {
            try
            {
                var userId = await GetUserIdAsync();
                var user = await _dal.ReadApplicationUserAsync(userId);

                var songsInPlaylist = await _dal.ReadSongsPlaylistAsync(playlistId);

                var numerOfSongsTemainingToAddToPlaylist = user.PlaylistSongCount - songsInPlaylist.Length;
                var counter = 0;

                SpotifyPlaylistTracksResponse spotifyTracks;

                //var nextURL = string.Empty;
                var tracks = new List<Lib.Responses.SpotifyPlaylistTracksResponse.Item>();

                //while (nextURL != null)
                //{
                //    using (var client = new HttpClient())
                //    {
                //        if (spotifyPlaylistId.IndexOf("/") > 0)
                //        {
                //            spotifyPlaylistId = spotifyPlaylistId.Split("/").Last();
                //        }

                //        client.SetBearerToken(Globals.SPOTIFY_ACCESS_TOKEN);
                //        var response = await client.GetStringAsync($"https://api.spotify.com/v1/playlists/{spotifyPlaylistId}/tracks");
                //        spotifyTracks = JsonConvert.DeserializeObject<SpotifyPlaylistTracksResponse>(response);

                //        if (spotifyTracks == null || spotifyTracks.items == null)
                //        {
                //            var spotifyCollabTracks = JsonConvert.DeserializeObject<SpotifyPlaylistTracksCollabResponse>(response);

                //            foreach (var track in spotifyCollabTracks.tracks.items)
                //            {
                //                if (counter < numerOfSongsTemainingToAddToPlaylist)
                //                {
                //                    counter++;
                //                    tracks.Add(track);
                //                }
                //            }
                //            nextURL = spotifyCollabTracks.tracks.next;
                //        }
                //        else
                //        {
                //            foreach (var track in spotifyTracks.items)
                //            {
                //                if (counter < numerOfSongsTemainingToAddToPlaylist)
                //                {
                //                    counter++;
                //                    tracks.Add(track);
                //                }
                //            }
                //            nextURL = spotifyCollabTracks.tracks.next;
                //        }
                //    }
                //}

                // Get Spotify Access Token
                var accessToken = await _service.GetSpotifyAccessToken();

                using (var client = new HttpClient())
                {
                    if (spotifyPlaylistId.IndexOf("/") > 0)
                    {
                        spotifyPlaylistId = spotifyPlaylistId.Split("/").Last();
                    }

                    client.SetBearerToken(accessToken);
                    var response = await client.GetStringAsync($"https://api.spotify.com/v1/playlists/{spotifyPlaylistId}/tracks");
                    spotifyTracks = JsonConvert.DeserializeObject<SpotifyPlaylistTracksResponse>(response);

                    if (spotifyTracks != null || spotifyTracks.items != null)
                    {
                        foreach (var track in spotifyTracks.items)
                        {
                            if (counter < numerOfSongsTemainingToAddToPlaylist)
                            {
                                counter++;
                                tracks.Add(track);
                            }
                        }
                    }
                }

                var songs = new List<SongVM>();
                foreach (var track in tracks)
                {
                    var searchSnippet = track.track.name + "-" + track.track.artists[0].name;
                    var song = await _dal.SearchYoutubeAndReturnFirstResultAsync(searchSnippet);

                    if (song != null && !songs.Any(s => s.Id == song.Id))
                    {
                        songs.Add(song);
                    }
                }

                var songsPlaylist = await _dal.AddSongsToPlaylistAsync(songs.ToArray(), playlistId, userId);
                await Clients.Caller.SendAsync("ReceiveAddSpotifyPlaylistToPlaylist", songsPlaylist);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            await Clients.Caller.SendAsync("ReceiveAddSpotifyPlaylistToPlaylist", null);
        }

        public async Task RequestClearProfileImage(Guid profileId)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestClearRoomImage(Guid roomId)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestClearPlaylistImage(Guid playlistId)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //public async Task RequestCurrencies()
        //{
        //    try
        //    {
        //        //var userId = await GetUserIdAsync();
        //        var currencies = await _services.ReadCurrenciesAsync();
        //        await Clients.Caller.SendAsync("ReceiveCurrencies", currencies);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
        //public async Task RequestCurrency(string id)
        //{
        //    try
        //    {
        //        if (Guid.TryParse(id, out var guid))
        //        {
        //            //var userId = await GetUserIdAsync();

        //            var currency = await _services.ReadCurrencyAsync(guid);
        //            await Clients.Caller.SendAsync("ReceiveCurrency", currency);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
        //public async Task CreateCurrency(CurrencyCreateRequest request)
        //{
        //    try
        //    {
        //        var userId = await GetUserIdAsync();
        //        if (userId != Guid.Empty)
        //        {
        //            var currency = request.Id == Guid.Empty
        //                ? await _services.CreateCurrencyAsync(request, userId)
        //                : await _services.UpdateCurrencyAsync(request, userId);
        //            await Clients.Caller.SendAsync("ReceiveCurrency", currency);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
        //public async Task DeleteCurrency(string id)
        //{
        //    try
        //    {
        //        if (Guid.TryParse(id, out var guid))
        //        {
        //            if (await _services.DeleteCurrencyAsync(guid))
        //            {
        //                await Clients.Caller.SendAsync("ReceiveCurrency");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}

        public async Task RequestCurrenciesRoom(string roomId)
        {
            try
            {
                if (Guid.TryParse(roomId, out var guid))
                {
                    var currenciesRoom = await _dal.ReadCurrenciesRoomAsync(guid);

                    await Clients.Caller.SendAsync("ReceiveCurrenciesRoom", currenciesRoom);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestCurrencyRoom(string id)
        {
            try
            {
                if (Guid.TryParse(id, out var guid))
                {
                    var currencyRoom = await _dal.ReadCurrencyAsync(guid);

                    await Clients.Caller.SendAsync("ReceiveCurrencyRoom", currencyRoom);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task RequestPurchasableItems()
        {
            try
            {
                var purchasableItems = await _dal.ReadPurchasableItemsAsync();
                await Clients.Caller.SendAsync("ReceivePurchasableItems", purchasableItems);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestPurchasableItem(Guid id)
        {
            try
            {
                var purchasableItem = await _dal.ReadPurchasableItemAsync(id);
                await Clients.Caller.SendAsync("ReceivePurchasableItem", purchasableItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        //public async Task RequestPurchasableItemsCurrencies(Guid currencyRoomId)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
        public async Task CreatePurchasableItem(PurchasableItemCreateRequest request)
        {
            try
            {
                var purchasableItem = request.Id == Guid.Empty
                    ? await _dal.CreatePurchasableItemAsync(request)
                    : await _dal.UpdatePurchasableItemAsync(request);

                await Clients.Caller.SendAsync("ReceivePurchasableItem", purchasableItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task DeletePurchasableItem(string id)
        {
            try
            {
                var userId = await GetUserIdAsync();
                if (Guid.TryParse(id, out var guid))
                {
                    if (await _dal.DeletePurchasableItemAsync(guid, userId))
                    {
                        await Clients.Caller.SendAsync("ReceivePurchasableItem");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //public async Task CreatePurchase(PurchaseCreateRequest request)
        //{
        //    try
        //    {
        //        var userId = await GetUserIdAsync();
        //        var purchase = await _dal.CreatePurchaseAsync(request, userId);
        //        await Clients.Caller.SendAsync("ReceivePurchase", purchase);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
        public async Task RequestAuthToLockedRoom(string roomKey, Guid roomId)
        {
            try
            {
                var room = await _dal.ReadRoomAsync(roomId);

                await Clients.Caller.SendAsync("ResponseAuthToLockedRoom", new AuthToLockedRoomResponse
                {
                    AuthToLockedRoomResponseType = await _dal.CheckAuthToLockedRoomAsync(roomKey, roomId)
                        ? AuthToLockedRoomResponseType.Success
                        : AuthToLockedRoomResponseType.Fail,
                    Room = room
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestProfile(string username)
        {
            try
            {
                var profile = await _dal.ReadProfileAsync(username);
                await Clients.Caller.SendAsync("ReceiveProfile", profile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestProfileUpdate(ProfileUpdateRequest request)
        {
            try
            {
                var userId = await GetUserIdAsync();
                var profile = await _dal.UpdateProfileAsync(request, userId);

                await Clients.Caller.SendAsync("ReceiveProfile", profile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task PingResponse()
        {
            var connection = await _dal.ReadApplicationUserRoomConnectionAsync(Context.ConnectionId);

            if (connection != null)
            {
                try
                {
                    await _dal.UpdateApplicationUserRoomConnectionAsync(new ApplicationUserRoomConnectionUpdateRequest
                    {
                        Id = connection.Id,
                        HasPingBeenSent = false,
                        IsOnline = true
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        
        public async Task QueuePlaylistInRoomHome(Guid playlistId, bool isRandomized)
        {
            try
            {
                var applicationUserRoomConnection = await _dal.ReadApplicationUserRoomConnectionAsync(Context.ConnectionId);

                if (applicationUserRoomConnection == null)
                {
                    applicationUserRoomConnection = await CheckConnectionAsync();

                    if (applicationUserRoomConnection == null)
                    {
                        await _listifyHub.Clients.Client(Context.ConnectionId).SendAsync("ForceServerDisconnect");
                    }
                }

                var applicationUserRoom = await _dal.ReadApplicationUserRoomAsync(applicationUserRoomConnection.ApplicationUserRoom.Id);
                var applicationUser = await _dal.ReadApplicationUserAsync(applicationUserRoom.ApplicationUser.Id);
                var roomHome = await _dal.ReadRoomAsync(applicationUserRoom.Room.Id);

                var songsQueued = await _dal.QueuePlaylistInRoomHomeAsync(playlistId, isRandomized, applicationUser.Id);

                await Clients.Caller.SendAsync("ReceiveQueuePlaylistInRoomHome", songsQueued);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task RequestPurchases()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var context = Context.GetHttpContext();
                var token = context.Request.Query["token"];

                if (!string.IsNullOrWhiteSpace(token))
                {
                    //var userInfoClient = new IdentityModel.Client.UserInfoClient();
                    var client = new HttpClient();

                    var disco = await client.GetDiscoveryDocumentAsync(Globals.IDENTITY_SERVER_AUTHORITY_URL);
                    var response = await client.GetUserInfoAsync(new UserInfoRequest
                    {
                        Address = disco.UserInfoEndpoint,
                        Token = token
                    });

                    var username = response.Claims.ToList().First(s => s.Type == "name").Value;
                    //var userId = response.Claims.ToList().First(s => s.Type == "preferred_username").Value;
                    var userId = response.Claims.ToList().First(s => s.Type == "sub").Value;

                    var applicationUser = await _dal.ReadApplicationUserAsync(userId);

                    if (applicationUser == null)
                    {
                        var roomCodeNew = username;
                        // this prevents 2 rooms from having the same code
                        var index = 0;
                        while (await _context.Rooms.AnyAsync(s => s.RoomCode.Trim().ToLower() == roomCodeNew.Trim().ToLower()))
                        {
                            roomCodeNew = username + index++;
                        }

                        // this prevents 2 users from having the same username
                        var i = 0;
                        while (await _context.ApplicationUsers.AnyAsync(s => s.Username.Trim().ToLower() == username.Trim().ToLower()))
                        {
                            username += i++;
                        }

                        // the room is attached here
                        applicationUser = await _dal.CreateApplicationUserAsync(new ApplicationUserCreateRequest
                        {
                            AspNetUserId = userId,
                            Username = username,
                            RoomCode = roomCodeNew
                        });
                    }

                    // if the room was not specified, then get the default
                    var room = await _dal.ReadRoomAsync(applicationUser.Room.Id);

                    if (room != null)
                    {
                        var applicationUserRoom = await _dal.ReadApplicationUserRoomAsync(applicationUser.Id, room.Id);

                        if (applicationUserRoom == null)
                        {
                            applicationUserRoom = await _dal.CreateApplicationUserRoomAsync(new ApplicationUserRoomCreateRequest
                            {
                                IsOnline = true,
                                RoomId = room.Id
                            }, applicationUser.Id);
                        }

                        await _dal.CheckCurrenciesRoomAsync(room.Id);

                        var connection = await _dal.ReadApplicationUserRoomConnectionAsync(Context.ConnectionId);

                        connection = connection == null
                            ? await _dal.CreateApplicationUserRoomConnectionAsync(new ApplicationUserRoomConnectionCreateRequest
                            {
                                ApplicationUserRoomId = applicationUserRoom.Id,
                                ConnectionId = Context.ConnectionId,
                                IsOnline = true,
                                ConnectionType = ConnectionType.ListifyHub
                            })
                            : await _dal.UpdateApplicationUserRoomConnectionAsync(new ApplicationUserRoomConnectionUpdateRequest
                            {
                                HasPingBeenSent = connection.HasPingBeenSent,
                                IsOnline = true,
                                Id = connection.Id
                            });

                        await base.OnConnectedAsync();

                        var applicationUserVM = _mapper.Map<ApplicationUserVM>(applicationUser);

                        await Clients.Caller.SendAsync("ReceiveApplicationUser", applicationUserVM);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                await base.OnDisconnectedAsync(exception);
                var connection = await _dal.ReadApplicationUserRoomConnectionAsync(Context.ConnectionId);
                if (connection != null)
                {
                    //// if the user which is not owner but logout from different user
                    //// just update his connection to offline
                    //if (!connection.ApplicationUserRoom.IsOwner)
                    //{
                    //    var applicationUserRoom = await _dal.ReadApplicationUserRoomAsync(connection.ApplicationUserRoom.Id);

                    //    if (applicationUserRoom != null)
                    //    {
                    //        var room = await _dal.ReadRoomAsync(applicationUserRoom.Room.Id);
                    //        await _dal.UpdateRoomAsync(new RoomUpdateRequest
                    //        {
                    //            IsRoomPlaying = false,
                    //            Id = room.Id,
                    //            IsRoomOnline = false,
                    //            RoomGenres = room.RoomGenres.ToArray()
                    //        });
                    //    }
                    //}

                    await _dal.UpdateApplicationUserRoomConnectionAsync(new ApplicationUserRoomConnectionUpdateRequest
                    {
                        Id = connection.Id,
                        HasPingBeenSent = connection.HasPingBeenSent,
                        IsOnline = false
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected virtual async Task<Guid> GetUserIdAsync()
        {
            var applicationUserRoomConnection = await _dal.ReadApplicationUserRoomConnectionAsync(Context.ConnectionId);

            if (applicationUserRoomConnection == null)
            {
                applicationUserRoomConnection = await CheckConnectionAsync();

                if (applicationUserRoomConnection == null)
                {
                    await _listifyHub.Clients.Client(Context.ConnectionId).SendAsync("ForceServerDisconnect");
                    return Guid.Empty;
                }
            }

            var applicationUserRoom = await _dal.ReadApplicationUserRoomAsync(applicationUserRoomConnection.ApplicationUserRoom.Id);
            return applicationUserRoom.ApplicationUser.Id;
        }
        protected virtual async Task<ApplicationUserRoomConnectionVM> CheckConnectionAsync()
        {
            var context = Context.GetHttpContext();
            var token = context.Request.Query["token"];

            //var userInfoClient = new IdentityModel.Client.UserInfoClient();
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync(Globals.IDENTITY_SERVER_AUTHORITY_URL);
            var response = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = token
            });

            var username = response.Claims.ToList().First(s => s.Type == "name").Value;
            var userId = response.Claims.ToList().First(s => s.Type == "sub").Value;

            if (!string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(userId))
            {
                var applicationUser = await _dal.ReadApplicationUserAsync(userId);

                if (applicationUser == null)
                {
                    var roomCodeNew = username;
                    // this prevents 2 rooms from having the same code
                    var index = 0;
                    while (await _context.Rooms.AnyAsync(s => s.RoomCode.Trim().ToLower() == roomCodeNew.Trim().ToLower()))
                    {
                        roomCodeNew = username + index++;
                    }

                    // the room is attached here
                    applicationUser = await _dal.CreateApplicationUserAsync(new ApplicationUserCreateRequest
                    {
                        AspNetUserId = userId,
                        Username = username,
                        RoomCode = roomCodeNew
                    });
                }

                // if the room was not specified, then get the default
                var room = await _dal.ReadRoomAsync(applicationUser.Room.Id);

                if (room != null)
                {
                    room = await _dal.UpdateRoomAsync(new RoomUpdateRequest
                    {
                        Id = room.Id,
                        IsRoomOnline = true,
                        IsRoomPlaying = true,
                        RoomGenres = room.RoomGenres.ToArray()
                    });

                    var applicationUserRoom = await _dal.ReadApplicationUserRoomAsync(applicationUser.Id, room.Id);

                    if (applicationUserRoom == null)
                    {
                        applicationUserRoom = await _dal.CreateApplicationUserRoomAsync(new ApplicationUserRoomCreateRequest
                        {
                            IsOnline = true,
                            RoomId = room.Id
                        }, applicationUser.Id);
                    }

                    var connection = await _dal.ReadApplicationUserRoomConnectionAsync(Context.ConnectionId);

                    connection = connection == null
                    ? await _dal.CreateApplicationUserRoomConnectionAsync(new ApplicationUserRoomConnectionCreateRequest
                    {
                        ApplicationUserRoomId = applicationUserRoom.Id,
                        ConnectionId = Context.ConnectionId,
                        IsOnline = true,
                        ConnectionType = ConnectionType.ListifyHub
                    })
                    : await _dal.UpdateApplicationUserRoomConnectionAsync(new ApplicationUserRoomConnectionUpdateRequest
                    {
                        HasPingBeenSent = connection.HasPingBeenSent,
                        IsOnline = true,
                        Id = connection.Id
                    });

                    return connection;
                }
            }
            return null;
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (_pingPoll != null)
        //    {
        //        _pingPoll.PollingEvent -= async (s, e) => await OnPingPollEvent(s, e);
        //    }

        //    base.Dispose(disposing);
        //}
    }
}
