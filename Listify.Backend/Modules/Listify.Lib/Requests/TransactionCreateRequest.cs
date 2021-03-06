﻿using Listify.Domain.Lib.Requests;
using System;

namespace Listify.Lib.Requests
{
    public class TransactionCreateRequest : BaseRequest
    {
        public int TransactionType { get; set; }
        public int QuantityChanged { get; set; }

        public Guid ApplicationUserRoomCurrencyId { get; set; }
    }
}
