﻿using Listify.Domain.Lib.DTOs;

namespace Listify.Lib.DTOs
{
    public class RoomDTO : BaseDTO
    {
        // the identifier for the room that can be chosen by the user
        public string RoomCode { get; set; }
        public string RoomTitle { get; set; }
        public bool AllowRequests { get; set; }
        public bool IsRoomLocked { get; set; }
        public bool IsRoomPublic { get; set; }
        public bool IsRoomOnline { get; set; }
        public int NumberUsersOnline { get; set; }
    }
}
